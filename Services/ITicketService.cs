using TransportAgency.Models.ViewModels;
using TransportAgency.Models.Entities;

namespace TransportAgency.Services
{
    public interface ITicketService
    {
        List<AvailableTripViewModel> GetAvailableTrips(string origin, string destination, DateTime travelDate);
        Task<int> CreateSaleAsync(PassengerViewModel passengerData);
        Task<ConfirmationViewModel?> GetSaleConfirmationAsync(int saleId);
        Task<List<SaleListItemViewModel>> GetAllSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<bool> UpdateSaleAsync(EditSaleViewModel model);
        Task<bool> DeleteSaleAsync(int id);
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
        List<string> GetAvailableCities();
        List<string> GetAvailableSeats(string origin, string destination, DateTime travelDate, string departureTime);
    }
}