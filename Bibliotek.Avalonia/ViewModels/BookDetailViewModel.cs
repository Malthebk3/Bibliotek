using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bibliotek.Core.Interfaces;
using Bibliotek.Core.Models;

namespace Bibliotek.Avalonia.ViewModels;

public partial class BookDetailViewModel : ViewModelBase
{
    private readonly IBook _book;
    private readonly Library _library;
    private readonly Action _onClose;

    [ObservableProperty] private string _editTitle;
    [ObservableProperty] private string _editAuthor;
    [ObservableProperty] private string? _borrowerInfo;
    [ObservableProperty] private string _statusMessage = "";

    public BookDetailViewModel(IBook book, Library library, Action onClose)
    {
        _book = book;
        _library = library;
        _onClose = onClose;

        _editTitle = book.Title;
        _editAuthor = book.Author;

        RefreshBorrowerInfo();
    }

    private void RefreshBorrowerInfo()
    {
        var borrower = _library.GetBorrower(_book);
        BorrowerInfo = borrower != null
            ? $"UDLÅNT TIL: {borrower.Name} (ID: {borrower.UserId})"
            : "Bogen er tilgængelig i biblioteket.";
    }

    [RelayCommand]
    private void SaveChanges()
    {
        if (string.IsNullOrWhiteSpace(EditTitle) || string.IsNullOrWhiteSpace(EditAuthor))
        {
            StatusMessage = "Fejl: Titel og forfatter må ikke være tomme.";
            return;
        }

        _book.UpdateInfo(EditTitle, EditAuthor);
        StatusMessage = "Ændringer gemt!";
    }
    [RelayCommand]
    private void DeleteBook()
    {
        try
        {
            _library.RemoveBook(_book);
            _onClose();
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = $"Fejl: {ex.Message}";
        }
    }
}