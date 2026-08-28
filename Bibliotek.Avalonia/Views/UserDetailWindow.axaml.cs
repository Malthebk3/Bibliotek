using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Bibliotek.Avalonia.ViewModels;

namespace Bibliotek.Avalonia.Views;

public partial class UserDetailWindow : Window
{
    public UserDetailWindow()
    {
        InitializeComponent();

        // Hook into the window closing event to refresh main window tables
        this.Closing += UserDetailWindow_Closing;
    }

    private void UserDetailWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        // Implement smth that refreshes tables in mainwindow on this.close
        
    }
}