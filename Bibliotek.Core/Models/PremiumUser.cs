using Bibliotek.Core.Interfaces;

namespace Bibliotek.Core.Models;

public class PremiumUser : User
{
    protected override int MaxBooks => 5;
    public PremiumUser(string name, string userId) : base(name, userId)
    {

    }
    public override bool BorrowBook(IBook book)
    {
        if (!book.IsAvailable) return false; // Tjek om bogen allerede er udlånt
        
        if (_borrowedBooks.Count >= MaxBooks) return false; // Tjek om bruger allerede har nået sin låne grænse.

        book.MarkAsBorrowed();
        _borrowedBooks.Add(book);
        return true;
    }
}