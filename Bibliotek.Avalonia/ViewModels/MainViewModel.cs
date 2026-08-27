using CommunityToolkit.Mvvm.ComponentModel;
using Bibliotek.Core.Data;
using Bibliotek.Core.Models;

namespace Bibliotek.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";

    private JsonLibraryRepository _repository;
    private Library _library;

    public MainViewModel(Library library, JsonLibraryRepository repository)
    {
        _library = library;
        _repository = repository;
    }

    public void SaveData()
    {
        // Save the library data to the JSON file when the application is closing
        _repository.Save(_library);
    }
}
