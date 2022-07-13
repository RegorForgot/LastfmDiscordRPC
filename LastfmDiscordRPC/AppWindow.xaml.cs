using System;
using System.Drawing;
using System.Resources;
using System.Windows;

namespace LastfmDiscordRPC
{

    /// <summary>
    /// Interaction logic for AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {
        public AppWindow()
        {
            InitializeComponent();
            ResourceManager resourceManager = new ResourceManager(typeof(Resources));
            
            System.Windows.Forms.NotifyIcon icon = new System.Windows.Forms.NotifyIcon();
            icon.Icon = (Icon) resourceManager.GetObject("Icon")!;
            icon.Visible = true;
            icon.Click +=
                delegate
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };  
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }
    }
}