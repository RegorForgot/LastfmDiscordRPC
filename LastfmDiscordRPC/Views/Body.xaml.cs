using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LastfmDiscordRPC.Models;
using static System.Windows.Visibility;

namespace LastfmDiscordRPC.Views;

public partial class Body
{
    public Body()
    {
        InitializeComponent();
    }

    private void OutputBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        OutputBox.ScrollToEnd();
    }

    private void LogFileButton_OnClick(object sender, RoutedEventArgs e)
    {
        LogFileButton.IsEnabled = false;
        string logFilePath = $@"{SaveAppData.FolderPath}\RPClog.log";
        SaveAppData.CheckFolderExists();

        if (!File.Exists(logFilePath))
        {
            using FileStream fs = File.Create(logFilePath);
        }
        ProcessStartInfo sInfo = new ProcessStartInfo(logFilePath)
        {
            UseShellExecute = true
        };
        Process.Start(sInfo);
        LogFileButton.IsEnabled = true;
    }

    private void UsernameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UsernamePlaceholder.Visibility = IsNullOrEmpty(UsernameTextBox.Text) ? Visible : Hidden;
    }

    private void APIKeyTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        APIKeyPlaceholder.Visibility = IsNullOrEmpty(APIKeyTextBox.Text) ? Visible : Hidden;
    }

    private void AppIDTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        AppIDPlaceholder.Visibility = IsNullOrEmpty(AppIDTextBox.Text) ? Visible : Hidden;
    }

}