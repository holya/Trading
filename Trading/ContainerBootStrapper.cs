﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.DataProviders.Common;
using Trading.Brokers.Fxcm;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Trading.DataBases.Interfaces;
using Trading.DataBases.XmlDataBase;
using Trading.DataManager;
using Trading.DataBases.MongoDb;

namespace Trading.Common
{
    public class ContainerBootStrapper
    {
        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IDataProvider, FxcmWrapper>(new ContainerControlledLifetimeManager());
            container.RegisterType<IDataBase, XmlDataBase>(new ContainerControlledLifetimeManager());

            container.RegisterInstance(typeof(DataManager.DataManager), 
                new DataManager.DataManager(container.Resolve<IDataProvider>(), container.Resolve<IDataBase>()), 
                new ContainerControlledLifetimeManager());

        }
    }
}
