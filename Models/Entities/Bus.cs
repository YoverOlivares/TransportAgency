using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.Entities
{
    public class Bus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string PlateNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Range(10, 60)]
        public int Capacity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}