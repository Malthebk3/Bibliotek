namespace Bibliotek.Core.Interfaces;

public interface IUser
{
    string Name { get; }
    string UserId { get; }
    bool IsPremium { get; }

    // IReadOnlyList so the UI/Tests can read the books, but not add/remove them directly
    // The actual list is managed internally by the User class
    IReadOnlyList<IBook> BorrowedBooks { get; }
    void UpdateName(string newName);
    bool BorrowBook(IBook book);
    bool ReturnBook(IBook book);
    void DisplayBorrowedBooks();
}