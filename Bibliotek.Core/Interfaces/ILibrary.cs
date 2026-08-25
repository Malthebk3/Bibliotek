namespace Bibliotek.Core.Interfaces;

public interface ILibrary
{
    IReadOnlyList<IBook> Books { get; }
    IReadOnlyList<IUser> Users { get; }

    void AddBook(IBook book);
    void RemoveBook(IBook book);
    void RegisterUser(IUser user);
    
    void DisplayAllBooks();
    void DisplayAvailableBooks();
    IBook? FindBookByISBN(string isbn);
    
    // TEACHER REQ #2: Function Pointer / Delegate for advanced searching
    IEnumerable<IBook> FindBooks(Func<IBook, bool> predicate);
}