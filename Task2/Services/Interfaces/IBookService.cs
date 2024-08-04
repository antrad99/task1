using Task2.Dtos;

namespace Task2.Services.Interfaces
{
    public interface IBookService
    {
        Task<BookDto> AddBook(BookDto book);
        Task<BookDto> UpdateBook(BookDto book);
        Task<BookDto> DeleteBook(int bookId);
        Task<BooksDto> SearchBook(string search);
        Task<BookDto> GetBookById(int id);
    }
}
