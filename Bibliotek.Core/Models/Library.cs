using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using Bibliotek.Core.Interfaces;

namespace Bibliotek.Core.Models;

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
    public void RemoveBook(IBook book)
    {
        if (!book.IsAvailable)
        throw new InvalidOperationException("Bogen er udlånt og kan ikke slettes.");

        _books.Remove(book);
    }
    public string GetNextUserId()
    {
        int highest = 0;

        foreach (var user in _users)
        {
            // Extract the digits from IDs like "U0003" -> 3
            string digits = new string(user.UserId.Where(char.IsDigit).ToArray());
            if (int.TryParse(digits, out int number) && number > highest)
            {
                highest = number;
            }
        }

        return $"U{highest + 1:D4}"; // U0001, U0002, U0003...
    }
    public void RegisterUser(IUser user)
    {
        if (_users.Any(u => u.UserId == user.UserId))
            throw new ArgumentException("En bruger med dette ID findes allerede.");
        
        _users.Add(user);
    }
    public void RemoveUser(IUser user)
    {
        if (user.BorrowedBooks.Count > 0)
        throw new InvalidOperationException("Brugeren har udlånte bøger og kan ikke slettes.");

        _users.Remove(user);
    }
    public IUser? GetBorrower(IBook book)
    {
        return _users.FirstOrDefault(user => user.BorrowedBooks.Contains(book));
    }
    public IBook? FindBookByISBN(string isbn) => _books.FirstOrDefault(b => b.ISBN == isbn);

    // Function Pointer / Delegate
    // This allows the caller to pass in ANY logic to filter books!
    // For example, you can pass in a lambda like: b => b.Author == "Some Author"
    // The method will then return all books that match that condition.
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