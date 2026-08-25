using Bibliotek.Core.Models;
using System;
using Xunit;

namespace Bibliotek.Tests;

public class UserTests
{
    [Fact]
    public void NormalUser_CanBorrowUpTo4Books()
    {
        var user = new User("Konrad", "U001");
        for (int i = 0; i < 4; i++)
        {
            var book = new Book($"Bog {i}", "Forfatter", $"{i}");
            Assert.True(user.BorrowBook(book));
        }

        Assert.Equal(4, user.BorrowedBooks.Count);
    }
    [Fact]
    public void NormalUser_CannotBorrow5thBook()
    {
        var user = new User("Konrad", "U001");
        for (int i = 0; i < 4; i++) user.BorrowBook(new Book($"Bog {i}", "Forfatter", $"{i}"));
        
        var fifthBook = new Book("5. Bog", "A", "5");

        Assert.False(user.BorrowBook(fifthBook));
        Assert.Equal(4, user.BorrowedBooks.Count);
    }
    [Fact]
    public void PremiumUser_CanBorrowUpTo5Books()
    {
        var user = new PremiumUser("Konrad", "U001");
        for (int i = 0; i < 5; i++)
        {
            var book = new Book($"Bog {i}", "Forfatter", $"{i}");
            Assert.True(user.BorrowBook(book));
        }

        Assert.Equal(5, user.BorrowedBooks.Count);
    }
    [Fact]
    public void PremiumUser_CannotBorrow6thBook()
    {
        var user = new PremiumUser("Konrad", "U001");
        for (int i = 0; i < 5; i++) user.BorrowBook(new Book($"Bog {i}", "Forfatter", $"{i}"));
        
        var sixthBook = new Book("5. Bog", "A", "5");

        Assert.False(user.BorrowBook(sixthBook));
        Assert.Equal(5, user.BorrowedBooks.Count);
    }
}