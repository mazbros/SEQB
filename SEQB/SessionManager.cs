using QBFC13Lib;
using System;
using System.Configuration;

namespace SEQB
{
    public class SessionManager : IDisposable
    {
        private static readonly object LockObject = new object();
        private static SessionManager _instance;
        private QBSessionManager _qbSessionManager;
        private bool _booSessionBegun;
        private static string QBFile;

        private SessionManager()
        {
            _qbSessionManager = new QBSessionManager();
        }

        public static SessionManager GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (LockObject)
                    {
                        if (_instance == null)
                            _instance = new SessionManager();
                    }
                }

                return _instance;
            }
        }
        /// <summary>
        /// Open session and return a referens to QBSessionManager.
        /// If the return value is null it means that the previous session has not been closed, it is necessary to wait for its completion
        /// </summary>
        /// <returns></returns>
        public QBSessionManager OpenSession()
        {
            if (_booSessionBegun)
                return null;

            QBFile = ConfigurationManager.AppSettings["QBFile"];

            _qbSessionManager.OpenConnection("SEQB", "Sample Express QuickBooks Integration");
            _qbSessionManager.BeginSession(QBFile, ENOpenMode.omDontCare);
            _booSessionBegun = true;
            return _qbSessionManager;
        }

        public void Dispose()
        {
            _qbSessionManager.EndSession();
            _booSessionBegun = false;
            _qbSessionManager.CloseConnection();
            _qbSessionManager = null;
            _instance = null;
        }
    }
}
