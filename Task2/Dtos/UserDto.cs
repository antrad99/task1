using Task2.Enums;
using Task2.Models;

namespace Task2.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<BookDto> Books { get; set; } = [];
        public List<string> Errors { get; set; } = [];
    }
}
