using Task2.Enums;

namespace Task2.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public BookStatus Availabity { get; set; } 
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
