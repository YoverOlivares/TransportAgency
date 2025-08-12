using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Route
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Destination { get; set; } = string.Empty;

        [Range(0.1, 999.99)]
        public decimal Distance { get; set; }

        [Range(0.01, 9999.99)]
        public decimal BasePrice { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}