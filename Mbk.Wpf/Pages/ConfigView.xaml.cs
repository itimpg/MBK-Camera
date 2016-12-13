using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Mbk.Wpf.Messages;
using System.Windows;

namespace Mbk.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for ConfigView.xaml
    /// </summary>
    public partial class ConfigView : MetroWindow
    {
        public ConfigView()
        {
            InitializeComponent();
            Messenger.Default.Register<CloseWindowNotificationMessage>(this, ReplyToCloseWindowMessage);
        }
        private void ReplyToCloseWindowMessage(CloseWindowNotificationMessage msg)
        {
            if (msg.TargetWindowType == WindowType.Config)
            {
                Close();
            }
        }
    }
}
