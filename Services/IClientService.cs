using Booking_System.DTOs.Client;

namespace Booking_System.Services
{
    public interface IClientService
    {
        Task<int> AddClientAsync(ClientCreateDto dto, int userId);
        Task UpdateClientAsync(int id, ClientUpdateDto dto);
        Task<object> GetClientsAsync(int userId, bool isAdmin, int pageNumber,
            int pageSize, string? search, string? serviceType);
        Task<bool> DeleteClientAsync(int id);

    }
}