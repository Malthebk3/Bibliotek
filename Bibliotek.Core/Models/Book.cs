using Bibliotek.Core.Interfaces;

namespace Bibliotek.Core.Models;

public class Book : IBook
{
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; }

    // Encapsulation: Can only be changed internally via MarkAsBorrowed/Returned
    public bool IsAvailable { get; private set; }

    public Book(string title, string author, string isbn)
    {
        Title = title;
        Author = author;
        ISBN = isbn;
        IsAvailable = true; // New books are always available
    }
    public void UpdateInfo(string newTitle, string newAuthor)
    {
        Title = newTitle;
        Author = newAuthor;
    }
    public void MarkAsBorrowed()
    {
        if (!IsAvailable) throw new InvalidOperationException("Bogen er allerede udlånt.");
        IsAvailable = false;
    }
    public void MarkAsReturned()
    {
        if (IsAvailable) throw new InvalidOperationException("Bogen er allerede afleveret.");
        IsAvailable = true;
    }
    public void DisplayInfo()
    {
        Console.WriteLine($"[{ISBN}] {Title} af {Author} - {(IsAvailable ? "Tilgængelig" : "Udlånt")}");
    }
}
