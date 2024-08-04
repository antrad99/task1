using Task2.Dtos;
using Task2.Models;

namespace Task2.Mapping
{
    public class BookMapping
    {
        public Book MapFromDto(Book book, BookDto bookDto)
        {
            book.Id = bookDto.Id;
            book.ISBN = bookDto.ISBN;
            book.Availabity = bookDto.Availabity;
            book.Author = bookDto.Author;
            book.Title = bookDto.Title;

            return book;

        }

        public BookDto MapToDto(Book book)
        {
            var bookDto = new BookDto();

            bookDto.Id = book.Id;
            bookDto.Author = book.Author;
            bookDto.ISBN = book.ISBN;
            bookDto.Title = book.Title;
            bookDto.Availabity = book.Availabity;

            return bookDto;
        }
    }
}
