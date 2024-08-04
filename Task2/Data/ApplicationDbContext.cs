using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Reflection.Emit;
using Task2.Enums;
using Task2.Models;

namespace Task2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Book> Book {  get; set; }
        public DbSet<User> User { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
          
            //Book Table
            builder.Entity<Book>().ToTable("Book");
            builder.Entity<Book>().Property(e => e.Author).IsRequired();
            builder.Entity<Book>().Property(e => e.Title).IsRequired();
            builder.Entity<Book>().Property(e => e.ISBN).IsRequired();
            builder.Entity<Book>().Property(e => e.Availabity).IsRequired();
            builder.Entity<Book>().Property(e => e.Availabity).HasDefaultValue(BookStatus.Available);
            builder.Entity<Book>().Property(e => e.Availabity)
                .HasConversion(
                             v => v.ToString(),
                            v => (BookStatus)Enum.Parse(typeof(BookStatus), v));


            //User Table
            builder.Entity<User>().ToTable("User");
            builder.Entity<User>().Property(e => e.Name).IsRequired();

            //Relation Many-To-Many between User and Book
            builder.Entity<Book>()
                    .HasMany(e => e.Users)
                    .WithMany(c => c.Books);

            builder.Entity<User>()
                .HasMany(e => e.Books)
                .WithMany(e => e.Users)
                .UsingEntity<UserBook>();


            base.OnModelCreating(builder);

        }
    }
}
