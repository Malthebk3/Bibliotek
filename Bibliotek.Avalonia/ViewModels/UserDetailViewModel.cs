using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bibliotek.Core.Interfaces;
using Bibliotek.Core.Models;

namespace Bibliotek.Avalonia.ViewModels;

public partial class UserDetailViewModel : ViewModelBase
{
    private readonly Library _library;
    private readonly Action _onClose;

    [ObservableProperty] private string _editName;
    [ObservableProperty] private IUser _selectedUser;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private IBook? _selectedBorrowedBook;
    [ObservableProperty] private IBook? _selectedAvailableBook; 
    [ObservableProperty] private string _statusMessage = "Klar.";

    // We use an ObservableCollection so the UI updates instantly when a book is returned
    public ObservableCollection<IBook> BorrowedBooks { get; } = new();
    public ObservableCollection<IBook> AvailableBooks { get; } = new();

    public UserDetailViewModel(IUser user, Library library, Action onClose)
    {
        _selectedUser = user;
        _library = library;
        _onClose = onClose;

        _editName = user.Name;
        // Initialize the lists
        foreach (var book in user.BorrowedBooks) BorrowedBooks.Add(book);
        RefreshAvailableBooks();
    }
    [RelayCommand]
    private void SaveChanges()
    {
        if (string.IsNullOrWhiteSpace(EditName))
        {
            StatusMessage = "Fejl: Navn må ikke være tomt.";
            return;
        }

        SelectedUser.UpdateName(EditName);
        StatusMessage = "Ændringer gemt!";
    }

    [RelayCommand]
    private void DeleteUser()
    {
        try
        {
            _library.RemoveUser(SelectedUser);
            _onClose();
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = $"Fejl: {ex.Message}";
        }
    }

    // This method is automatically called by CommunityToolkit whenever SearchQuery changes!
    partial void OnSearchQueryChanged(string value)
    {
        RefreshAvailableBooks();
    }

    private void RefreshAvailableBooks()
    {
        AvailableBooks.Clear();

        // TEACHER REQ #2: Function Pointer (Func<IBook, bool>) in action!
        // We filter for books that are available AND match the search query (if any)
        var filteredBooks = _library.FindBooks(b => 
            b.IsAvailable && 
            (string.IsNullOrWhiteSpace(SearchQuery) || b.Title.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) || b.Author.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) || b.ISBN.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
        );

        foreach (var book in filteredBooks)
        {
            AvailableBooks.Add(book);
        }
    }

    [RelayCommand]
    private void ReturnBook()
    {
        if (SelectedBorrowedBook == null)
        {
            StatusMessage = "Vælg venligst en bog at returnere.";
            return;
        }

        // The Core logic handles the encapsulation and state change
        bool success = SelectedUser.ReturnBook(SelectedBorrowedBook);

        if (success)
        {
            StatusMessage = $"Returnerede: {SelectedBorrowedBook.Title}";
            BorrowedBooks.Remove(SelectedBorrowedBook);
            SelectedBorrowedBook = null; // Clear selection
            RefreshAvailableBooks(); // The book is now available again!
        }
        else
        {
            StatusMessage = "Kunne ikke returnere bogen.";
        }
    }
    [RelayCommand]
    private void BorrowBook()
    {
        if (SelectedAvailableBook == null)
        {
            StatusMessage = "Vælg venligst en bog at låne.";
            return;
        }

        // The Core logic handles the encapsulation and state change
        bool success = SelectedUser.BorrowBook(SelectedAvailableBook);

        if (success)
        {
            StatusMessage = $"Lånte: {SelectedAvailableBook.Title}";
            
            // Move the book from Available to Borrowed in the UI
            BorrowedBooks.Add(SelectedAvailableBook);
            AvailableBooks.Remove(SelectedAvailableBook);
            
            SelectedAvailableBook = null; // Clear selection
        }
        else
        {
            // This catches edge cases (like if a Premium user somehow hits their 5 book limit)
            StatusMessage = "Kunne ikke låne bogen (maksimum nået?).";
        }
    }
}