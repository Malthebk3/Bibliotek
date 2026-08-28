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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var repo = new JsonLibraryRepository("library_data.json");
            var library = repo.Load();

            SeedIfEmpty(library);

            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel(library, repo)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void SeedIfEmpty(Library library)
    {
        if (library.Books.Count > 0 || library.Users.Count > 0) return;

        // Books
        var hobbit  = new Book("The Hobbit", "J.R.R. Tolkien", "978-0547928227");
        var dune    = new Book("Dune", "Frank Herbert", "978-0441172719");
        var b1984   = new Book("1984", "George Orwell", "978-0451524935");
        var hp      = new Book("Harry Potter og De Vises Sten", "J.K. Rowling", "978-8770311786");
        var smilla  = new Book("Frøken Smillas fornemmelse for sne", "Peter Høeg", "978-8702034851");
        var prins   = new Book("Den lille prins", "Antoine de Saint-Exupéry", "978-8702232158");

        library.AddBook(hobbit);
        library.AddBook(dune);
        library.AddBook(b1984);
        library.AddBook(hp);
        library.AddBook(smilla);
        library.AddBook(prins);

        // Users: one Premium, two standard
        var anders    = new User("Anders Jensen", "U0001");
        var bente     = new PremiumUser("Bente Nielsen", "U0002");
        var christian = new User("Christian Poulsen", "U0003");

        library.RegisterUser(anders);
        library.RegisterUser(bente);
        library.RegisterUser(christian);

        // Pre-borrow so the UI shows state on first launch
        anders.BorrowBook(dune);
        bente.BorrowBook(hp);
    }
}