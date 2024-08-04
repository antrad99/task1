using Task2.Dtos;
using Task2.Models;

namespace Task2.Mapping
{
    public class UserMapping
    {
        public User MapFromDto(User user, UserDto userDto)
        {
            user.Id = userDto.Id;
            user.Name = userDto.Name;

            return user;

        }

        public UserDto MapToDto(User user)
        {
            var userDto = new UserDto();

            userDto.Id = user.Id;
            userDto.Name = user.Name;


            List<BookDto> booksDto = new List<BookDto>();
            var bookMapping = new BookMapping();
            foreach (var book in user.Books)
            {
                var bookDto = bookMapping.MapToDto(book);               
                booksDto.Add(bookDto);
            }

            userDto.Books = booksDto;

            return userDto;
        }
    }
}
