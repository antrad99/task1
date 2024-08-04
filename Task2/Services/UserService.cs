using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using Task2.Data;
using Task2.Dtos;
using Task2.Mapping;
using Task2.Models;
using Task2.Services.Interfaces;

namespace Task2.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserService (ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<UserDto> AddUser(UserDto userDto)
        {
            try
            {
                userDto.Errors = [];

                if (string.IsNullOrEmpty(userDto.Name))
                {
                    userDto.Errors.Add("Please insert Name");
                    return userDto;
                }

                var userMapping = new UserMapping();
                var user = userMapping.MapFromDto(new User(), userDto);

                await _context.User.AddAsync(user);

                await _context.SaveChangesAsync();

                userDto.Id = user.Id;

                return userDto;

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on AddUser");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when adding the user");

                return userDto;
            }
        }

        public async Task<UserDto> UpdateUser(UserDto userDto)
        {
            try
            {

                userDto.Errors = [];

                if (string.IsNullOrEmpty(userDto.Name))
                {
                    userDto.Errors.Add("Please insert Name");
                    return userDto;
                }

                //Check User
                var user = _context.User.FirstOrDefault(t => t.Id == userDto.Id);
                if (user == null)
                {
                    userDto.Errors.Add($"UserId ${userDto.Id} not found");
                    return userDto;
                }

                //Map User
                var userMapping = new UserMapping();
                user = userMapping.MapFromDto(user, userDto);

                await _context.SaveChangesAsync();

                return userDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on UpdateUser");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when updating the user");

                return userDto;
            }
        }

        public async Task<UserDto> DeleteUser(int userId)
        {
            var userDto = new UserDto();

            try
            {
                //Check User
                var user = _context.User.FirstOrDefault(t => t.Id == userId);
                if (user == null)
                {
                    userDto.Errors.Add($"UserId ${userId} not found");
                    return userDto;
                }

                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                return userDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on DeleteUser");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when deleting the user");

                return userDto;
            }
        }

        public async Task<UserDto> GetUserById(int userId)
        {
            var userDto = new UserDto();

            try
            {
                var user = await _context.User
                        .Include(n => n.Books)
                        .FirstOrDefaultAsync(t => t.Id == userId);
                if (user == null)
                {
                    userDto.Errors.Add($"UserId ${userId} not found");
                    return userDto;
                }

                var userMapping = new UserMapping();
                return userMapping.MapToDto(user);

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on GetUserById");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when getting user by id");

                return userDto;
            }
        }

        public async Task<UserDto> BorrowBook(int userId, int bookId)
        {
            var userDto = new UserDto();

            try
            {
                var user = await _context.User.FirstOrDefaultAsync(t => t.Id == userId);
                if (user == null)
                    userDto.Errors.Add($"UserId ${userId} not found");

                var book = await _context.Book.FirstOrDefaultAsync(t => t.Id == bookId);
                if (book == null)
                    userDto.Errors.Add($"BookId ${bookId} not found");                  

                if (userDto.Errors.Count >  0)
                    return userDto;

                if (book.Availabity == Enums.BookStatus.Borrowed)
                {
                    userDto.Errors.Add($"BookId ${bookId} not available");
                    return userDto;
                }

                user.Books.Add(book);

                book.Availabity = Enums.BookStatus.Borrowed;

                await _context.SaveChangesAsync();


                return await GetUserById(userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on BorrowBook");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when borrowing the book");

                return userDto;
            }
        }

        public async Task<UserDto> ReturnBook(int userId, int bookId)
        {
            var userDto = new UserDto();

            try
            {
                var user = await _context.User.Include(t => t.Books).FirstOrDefaultAsync(t => t.Id == userId);
                if (user == null)
                    userDto.Errors.Add($"UserId ${userId} not found");

                var borrowedBook = user.Books.FirstOrDefault(t => t.Id == bookId);
                if (borrowedBook == null)
                    userDto.Errors.Add($"BookId ${bookId} not found for UserId {userId}");

                var book = await _context.Book.FirstOrDefaultAsync(t => t.Id == bookId);
                if (book == null)
                    userDto.Errors.Add($"BookId ${bookId} not found");

                if (userDto.Errors.Count > 0)
                    return userDto;

                user.Books.Remove(borrowedBook);

                book.Availabity = Enums.BookStatus.Available;

                await _context.SaveChangesAsync();

                return await GetUserById(userId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on ReturnBook");
                if (_env.IsDevelopment())
                    userDto.Errors.Add(ex.ToString());
                else
                    userDto.Errors.Add("An error occurred when returning the book");

                return userDto;
            }
        }

    }
}
