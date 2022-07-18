using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using LastfmDiscordRPC.Models;
using static System.String;
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

    }

    private void UsernameTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        UsernamePlaceholder.Visibility = IsNullOrEmpty(UsernameTextBox.Text) ? Visible : Hidden;
    }

    private void APIKeyTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        APIKeyPlaceholder.Visibility = IsNullOrEmpty(APIKeyTextBox.Text) ? Visible : Hidden;
    }

    private void AppKeyTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        AppKeyPlaceholder.Visibility = IsNullOrEmpty(AppKeyTextBox.Text) ? Visible : Hidden;
    }

}