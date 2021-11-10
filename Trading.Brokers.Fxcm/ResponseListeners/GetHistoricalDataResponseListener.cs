using fxcore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trading.Brokers.Fxcm
{
    class GetHistoricalDataResponseListener : IO2GResponseListener, IDisposable
    {
        private readonly O2GSession session;
        private string requestId;
        private O2GResponse response;
        private readonly EventWaitHandle eventWaitHandle;

        public string Error { get; private set; }

        public GetHistoricalDataResponseListener(O2GSession session)
        {
            requestId = string.Empty;
            response = null;
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.session = session;
        }

        public void SetRequestID(string sRequestID)
        {
            response = null;
            requestId = sRequestID;
        }

        public bool WaitEvents()
        {
            return eventWaitHandle.WaitOne(30000);
        }

        public O2GResponse GetResponse()
        {
            return response;
        }

        #region IO2GResponseListener interface Members

        public void onRequestCompleted(string sRequestId, O2GResponse response)
        {
            if (requestId.Equals(response.RequestID))
            {
                this.response = response;
                eventWaitHandle.Set();
            }
        }

        public void onRequestFailed(string sRequestID, string sError)
        {
            if (requestId.Equals(sRequestID))
            {
                Error = sError;
                eventWaitHandle.Set();
            }
        }

        public void onTablesUpdates(O2GResponse data)
        {
        }

        public void Dispose()
        {
            eventWaitHandle.Dispose();
        }

        #endregion

    }
}
