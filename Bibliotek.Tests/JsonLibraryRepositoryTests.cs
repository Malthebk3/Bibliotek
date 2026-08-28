using System;
using System.IO;
using System.Linq;
using Bibliotek.Core.Data;
using Bibliotek.Core.Models;
using Xunit;

namespace Bibliotek.Tests;

public class JsonLibraryRepositoryTests
{
    // Unique file per test so they can't interfere with each other
    private static string UniqueFileName() => $"test_{Guid.NewGuid():N}.json";

    [Fact]
    public void Load_WhenFileDoesNotExist_ReturnsEmptyLibrary()
    {
        var repo = new JsonLibraryRepository(UniqueFileName());

        var library = repo.Load();

        Assert.Empty(library.Books);
        Assert.Empty(library.Users);
    }

    [Fact]
    public void SaveAndLoad_PreservesBooks()
    {
        string file = UniqueFileName();
        var repo = new JsonLibraryRepository(file);
        var original = new Library();
        original.AddBook(new Book("Dune", "Herbert", "111"));
        original.AddBook(new Book("1984", "Orwell", "222"));

        try
        {
            repo.Save(original);
            var loaded = repo.Load();

            Assert.Equal(2, loaded.Books.Count);
            var dune = loaded.FindBookByISBN("111");
            Assert.NotNull(dune);
            Assert.Equal("Dune", dune.Title);
            Assert.Equal("Herbert", dune.Author);
            Assert.True(dune.IsAvailable);
        }
        finally
        {
            File.Delete(file); // Always clean up, even if the test fails
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesBorrowedBooksAndAvailability()
    {
        string file = UniqueFileName();
        var repo = new JsonLibraryRepository(file);
        var original = new Library();
        var book = new Book("Dune", "Herbert", "111");
        var user = new User("Anders", "U0001");
        original.AddBook(book);
        original.RegisterUser(user);
        user.BorrowBook(book);

        try
        {
            repo.Save(original);
            var loaded = repo.Load();

            var loadedBook = loaded.FindBookByISBN("111");
            var loadedUser = loaded.Users.Single();

            Assert.NotNull(loadedBook);
            Assert.False(loadedBook.IsAvailable);          // State survived!
            Assert.Single(loadedUser.BorrowedBooks);
            Assert.Same(loadedBook, loadedUser.BorrowedBooks[0]); // Relational link intact!
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesPremiumUser()
    {
        string file = UniqueFileName();
        var repo = new JsonLibraryRepository(file);
        var original = new Library();
        original.RegisterUser(new PremiumUser("Bente", "U0002"));

        try
        {
            repo.Save(original);
            var loaded = repo.Load();

            var loadedUser = loaded.Users.Single();
            Assert.IsType<PremiumUser>(loadedUser); // Still a PremiumUser, not just a User!
            Assert.True(loadedUser.IsPremium);
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Fact]
    public void SaveAndLoad_PreservesEditedNames()
    {
        string file = UniqueFileName();
        var repo = new JsonLibraryRepository(file);
        var original = new Library();
        var user = new User("Old Name", "U0001");
        original.RegisterUser(user);
        user.UpdateName("New Name");

        try
        {
            repo.Save(original);
            var loaded = repo.Load();

            Assert.Equal("New Name", loaded.Users.Single().Name);
        }
        finally
        {
            File.Delete(file);
        }
    }
}