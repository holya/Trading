using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trading.Common;
using Trading.DataBases.Interfaces;
using Trading.DataProviders.Common;

namespace Trading.DataManager.Common
{
    public interface IDataManager : IDataProvider, IDataBase, IDisposable
    {
    }
}
