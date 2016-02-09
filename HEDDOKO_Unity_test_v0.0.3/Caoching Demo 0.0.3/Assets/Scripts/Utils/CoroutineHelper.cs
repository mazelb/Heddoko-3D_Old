
/** 
* @file CoroutineHelper.cs
* @brief Contains the CoroutineHelper class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    /// <summary>
    /// a couroutine helper that helps non-monobehaviour classes with starting a coroutine
    /// </summary>
   public class CoroutineHelper : MonoBehaviour
    {
        private static CoroutineHelper sInstance; 
        private Queue<Action> Queue = new Queue<Action>();
        private Thread mUnityThread;
    
        public static CoroutineHelper Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType<CoroutineHelper>();
                    if (sInstance == null)
                    {
                        GameObject vGo = new GameObject();
                        vGo.name = "Non unity thread to unity thread helper";
                        sInstance = vGo.AddComponent<CoroutineHelper>();
                    }
                }
                return sInstance;
            }
        }
        public void Start()
        {
            mUnityThread = Thread.CurrentThread;
        }

        public static bool IsUnityThread(Thread vThread)
        {
            return vThread.Equals(Instance.mUnityThread);
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
        }

 
    }
}
