using System.IO;
using System.Text.Json;
using Bibliotek.Core.Interfaces;
using Bibliotek.Core.Models;

namespace Bibliotek.Core.Data;

public class JsonLibraryRepository : ILibraryRepository
{
    private readonly string _filePath;

    public JsonLibraryRepository(string filePath = "library_data.json")
    {
        _filePath = filePath;
    }

    public void Save(Library library)
    {
        // Create a simple object to hold our data for serialization
        var data = new { Books = library.Books, Users = library.Users };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(data, options);

        File.WriteAllText(_filePath, jsonString);
    }

    public Library Load()
    {
        var library = new Library();

        if (!File.Exists(_filePath)) return library;

        string jsonString = File.ReadAllText(_filePath);

        // Deserialize the data back into our lists
        var data = JsonSerializer.Deserialize<LibraryData>(jsonString);

        if (data != null)
        {
            foreach (var book in data.Books) library.AddBook(book);
            foreach (var user in data.Users) library.RegisterUser(user);
        }

        return library;
    }

    // A private class just for mapping the JSON structure
    private class LibraryData
    {
        public List<IBook> Books { get; set; } = new();
        public List<IUser> Users { get; set; } = new();
    }
}