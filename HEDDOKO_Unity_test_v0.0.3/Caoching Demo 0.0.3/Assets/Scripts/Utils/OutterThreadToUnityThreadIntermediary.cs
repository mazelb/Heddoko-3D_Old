﻿
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

        /// <summary>
        /// this is used when a thread needs to communicate a message to the unity thread but 
        /// it can be overwritten by subsequent message types
        /// </summary>
        private Dictionary<string, Action> mOverWrittableActionQueue = new Dictionary<string, Action>();
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
            ApplicationPath = Application.persistentDataPath;
        }


        /// <summary>
        /// checks if the current passed in thread is a unity thread
        /// </summary>
        /// <param name="vThread"></param>
        /// <returns></returns>
        public static bool InUnityThread()
        {
            return Instance.mUnityThread.Equals(Thread.CurrentThread);
        }

        public static Coroutine HelpStartCoroutine(IEnumerator vCoroutine)
        {
            return Instance.StartCoroutine(vCoroutine);
        }

        /// <summary>
        /// Queues up an action in unity
        /// </summary>
        /// <param name="vAction"></param>
        public static void QueueActionInUnity(Action vAction)
        {
            Instance.Queue.Enqueue(vAction);
        }

        public static void EnqueueOverwrittableActionInUnity(string vKey, Action vAction)
        {
            if (Instance.mOverWrittableActionQueue.ContainsKey(vKey))
            {
                Instance.mOverWrittableActionQueue[vKey] = vAction;
            }
            else
            {
                Instance.mOverWrittableActionQueue.Add(vKey, vAction);
            }
        }

        void Update()
        {
            if (mOverWrittableActionQueue.Count > 0)
            {
                List<string> vKeys = new List<string>(mOverWrittableActionQueue.Keys);
                for (int i = 0; i < vKeys.Count; i++)
                {
                    lock (mOverWrittableActionQueue)
                    {
                        mOverWrittableActionQueue[vKeys[i]].Invoke();
                        mOverWrittableActionQueue.Remove(vKeys[i]);
                    }
                }
            }
            if (Queue.Count > 0)
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
