using fxcore2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trading.DataProviders;
using Trading.DataProviders.Common;

namespace Trading.Brokers.Fxcm
{
    class SessionStatusResponseListener : IO2GSessionStatus, IDisposable
    {
        private string sessionId;
        private string pin;
        private O2GSession session;
        private EventWaitHandle waitHandle;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="session"></param>
        public SessionStatusResponseListener(O2GSession session, string sSessionID, string sPin)
        {
            //this.session = session;
            sessionId = sSessionID;
            pin = sPin;
            Reset();
            waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        }

        public O2GSessionStatusCode SessionStatus { get; private set; }
        public bool Connected { get; private set; } = false;
        public bool Disconnected { get; private set; } = true;
        public bool Error { get; private set; } = false;
        public string ErrorMessage { get; private set; }

        public void Reset()
        {
            SessionStatus = O2GSessionStatusCode.Disconnected;
        }

        public bool WaitEvents()
        {
            return waitHandle.WaitOne(30000);
        }
        public void onSessionStatusChanged(O2GSessionStatusCode status)
        {
            SessionStatus = status;

            switch (status)
            {
                case O2GSessionStatusCode.Disconnected:
                    waitHandle.Set();
                    break;
                case O2GSessionStatusCode.Connecting:
                    break;
                case O2GSessionStatusCode.TradingSessionRequested:
                    break;
                case O2GSessionStatusCode.Connected:
                    waitHandle.Set();
                    break;
                case O2GSessionStatusCode.Reconnecting:
                    break;
                case O2GSessionStatusCode.Disconnecting:
                    break;
                case O2GSessionStatusCode.SessionLost:
                    waitHandle.Set();
                    break;
                case O2GSessionStatusCode.PriceSessionReconnecting:
                    //waitHandle.Set();
                    break;
                case O2GSessionStatusCode.Unknown:
                    waitHandle.Set();
                    break;
            }
        }

        public void onLoginFailed(string error)
        {
            Error = true;
            ErrorMessage = error;
            waitHandle.Set();
        }

        public void Dispose()
        {
            waitHandle.Dispose();
        }
    }
}
