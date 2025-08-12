using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Trip
    {
        public int Id { get; set; }

        public int BusId { get; set; }
        public int RouteId { get; set; }

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        [Range(0.01, 9999.99)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Bus Bus { get; set; } = null!;
        public virtual Route Route { get; set; } = null!;
        public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}