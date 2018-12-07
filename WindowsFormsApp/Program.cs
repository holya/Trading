using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Trading.Brokers.Fxcm;
using Trading.DataProviders.Common;
using Unity;

namespace WindowsFormsApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            setupDependencyInjection();

            Application.Run(new MainForm());
        }

        private static void setupDependencyInjection()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IDataProvider, FxcmWrapper>();
        }
    }
}
