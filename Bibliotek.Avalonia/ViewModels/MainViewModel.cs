using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Bibliotek.Avalonia.Views;
using Bibliotek.Core.Interfaces;
using Bibliotek.Core.Models;
using System.Linq;
using System;

namespace Bibliotek.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILibraryRepository _repository;

    // We keep a reference to the Core Library to perform actions
    public Library Library { get; }

    // ObservableCollections automatically update the UI when items are added/removed
    public ObservableCollection<IBook> Books { get; } = new();
    public ObservableCollection<IUser> Users { get; } = new();

    // --- Properties for the "Add Book" Form ---
    [ObservableProperty] private string newBookTitle = string.Empty;
    [ObservableProperty] private string newBookAuthor = string.Empty;
    [ObservableProperty] private string newBookIsbn = string.Empty;

    // --- Properties for the "Add User" Form ---
    [ObservableProperty] private string newUserName = string.Empty;
    [ObservableProperty] private bool isNewUserPremium = false;

    // --- Properties for UI Feedback and Selection ---
    [ObservableProperty] private string statusMessage = "Klar.";
    
    // This will be updated when a user clicks a row in the Users DataGrid
    [ObservableProperty] private IUser? selectedUser; 
    [ObservableProperty] private IBook? selectedBook;

    public MainViewModel(Library library, ILibraryRepository repository)
    {
        Library = library;
        _repository = repository;

        // Sync the Core lists with our UI collections
        foreach (var book in library.Books) Books.Add(book);
        foreach (var user in library.Users) Users.Add(user);
    }
    // --- Commands ---

    [RelayCommand]
    private void AddBook()
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(NewBookTitle) || string.IsNullOrWhiteSpace(NewBookIsbn))
        {
            StatusMessage = "Fejl: Titel og ISBN er påkrævet.";
            return;
        }

        try
        {
            var book = new Book(NewBookTitle, NewBookAuthor, NewBookIsbn);
            Library.AddBook(book); // Adds to Core
            Books.Add(book);       // Updates UI
            
            StatusMessage = $"Tilføjet: {NewBookTitle}";
            
            // Clear the form
            NewBookTitle = string.Empty;
            NewBookAuthor = string.Empty;
            NewBookIsbn = string.Empty;
        }
        catch (ArgumentException ex)
        {
            // Catches duplicate ISBNs
            StatusMessage = $"Fejl: {ex.Message}";
        }
    }

    [RelayCommand]
    private void AddUser()
    {
        if (string.IsNullOrWhiteSpace(NewUserName))
        {
            StatusMessage = "Fejl: Navn er påkrævet.";
            return;
        }

        try
        {
            string nextId = Library.GetNextUserId(); // Get the next available user ID
            // Polymorphism in action: Create the correct type based on the checkbox
            IUser user = IsNewUserPremium 
                ? new PremiumUser(NewUserName, nextId) 
                : new User(NewUserName, nextId);
                
            Library.RegisterUser(user);
            Users.Add(user);

            StatusMessage = $"Registreret: {NewUserName} (ID: {nextId})";
            
            // Clear the form
            NewUserName = string.Empty;
        }
        catch (ArgumentException ex)
        {
            StatusMessage = $"Fejl: {ex.Message}";
        }
    }
    [RelayCommand]
    private void OpenUserDetails()
    {
        if (SelectedUser == null)
        {
            StatusMessage = "Vælg venligst en bruger i tabellen først.";
            return;
        }

        StatusMessage = $"Åbner detaljer for {SelectedUser.Name}...";

        var user = SelectedUser;

        // Create the new window and pass it the specific user, the main library, and a callback to close the window
        var detailWindow = new UserDetailWindow();
        
        detailWindow.DataContext = new UserDetailViewModel(
            user, 
            Library, 
            () => detailWindow.Close()
        );

        detailWindow.Closed += (s, e) =>
        {
            RefreshTables();
            // If the user was deleted, SelectedUser will be null, so we check if the user still exists in the library
            StatusMessage = Library.Users.Contains(user)
                ? $"Lukkede detaljer for {user.Name}."
                : $"Brugeren {user.Name} blev slettet.";
        };

        detailWindow.Show();
    }
    [RelayCommand]
    private void OpenBookDetails()
    {
        if (SelectedBook == null)
        {
            StatusMessage = "Vælg venligst en bog i tabellen først.";
            return;
        }

        StatusMessage = $"Åbner detaljer for {SelectedBook.Title}...";

        var book = SelectedBook;

        // Create the new window and pass it the specific book, the main library, and a callback to close the window
        var detailWindow = new BookDetailWindow();
        
        detailWindow.DataContext = new BookDetailViewModel(book, Library, () => detailWindow.Close());


        // Refresh tables when the window closes (after the user has saved or deleted the book)
        detailWindow.Closed += (s, e) =>
        {
            RefreshTables();
            StatusMessage = Library.Books.Contains(book)
                ? $"Lukkede detaljer for {book.Title}."
                : $"Bogen {book.Title} blev slettet.";
        };

        // Show the window (non-blocking, so the main window stays open)
        detailWindow.Show();
    }

    // Refreshes both tables from the Core Library
    public void RefreshTables()
    {
        Books.Clear();
        foreach (var book in Library.Books) Books.Add(book);

        Users.Clear();
        foreach (var user in Library.Users) Users.Add(user);
    }

    // --- Utility ---

    // This method will be called automatically when the window closes
    public void SaveData()
    {
        // Save the library data to the JSON file when the application is closing
        _repository.Save(Library);
    }
}
