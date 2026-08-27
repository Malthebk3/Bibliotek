using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bibliotek.Avalonia.ViewModels;
using Bibliotek.Avalonia.Views;
using Bibliotek.Core.Data;

namespace Bibliotek.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var repo = new JsonLibraryRepository("library_data.json");
        var library = repo.Load(); // Auto-load on startup

        var mainViewModel = new MainViewModel(library, repo);
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}