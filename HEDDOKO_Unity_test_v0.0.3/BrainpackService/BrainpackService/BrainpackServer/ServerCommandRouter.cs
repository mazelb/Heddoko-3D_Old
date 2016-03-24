/** 
* @file ServerCommandRouter.cs
* @brief Contains the ServerCommandRouter class 
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using BrainpackService.bluetooth_connector;
using BrainpackService.brainpack_serial_connect;
using BrainpackService.Tools_and_Utilities.Debugging;
using HeddokoLib.networking;
using HeddokoLib.utils;
using InTheHand.Net.Sockets;

namespace BrainpackService.BrainpackServer
{
    /**
    * ServerCommandRouter class 
    * @brief ServerCommandRouter class: a class whos function is to register command delegates and route delegates
    */

    public class ServerCommandRouter
    {
        private static ServerCommandRouter sInstance;

        public static ServerCommandRouter Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new ServerCommandRouter();
                }
                return sInstance;
            }
        }

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Command mHeddokoCommand = new Command();

#pragma warning disable 649
        private BrainpackServer mServer;
#pragma warning restore 649
        // public BrainpackServer ServerConnection { set { mServer = value; } }
        /**
       * BeginRegistration 
       * @brief Begins command delegate registration. Please see documentation for HeddokoCommand for further information on what each numerical value 
       * represents  
       * @note The document can be found here, you need to have a valid Heddoko sharepoint account in order to access the link. 
       https://heddoko.sharepoint.com/_layouts/OneNote.aspx?id=%2FSiteAssets%2FTeam%20Site%20Notebook&wd=target%28Data%20structures.one%7CCB02DD1E-F126-43C5-A4F2-10252682A3FA%2F%29onenote:https://heddoko.sharepoint.com/SiteAssets/Team%20Site%20Notebook/Data%20structures.one#section-id={CB02DD1E-F126-43C5-A4F2-10252682A3FA}&end
       */
        public void BeginRegistration()
        {
            mHeddokoCommand.Register(HeddokoCommands.RequestBrainpackList, RequestAvailableBtDevicesCommand);
            mHeddokoCommand.Register(HeddokoCommands.RequestToConnectToBP, BrainpackDeviceConnectionRequest);
            mHeddokoCommand.Register(HeddokoCommands.SendBTCommand, SendRawFrameData);
            mHeddokoCommand.Register(HeddokoCommands.CloseConnectionToServer, CloseSocketToServer);
            mHeddokoCommand.Register(HeddokoCommands.SendBPData, SendBrainPackData);
            mHeddokoCommand.Register(HeddokoCommands.RequestBPData, RequestBrainPackData);
            mHeddokoCommand.Register(HeddokoCommands.RequestConnection, RequestConnectionToServer);
            mHeddokoCommand.Register(HeddokoCommands.DisconnectBrainpack, DisconnectBrainpack);
            mHeddokoCommand.Register(HeddokoCommands.SetRecordingPrefixReq, SetBrainpackRecordingPrefix);
            mHeddokoCommand.Register(HeddokoCommands.GetBrainpackStateReq, GetBrainpackStateReq);
            mHeddokoCommand.Register(HeddokoCommands.GetBrainpackVersionReq, GetBrainpackVersionReq);
            mHeddokoCommand.Register(HeddokoCommands.ResetBrainpackReq, ResetBrainpackReq);
            mHeddokoCommand.Register(HeddokoCommands.StartRecordingReq, StartRecording);
            mHeddokoCommand.Register(HeddokoCommands.ShutdownBrainpackReq, ShutdownBrainpackReq);
            mHeddokoCommand.Register(HeddokoCommands.SetBrainpackTimeReq, SetBrainpackTimeReq);
            mHeddokoCommand.Register(HeddokoCommands.GetResponseMessageReq, RequestMessageResponse);
            mHeddokoCommand.Register(HeddokoCommands.StopRecordingReq, StopRecording);
            mHeddokoCommand.Register(HeddokoCommands.ClearBuffer, ClearBrainpackSerialConnectorBuffer);
            mHeddokoCommand.Register(HeddokoCommands.EnableSleepTimerReq, EnableSleepTimer);
            mHeddokoCommand.Register(HeddokoCommands.DisableSleepTimerReq, DisableBrainpackSleepTimer);

        }

        /// <summary>
        /// Disables the brainpack sleep timer
        /// </summary>
        /// <param name="vsender"></param>
        /// <param name="vargs"></param>
        private void DisableBrainpackSleepTimer(object vSender, object vargs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("AutoOff0");
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Request to disable the sleep timer");
            string vResponses = "";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.DisableSleepTimerResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        /// <summary>
        ///  enables the brainpack Sleep timer
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vargs"></param>
        private void EnableSleepTimer(object vSender, object vargs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("AutoOff1");
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Request to enable sleep timer");
            string vResponses = "";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.EnableSleepTimerResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        /// <summary>
        /// Clears the brainpack buffers
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void ClearBrainpackSerialConnectorBuffer(object vSender, object vArgs)
        {
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Buffer clear request");
            BrainpackSerialConnector.Instance.Clear();
            string vResponses = BrainpackSerialConnector.Instance.GetLatestState(); ;
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.GetBrainpackStateResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        private void SetBrainpackTimeReq(object vSender, object vVargs)
        {
            DateTime vTime = DateTime.Now;
            int vDayOfWeekNumber = (int)vTime.DayOfWeek;
            string vTimeCommand = "setTime" + vTime.Year + "-" + vTime.Month + "-" + vTime.Day + "-" +
             vDayOfWeekNumber + "-" + vTime.Hour + ":" + vTime.Minute + ":" + vTime.Second;
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vTimeCommand);
            BrainpackSerialConnector.Instance.SendCommandToBrainpack(vTimeCommand);
            string vResponses = "";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.SetBrainpackTimeResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }
        private void ShutdownBrainpackReq(object vSender, object vVargs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("Power");
            string vResponses = "";//BrainpackSerialConnector.Instance.GetResponse();
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.ShutdownBrainpackResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }
        private void StartRecording(object vSender, object vVargs)
        {
            //check if already recording
            bool vIsRecording = BrainpackSerialConnector.Instance.IsRecording();
            if (!vIsRecording)
            {
                BrainpackSerialConnector.Instance.SendCommandToBrainpack("Record");
                DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Record");
            }
            string vResponses = "";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.StartRecordingResp, vResponses);
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, HeddokoCommands.StartRecordingResp);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        /// <summary>
        /// Attempts to stop recording
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void StopRecording(object vSender, object vArgs)
        {
            bool vIsRecording = BrainpackSerialConnector.Instance.IsRecording();
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Req to stop recording");
            if (vIsRecording)
            {
                BrainpackSerialConnector.Instance.SendCommandToBrainpack("Record");
                Thread.Sleep(50);
            }
            string vResponse = !BrainpackSerialConnector.Instance.IsRecording()
                ? "Idle"
                : BrainpackSerialConnector.Instance.GetLatestState();
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, vResponse);
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.GetBrainpackStateResp, vResponse);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }
        private void ResetBrainpackReq(object vSender, object vVargs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("Reset");
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Reset");
            string vResponses = "";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.ResetBrainpackResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        private void GetBrainpackVersionReq(object vSender, object vArgs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("?");
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "?");
            string vResponses = "";
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, HeddokoCommands.GetBrainpackVersionResp);
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.GetBrainpackVersionResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        /// <summary>
        /// Send a command to the brainpack serial connector to get the current brainpack state
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void GetBrainpackStateReq(object vSender, object vArgs)
        {
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("GetState");
            //Sleep for 75 ms
            Thread.Sleep(75);
            string vResponse = "Disconnected";
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "GetState");

            //Verify brainpack connection
            if (BrainpackSerialConnector.Instance.IsConnected)
            {
                vResponse = BrainpackSerialConnector.Instance.GetLatestState();
            }
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, vResponse);

            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.GetBrainpackStateResp, vResponse);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send((Socket)vSender, vWrapped);
        }

        /// <summary>
        /// Sends a frame of brain pack data back to the sender
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SendBrainPackData(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;

            string vPacketBody = BrainpackSerialConnector.Instance.GetNextFrame();
            DebugLogger.Instance.LogMessage(LogType.ApplicationFrame, vPacketBody);
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.SendBPData, vPacketBody);
            AsynchronousSocketListener.Send(vSocket, vHeddokoPacket.Payload);
        }


        /// <summary>
        /// Sends a message that the request has been ack
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void RequestConnectionToServer(object vSender, object vArgs)
        {
            if (mServer.IsConnected())
            {
                Socket vSocket = (Socket)vSender;
                string vAckCmd = HeddokoCommands.ConnectionAck;
                StringBuilder vSb = new StringBuilder();
                string vConnectionResult = "ack";
          
                vSb.AppendLine(vAckCmd);
                vSb.Append(vConnectionResult);
                vSb.Append(PacketSetting.EndOfPacketDelim);
                mServer.Send(vSocket, vSb.ToString());
            }
        }
        /// <summary>
        /// Sets the recording prefix of the brainpack
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SetBrainpackRecordingPrefix(object vSender, object vArgs)
        {
            Socket vHandler = (Socket)vSender;
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPayload = vHeddokoPacket.Payload;
            string vUnwrappedPayload = HeddokoPacket.Unwrap(vPayload);
            BrainpackSerialConnector.Instance.SendCommandToBrainpack("SetRecordName" + vUnwrappedPayload);
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "SetRecordName" + vUnwrappedPayload);
            string vResponses = "";
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, HeddokoCommands.SetRecordingPrefixResp);

            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.SetRecordingPrefixResp, vResponses);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send(vHandler, vWrapped);
        }


        /// <summary>
        /// A client is requesting a frame of brain pack data
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void RequestBrainPackData(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            string vPacketBody = BrainpackSerialConnector.Instance.GetNextFrame();
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "Req bp data");

            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.SendBPData, vPacketBody);
            //wrap the data and then send the data to (SendBrainPackData)
            string vWrappedPacket = HeddokoPacket.Wrap(vHeddokoPacket);
            AsynchronousSocketListener.Send(vSocket, vWrappedPacket);
        }
       
        private void RequestMessageResponse(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            string vPacketBody = BrainpackSerialConnector.Instance.GetResponse();
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, HeddokoCommands.GetResponseMessageReq);

            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.GetResponseMessageResp, vPacketBody);
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, vPacketBody);

            string vWrappedPacket = HeddokoPacket.Wrap(vHeddokoPacket);
            AsynchronousSocketListener.Send(vSocket, vWrappedPacket);
        }


        private void DisconnectBrainpack(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            string vPacketBody = "Brainpack connection closed";
            BrainpackSerialConnector.Instance.CloseSerialPort();
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, vPacketBody +"Attempting to disconnect from brainpack");
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.DiscoAcknowledged, vPacketBody);
            //wrap the data and then send the data to (SendBrainPackData)
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, "BP connection status"+BrainpackSerialConnector.Instance.IsConnected.ToString());
            string vWrappedPacket = HeddokoPacket.Wrap(vHeddokoPacket);
            AsynchronousSocketListener.Send(vSocket, vWrappedPacket);
        }


        /**
        * Process(object vSender, HeddokoPacket vPacket)
        * @brief Attempt to prcess the packet. Wrapper function for the Command.Process command
        * @param  object vSender: the sender. object vArgs: the requested list of devices
        * @note will not invoke a CommandDelegate if the packet contains an invalid command
        * @return bool that indicates that the command delegate was succesfully invoked
        */
        public bool Process(object vSender, HeddokoPacket vPacket)
        {
            return mHeddokoCommand.Process(vSender, vPacket);
        }
        /**
       * RequestAvailableBtDevicesCommand(object vSender, object vArgs) 
       * @brief The sender requests a list of bluetooth devices that are available
       * @param  object vSender: the sender. object vArgs: the requested list of devices
       * @ note: please note if no devices are found, then a string of length zero is sent
       */
        private void RequestAvailableBtDevicesCommand(object vSender, object vArgs)
        {
            StringBuilder vSb = new StringBuilder();
            vSb.AppendLine(HeddokoCommands.SendBrainpackList);
            //in actuality, there is no need to extract information out of the args, since this command is just a request of available suits
            Socket vSocket = (Socket)vSender;
            //get the available list of bluetooth devices
            BluetoothDeviceInfo[] vDevicesInformation = BrainpackSearcher.FilteredBluetoothDevices;
            if (vDevicesInformation != null && vDevicesInformation.Length != 0) //there is a possiblity that no devices were found
            {
                for (int vDevIndex = 0; vDevIndex < vDevicesInformation.Length; vDevIndex++) //vDevIndex : Device index
                {
                    vSb.AppendLine(vDevicesInformation[vDevIndex].DeviceAddress.ToString());
                }
            }

            string vOutputInfo = vSb.ToString();
            //dont use this server: it doesn't check for received data streams well

            //BrainpackServer.SendData(vSocket, vOutputInfo); //send the data to the interested socket, I 
            // todo send  AsynchronousSocketListener.SendData
            AsynchronousSocketListener.Send(vSocket, vOutputInfo);
        }


        private void CloseSocketToServer(object vSender, object vArgs)
        {

        }
        /**
       * BlueDeviceConnectionRequest(object vSender, object vArgs) 
       * @brief The sender requests a connection to a bluetooth device
       * @param vSender: the sender, vArgs: the bluetooth device address
       */
        private void BrainpackDeviceConnectionRequest(object vSender, object vArgs)
        {
            Socket vHandler = (Socket)vSender;
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPayload = vHeddokoPacket.Payload;
            string vUnwrappedPayload = HeddokoPacket.Unwrap(vPayload);
            DebugLogger.Instance.LogMessage(LogType.ApplicationCommand, "BP Connection request " + vPayload);

            // bool vConnectionSuccessfull = BrainpackConnectionManager.Instance.ConnectToBrainpack(vUnwrappedPayload);
            BrainpackSerialConnector.Instance.Initialize(vUnwrappedPayload);
            bool vConnectionSuccessfull = BrainpackSerialConnector.Instance.IsConnected;
            string vConnectionResult = vConnectionSuccessfull ? "true" : "false";
            DebugLogger.Instance.LogMessage(LogType.ApplicationResponse, "BP Connection request " + vConnectionResult);
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.BPConnectionSucess, vConnectionResult);
            string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            AsynchronousSocketListener.Send(vHandler, vWrapped);
        }
        /**
        * SendRawFrameData(object vSender, object vArgs) 
        * @brief Send a raw frame to the vSender
        * @param vSender: the sender, vArgs: the raw frame data
        */
        private void SendRawFrameData(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            // HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPacketBody = BrainpackSerialConnector.Instance.GetNextFrame();
            AsynchronousSocketListener.Send(vSocket, vPacketBody);
        }

    }
}
