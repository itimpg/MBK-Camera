using GalaSoft.MvvmLight.Messaging;

namespace Mbk.Wpf.Messages
{
    public class OpenWindowNotificationMessage : NotificationMessage
    {
        public WindowType TargetWindowType { get; private set; }

        public OpenWindowNotificationMessage(string notification, WindowType targetWindowType)
            : base(notification)
        {
            TargetWindowType = targetWindowType;
        }
    }

    public enum WindowType
    {
        Config,
        Camera,
    }
}
