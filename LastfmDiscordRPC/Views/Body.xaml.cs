using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using LastfmDiscordRPC.Models;

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
        ProcessStartInfo sInfo = new ProcessStartInfo(logFilePath)
        {
            UseShellExecute = true
        };
        Process.Start(sInfo);
    }
}