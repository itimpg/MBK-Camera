using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mbk.Wpf.Messages
{
    public class DataChangedNotificationMessage : NotificationMessage
    {
        public DataChangedType DataChanged { get; private set; }

        public DataChangedNotificationMessage(string notification, DataChangedType dataChanged)
            : base(notification)
        {
            DataChanged = dataChanged;
        }
    }
    
    public enum DataChangedType
    {
        Config,
        Camera,
    }
}
