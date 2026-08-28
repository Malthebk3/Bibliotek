using System.Text.Json.Serialization;
using Bibliotek.Core.Interfaces;

namespace Bibliotek.Core.Models;

public class User : IUser
{
    public string Name { get; private set; }
    public string UserId { get; }
    protected readonly List<IBook> _borrowedBooks = new();
    public IReadOnlyList<IBook> BorrowedBooks => _borrowedBooks.AsReadOnly();
    public virtual bool IsPremium => false;
    public virtual int MaxBooks => 4;

    public User(string name, string userId)
    {
        Name = name;
        UserId = userId;
    }
    public void UpdateName(string newName)
    {
        Name = newName;
    }
    public virtual bool BorrowBook(IBook book)
    {
        if (!book.IsAvailable) return false; // Tjek om bogen allerede er udlånt
        
        if (_borrowedBooks.Count >= MaxBooks) return false; // Tjek om bruger allerede har nået sin låne grænse.

        book.MarkAsBorrowed();
        _borrowedBooks.Add(book);
        return true;
    }
    public bool ReturnBook(IBook book)
    {
        if (!_borrowedBooks.Contains(book)) return false;

        book.MarkAsReturned();
        _borrowedBooks.Remove(book);
        return true;
    }
    public void DisplayBorrowedBooks()
    {
        Console.WriteLine($"\n--- {Name}'s udlånte bøger ---");
        if (_borrowedBooks.Count == 0) Console.WriteLine("Ingen bøger udlånt.");
        foreach (var book in _borrowedBooks) book.DisplayInfo();
    }
}
