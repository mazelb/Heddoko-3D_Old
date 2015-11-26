/** 
* @file BrainpackConnectionController.cs
* @brief Contains the BrainpackConnectionModule class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Collections;
using Assets.Scripts.Communication.View;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UnityEngine;
using System;
using Assets.Scripts.Utils.UnityUtilities;
using Assets.Scripts.Utils.UnityUtilities.Repos;

namespace Assets.Scripts.Communication.Controller
{
    /**
    * BrainpackConnectionController class that controls the connection to the heddoko body suit
    * @brief BrainpackConnectionModule class that  
    */
    /// <summary>
    /// BrainpackConnectionController class that controls the connection to the heddoko body suit
    /// </summary>
    public class BrainpackConnectionController : MonoBehaviour
    {
        public delegate void BpConnectionControllerDel();

        private static BrainpackConnectionController mInstance;
        public static BpConnectionControllerDel ConnectingStateEvent;
        public static BpConnectionControllerDel ConnectedStateEvent;
        public static BpConnectionControllerDel DisconnectedStateEvent;
        public static BpConnectionControllerDel FailedToConnectStateEvent;
        public OutterThreadToUnityTrigger BrainpackConnectedTrigger = new OutterThreadToUnityTrigger();
        public OutterThreadToUnityTrigger SocketClientErrorTrigger = new OutterThreadToUnityTrigger();

        public string Output = "";
        [SerializeField]
        private BrainpackConnectionState mCurrentConnectionState = BrainpackConnectionState.Idle;
     
        public string BrainpackComPort = "COM15";
        [SerializeField]
        private int vTotalTries;

        public float Timeout =4;
        public int MaxConnectionAttempts = 4; 
        private GameObject mLoadingScreen;
        private BodyFrameThread mBodyFrameThread;

        private GameObject LoadingScreen
        {
            get
            {
                if (mLoadingScreen == null)
                {
                    GameObject vCanvas = GameObject.FindGameObjectWithTag("UiCanvas");
                    mLoadingScreen = Instantiate(PrefabRepo.LoadingScreenPrefab);
                    mLoadingScreen.transform.SetParent(vCanvas.transform, false);
                    mLoadingScreen.SetActive(false);
                    mLoadingScreen.transform.SetAsLastSibling();
                }
                return mLoadingScreen;
            }
        }

        /// <summary>
        /// returns the current state of the controller
        /// </summary>
        /**
        * ConnectionState 
        * @brief returns the current state of the controller
        */

        public BrainpackConnectionState ConnectionState
        {
            get { return mCurrentConnectionState; }
        }
        private BrainpackConnectionView mView;
        /// <summary>
        /// Returns the view associated with the controller
        /// </summary>
        /**
        * View 
        * @brief Returns the view associated with the controller
        */
        public BrainpackConnectionView View
        {
            get
            {
                if (mView == null)
                {
                    GameObject vCanvas = GameObject.FindGameObjectWithTag("UiCanvas");
                    GameObject vInstantiated = Instantiate(PrefabRepo.BrainpackConnectionViewPrefab);
                    vInstantiated.transform.SetParent(vCanvas.transform, false);
                    vInstantiated.transform.SetAsLastSibling();
                    mView = vInstantiated.GetComponent<BrainpackConnectionView>();
                }
                return mView;
            }
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        /**
        * View 
        * @brief Singleton instance
        */
        public static BrainpackConnectionController Instance
        {
            get
            {
                if (mInstance == null)
                {
                    GameObject vGo = GameObject.FindGameObjectWithTag("Controllers");
                    mInstance = vGo.GetComponent<BrainpackConnectionController>();
                    if (mInstance == null)
                    {
                        mInstance = vGo.AddComponent<BrainpackConnectionController>();
                    }
                    DontDestroyOnLoad(mInstance.gameObject);
                }
                return mInstance;
            }
        }


        /// <summary>
        /// Request to connect to brainpack
        /// </summary>
        /**
        * ConnectToBrainpack() 
        * @brief Request to connect to brainpack
        */
        public void ConnectToBrainpack()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.RequestToConnectToBP, BrainpackComPort);
            ChangeCurrentState(BrainpackConnectionState.Connecting);
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }

        /// <summary>
        /// FailedToConnect listener: listen to events that the brainpack failed to connect
        /// </summary>
        /// <param name="vArgs">The failure message</param>
        /**
        * FailedToConnectListener(string vArgs) 
        * @brief FailedToConnect listener: listen to events that the brainpack failed to connect
        * @param string vArgs: The failure message
        */
        internal void FailedToConnectListener(string vArgs)
        {
            SocketClientErrorTrigger.Triggered = true;
            SocketClientErrorTrigger.Args = vArgs;
           
        }
        /// <summary>
        /// The result of the brainpack connection
        /// </summary>
        /// <param name="vConnectionRes">The connection result</param>
        /**
        * BrainpackConnectionResult(bool vConnectionRes)
        * @brief The result of the brainpack connection
        * @param  bool vConnectionRes: The connection result
        */
        public void BrainpackConnectionResult(bool vConnectionRes)
        {
            if (!vConnectionRes)
            {
                //check if the max connection attempt has not been achieved
                if (vTotalTries < MaxConnectionAttempts)
                {
                    StartCoroutine(WaitAndThenReconnect(Timeout));
                    ChangeCurrentState(BrainpackConnectionState.Connecting);
                }
                else
                {
                    ChangeCurrentState(BrainpackConnectionState.Failure);
                }
            }
            else
            {
                vTotalTries = 0;
                ChangeCurrentState(BrainpackConnectionState.Connected);

            }
        }
        /// <summary>
        /// Changes the scene to the main 3d scene
        /// </summary>
        /**
        * ChangeTo3DSceneView()
        * @brief Changes the scene to the main 3d scene
        */
        private void ChangeTo3DSceneView()
        {
            LoadingScreen.SetActive(true);
            StartCoroutine(LoadMainScene());
        }

        /// <summary>
        /// Helper method that asynchrounously loads the main 3d scene
        /// </summary>
        /// <returns></returns>
        /**
        * LoadMainScene()
        * @brief Helper method that asynchrounously loads the main 3d scene
        */
        private IEnumerator LoadMainScene()
        {
            AsyncOperation async = Application.LoadLevelAsync(1);
            yield return async;
            LoadingScreen.SetActive(false);
        }

        /// <summary>
        /// Wait and then try to reconnect
        /// </summary>
        /// <param name="vSeconds">the number of seconds to wait</param>
        /// <returns></returns>
        /**
        * WaitAndThenReconnect(float vSeconds)
        * @brief Wait and then try to reconnect
        * @param float vSeconds: the number of seconds to wait
        */
        private IEnumerator WaitAndThenReconnect(float vSeconds)
        {
            yield return new WaitForSeconds(vSeconds);
            ConnectToBrainpack();
            vTotalTries++;
        }
        /// <summary>
        /// Change the current state of the controller and send out appropriate events
        /// </summary>
        /// <param name="vNewState">The new state to change to</param>
        /**
        * ChangeCurrentState(BrainpackConnectionState vNewState))
        * @brief Change the current state of the controller and send out appropriate events
        * @param BrainpackConnectionState vNewState: The new state to change to
        */
        private void ChangeCurrentState(BrainpackConnectionState vNewState)
        {
            switch (mCurrentConnectionState)
            {
                case BrainpackConnectionState.Idle:
                    {
                        if (vNewState == BrainpackConnectionState.Connecting)
                        {
                            if (ConnectingStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                ConnectingStateEvent(); 
                            } 
                        }
                        break;
                    }
                case BrainpackConnectionState.Connecting:
                    {
                        if (vNewState == BrainpackConnectionState.Failure)
                        {
                            if (FailedToConnectStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                FailedToConnectStateEvent();
                            }
                          
                            break;
                        }
                        if (vNewState == BrainpackConnectionState.Connected)
                        {
                            if (ConnectedStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                ConnectedStateEvent();
                                //start pulling data
                            }
                        
                        }
                        break;
                    }
                case BrainpackConnectionState.Connected:
                    {
                        if (vNewState == BrainpackConnectionState.Disconnected)
                        {
                            if (DisconnectedStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                DisconnectedStateEvent();
                            } 
                            break;
                        }
                        if (vNewState == BrainpackConnectionState.Connecting)
                        {
                            if (ConnectingStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                ConnectingStateEvent();
                            } 
                        }
  

                        break;
                    }
                case BrainpackConnectionState.Failure:
                    {
                        if (vNewState == BrainpackConnectionState.Connecting)
                        {
                            if (ConnectingStateEvent != null)
                            {
                                mCurrentConnectionState = vNewState;
                                ConnectingStateEvent();
                            } 
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// Initialize the instance
        /// </summary>
        /**
        * Init()
        * @brief Initialize the instance 
        */
        private void Init()
        {

        }
        #region unity functions
        /// <summary>
        /// on Awake: Initialize the instance 
        /// </summary>
        /**
        * Awake()
        * @brief on Awake: Initialize the instance 
        */
        void Awake()
        {
            Instance.Init();
        }
        /// <summary>
        /// On application quit, stop the socket client
        /// </summary>
        /**
        * OnApplicationQuit()
        * @brief On application quit, stop the socket client
        */
        void OnApplicationQuit()
        {
            HeddokoPacket vHeddokoPacket = new HeddokoPacket(HeddokoCommands.StopHeddokoUnityClient, "");
            PacketCommandRouter.Instance.Process(this, vHeddokoPacket);
        }
        /// <summary>
        /// reset the number of reconnect tries
        /// </summary>
        /**
        * OnApplicationQuit()
        * @brief reset the number of reconnect tries
        */
        public void ResetTries()
        {
            vTotalTries = 0;
        }
        /// <summary>
        /// The update function monitors the connected flag and starts to pull data
        /// </summary>
        /**
        * Update()
        * @brief The update function monitors the connected flag and starts to pull data
        */
        void Update()
        {
            if (BrainpackConnectedTrigger.Triggered)
            {
                BrainpackConnectionResult(BrainpackConnectedTrigger.InterestedVariable);
                BrainpackConnectedTrigger.Reset();
            }
            if (SocketClientErrorTrigger.Triggered)
            {
                SocketClientErrorTrigger.Reset();
                View.SetWarningBoxMessage((string)SocketClientErrorTrigger.Args);
                ChangeCurrentState(BrainpackConnectionState.Disconnected); 
            }
            //start pulling data
            if (mCurrentConnectionState == BrainpackConnectionState.Connected)
            {
                HeddokoPacket vRequestBrainpackData = new HeddokoPacket(HeddokoCommands.RequestBPData, "Requesting data");
                PacketCommandRouter.Instance.Process(this, vRequestBrainpackData);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                Instance.ChangeTo3DSceneView();
            }
        }
        #endregion
        /**
        * ReadyToLinkBodyToBP(BodyFrameThread vBodyFrameThread) 
        * @brief Sets the body as being ready to connect to the brainpack, once the brainpack is connected, the body will stream the brainpack
        * @param BodyFrameThread vBodyFrameThread:The body frame thread to connect to
        */
        /// <summary>
        /// Sets the body as being ready to connect to the brainpack, once the brainpack is connected, the body will stream the brainpack
        /// </summary>
        /// <param name="vBodyFrameThread">The body frame thread to connect to </param>
        public void ReadyToLinkBodyToBP(BodyFrameThread vBodyFrameThread)
        {
            PacketCommandRouter.Instance.SetBodyFrameThread(vBodyFrameThread);
        }

/*        /**
        * RequestNewConnectionFromBody() 
        * @brief Sets the body as being ready to connect to the brainpack, once the brainpack is connected, the body will stream the brainpack
        * @param BodyFrameThread vBodyFrameThread:The body frame thread to connect to
        #1#
        public void RequestNewConnectionFromBody()
        {
            //reset tries first
            ResetTries();
            ConnectToBrainpack();

        }*/

        /// <summary>
        /// Disconnects the current body from the brainpack
        /// </summary>
        public void DisconnectBodyFromBP()
        {
            PacketCommandRouter.Instance.DisconnectFrameThread();
        }
    }
 


    public enum BrainpackConnectionState
    {
        Idle,
        Connecting,
        Connected,
        Disconnected,
        Failure
    }
}

