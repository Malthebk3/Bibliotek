using Bibliotek.Core.Models;
using System;
using Xunit;

namespace Bibliotek.Tests;

public class BookTests
{
    [Fact]
    public void NewBook_IsAvailableByDefault()
    {
        var book = new Book("Test bog", "Forfatter", "12345");
        Assert.True(book.IsAvailable);
    }
    [Fact]
    public void MarkAsBorrowed_ChangesAvailabilityToFalse()
    {
        var book = new Book("Test bog", "Forfatter", "12345");
        book.MarkAsBorrowed();
        Assert.False(book.IsAvailable);
    }
    [Fact]
    public void MarkAsBorrowed_WhenAlreadyBorrowed_ThrowsException()
    {
        var book = new Book("Test bog", "Forfatter", "12345");
        book.MarkAsBorrowed();

        Assert.Throws<InvalidOperationException>(() => book.MarkAsBorrowed());
    }
}