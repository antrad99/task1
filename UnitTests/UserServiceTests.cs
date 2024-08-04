using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Task2.Data;
using Task2.Dtos;
using Task2.Enums;
using Task2.Models;
using Task2.Services;

namespace UnitTests
{
    public class UserServiceTests
    {
        private ServiceCollection _services = new ServiceCollection();
        private IWebHostEnvironment _env;
        private UserDto user1;
        private BookDto book1;

        public UserServiceTests()
        {

            //Set up Environments - begin
            var mockEnvironment = new Mock<IWebHostEnvironment>();
            mockEnvironment
                .Setup(m => m.EnvironmentName)
                .Returns(Environments.Production);
            _env = mockEnvironment.Object;
            //Set up Environments - end

            _services.AddDbContext<ApplicationDbContext>(o => o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            user1 = new UserDto()
            {
                Id = 1,
                Name = "Luca"
            };

            book1 = new BookDto()
            {
                Id = 1,
                Author = "Luca",
                Title = "My Book",
                ISBN = "XSD345",
                Availabity = BookStatus.Available
            };

        }

        [Fact]
        public async void AddUser()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var userService = new UserService(context, _env);

                    var result = await userService.AddUser(user1);

                    Assert.Empty(result.Errors);

                    var user = await context.User.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Equal(user.Id, user1.Id);
                    Assert.Equal(user.Name, user1.Name);
                }
            }
        }

        [Fact]
        public async void UpdateUser()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var userService = new UserService(context, _env);

                    await userService.AddUser(user1);

                    var updUser = new UserDto()
                    {
                        Id = 1,
                        Name = "Luca1"
                    };

                    var result = await userService.UpdateUser(updUser);

                    Assert.Empty(result.Errors);

                    var user = await context.User.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Equal(user.Id, updUser.Id);
                    Assert.Equal(user.Name, updUser.Name);
                }
            }
        }

        [Fact]
        public async void DeleteUser()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var userService = new UserService(context, _env);

                    await userService.AddUser(user1);

                    var result = await userService.DeleteUser(user1.Id);

                    Assert.Empty(result.Errors);

                    var user = await context.User.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Null(user);
                }
            }
        }

        [Fact]
        public async void GetUserById()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var userService = new UserService(context, _env);

                    var user = new User { Id = 1, Name = "Luca" };
                    var book = new Book { Id = 1, Author = "Test", ISBN = "ISBN1", Title = "My Book", Availabity = BookStatus.Available };
                    user.Books.Add(book);


                    await context.User.AddAsync(user);

                    await context.SaveChangesAsync();

                    var result = await userService.GetUserById(user1.Id);

                    Assert.Empty(result.Errors);
                    
                    var borrowedBook = result.Books.FirstOrDefault(x => x.Id == 1);

                    Assert.Equal(result.Id, user.Id);
                    Assert.Equal(result.Name, user.Name);

                    Assert.Equal(borrowedBook.Id, book.Id);
                    Assert.Equal(borrowedBook.Title, book.Title);
                    Assert.Equal(borrowedBook.ISBN, book.ISBN);
                    Assert.Equal(borrowedBook.Availabity, book.Availabity);
                    Assert.Equal(borrowedBook.Author, book.Author);

                }
            }
        }

        [Fact]
        public async void ReturnBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = new User { Id = 1, Name = "Luca" };
                    var book = new Book { Id = 1, Author = "Test", ISBN = "ISBN1", Title = "My Book", Availabity = BookStatus.Borrowed };
                    user.Books.Add(book);
                    await context.User.AddAsync(user);
                    await context.Book.AddAsync(book);
                    await context.SaveChangesAsync();

                    var userService = new UserService(context, _env);

                    var result = await userService.ReturnBook(user.Id, book.Id);

                    Assert.Empty(result.Errors);

                    var returnedBook = result.Books.FirstOrDefault(x => x.Id == 1);

                    var returnedBookAfter = await context.Book.FirstOrDefaultAsync(x => x.Id == 1);

                    Assert.Null(returnedBook);

                    Assert.Equal(user.Id, result.Id);
                    Assert.Equal(user.Name, result.Name);
                    Assert.Equal(BookStatus.Available, returnedBookAfter.Availabity);
                }
            }
        }

        [Fact]
        public async void BorrowedBook()
        {
            using (var provider = _services.BuildServiceProvider())
            {
                using (var scope = provider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var user = new User { Id = 1, Name = "Luca" };
                    await context.User.AddAsync(user);
                    await context.SaveChangesAsync();
                    var book = new Book { Id = 1, Author = "Test", ISBN = "ISBN1", Title = "My Book", Availabity = BookStatus.Available };
                    await context.Book.AddAsync(book);
                    await context.SaveChangesAsync();


                    var userService = new UserService(context, _env);

                    var result = await userService.BorrowBook(user.Id, book.Id);

                    Assert.Empty(result.Errors);

                    var borrowedBook = result.Books.FirstOrDefault(x => x.Id == 1);

                    Assert.Equal(user.Id, result.Id);
                    Assert.Equal(user.Name, result.Name);
                    Assert.Equal(BookStatus.Borrowed, borrowedBook.Availabity);
                }
            }
        }
    }
}