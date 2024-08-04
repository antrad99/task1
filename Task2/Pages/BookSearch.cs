using Microsoft.AspNetCore.Components;
using Task2.Dtos;
using Task2.Services;
using Task2.Services.Interfaces;

namespace Task2.Pages
{
    public partial class BookSearch
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IBookService bookService { get; set; }
        private bool Loading { get; set; }
        private string Error { get; set; } = string.Empty;
        private bool ShowError { get; set; }
        private bool DisplayResults { get; set; }
        private bool NoResults { get; set; }
        private bool hasSearchInput { get; set; }
        private string inputSearch { get; set; } = string.Empty;
        private List<BookDto> books { get; set; } = [];

        protected override void OnInitialized()
        {
            DisplayResults = true;
        }

        private void addBook()
        {
            NavigationManager.NavigateTo("/edit-book/-1");
        }

        private void editBook(int id)
        {
            NavigationManager.NavigateTo("/edit-book/" + id);
        }

        private async void search()
        {
            var search = inputSearch;

            Loading = true;
            ShowError = false;
            DisplayResults = false;
            NoResults = false;
            StateHasChanged();


            var result = await bookService.SearchBook(search);

            if (result.Errors.Count == 0)
            {
                books = result.Books;
                if (books.Count == 0)
                    NoResults = true;
                else
                    DisplayResults = true;
            }
            else
            {
                Error = string.Join("; ",  result.Errors);
                ShowError = true;
            }

            Loading = false;
            StateHasChanged();
        }

        private void search_OnChange(ChangeEventArgs args)
        {
            var inputValue = args.Value.ToString();

            hasSearchInput = !string.IsNullOrEmpty(inputValue);

        }
    }
}
