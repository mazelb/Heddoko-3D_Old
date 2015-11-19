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
using BrainpackService.bluetooth_connector;
using BrainpackService.brainpack_serial_connect;
using HeddokoLib.networking;
using HeddokoLib.utils;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using Command = BrainpackService.Tools_and_Utilities.Command;

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

        private Command mHeddokoCommand = new Command();

        private BrainpackServer mServer;
        public BrainpackServer ServerConnection { set { mServer = value; } }
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
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.SendBPData,vPacketBody);
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
                Socket vSocket = (Socket) vSender;
                string vAckCmd = HeddokoCommands.ConnectionAck;
                StringBuilder vSb = new StringBuilder();
                string vConnectionResult = "ack";
               
                vSb.AppendLine(vAckCmd);
                vSb.Append(vConnectionResult);
                vSb.Append(PacketSetting.EndOfPacketDelim);
                mServer.Send(vSocket,vSb.ToString());
                //string vWrapped = HeddokoPacket.Wrap(vResultPacket);
            }
        }
        /// <summary>
        /// A client is requesting a frame of brain pack data
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void RequestBrainPackData(object vSender, object vArgs)
        { 
            Socket vSocket = (Socket)vSender; 
            StringBuilder vSb = new StringBuilder();
            vSb.AppendLine(HeddokoCommands.SendBPData); 
            string vPacketBody = BrainpackSerialConnector.Instance.GetNextFrame();
            vSb.AppendLine(vPacketBody);
            vSb.Append(PacketSetting.EndOfPacketDelim);
           //HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.SendBPData, vPacketBody);
            //wrap the data and then send the data to (SendBrainPackData)
           // string vWrappedPacket = HeddokoPacket.Wrap(vHeddokoPacket);
            //AsynchronousSocketListener.Send(vSocket, vWrappedPacket);
            mServer.Send(vSocket,vSb.ToString() );
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
            Socket vHandler = (Socket) vSender;
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPayload = vHeddokoPacket.Payload;
            string vUnwrappedPayload = HeddokoPacket.Unwrap(vPayload);
            // bool vConnectionSuccessfull = BrainpackConnectionManager.Instance.ConnectToBrainpack(vUnwrappedPayload);
            BrainpackSerialConnector.Instance.Initialize(vUnwrappedPayload);
            bool vConnectionSuccessfull = BrainpackSerialConnector.Instance.IsConnected;
            string vConnectionResult = vConnectionSuccessfull ? "true" : "false";
            HeddokoPacket vResultPacket = new HeddokoPacket(HeddokoCommands.BPConnectionSucess,vConnectionResult);
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
            AsynchronousSocketListener.Send(vSocket,vPacketBody);
        }

    }
}
