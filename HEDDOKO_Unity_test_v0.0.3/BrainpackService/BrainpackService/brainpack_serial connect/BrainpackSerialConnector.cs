using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using BrainpackService.bluetooth_connector.BrainpackInterfaces;
using BrainpackService.bluetooth_connector.DebugContext;
using BrainpackService.Tools_and_Utilities;
using HeddokoLib.adt;


namespace BrainpackService.brainpack_serial_connect
{
    public class BrainpackSerialConnector : IBrainpackConnection
    {
        private Thread mWorkerThread;
        private bool mIsWorking { get; set; }
        //public FileStream dataFile;
        //public DataTable analysisData;
        private string mPortName;
        private string mNextLine;
        private static BrainpackSerialConnector sInstance;
        private object mSerialPortLock = new object();
        private object mLineLock = new object();
        bool mMessageSent = false;
        private DebugBodyFrameLogger mBodyFrameLogger = new DebugBodyFrameLogger("BPService");
        SerialPort mSerialport { get; set; } = new SerialPort();

        CircularQueue<string> OutboundBuffer { get; set; } = new CircularQueue<string>(10, true);

        public static BrainpackSerialConnector Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new BrainpackSerialConnector();
                }
                return sInstance;
            }
        }
        /// <summary>
        /// Initialize the serialport with the given serialport name
        /// </summary>
        /// <param name="vBrainpackName"> The serial portn name</param>
        public void Initialize(string vBrainpackName)
        {
            SerialPortOpen(vBrainpackName);

        }
        /// <summary>
        /// Try to set the serial port
        /// </summary>
        /// <param name="vNewPort">The name of the serial port</param>
        private void SerialPortOpen(string vNewPort)
        {
            try
            {
                lock (mSerialPortLock)
                {
                    if (vNewPort != mPortName)
                    {
                        Stop();
                        mSerialport = new SerialPort();
                        mSerialport.ReadTimeout = 30000;//30second timeout
                        mSerialport.WriteTimeout = 30000;
                        mPortName = vNewPort;
                        mSerialport.PortName = mPortName;
                        mSerialport.NewLine = "\r\n";
                        mWorkerThread = new Thread(ThreadedFunction);
                        this.mSerialport.BaudRate = 115200;
                        mSerialport.Open();
                        Start();
                    }
                    if (vNewPort == mPortName && mSerialport != null && !mSerialport.IsOpen)
                    { 
                        mWorkerThread = new Thread(ThreadedFunction);
                        this.mSerialport.BaudRate = 115200;
                        mSerialport.Open();
                        Start(); 
                    }

                    /*       //check if the port isn't already opened
                           else if (vNewPort == mPortName && mSerialport != null && !mSerialport.IsOpen)
                           {
                               mWorkerThread = new Thread(ThreadedFunction);
                               this.mSerialport.BaudRate = 115200;
                               mSerialport.Open();
                               Start();
                           } */

                    else
                    {
                        OutboundBuffer.Clear();
                        Start();
                    }
                }
            }

            catch (Exception e)
            {
                BrainpackEventLogManager.InvokeEventLogError("Could not open serial port " + mSerialport + "\n Error:" + e.StackTrace + "\n" + e.ToString());
            }

        }

        /// <summary>
        /// pulls data from the brainpack
        /// </summary>
        public void ThreadedFunction()
        {

            while (true)
            {
                Stopwatch vStopwatch = new Stopwatch();
                vStopwatch.Start();
                
                lock (mSerialPortLock)
                {
                    if (mSerialport.IsOpen)
                    {
                        try
                        {
                            if (!mMessageSent)
                            {
                                string line = mSerialport.ReadLine();
                                //
                                if (line.Length != 176)
                                {
                                    lock (mLineLock)
                                    {
                                        mNextLine = "";
                                    }
                                    BrainpackEventLogManager.InvokeEventLogMessage("Is less than 176 chars");
                                    continue;
                                }
                                OutboundBuffer.Enqueue(line);
                                lock (mLineLock)
                                {
                                    mNextLine = line;

                                }
                                
                            }

                        }
                        catch (IOException vIoException)
                        {
                            BrainpackEventLogManager.InvokeEventLogError(vIoException + "\r\n" + vIoException.StackTrace);
                            mNextLine = "";
                        }
                        catch (NullReferenceException vNullReferenceException)
                        {
                            BrainpackEventLogManager.InvokeEventLogError(vNullReferenceException + "\r\n" + vNullReferenceException.StackTrace);
                        }
                        catch (TimeoutException vTimeout)
                        {
                            try
                            {
                                var vResult = QuestionBrainpack(); //ask the brainpack if it's still alive

                                if (!vResult)
                                {
                                    lock (mLineLock)
                                    {
                                        mNextLine = "";

                                    }
                                    
                                    mSerialport.Close();
                                }
                            }
                            catch (IOException vIoException)
                            {
                                BrainpackEventLogManager.InvokeEventLogError(vIoException + "\r\n" + vIoException.StackTrace);
                                lock (mLineLock)
                                {
                                    mNextLine = "";

                                }
                            }
                            catch (InvalidOperationException vInvalidOperationException)
                            {
                                BrainpackEventLogManager.InvokeEventLogError(vInvalidOperationException + "\r\n" + vInvalidOperationException.StackTrace);
                                lock (mLineLock)
                                {
                                    mNextLine = "";

                                }
                            }
                        }
                    }
                }
                vStopwatch.Stop();
                
                //mBodyFrameLogger.WriteLog(vStopwatch.Elapsed.TotalMilliseconds,mNextLine);
                if (!mIsWorking)
                {
                    Stop();
                    break;
                }
            }
        }

        private void SerialConnectionLost()
        {

        }

        public void SendCommandToBrainpack(string vMsg)
        {
            lock (mSerialPortLock)
            {
                mSerialport.Write(vMsg + "\r\n");
            }
        }

        private void WriteLine(string vLine)
        {
            
        }
        public void Start()
        {
            if (!mIsWorking)
            {
                mIsWorking = true;
                mWorkerThread.Start();
            }

        }

        private bool QuestionBrainpack()
        {
            bool vResult = true;
            mMessageSent = true;
            try
            {
                //SendCommandToBrainpack("?");
                SendCommandToBrainpack("GetState");
                lock (mSerialPortLock)
                {
                    if (mSerialport.IsOpen)
                    {
                        string line = mSerialport.ReadLine();
                    }
                }
            }
            catch (IOException vException)
            {
                vResult = false;
            }
            catch (TimeoutException vException)
            {
                vResult = false;
            }

            mMessageSent = false;
            return vResult;
        }
        /// <summary>
        /// Stop the connection
        /// </summary>
        public void Stop()
        {
            mNextLine = "";
            mIsWorking = false;
            OutboundBuffer.Clear();
            if (IsConnected)
            {
                lock (mSerialPortLock)
                {
                    mSerialport.Close();
                }
            }
        }

        /// <summary>
        /// Verify if the serial port is open and ready to send message back
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (mSerialport != null)
                {
                    return mSerialport.IsOpen;
                    bool vResult = true;
                    if (!mSerialport.IsOpen)
                    {
                        return false;
                    }
                    //try to send a message to the serial port
                    /*vResult = QuestionBrainpack();
                    if (!vResult)
                    {
                        lock (mSerialPortLock)
                        {
                            mSerialport.Close();
                        }
                    }*/
                   // return vResult;
                }
                return false;
            }
        }

        public string GetNextFrame()
        {
            string vReturnMsg = "";
            lock (mLineLock)
            {
                vReturnMsg =mNextLine ;

            }
            return vReturnMsg;
/*               if (OutboundBuffer.Count == 0)
               {
                   return "";
               }
               else
               {
                   return OutboundBuffer.Dequeue();
               } */
        }
    }
}
