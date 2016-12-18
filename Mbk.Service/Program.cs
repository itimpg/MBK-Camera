using System.Globalization;
using System.ServiceProcess;
using System.Threading;

namespace Mbk.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MbkCameraService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
