using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bibliotek.Avalonia.ViewModels;
using Bibliotek.Avalonia.Views;
using Bibliotek.Core.Data;
using Bibliotek.Core.Models;

namespace Bibliotek.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Auto-load on startup
        var repo = new JsonLibraryRepository();
        var library = repo.Load();

        // Seed default data if the file was empty
        if (library.Books.Count == 0)
        {
            library.AddBook(new Book("The Hobbit", "J.R.R. Tolkien", "978-0547928227"));
            library.AddBook(new Book("1984", "George Orwell", "978-0451524935"));
            library.RegisterUser(new User("Anders", "U001"));
            library.RegisterUser(new PremiumUser("Bente", "U002"));
        }

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Pass the loaded library and repository into the ViewModel
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(library, repo)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}