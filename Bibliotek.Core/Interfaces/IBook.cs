using System.Text.Json.Serialization;
using Bibliotek.Core.Models;

namespace Bibliotek.Core.Interfaces;

[JsonDerivedType(typeof(Book), "standard")]
public interface IBook
{
    public string Title { get; }
    public string Author { get; }
    public string ISBN { get; }
    public bool IsAvailable { get; }


    void MarkAsBorrowed();
    void MarkAsReturned();

    void DisplayInfo();
}