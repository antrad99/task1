using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using Task2.Services.Interfaces;
using Task2.Dtos;
using Task2.Services;
using Azure;

namespace Task2.Pages
{
    public partial class EditBook
    {
        [Parameter]
        public string id { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        public bool Loading { get; set; }
        private string Error { get; set; } = string.Empty;
        private bool ShowError { get; set; }
        private bool ShowMessage { get; set; }
        private bool DataUpdated { get; set; }
        private string Message { get; set; }
        private BookDto bookDto { get; set; } = new BookDto();
        private bool IsNew { get; set; } = false;
        private bool ShowErrorAuthor { get; set; } = false;
        private bool ShowErrorTitle { get; set; } = false;
        private bool ShowErrorISBN { get; set; } = false;

        [Inject]
        private IBookService bookService { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await Task.Run(() => Init());
        }

        private void Init()
        {
            Loading = true;
            ShowError = false;
            ShowMessage = false;

            int _id;
            int.TryParse(id, out _id);
            if (_id == -1)
                IsNew = true;
        }

        protected async override void OnAfterRender(bool firstRender)
        {

            if (firstRender)
            {
                int _id;
                if (!int.TryParse(id, out _id))
                    _id = -1;

                if (_id != -1)
                {
                    Loading = true;
                    ShowMessage = false;
                    ShowError = false;
                    StateHasChanged();

                    var result = await bookService.GetBookById(_id);

                    if (result.Errors.Count > 0)
                    {
                        Message = string.Join("; ", result.Errors);
                        Loading = false;
                        ShowMessage = true;
                        StateHasChanged();
                        return;
                    }
                    else
                    {
                        bookDto = result;
                        ShowMessage = false;
                        Loading = false;
                        StateHasChanged();
                        return;
                    }
                }
                else
                {
                    //DO NOTHING --> ADD NEW BOOK
                    Loading = false;
                    ShowError = false;
                    ShowMessage = false;
                    StateHasChanged();

                }
            }
        }

        private async void onAddBook()
        {
            ValidateInput();

            Loading = true;
            ShowError = false;
            ShowMessage = false;
            StateHasChanged();

            var response = await bookService.AddBook(bookDto);


            if (response.Errors.Count > 0)
            {
                Error = string.Join("; ", response.Errors);
                Loading = false;
                ShowError = true;
                StateHasChanged();
                return;
            }

            Message = "Book has been added successfully.";
            ShowMessage = true;
            Loading = false;
            ShowError = false;
            StateHasChanged();
        }

        private async void onUpdateBook()
        {

            ValidateInput();

            Loading = true;
            ShowError = false;
            ShowMessage = false;
            StateHasChanged();

            var response = await bookService.UpdateBook(bookDto);


            if (response.Errors.Count > 0)
            {
                Error = string.Join("; ", response.Errors);
                Loading = false;
                ShowError = true;
                StateHasChanged();
                return;
            }

            Message = "Book has been updated successfully.";
            ShowMessage = true;
            Loading = false;
            ShowError = false;
            StateHasChanged();
        }

        private void ResetErrorMessages()
        {
            ShowErrorAuthor = false;
            ShowErrorTitle = false;
            ShowErrorISBN = false;
        }

        private Task onInput_Author(ChangeEventArgs args)
        {
            bookDto.Author = args.Value.ToString();
            if (string.IsNullOrEmpty(bookDto.Author))
                ShowErrorAuthor = true;
            else
                ShowErrorAuthor = false;

            return Task.CompletedTask;
        }

        private Task onInput_Title(ChangeEventArgs args)
        {
            bookDto.Title = args.Value.ToString();
            if (string.IsNullOrEmpty(bookDto.Title))
                ShowErrorTitle = true;
            else
                ShowErrorTitle = false;

            return Task.CompletedTask;
        }

        private Task onInput_ISBN(ChangeEventArgs args)
        {
            bookDto.ISBN = args.Value.ToString();
            if (string.IsNullOrEmpty(bookDto.ISBN))
                ShowErrorISBN = true;
            else
                ShowErrorISBN = false;

            return Task.CompletedTask;
        }

        private void ValidateInput()
        {
            ResetErrorMessages();

            if (string.IsNullOrEmpty(bookDto.Author))
                ShowErrorAuthor = true;
            else
                ShowErrorAuthor = false;

            if (string.IsNullOrEmpty(bookDto.Title))
                ShowErrorTitle = true;
            else
                ShowErrorTitle = false;

            if (string.IsNullOrEmpty(bookDto.ISBN))
                ShowErrorISBN = true;
            else
                ShowErrorISBN = false;

        }
    }
}
