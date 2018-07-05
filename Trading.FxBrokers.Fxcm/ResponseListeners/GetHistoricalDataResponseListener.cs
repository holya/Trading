using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using fxcore2;

namespace Trading.FxBrokers.Fxcm
{
    class GetHistoricalDataResponseListener : IO2GResponseListener
    {
        private O2GSession session;
        private string requestId;
        private O2GResponse response;
        private EventWaitHandle eventWaitHandle;

        public string Error { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="session"></param>
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

        #endregion

    }
}

