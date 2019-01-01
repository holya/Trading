using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trading.Brokers.Fxcm;
using Trading.DataManager;
using Trading.DataManager.Common;
using Trading.DataProviders.Common;
using Unity;

namespace WindowsFormsApp
{
    static class Program
    {
        //public static Lazy<IUnityContainer> UnityContainer = new Lazy<IUnityContainer>(setupDependencyInjection);
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Lazy<IUnityContainer> uContainer = new Lazy<IUnityContainer>(setupDependencyInjection);
            var uContainer = setupDependencyInjection();

            //Application.Run(new MainForm(uContainer.Value));
            Application.Run(new MainForm(uContainer.Resolve<IDataManager>()));
        }

        private static IUnityContainer setupDependencyInjection()
        {
            IUnityContainer c = new UnityContainer();

            Trading.Common.ContainerBootStrapper.RegisterTypes(c);

            return c;
        }
    }
}
