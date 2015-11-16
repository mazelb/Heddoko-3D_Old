using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrainpackService.bluetooth_connector;
using BrainpackService.brainpack_serial_connect;
using BrainpackService.BrainpackServer;
using BrainpackService.Tools_and_Utilities;

namespace BrainpackService
{
    public partial class BrainpackService : ServiceBase
    {
        private int mEventId;
        //private BrainpackSearcher mBrainPackBtPoller = new BrainpackSearcher();
        private Thread mNetworkThread = new Thread(new ThreadStart(AsynchronousSocketListener.StartListening));
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public BrainpackService(string[] vArgs)
        {
            InitializeComponent();
            string vEventSourceName = DirectoryPathRepository.sEventLogSource;
            string vLogName = DirectoryPathRepository.sBrainpackLog;

            if (vArgs.Length > 0)
            {
                vEventSourceName = vArgs[0];
            }
            if (vArgs.Length > 1)
            {
                vLogName = vArgs[1];
            }

            mEventLog = new EventLog();
            if (!EventLog.SourceExists(vEventSourceName))
            {
                EventLog.CreateEventSource(vEventSourceName, vLogName);

            }

            mEventLog.Source = vEventSourceName;
            mEventLog.Log = vLogName;
            //mBrainPackBtPoller.Init();
        }


        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public long dwServiceType;
            public ServiceState dwCurrentState;
            public long dwControlsAccepted;
            public long dwWin32ExitCode;
            public long dwServiceSpecificExitCode;
            public long dwCheckPoint;
            public long dwWaitHint;
        };
        /**
         * OnStart(string[] args)
         * @param string[] args
         * @brief Defines what happens when the service starts   
         */
        protected override void OnStart(string[] args)
        {
            mEventLog.WriteEntry("Brainpack service started");
            //start a periodic polling
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += this.OnTimer;
            timer.Start();

            //change the service status
            ServiceStatus vServiceStatus = new ServiceStatus();
            vServiceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            vServiceStatus.dwWaitHint = 10000;
            SetServiceStatus(ServiceHandle, ref vServiceStatus);
            //update the service state to running
            vServiceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(ServiceHandle, ref vServiceStatus);
            //start the server

            // BrainpackServer.BrainpackServer vServer = new BrainpackServer.BrainpackServer();
            // vServer.SetupServer();


            /**
            If your OnStart and OnStop methods run long, 
            your service can request more time by calling SetServiceStatus again with an incremented dwCheckPoint value.
            */
            //BrainpackSearcher connector = new BrainpackSearcher();
            ServerCommandRouter.Instance.BeginRegistration();
           // connector.Init();
            mNetworkThread.Start();
            BrainpackEventLogManager.RegisterEventLogMessage(WriteEventLogMessage);
        }

        private void WriteEventLogMessage(string vMsg)
        {
            mEventLog.WriteEntry(vMsg);

        }
        protected void OnTimer(object vSender, System.Timers.ElapsedEventArgs args)
        {
            //todo : insert montoring activities here
            

        }
        /**
        * OnStop() 
        * @brief Defines what happens when the stops starts   
        */
        protected override void OnStop()
        { 
           //mBrainPackBtPoller.Stop(); // stop the connector
            AsynchronousSocketListener.StopService();
            BrainpackEventLogManager.DeRegisterEventLogMessage(WriteEventLogMessage);
            //BrainpackConnectionManager.Instance.Stop();
            BrainpackSerialConnector.Instance.Stop();
        }


        /**
        * OnStop() 
        * @brief Defines what happens when the service is running 
        */
        protected override void OnContinue()
        {
            
        }

        protected override void OnCustomCommand(int command)
        {
            switch (command)
            {
                case 128:
                    mEventLog.WriteEntry("Command " + command + " successfully called.");
                    break;
                default:
                    break;
            }
        }
    }
}
