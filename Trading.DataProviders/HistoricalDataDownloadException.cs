using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trading.DataProviders.Common
{
    public class HistoricalDataDownloadException : Exception
    {
        private HistoricalDataDownloadMessageEnum messageEnum;

        public HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum messageEnum, string message) : base(message)
        {
            this.messageEnum = messageEnum;
        }

        public HistoricalDataDownloadException(HistoricalDataDownloadMessageEnum messageEnum, string message, Exception innerException) : base(message, innerException)
        {
            this.messageEnum = messageEnum;
        }
    }
}
