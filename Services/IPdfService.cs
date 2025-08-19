namespace TransportAgency.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateTicketPdfAsync(int saleId);
    }
}