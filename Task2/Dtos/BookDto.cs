using Task2.Enums;

namespace Task2.Dtos
{
    public class BooksDto
    {
        public List<BookDto> Books { get; set; } = [];
        public List<string> Errors { get; set; } = [];
    }
}
