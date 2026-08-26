using Bibliotek.Core.Data;
using Bibliotek.Core.Models;
using System;
using Xunit;

namespace Bibliotek.Tests;

public class SaveTests
{
    [Fact]
    public void SaveAndLoad_PreservesData()
    {
        var repo = new JsonLibraryRepository("test_data.json");
        var originalLibrary = new Library();
        originalLibrary.AddBook(new Book("Test", "Author", "123"));
        originalLibrary.RegisterUser(new User("Test", "U123"));

        repo.Save(originalLibrary);

        var loadedLibrary = repo.Load();

        Assert.Multiple(
            () => Assert.Single(loadedLibrary.Books),
            () => Assert.Single(loadedLibrary.Users)
        );
        File.Delete("test_data.json"); // Clean up
    }
}