namespace Mbk.Wpf.Models
{
    public class AppSessionModel
    {
        public int CameraId { get; set; }

        private AppSessionModel() { }

        private static AppSessionModel _session;
        public static AppSessionModel Instance()
        {
            return _session ?? (_session = new AppSessionModel());
        }
    }
}
