using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LastfmDiscordRPC2.Views;

public partial class DialogWindow : Window
{
    private static bool IsCurrentOn { get; set; }

    public DialogWindow()
    {
        InitializeComponent();
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };
        Background = OperatingSystem.CurrentOS == DataTypes.OperatingSystem.Windows ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Black);


        WindowStateProperty.Changed.AddClassHandler<Window>((window, args) =>
        {
            if (window.PlatformImpl == null || (WindowState)(args.NewValue ?? WindowState.Maximized) != WindowState.Minimized)
            {
                return;
            }
            Close();
            IsCurrentOn = false;
        });

        IsVisibleProperty.Changed.AddClassHandler<Window>((window, args) =>
        {
            if (window.PlatformImpl != null && (bool)args.NewValue)
            {
                WindowState = WindowState.Normal;
            }
        });

        Closing += (_, _) => IsCurrentOn = false;
    }

    public new static void Show(Window? parent)
    {
        if (IsCurrentOn)
        {
            return;
        }
        IsCurrentOn = true;
        DialogWindow dialogWindow = new DialogWindow();
        Button? btn = dialogWindow.FindControl<Button>("Ok");

        btn.Click += (_, _) =>
        {
            dialogWindow.Close();
        };

        dialogWindow.Show();
    }
}