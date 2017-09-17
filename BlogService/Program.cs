using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace BlogService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

#if DEBUG
            try
            {
                StartTopshelf();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


#elif !DEBUG
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new BlogService()
            };
            ServiceBase.Run(ServicesToRun);

#endif



        }

        private static void StartTopshelf()
        {
            HostFactory.Run(x =>
            {
                x.Service<BlogService>(s =>
                {
                    s.ConstructUsing(name => new BlogService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Blog Service host");
                x.SetDisplayName("Blog Service");
                x.SetServiceName("Blog Service");
            });
        }
    }
}
