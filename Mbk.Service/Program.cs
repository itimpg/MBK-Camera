using System.ServiceProcess;

namespace Mbk.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new MbkCameraService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
