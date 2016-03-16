
/** 
* @file OutterThreadToUnityThreadIntermediary.cs
* @brief Contains the OutterThreadToUnityThreadIntermediary class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic; 
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// This helper class allows for non unity-based threads to send messages to the main unity thread by enqueing an action
    /// </summary>
   public class OutterThreadToUnityThreadIntermediary : MonoBehaviour
    {
        private static OutterThreadToUnityThreadIntermediary sInstance; 
        private Queue<Action> Queue = new Queue<Action>();
        private Thread mUnityThread;
        public static float FrameDeltaTime { get; private set; }
    
        public static OutterThreadToUnityThreadIntermediary Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType<OutterThreadToUnityThreadIntermediary>();
                    if (sInstance == null)
                    {
                        GameObject vGo = new GameObject();
                        vGo.name = "Non unity thread to unity thread helper";
                        sInstance = vGo.AddComponent<OutterThreadToUnityThreadIntermediary>();
                    }
                } 
                return sInstance;
            }
        }
        /// <summary>
        /// Unity time
        /// </summary>
        public static float UnityTime { get; private set; }

        public string ApplicationPath { get; set; }

        void Awake()
        {
           
        }

        /// <summary>
        /// Initialize parameters
        /// </summary>
        public void Init()
        {
            mUnityThread = Thread.CurrentThread;
            ApplicationPath = Application.dataPath;
        }
     

        /// <summary>
        /// checks if the current passed in thread is a unity thread
        /// </summary>
        /// <param name="vThread"></param>
        /// <returns></returns>
        public static bool InUnityThread( )
        {
            return Instance.mUnityThread.Equals(Thread.CurrentThread);
        }
 
        public static Coroutine HelpStartCoroutine(IEnumerator vCoroutine)
        {
         return   Instance.StartCoroutine(vCoroutine);
        }

        public static void TriggerActionInUnity(Action vAction)
        {
           Instance.Queue.Enqueue(vAction);
        }

        void Update()
        {
            if(Queue.Count> 0 )
            {
                Action vAction = Queue.Dequeue();
                if (vAction != null)
                {
                        vAction.Invoke();
                   
                }
                else
                {
                    print("found null action!");
                }
            }
            FrameDeltaTime = Time.deltaTime;
            UnityTime = Time.time;
        }

 
    }
}
