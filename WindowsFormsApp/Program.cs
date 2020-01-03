using System;
using System.Windows.Forms;
using Trading.DataManager.Common;
using Unity;
using Trading;

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
            IUnityContainer c = new Unity.UnityContainer();

            Trading.ContainerBootStrapper.RegisterTypes(c);

            return c;
        }
    }
}
