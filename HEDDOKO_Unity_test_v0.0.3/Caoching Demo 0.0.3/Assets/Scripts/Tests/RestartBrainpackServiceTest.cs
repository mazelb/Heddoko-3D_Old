///*@file RestartBrainpackServiceTest.cs
//* @brief Contains the RestartBrainpackServiceTest class
//* @author Mohammed Haider(mohammed@heddoko.com)
//* @date March 2016
//* Copyright Heddoko(TM) 2016, all rights reserved
//*/

<<<<<<< HEAD
using System;
using System.Collections;
using System.ServiceProcess;
using Assets.Scripts.Communication.Controller;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI;
=======
//using System;
//using System.Collections;
//using System.ServiceProcess;
//using UnityEngine;
>>>>>>> 429b6a317bc8b12842275a01c687223d34e24d22

//namespace Assets
//{

<<<<<<< HEAD
    public class RestartBrainpackServiceTest : MonoBehaviour
    {
        public Button RestartServiceButton;
        private ServiceController mServiceController;
        
        public void Awake()
        {
            mServiceController = new ServiceController("BrainpackService");
            RestartServiceButton.onClick.AddListener(RestartBrainpackService);
        }
        public void RestartBrainpackService()
        {
            RestartServiceButton.interactable = false;
            BrainpackConnectionController.Instance.ChangeCurrentState(BrainpackConnectionState.Disconnected);
            StartCoroutine(RestartBpService(2));
        }
=======
//    public class RestartBrainpackServiceTest : MonoBehaviour
//    {
//        private ServiceController mServiceController;

//        public void Awake()
//        {
//            mServiceController = new ServiceController("BrainpackService");
//            RestartBrainpackService();
//        }
//        public void RestartBrainpackService()
//        {
//            StartCoroutine(RestartBpService(2));
//        }
>>>>>>> 429b6a317bc8b12842275a01c687223d34e24d22

//        /// <summary>
//        /// Restart the bp service. Timeout before failing
//        /// </summary>
//        /// <param name="vTimeout"></param>
//        /// <returns></returns>
//        private IEnumerator RestartBpService(float vTimeout)
//        {

//            float vRemainingTime = vTimeout;

<<<<<<< HEAD
            bool mStopSuccess = false;
            bool mServiceStartSuccess = false;
            try
            {
                mServiceController.Stop();
                mStopSuccess = true;
            }
            catch (Exception vE)
            {
                Debug.Log("Problem stopping" + vE);
                mStopSuccess = false; 
            }

            if (mStopSuccess)
            {
                while (vRemainingTime >= 0)
                {
                    vRemainingTime -= Time.time;
                    //check status
                    if (mServiceController.Status == ServiceControllerStatus.Stopped)
                    {
                        Debug.Log("Service stopped.");
                        mStopSuccess = true;
                        yield break;
                    }
                    yield return null;
                }
            }
            try
            {
                if (mStopSuccess)
                {
                    mServiceController.Start();
                    mServiceStartSuccess = true;
                    vRemainingTime = vTimeout;
                }
            }
             
            catch (Exception vE)
            {
                mServiceStartSuccess = false;
                Debug.Log("Problem starting" + vE);
                throw;
            }
            if (mServiceStartSuccess && mStopSuccess)
            {
                while (vRemainingTime >= 0)
                {
                    vRemainingTime -= Time.time;
                    //check status
                    if (mServiceController.Status == ServiceControllerStatus.Running)
                    {
                        Debug.Log("Service started again.");
                        yield break;
                    }
                    yield return null;
                }
            }
            if (!mStopSuccess)
            {
                var message =
               "there was a problem with stopping the brainpack service. Verify that you are running the application in an administrative capacity or try again";
                Notify.Template("FadingNotifyTemplate").Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First, clearSequence: true);
            }
            if (!mServiceStartSuccess)
            {
                var message =
               "there was a problem with starting the brainpack service. Verify that you are running the application in an administrative capacity or try again";
                Notify.Template("FadingNotifyTemplate").Show(message, 4.5f, hideAnimation: Notify.FadeOutAnimation, showAnimation: Notify.FadeInAnimation, sequenceType: NotifySequence.First, clearSequence: true);
            }
            RestartServiceButton.interactable = true;
        }
=======
//            bool mStopSuccess = false;
//            bool mServiceStartSuccess = false;
//            try
//            {
//                mServiceController.Stop();
//                mStopSuccess = true;
//            }
//            catch (Exception vE)
//            {

//                Debug.Log("Problem stopping" + vE);
//            }

//            if (mStopSuccess)
//            {
//                while (vRemainingTime >= 0)
//                {
//                    vRemainingTime -= Time.time;
//                    //check status
//                    if (mServiceController.Status == ServiceControllerStatus.Stopped)
//                    {
//                        Debug.Log("Service stopped.");
//                        yield break;
//                    }
//                    yield return null;
//                }
//            }
//            try
//            {
//                mServiceController.Start();
//                mServiceStartSuccess = true;
//                vRemainingTime = vTimeout;
//            }
//            catch (Exception vE)
//            {

//                Debug.Log("Problem starting" + vE);
//            }
//            if (mServiceStartSuccess)
//            {
//                while (vRemainingTime >= 0)
//                {
//                    vRemainingTime -= Time.time;
//                    //check status
//                    if (mServiceController.Status == ServiceControllerStatus.Running)
//                    {
//                        Debug.Log("Service started again.");
//                        yield break;
//                    }
//                    yield return null;
//                }
//            }
//        }
>>>>>>> 429b6a317bc8b12842275a01c687223d34e24d22

//    }
//}
