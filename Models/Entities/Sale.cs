using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        [Required, MaxLength(100)]
        public string Origin { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Destination { get; set; } = string.Empty;

        public DateTime TravelDate { get; set; }

        [Required, MaxLength(10)]
        public string SeatNumber { get; set; } = string.Empty;

        [Range(0.01, 9999.99)]
        public decimal Price { get; set; }

        public DateTime PurchaseDate { get; set; } = DateTime.Now;

        [Required, MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string TravelTime { get; set; } = string.Empty;
    }
}