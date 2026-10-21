using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Piew.Desktop.App.ViewModels;
using Piew.Desktop.Core.Models;

namespace Piew.Desktop.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        _viewModel.ThemeChangeRequested += ApplyTheme;
        ApplyTheme(_viewModel.CurrentTheme);
    }

    private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            DragMove();
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // Minimize ke System Tray atau sembunyikan jendela
        Hide();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized 
            ? WindowState.Normal 
            : WindowState.Maximized;
    }

    public void ApplyTheme(ThemeType theme)
    {
        var themeDictUri = theme switch
        {
            ThemeType.WarmCream => "Themes/WarmCream.xaml",
            ThemeType.DarkObsidian => "Themes/DarkObsidian.xaml",
            _ => "Themes/PureSnow.xaml"
        };

        try
        {
            var dict = new ResourceDictionary
            {
                Source = new Uri(themeDictUri, UriKind.RelativeOrAbsolute)
            };

            // Hapus kamus tema lama dan masukkan yang baru
            var merged = Application.Current.Resources.MergedDictionaries;
            var existingTheme = merged.FirstOrDefault(d => 
                d.Source != null && (
                    d.Source.OriginalString.Contains("PureSnow") ||
                    d.Source.OriginalString.Contains("WarmCream") ||
                    d.Source.OriginalString.Contains("DarkObsidian")
                ));

            if (existingTheme != null)
            {
                merged.Remove(existingTheme);
            }

            merged.Insert(0, dict);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MainWindow] Gagal menerapkan tema: {ex.Message}");
        }
    }
}
