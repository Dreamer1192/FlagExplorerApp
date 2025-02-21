using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlagExplorer.Api.Models
{
    public class Country
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Country name is required.")]
        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters.")]
        public string Name { get; set; } = default!;

        [StringLength(300, ErrorMessage = "Flag URL cannot exceed 300 characters.")]
        public string? Flag { get; set; }

        [Required(ErrorMessage = "Population is required.")]
        [Range(0, long.MaxValue, ErrorMessage = "Population must be a non-negative value.")]
        public long Population { get; set; }

        [StringLength(100, ErrorMessage = "Capital name cannot exceed 100 characters.")]
        public string? Capital { get; set; }

        // If you want a computed property, you can do that as well:
        // [NotMapped]
        // public double SomeComputedValue => ... 
    }
}
