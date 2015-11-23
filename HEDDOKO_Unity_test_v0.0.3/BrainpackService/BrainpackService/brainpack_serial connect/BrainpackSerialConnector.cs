using System;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Threading; 
using BrainpackService.bluetooth_connector.BrainpackInterfaces;
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
        private static BrainpackSerialConnector sInstance;
        private object mSerialPortLock = new object();
        bool mMessageSent = false;
        SerialPort mSerialport { get; set; } = new SerialPort();

        CircularQueue<string> OutboundBuffer { get; set; } = new CircularQueue<string>(2048, true);

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
        /// Try to set p the serial port
        /// </summary>
        /// <param name="vNewPort">The name of the serial port</param>
        private void SerialPortOpen(string vNewPort)
        {
            try
            {
                if (vNewPort != mPortName)
                {
                    Stop();
                    mSerialport = new SerialPort();
                    mSerialport.ReadTimeout = 2000;//2 second timeout
                    mSerialport.WriteTimeout = 2000;
                    mPortName = vNewPort;
                    mSerialport.PortName = mPortName;
                    mSerialport.NewLine = "\r\n";
                    mWorkerThread = new Thread(ThreadedFunction);
                    this.mSerialport.BaudRate = 115200;
                    Start(); 
                }
                else
                {
                    OutboundBuffer.Clear();
                    Start();
                }

            }
            catch (Exception e)
            {
                BrainpackEventLogManager.InvokeEventLogError("Could not open serial port " + mSerialport + "\n Error:" + e.StackTrace + "\n" + e.ToString());
            }


        }

        
        public void ThreadedFunction()
        {

            while (true)
            {
                if (mSerialport.IsOpen)
                {
                    try
                    {
                        if (!mMessageSent)
                        {
                            lock (mSerialPortLock)
                            {
                                string line = mSerialport.ReadLine();
                                if (line.Length < 100)
                                {
                                    BrainpackEventLogManager.InvokeEventLogMessage("Is less than 100 chars");
                                }
                                OutboundBuffer.Enqueue(line);
                            }


                            //mSerialport.DiscardInBuffer();

                        }

                    }
                    catch (IOException vIoException)
                    {
                        BrainpackEventLogManager.InvokeEventLogError(vIoException + "\r\n" + vIoException.StackTrace);
                    }
                    catch (TimeoutException vTimeout)
                    {
                        try
                        {
                            var vResult = QuestionBrainpack(); //ask the brainpack if it's still alive
                            if (!vResult)
                            {

                                mSerialport.Close();
                            }
                        }
                        catch (IOException vIoException)
                        {
                            BrainpackEventLogManager.InvokeEventLogError(vIoException + "\r\n" + vIoException.StackTrace);
                        }
                        catch (InvalidOperationException vInvalidOperationException)
                        {
                            BrainpackEventLogManager.InvokeEventLogError(vInvalidOperationException + "\r\n" + vInvalidOperationException.StackTrace);
                        }
                    }



                }
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
                mSerialport.Write(vMsg + "/r/n");
            }
        }


        public void Start()
        {
            try
            {

                //first check if the serial port isn't open and try to open it
                if (!mSerialport.IsOpen)
                {
                    mSerialport.Open();
                }
                /*      //if it is already open, then send a message to it. See what its response is.
                      else
                      {
                          var vResult = QuestionBrainpack();
                          if (!vResult)
                          {
                              mSerialport.Close();
                          }
                      }*/
            }
            catch (IOException e)
            {
                BrainpackEventLogManager.InvokeEventLogError(e.ToString());
            }
            catch (TimeoutException vTimeout)
            {
                mSerialport.Close();
                mMessageSent = false;
                BrainpackEventLogManager.InvokeEventLogError(vTimeout + "\r\n" + vTimeout.StackTrace);
            }
            catch (InvalidOperationException vInvalid)
            {
                BrainpackEventLogManager.InvokeEventLogError(vInvalid + "\r\n" + vInvalid.StackTrace);
            }
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
                SendCommandToBrainpack("/?");
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
            mIsWorking = false;
            OutboundBuffer.Clear();
            if (IsConnected)
            {
                mSerialport.Close();
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
                    bool vResult = true;
                    if (!mSerialport.IsOpen)
                    {
                        return false;
                    }
                    //try to send a message to the serial port
                    vResult = QuestionBrainpack();
                    if (!vResult)
                    {
                        mSerialport.Close();
                    }
                    return vResult;
                }
                return false;
            }
        }

        public string GetNextFrame()
        {
            if (OutboundBuffer.Count == 0)
            {
                return "";
            }
            else
            {
                return OutboundBuffer.Dequeue();
            }
        }
    }
}
