using System.ServiceProcess;
 
namespace BrainpackService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BrainpackService(args)
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
