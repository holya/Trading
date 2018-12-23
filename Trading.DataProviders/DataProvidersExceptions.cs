using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Trading.DataProviders
{
    [Serializable]
    public class DataProvidersExceptions : Exception
    {
        public DataProvidersExceptions()
        {

        }

        public DataProvidersExceptions(string message) : base(message)
        {

        }

        public DataProvidersExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }
}
