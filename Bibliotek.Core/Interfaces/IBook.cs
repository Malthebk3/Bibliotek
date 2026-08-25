namespace Bibliotek.Core.Interfaces;

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