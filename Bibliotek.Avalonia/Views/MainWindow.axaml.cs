using Avalonia.Controls;
using Bibliotek.Avalonia.ViewModels;

namespace Bibliotek.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Hook into the window closing event to save the library data when the application is closed
        this.Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Get the ViewModel and tell it to save the library data
        if (DataContext is MainViewModel vm)
        {
            vm.SaveData();
        }
    }
}