using Bibliotek.Core.Models;
using Bibliotek.Core.Services;

namespace Bibliotek.Tests;

public class LibraryTests
{
    [Fact]
    public void AddBook_AddsSuccessfully()
    {
        var library = new Library();
        var book = new Book("Test", "Test", "111");
        library.AddBook(book);

        Assert.Contains(book, library.Books);
    }
    [Fact]
    public void AddBook_DuplicateISBN_ThrowsException()
    {
        var library = new Library();
        library.AddBook(new Book("B1", "A1", "111"));

        // Proves ISBN uniqueness check works
        Assert.Throws<ArgumentException>(() => library.AddBook(new Book("B2", "A2", "111")));
    }
    [Fact]
    public void FindBooks_WithFuncPointer_FiltersCorrectly()
    {
        var library = new Library();
        library.AddBook(new Book("Bog1", "Forfatter1", "1"));
        library.AddBook(new Book("Bog2", "Forfatter2", "2"));
        library.AddBook(new Book("Bog3", "Forfatter1", "3"));

        // Krav #2: Using the Function Pointer (Func<IBook, bool>) to filter!
        var forfatter1Books = library.FindBooks(b => b.Author == "Forfatter1");

        Assert.Equal(2, forfatter1Books.Count());
    }
}