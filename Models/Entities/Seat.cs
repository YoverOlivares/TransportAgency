using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Seat
    {
        public int Id { get; set; }

        public int TripId { get; set; }

        [Required]
        [StringLength(5)]
        public string SeatNumber { get; set; } = string.Empty;

        public bool IsOccupied { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Trip Trip { get; set; } = null!;
        public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }
}