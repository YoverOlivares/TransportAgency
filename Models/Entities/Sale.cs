using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public int SeatId { get; set; }

        [Range(0.01, 9999.99)]
        public decimal Amount { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string ReceiptNumber { get; set; } = string.Empty;

        // Navigation properties
        public virtual Customer Customer { get; set; } = null!;
        public virtual Seat Seat { get; set; } = null!;
    }
}