/** 
* @file PlaybackControlPool.cs
* @brief Contains the PlaybackControlPool  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.Utils;
using UnityEngine.UI;

namespace Assets.Scripts.UI.PlaybackAndRecording
{
    /// <summary>
    /// Provides a PlaybackControl object from a pool
    /// </summary>
   public static class PlaybackControlPool
    {
        private static object sAvControlsLock = new object();
        private static List<PlaybackControl> sAvailableControls = new List<PlaybackControl>();
        private static List<PlaybackControl> sInUse = new List<PlaybackControl>();

        /// <summary>
        /// Returns a playback control that is available to be used
        /// </summary>
        public static PlaybackControl GetPlaybackControl
        {
            get
            {
                PlaybackControl vPooledObject = null;
             
               
                //Check if the unity3d main thread is the current caller
               bool vIsUnityThread = OutterThreadToUnityThreadIntermediary.IsUnityThread(Thread.CurrentThread);
                lock (sAvControlsLock)
                {
                    if (sAvailableControls.Count != 0)
                    {
                        //pop the first element
                        vPooledObject = sAvailableControls[0];
                        sAvailableControls.RemoveAt(0);
                        sInUse.Add(vPooledObject);
                    }
                    //create an action,depending on if the current thread is the unity thread
                    //or a non-unity thread. Call the intermediary to trigger the action safely in unity
                    //This is not a synchronized action.
                    else
                    {
                        Action vCreateAction = () =>
                        {

                        };
                    }
                }
                return vPooledObject;
            }
        }
    }
}
