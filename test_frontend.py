import json
import pytest
import requests
from app import app, get_all_countries, get_country_by_name

# -------------------------------------------------
# Unit Tests for Helper Functions
# -------------------------------------------------

class FakeResponse:
    """A fake response object to simulate requests responses."""
    def __init__(self, json_data, status_code=200):
        self.json_data = json_data
        self.status_code = status_code

    def json(self):
        return self.json_data

    def raise_for_status(self):
        if self.status_code != 200:
            raise requests.exceptions.HTTPError(f"HTTP {self.status_code}")

def fake_requests_get_all(url, timeout):
    """Fake requests.get for get_all_countries."""
    # Simulate a backend API response with one test country.
    data = [
        {
            "name": "TestCountry",
            "flag": "http://example.com/flag.png",
            "population": 1000,
            "capital": "TestCapital"
        }
    ]
    return FakeResponse(data, 200)

def fake_requests_get_by_name(url, timeout):
    """Fake requests.get for get_country_by_name."""
    if "TestCountry" in url:
        data = {
            "name": "TestCountry",
            "flag": "http://example.com/flag.png",
            "population": 1000,
            "capital": "TestCapital"
        }
        return FakeResponse(data, 200)
    # Simulate a not found response.
    return FakeResponse({}, 404)

def test_get_all_countries_unit(monkeypatch):
    """Unit test for get_all_countries using a fake requests.get."""
    monkeypatch.setattr(requests, "get", fake_requests_get_all)
    data = get_all_countries()
    assert isinstance(data, list)
    assert len(data) == 1
    assert data[0]["name"] == "TestCountry"

def test_get_country_by_name_unit(monkeypatch):
    """Unit test for get_country_by_name using a fake requests.get."""
    monkeypatch.setattr(requests, "get", fake_requests_get_by_name)
    data = get_country_by_name("TestCountry")
    assert isinstance(data, dict)
    assert data["name"] == "TestCountry"
    # Test for a non-existent country.
    data_none = get_country_by_name("NonExistent")
    assert data_none is None

# -------------------------------------------------
# Integration Tests Using Flask's Test Client
# -------------------------------------------------

@pytest.fixture
def client():
    app.config["TESTING"] = True
    with app.test_client() as client:
        yield client

def test_home_integration(monkeypatch, client):
    """Integration test for the home route."""
    # Monkey-patch requests.get so the home route uses our fake data.
    monkeypatch.setattr(requests, "get", fake_requests_get_all)
    response = client.get("/")
    assert response.status_code == 200
    # Check that the response data contains the test country name.
    assert b"TestCountry" in response.data

def test_country_detail_integration(monkeypatch, client):
    """Integration test for the country detail route (existing country)."""
    monkeypatch.setattr(requests, "get", fake_requests_get_by_name)
    response = client.get("/country/TestCountry")
    assert response.status_code == 200
    assert b"TestCountry" in response.data

def test_country_detail_not_found_integration(monkeypatch, client):
    """Integration test for the country detail route (non-existent country)."""
    monkeypatch.setattr(requests, "get", fake_requests_get_by_name)
    response = client.get("/country/NonExistent")
    assert response.status_code == 404
