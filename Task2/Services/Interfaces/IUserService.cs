using Task2.Dtos;

namespace Task2.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AddUser(UserDto book);
        Task<UserDto> UpdateUser(UserDto book);
        Task<UserDto> DeleteUser(int userId);
        Task<UserDto> GetUserById(int userId);
        Task<UserDto> BorrowBook(int userId, int bookId);
    }
}
