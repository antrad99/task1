using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Task2.Data;
using Task2.Dtos;
using Task2.Enums;
using Task2.Services;

namespace UnitTests
{
    public class BookServiceTests
    {
        private ServiceCollection _services = new ServiceCollection();
        private IWebHostEnvironment _env;
        private BookDto book1;
        private BookDto book2;
        private BookDto book3;

        public BookServiceTests()
        {

            //Set up Environments - begin
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns(Environments.Production);
            _env = mockEnvironment.Object;
            //Set up Environments - end

            _services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            book1 = new BookDto()
            {
                Id = 1,
                Author = "Luca",
                Title = "My Book",
                ISBN = "XSD345",
                Availabity = BookStatus.Available
            };

            book2 = new BookDto()
            {
                Id = 2,
                Author = "Luca1",
                Title = "My Book1",
                ISBN = "XSD345A",
                Availabity = BookStatus.Available
            };

            book3 = new BookDto()
            {
                Id = 3,
                Author = "Luca2",
                Title = "My Book2",
                ISBN = "XSD345AB",
                Availabity = BookStatus.Available
            };

        }

        [Fact]
        public async void AddBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    var result = await bookService.AddBook(book1);

                    Assert.Empty(result.Errors);

                    var book = await context.Book.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Equal(book.Id, book1.Id);
                    Assert.Equal(book.Title, book1.Title);
                    Assert.Equal(book.ISBN, book1.ISBN);
                    Assert.Equal(book.Availabity, book1.Availabity);
                    Assert.Equal(book.Author, book1.Author);
                }
            }
        }

        [Fact]
        public async void UpdateBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    await bookService.AddBook(book1);

                    var updBook = new BookDto()
                    {
                        Id = 1,
                        Author = "Luca1",
                        Title = "My Book1",
                        ISBN = "XSD3451",
                        Availabity = BookStatus.Borrowed
                    };

                    var result = await bookService.UpdateBook(updBook);

                    Assert.Empty(result.Errors);

                    var book = await context.Book.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Equal(book.Id, updBook.Id);
                    Assert.Equal(book.Title, updBook.Title);
                    Assert.Equal(book.ISBN, updBook.ISBN);
                    Assert.Equal(book.Availabity, updBook.Availabity);
                    Assert.Equal(book.Author, updBook.Author);
                }
            }
        }

        [Fact]
        public async void DeleteBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    await bookService.AddBook(book1);

                    var result = await bookService.DeleteBook(book1.Id);

                    Assert.Empty(result.Errors);

                    var book = await context.Book.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Null(book);
                }
            }
        }

        [Fact]
        public async void SearchBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    await bookService.AddBook(book1);
                    await bookService.AddBook(book2);
                    await bookService.AddBook(book3);

                    var result = await bookService.SearchBook("Luca");

                    Assert.Empty(result.Errors);
                    Assert.Equal(3, result.Books.Count);

                    var result1 = result.Books[0];
                    Assert.Equal(book1.Id, result1.Id);
                    Assert.Equal(book1.Title, result1.Title);
                    Assert.Equal(book1.ISBN, result1.ISBN);
                    Assert.Equal(book1.Availabity, result1.Availabity);
                    Assert.Equal(book1.Author, result1.Author);

                    var result2 = result.Books[1];
                    Assert.Equal(book2.Id, result2.Id);
                    Assert.Equal(book2.Title, result2.Title);
                    Assert.Equal(book2.ISBN, result2.ISBN);
                    Assert.Equal(book2.Availabity, result2.Availabity);
                    Assert.Equal(book2.Author, result2.Author);

                    var result3 = result.Books[2];
                    Assert.Equal(book3.Id, result3.Id);
                    Assert.Equal(book3.Title, result3.Title);
                    Assert.Equal(book3.ISBN, result3.ISBN);
                    Assert.Equal(book3.Availabity, result3.Availabity);
                    Assert.Equal(book3.Author, result3.Author);



                }
            }
        }

        [Fact]
        public async void SearchBook1()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    await bookService.AddBook(book1);
                    await bookService.AddBook(book2);
                    await bookService.AddBook(book3);

                    var result = await bookService.SearchBook("Luca1");
                    Assert.Single(result.Books);
                    Assert.Equal(2, result.Books[0].Id);
                    var result1 = await bookService.SearchBook("Luca2");
                    Assert.Single(result1.Books);
                    Assert.Equal(3, result1.Books[0].Id);
                    var result2 = await bookService.SearchBook("XSD345A");
                    Assert.Equal(2, result2.Books.Count);
                    Assert.Equal(2, result2.Books[0].Id);
                    Assert.Equal(3, result2.Books[1].Id);
                    var result3 = await bookService.SearchBook("My Book2");
                    Assert.Single(result3.Books);
                    Assert.Equal(3, result3.Books[0].Id);
                }
            }
        }

        [Fact]
        public async void GetBookById()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var bookService = new BookService(context, _env);

                    await bookService.AddBook(book1);

                    var result = await bookService.GetBookById(1);

                    Assert.Equal(book1.Id, result.Id);
                    Assert.Equal(book1.Title, result.Title);
                    Assert.Equal(book1.ISBN, result.ISBN);
                    Assert.Equal(book1.Availabity, result.Availabity);
                    Assert.Equal(book1.Author, result.Author);

                }
            }
        }
    }
}