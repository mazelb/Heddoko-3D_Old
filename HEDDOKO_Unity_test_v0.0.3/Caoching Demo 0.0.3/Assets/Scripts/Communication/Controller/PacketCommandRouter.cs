
/** 
* @file SocketCommandRouter.cs
* @brief Contains the SocketCommandRouter class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils; 
using HeddokoLib.networking;
using HeddokoLib.utils;


namespace Assets.Scripts.Communication
{
    /**
    * Body class 
    * @brief SocketCommandRouter class. This classes is responsible to reroute incoming and outgoing command from and to the socket client. 
    * @note Do not process a command within a registered command. This will cause a cause a stackoverflow exception. 
    * Reroute commands to other functions and not to itself. 
    */
    public class PacketCommandRouter
    {
        private static PacketCommandRouter sInstance;


        private SynchronousClient mSocketClient;

        public SynchronousClient ClientSocket
        {
            get
            {
                if (mSocketClient == null)
                {
                    mSocketClient = new SynchronousClient();
                }
                return mSocketClient;
            }
        }

        public static PacketCommandRouter Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new PacketCommandRouter();
                    sInstance.Initialize();
                }
                return sInstance;
            }
        }
        private Command mCommand = new Command();
        private object mFrameTheadAccessLock = new object();

        //On brainpack data retreival, send the data to the bodyframethread
        private BodyFrameThread mBodyFrameThread;

        private BodyFrameThread FrameThread
        {
            get
            {
                lock (mFrameTheadAccessLock)
                {
                    return mBodyFrameThread;
                }
            }
            set
            {
                BodyFrameThread vTemp = value;
                lock (mFrameTheadAccessLock)
                {
                    mBodyFrameThread = vTemp;
                }
            }
        }

        /**
        * Initialize 
        * @brief Begins command delegate registration. Please see documentation for HeddokoCommand for further information on what each numerical value 
        * represents  
        * @note The document can be found here, you need to have a valid Heddoko sharepoint account in order to access the link. 
        https://heddoko.sharepoint.com/_layouts/OneNote.aspx?id=%2FSiteAssets%2FTeam%20Site%20Notebook&wd=target%28Data%20structures.one%7CCB02DD1E-F126-43C5-A4F2-10252682A3FA%2F%29onenote:https://heddoko.sharepoint.com/SiteAssets/Team%20Site%20Notebook/Data%20structures.one#section-id={CB02DD1E-F126-43C5-A4F2-10252682A3FA}&end
        */
        public void Initialize()
        {
            mCommand.Register(HeddokoCommands.BPConnectionSucess, SuitConnectionSuccess);
            mCommand.Register(HeddokoCommands.RequestToConnectToBP, BrainpackDeviceConnectionRequest);
            mCommand.Register(HeddokoCommands.SendBPData, ReRouteRawFrameData);
            mCommand.Register(HeddokoCommands.RequestBPData, RequestBrainPackData);
            mCommand.Register(HeddokoCommands.ConnectionAck, ConnectionAcknowledged);
            mCommand.Register(HeddokoCommands.StopHeddokoUnityClient, Stop);
            mCommand.Register(HeddokoCommands.StopRecordingReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.ClientError, SocketClientError);
            mCommand.Register(HeddokoCommands.DisconnectBrainpack, DisconnectBrainpackRequest);
            mCommand.Register(HeddokoCommands.DiscoAcknowledged, DisconnectAcknowledged);
            mCommand.Register(HeddokoCommands.SetRecordingPrefixReq, SetBrainpackRecordingPrefix);
            mCommand.Register(HeddokoCommands.ShutdownBrainpackReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.ShutdownBrainpackResp, ShutdownBrainpackResp);
            mCommand.Register(HeddokoCommands.ResetBrainpackReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.ResetBrainpackResp, ResetBrainpackResp);
            mCommand.Register(HeddokoCommands.GetBrainpackStateReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.GetBrainpackStateResp, GetBrainpackStateResp);
            mCommand.Register(HeddokoCommands.SetBrainpackTimeReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.SetBrainpackTimeResp, SetBrainpackTimeResp);
            mCommand.Register(HeddokoCommands.GetResponseMessageReq, RequestResponseMessage);
            mCommand.Register(HeddokoCommands.GetResponseMessageResp, RerouteResponseMessage);
            mCommand.Register(HeddokoCommands.GetBrainpackVersionReq, WrapPacketAndSendMessage);
            mCommand.Register(HeddokoCommands.StartRecordingReq, WrapPacketAndSendMessage);
        }


        private void RequestResponseMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            ClientSocket.Requests.Enqueue(vPayload);
        }
     
        /// <summary>
        /// Reroute status message responses from the brainpack
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void RerouteResponseMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload); 
            BrainpackConnectionController.Instance.Output = vPayload;
            if (!string.IsNullOrEmpty(vPayload))
            {
                Action vAction = () =>
                {
                    if (BrainpackConnectionController.Instance.BrainpackStatusResponse != null)
                    {
                        BrainpackConnectionController.Instance.BrainpackStatusResponse.Invoke(vPayload);
                    }
                };
                OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);
            } 
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
            bool vIsProcessed = mCommand.Process(vSender, vPacket);
            return vIsProcessed;
        }
        /// <summary>
        /// Sets the bodyframe thread to the passed in parameter
        /// </summary>
        /// <param name="vThread"></param>
        public void SetBodyFrameThread(BodyFrameThread vThread)
        {
            FrameThread = vThread;
        }


        /// <summary>
        /// Disconnect the current body frame thread 
        /// </summary>
        /// <param name="vThread"></param>
        public void DisconnectFrameThread()
        {
            FrameThread = null;
        }
        /**
       * RequestAvailableBtDevicesCommand(object vSender, object vArgs) 
       * @brief Request a list of available bluetooth devices from the server
       * @param  object vSender: not used  object vArgs: not used
       */
        private void RequestAvailableBtDevicesCommand(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vOutBound = HeddokoPacket.Wrap(vHeddokoPacket); 
            ClientSocket.Requests.Enqueue(vOutBound);
        }

        /// <summary>
        /// Connection has been acknowledged by the brainpack server
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void ConnectionAcknowledged(object vSender, object vArgs)
        {
            //todo: send message to interested parties that need to know if the brainpack connection is finished
        }
        /**
       * BrainpackListRxRequest(object vSender, object vArgs) 
       * @brief Server request responded with a list of currently available brainpack device
       * @param  object vSender: the sender. object vArgs: the requested list of devices
       * @ note: please note if no devices are found, then a string of length zero is sent
       * Also note that this is a todo in case we ever need to monitor multiple suits in any given one instance
       */
        private void BrainpackListRequestReceived(object vSender, object vArgs)
        {
            StringBuilder vSb = new StringBuilder();
            HeddokoPacket vPacket = (HeddokoPacket)vArgs;
            byte[] vDataBytes = vPacket.Payload;
            vSb.Append(Encoding.UTF8.GetString(vDataBytes, 0, vDataBytes.Length));
            string vPreExploded = vSb.ToString();
            string[] aList = vPreExploded.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> vMacAddresses = new List<string>();
            if (aList.Length != 0)
            {
                for (int i = 0; i < aList.Length; i++)
                {
                    vMacAddresses.Add(aList[i]);
                }
            }
        }
        /**
       * BlueDeviceConnectionRequest(object vSender, object vArgs) 
       * @brief The sender requests a connection to a bluetooth device
       * @param vSender: the sender, vArgs: the bluetooth device address
       */
        private void BrainpackDeviceConnectionRequest(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);

            ClientSocket.Requests.Enqueue(vPayload);
        }

        private void DisconnectBrainpackRequest(object vSender, object vArg)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArg;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            // AsynchronousClient.SendMessage(vPayload);
            ClientSocket.Requests.Enqueue(vPayload);
            ClientSocket.StartClientAndSendData(vPayload);
        }

        private void DisconnectAcknowledged(object vSender, object vArg)
        {

            //todo:
        }

        /// <summary>
        /// A error produced by the socket client and informs the BrainpackConnectionController
        /// </summary>
        /// <param name="vSender"></param>
        /// <param name="vArgs"></param>
        private void SocketClientError(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            //tell the brainpackconnection controller of the error
            BrainpackConnectionController.Instance.FailedToConnectListener(vPayload);
        }
        /**
     * ReRouteRawFrameData(object vSender, object vArgs) 
     * @brief Routes the incoming raw frame data to the BodyFrameThread. 
     * @param vSender: the sender, vArgs: the heddokopack containing the brainpack data. 
     */

        private void ReRouteRawFrameData(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            // mClientSocket.WriteToServer(vPayload);
            BrainpackConnectionController.Instance.Output = vPayload;
            if (FrameThread != null && FrameThread.InboundSuitBuffer != null)
            {
                FrameThread.InboundSuitBuffer.Enqueue(vHeddokoPacket);
            }
            //todo send to buffer to be processed
        }
        private void RequestBrainPackData(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            ClientSocket.Requests.Enqueue(vPayload);
        }


        /// <summary>
        /// Server's response with the status of the connection to the server
        /// </summary>
        /// <param name="vSender">ignored parameter</param>
        /// <param name="vArgs">The HeddokoPacket that was sent. It's payload contains its status</param>
        /**
        * SuitConnectionSuccess(object vSender, object vArgs)
        * @brief Server's response with the status of the connection to the server
        * @param object args: the parameters necessary for this
        * function to perform
        * @note Please not that this will throw an exception if
        * y requirements are not met with the given parameter
        * @return returns an arbitrary value
        */
        private void SuitConnectionSuccess(object vSender, object vArgs)
        {
            //BPConnectionSucess 
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPayload = vHeddokoPacket.Payload;
            string vUnwrappedPayload = HeddokoPacket.Unwrap(vPayload);
            bool vConnectionSuccessfull = (vUnwrappedPayload == "true");

            BrainpackConnectionController.Instance.BrainpackConnectedTrigger.Trigger(vConnectionSuccessfull);
            //BrainpackConnectionResult(vConnectionSuccessfull); //send a message to the BrainpackConnectionController with the result of the connection

        }

        /// <summary>
        /// Send a raw frame to the vSender
        /// </summary>
        /// <param name="vSender">the sender</param>
        /// <param name="vArgs">the raw frame data</param>
        /**
       * SendRawFrameData(object vSender, object vArgs) 
       * @brief Send a raw frame to the vSender
       * @param vSender: the sender, vArgs: the raw frame data
       */
        private void SendRawFrameData(object vSender, object vArgs)
        {
            Socket vSocket = (Socket)vSender;
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            byte[] vPacketBody = vHeddokoPacket.Payload;
            //  AsynchronousSocketListener.Send(vSocket, vPacketBody); 
        }

        private void SetBrainpackRecordingPrefix(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            ClientSocket.Requests.Enqueue(vPayload);
        }

        private void Stop(object vSender, object vArgs)
        {
            ClientSocket.Stop();

        }

        /// <summary>
        /// response received after a state request
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void GetBrainpackStateResp(object vVsender, object vArgs)
        {
            //unwrap the message

            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Unwrap(vHeddokoPacket.Payload);
            Action vAction = () =>
            {
                if (BrainpackConnectionController.Instance.BrainpackStatusResponse != null)
                {
                    BrainpackConnectionController.Instance.BrainpackStatusResponse.Invoke(vPayload);
                }
            };
            OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);
            /*
                        Action vAction = () =>
                        {
                            if (BrainpackConnectionController.BrainpackStatusResponse != null)
                            {
                                BrainpackConnectionController.BrainpackStatusResponse.Invoke(vPayload);
                            }
                        };
                        OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);*/
        }

        /// <summary>
        /// response from the brainpack
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void SetBrainpackTimeResp(object vVsender, object vArgs)
        {
          /*  Action vAction = () =>
            {
                if (BrainpackConnectionController.BrainpackTimeSetResp != null)
                {
                    BrainpackConnectionController.BrainpackTimeSetResp.Invoke();
                }
            };
            OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);*/
        }

        /// <summary>
        /// Response after the brainpack has been reset
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vArgs"></param>
        private void ResetBrainpackResp(object vVsender, object vArgs)
        {
          /*  Action vAction = () =>
            {
                if (BrainpackConnectionController.ResetBrainpackResp != null)
                {
                    BrainpackConnectionController.ResetBrainpackResp.Invoke();
                }
            };
            OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);*/
        }

        /// <summary>
        /// Response to when the brainpack has been shut down
        /// </summary>
        /// <param name="vVsender"></param>
        /// <param name="vVargs"></param>
        private void ShutdownBrainpackResp(object vVsender, object vVargs)
        {
           /* Action vAction = () =>
            {
                BrainpackConnectionController.BrainpackShutdown.Invoke();
            };
            OutterThreadToUnityThreadIntermediary.TriggerActionInUnity(vAction);*/
        }




        private void WrapPacketAndSendMessage(object vSender, object vArgs)
        {
            HeddokoPacket vHeddokoPacket = (HeddokoPacket)vArgs;
            string vPayload = HeddokoPacket.Wrap(vHeddokoPacket);
            ClientSocket.Requests.Enqueue(vPayload);
        }
    }
}
