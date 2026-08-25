using System.Runtime.CompilerServices;
using Bibliotek.Core.Interfaces;

namespace Bibliotek.Core.Services;

public class Library : ILibrary
{
    private readonly List<IBook> _books = new();
    private readonly List<IUser> _users = new();

    public IReadOnlyList<IBook> Books => _books.AsReadOnly();
    public IReadOnlyList<IUser> Users => _users.AsReadOnly();

    public void AddBook(IBook book)
    {
        if (FindBookByISBN(book.ISBN) != null)
            throw new ArgumentException("En bog med dette ISBN findes allerede.");
        
        _books.Add(book);
    }
    public void RemoveBook(IBook book) => _books.Remove(book);

    public void RegisterUser(IUser user)
    {
        if (_users.Any(u => u.UserId == user.UserId))
            throw new ArgumentException("En bruger med dette ID findes allerede.");
        
        _users.Add(user);
    }

    public IBook? FindBookByISBN(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

    // Krav #2: Function Pointer / Delegate
    // This allows the caller to pass in ANY logic to filter books!
    public IEnumerable<IBook> FindBooks(Func<IBook, bool> predicate) => _books.Where(predicate);

    public void DisplayAllBooks()
    {
        Console.WriteLine("\n--- Alle bøger i bibliotek ---");
        foreach (var book in _books) book.DisplayInfo();
    }

    public void DisplayAvailableBooks()
    {
        Console.WriteLine("\n--- Tilgængelige bøger ---");
        // Using function pointer method
        var available = FindBooks(b => b.IsAvailable);
        foreach (var book in available) book.DisplayInfo();
    }
}