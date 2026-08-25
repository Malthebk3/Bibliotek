namespace Bibliotek.Core.Interfaces;

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