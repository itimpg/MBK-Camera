using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using Mbk.Wpf.Messages;

namespace Mbk.Wpf.Pages
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : MetroWindow
    {
        public CameraView()
        {
            InitializeComponent();
            Messenger.Default.Register<CloseWindowNotificationMessage>(this, ReplyToMessage);
        }
        private void ReplyToMessage(CloseWindowNotificationMessage msg)
        {
            if (msg.TargetWindowType == WindowType.Camera)
            {
                Close();
            }
        }
    }
}
