///*@file RestartBrainpackServiceTest.cs
//* @brief Contains the RestartBrainpackServiceTest class
//* @author Mohammed Haider(mohammed@heddoko.com)
//* @date March 2016
//* Copyright Heddoko(TM) 2016, all rights reserved
//*/

//using System;
//using System.Collections;
//using System.ServiceProcess;
//using UnityEngine;

//namespace Assets
//{

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

//        /// <summary>
//        /// Restart the bp service. Timeout before failing
//        /// </summary>
//        /// <param name="vTimeout"></param>
//        /// <returns></returns>
//        private IEnumerator RestartBpService(float vTimeout)
//        {

//            float vRemainingTime = vTimeout;

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

//    }
//}
