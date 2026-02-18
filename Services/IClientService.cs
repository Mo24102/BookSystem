using Booking_System.DTOs;

namespace Booking_System.Services
{
    public interface IClientService
    {
        Task<int> AddClientAsync(ClientResponseItemDto dto, int userId);
        Task<object> GetClientsAsync(int userId, bool isAdmin, int pageNumber,
            int pageSize, string? search, string? serviceType);
        Task<bool> DeleteClientAsync(int id);
        Task UpdateClientAsync(int id, ClientResponseItemDto dto);

    }
}