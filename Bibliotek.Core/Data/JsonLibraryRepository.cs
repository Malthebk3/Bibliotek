using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bibliotek.Core.Interfaces;
using Bibliotek.Core.Models;

namespace Bibliotek.Core.Data;

public class JsonLibraryRepository : ILibraryRepository
{
    private readonly string _filePath;

    public JsonLibraryRepository(string fileName = "library_data.json")
    {
        //_filePath = filePath;
        string directory = AppDomain.CurrentDomain.BaseDirectory;
        _filePath = Path.Combine(directory, fileName);
    }

    public void Save(Library library)
    {
        var saveData = new LibrarySaveData
        {
            Books = library.Books.Cast<Book>().ToList(),
            Users = library.Users.Select(user => new UserSaveData
            {
                Name = user.Name,
                UserId = user.UserId,
                IsPremium = user.IsPremium,
                BorrowedBookIsbns = user.BorrowedBooks.Select(book => book.ISBN).ToList()
            }).ToList()
        };
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(saveData, options);
        File.WriteAllText(_filePath, jsonString);

        // // Create a simple object to hold our data for serialization
        // var data = new { Books = library.Books, Users = library.Users };

        // var options = new JsonSerializerOptions
        // {
        //     WriteIndented = true,
        //     // The line below saves $id and $ref so it know it's the same object
        //     ReferenceHandler = ReferenceHandler.Preserve
        // };
        // string jsonString = JsonSerializer.Serialize(data, options);

        // File.WriteAllText(_filePath, jsonString);
    }

    public Library Load()
    {
        var library = new Library();

        if (!File.Exists(_filePath)) return library;

        string jsonString = File.ReadAllText(_filePath);
        var saveData = JsonSerializer.Deserialize<LibrarySaveData>(jsonString);

        if (saveData == null) return library;

        foreach (var book in saveData.Books) library.AddBook(book);
        foreach (var userData in saveData.Users)
        {
            IUser user = userData.IsPremium
                ? new PremiumUser(userData.Name, userData.UserId)
                : new User(userData.Name, userData.UserId);

            library.RegisterUser(user);

            foreach (var isbn in userData.BorrowedBookIsbns)
            {
                var book = library.FindBookByISBN(isbn);
                if (book != null)
                {
                    user.BorrowBook(book);
                }
            }
        }

        return library;
    }

    // A private class just for mapping the JSON structure
    private class LibraryData
    {
        public List<IBook> Books { get; set; } = new();
        public List<IUser> Users { get; set; } = new();
    }
    private class LibrarySaveData
    {
        public List<Book> Books { get; set; } = new();
        public List<UserSaveData> Users { get; set; } = new();
    }

    private class UserSaveData
    {
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool IsPremium { get; set; }
        public List<string> BorrowedBookIsbns { get; set; } = new();
    }
}