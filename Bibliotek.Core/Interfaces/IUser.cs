using System.Text.Json.Serialization;
using Bibliotek.Core.Models;

namespace Bibliotek.Core.Interfaces;

// This tells the Json serializer to remember if a user is standard or premium
[JsonDerivedType(typeof(User), "standard")]
[JsonDerivedType(typeof(PremiumUser), "premium")]
public interface IUser
{
    string Name { get; }
    string UserId { get; }

    // IReadOnlyList so the UI/Tests can read the books, but not add/remove them directly
    IReadOnlyList<IBook> BorrowedBooks { get; }

    bool BorrowBook(IBook book);
    bool ReturnBook(IBook book);
    void DisplayBorrowedBooks();
}