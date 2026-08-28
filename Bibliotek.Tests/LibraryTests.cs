using Bibliotek.Core.Models;
using System;
using Xunit;

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
    [Fact]
    public void GetNextUserId_EmptyLibrary_ReturnsU0001()
    {
        var library = new Library();

        Assert.Equal("U0001", library.GetNextUserId());
    }

    [Fact]
    public void GetNextUserId_WithExistingUsers_ReturnsNextNumber()
    {
        var library = new Library();
        library.RegisterUser(new User("A", "U0001"));
        library.RegisterUser(new User("B", "U0002"));

        Assert.Equal("U0003", library.GetNextUserId());
    }

    [Fact]
    public void GetNextUserId_HandlesOldIdFormat()
    {
        var library = new Library();
        library.RegisterUser(new User("A", "U001")); // seed format with 3 digits

        Assert.Equal("U0002", library.GetNextUserId());
    }

    [Fact]
    public void GetBorrower_ReturnsUser_WhenBookIsBorrowed()
    {
        var library = new Library();
        var book = new Book("Dune", "Herbert", "1");
        var user = new User("Anders", "U0001");
        library.AddBook(book);
        library.RegisterUser(user);
        user.BorrowBook(book);

        var borrower = library.GetBorrower(book);

        Assert.Same(user, borrower); // Same object reference!
    }

    [Fact]
    public void GetBorrower_ReturnsNull_WhenBookIsAvailable()
    {
        var library = new Library();
        var book = new Book("Dune", "Herbert", "1");
        library.AddBook(book);

        Assert.Null(library.GetBorrower(book));
    }

    [Fact]
    public void RemoveBook_RemovesBookFromLibrary()
    {
        var library = new Library();
        var book = new Book("Dune", "Herbert", "1");
        library.AddBook(book);

        library.RemoveBook(book);

        Assert.DoesNotContain(book, library.Books);
    }

    [Fact]
    public void RemoveUser_RemovesUserFromLibrary()
    {
        var library = new Library();
        var user = new User("Anders", "U0001");
        library.RegisterUser(user);

        library.RemoveUser(user);

        Assert.DoesNotContain(user, library.Users);
    }
    [Fact]
    public void RemoveBook_BorrowedBook_ThrowsException()
    {
        var library = new Library();
        var book = new Book("Dune", "Herbert", "1");
        var user = new User("Anders", "U0001");
        library.AddBook(book);
        library.RegisterUser(user);
        user.BorrowBook(book);

        Assert.Throws<InvalidOperationException>(() => library.RemoveBook(book));
        Assert.Contains(book, library.Books);
    }

    [Fact]
    public void RemoveUser_UserWithLoans_ThrowsException()
    {
        var library = new Library();
        var book = new Book("Dune", "Herbert", "1");
        var user = new User("Anders", "U0001");
        library.AddBook(book);
        library.RegisterUser(user);
        user.BorrowBook(book);

        Assert.Throws<InvalidOperationException>(() => library.RemoveUser(user));
        Assert.Contains(user, library.Users);
    }
}