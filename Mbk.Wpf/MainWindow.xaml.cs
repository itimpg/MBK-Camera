using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Mbk.Wpf.Messages;
using Mbk.Wpf.Pages;
using System.Windows;

namespace Mbk.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<OpenWindowNotificationMessage>(this, ReplyToOpenWindowMessage);
        }
        private void ReplyToOpenWindowMessage(OpenWindowNotificationMessage msg)
        {
            Window window = null;
            switch (msg.TargetWindowType)
            {
                case WindowType.Config:
                    window = new ConfigView();
                    break;
                case WindowType.Camera:
                    window = new CameraView();
                    break;
            }
            if(window != null)
            {
                window.Owner = this;
                window.ShowDialog();
            }
        }
    }
}
