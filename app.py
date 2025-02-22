from flask import Flask, render_template, abort, url_for
from werkzeug.exceptions import HTTPException
import logging
import requests

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

app = Flask(__name__)

# Set the base URL for your backend API.
BACKEND_API_URL = "http://localhost:5287/api/Countries"

def get_all_countries():
    """Fetch all country data from the backend API."""
    try:
        response = requests.get(BACKEND_API_URL, timeout=10)
        response.raise_for_status()  # Raise an exception for HTTP errors
        return response.json()
    except requests.exceptions.RequestException as e:
        logger.exception("Error fetching all countries from backend API.")
        raise

def get_country_by_name(country_name):
    """Fetch details for a specific country from the backend API."""
    try:
        url = f"{BACKEND_API_URL}/{country_name}"
        response = requests.get(url, timeout=10)
        if response.status_code == 404:
            return None
        response.raise_for_status()
        return response.json()
    except requests.exceptions.RequestException as e:
        logger.exception("Error fetching country '%s' from backend API.", country_name)
        raise

@app.route('/')
def home():
    try:
        # Fetch all countries from the backend API.
        countries = get_all_countries()
        # Sort countries by name
        countries_sorted = sorted(countries, key=lambda c: c.get("name", ""))
        return render_template("home.html", countries=countries_sorted)
    except Exception as e:
        logger.exception("Error in home route.")
        return "An error occurred while fetching country data.", 500

@app.route('/country/<string:country_name>')
def country_detail(country_name):
    try:
        # Fetch specific country details from the backend API.
        country = get_country_by_name(country_name)
        if not country:
            abort(404)
        return render_template("detail.html", country=country)
    except HTTPException as http_exc:
        # Re-raise HTTP exceptions so that Flask handles them (e.g., 404 errors)
        raise http_exc
    except Exception as e:
        logger.exception("Error in country_detail route for country: %s", country_name)
        return "An error occurred while fetching country details.", 500

if __name__ == '__main__':
    try:
        app.run(debug=True, port=5001, use_reloader=False)
    except Exception as e:
        logger.exception("Error starting Flask app.")
