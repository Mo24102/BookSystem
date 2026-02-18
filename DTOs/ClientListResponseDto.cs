namespace Booking_System.DTOs
{
    public class ClientListResponseDto
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<ClientResponseItemDto> Data { get; set; } = new();
    }
}
