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
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<BookDto> AddBook(BookDto bookDto)
        {
            try
            {
                bookDto.Errors = [];

                if (string.IsNullOrEmpty(bookDto.Author))
                    bookDto.Errors.Add("Please insert Author");

                if (string.IsNullOrEmpty(bookDto.Title))
                    bookDto.Errors.Add("Please insert Title");

                if (string.IsNullOrEmpty(bookDto.ISBN))
                    bookDto.Errors.Add("Please insert ISBN");

                if (bookDto.Errors.Count>0)
                    return bookDto;

                var bookMapping = new BookMapping();
                var book = bookMapping.MapFromDto(new Book(), bookDto);

                await _context.Book.AddAsync(book);

                await _context.SaveChangesAsync();

                bookDto.Id = book.Id;

                return bookDto;

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on AddBook");
                if (_env.IsDevelopment())
                    bookDto.Errors.Add(ex.ToString());
                else
                    bookDto.Errors.Add("An error occurred when adding the book");

                return bookDto;
            }
        }

        public async Task<BookDto> UpdateBook(BookDto bookDto)
        {
            try
            {
                bookDto.Errors = [];

                if (string.IsNullOrEmpty(bookDto.Author))
                    bookDto.Errors.Add("Please insert Author");

                if (string.IsNullOrEmpty(bookDto.Title))
                    bookDto.Errors.Add("Please insert Title");

                if (string.IsNullOrEmpty(bookDto.ISBN))
                    bookDto.Errors.Add("Please insert ISBN");

                if (bookDto.Errors.Count > 0)
                    return bookDto;


                //Check Book
                var book = _context.Book.FirstOrDefault(t => t.Id == bookDto.Id);
                if (book == null)
                {
                    bookDto.Errors.Add($"BookId ${bookDto.Id} not found");
                    return bookDto;
                }

                //Map Book
                var bookMapping = new BookMapping();
                book = bookMapping.MapFromDto(book, bookDto);

                await _context.SaveChangesAsync();

                return bookDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on UpdateBook");
                if (_env.IsDevelopment())
                    bookDto.Errors.Add(ex.ToString());
                else
                    bookDto.Errors.Add("An error occurred when updating the book");

                return bookDto;
            }
        }

        public async Task<BookDto> DeleteBook(int bookId)
        {
            var bookDto = new BookDto();

            try
            {
                //Check Book
                var book = _context.Book.FirstOrDefault(t => t.Id == bookId);
                if (book == null)
                {
                    bookDto.Errors.Add($"BookId ${bookId} not found");
                    return bookDto;
                }

                _context.Book.Remove(book);
                await _context.SaveChangesAsync();

                return bookDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on DeleteBook");
                if (_env.IsDevelopment())
                    bookDto.Errors.Add(ex.ToString());
                else
                    bookDto.Errors.Add("An error occurred when deleting the book");

                return bookDto;
            }
        }

        public async Task<BooksDto> SearchBook(string search)
        {
            var booksDto = new BooksDto();

            try
            {
                if (string.IsNullOrEmpty(search))
                {
                    booksDto.Errors.Add("Please insert a string to search");
                    return booksDto;
                }
                
                var books = await _context.Book.Where(t => t.Author.Contains(search) || t.Title.Contains(search) || t.ISBN.Contains(search)).Take(20).OrderBy(t => t.Title).ToListAsync();

                var bookMapping = new BookMapping();
                foreach (var book in books)
                {
                    var bookDto = bookMapping.MapToDto(book);
                    booksDto.Books.Add(bookDto);
                }

                return booksDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on SearchBook");
                if (_env.IsDevelopment())
                    booksDto.Errors.Add(ex.ToString());
                else
                    booksDto.Errors.Add("An error occurred when search books");

                return booksDto;
            }
        }

        public async Task<BookDto> GetBookById(int id)
        {
            var bookDto = new BookDto();

            try
            {
                var book = await _context.Book.FirstOrDefaultAsync(t => t.Id == id);

                if (book == null)
                {
                    bookDto.Errors.Add("Book not found");
                    return bookDto;
                }

                var bookMapping = new BookMapping();
                bookDto = bookMapping.MapToDto(book);
                
                return bookDto;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error on SearchBook");
                if (_env.IsDevelopment())
                    bookDto.Errors.Add(ex.ToString());
                else
                    bookDto.Errors.Add("An error occurred when getting book by id");

                return bookDto;
            }
        }
    }
}
