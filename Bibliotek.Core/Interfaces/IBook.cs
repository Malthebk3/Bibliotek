namespace Bibliotek.Core.Interfaces;

public interface IBook
{
    public string Title { get; }
    public string Author { get; }
    public string ISBN { get; }
    public bool IsAvailable { get; }

    void UpdateInfo(string newTitle, string newAuthor);
    void MarkAsBorrowed();
    void MarkAsReturned();
    void DisplayInfo();
}