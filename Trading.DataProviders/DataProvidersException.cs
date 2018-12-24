using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders
{
    public class DataProvidersException : Exception
    {
        public DataProvidersException()
        {

        }

        public DataProvidersException(string message) : base(message)
        {

        }
    }
}
