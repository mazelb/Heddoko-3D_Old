
/** 
* @file HeddokoDebugKeyMappings.cs
* @brief Contains the HeddokoDebugKeyMappings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System.Security.Policy;
using UnityEngine;
namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// keycodes for different functions
    /// </summary>
  public  static class HeddokoDebugKeyMappings
  {
        public static KeyCode ResetFrame = KeyCode.Home;
        public static KeyCode Pause = KeyCode.Menu;
        public static KeyCode MoveNext = KeyCode.RightArrow;
        public static KeyCode MoveToPrev = KeyCode.LeftArrow;
  
        public static KeyCode IsTrackingHeight = KeyCode.Alpha0;
        public static KeyCode IsHipsEstimateForward = KeyCode.Alpha9;
        public static KeyCode IsHipsEstimateUp = KeyCode.Alpha8;
        public static KeyCode IsUsingInterpolationForBody = KeyCode.Alpha7;
        public static KeyCode IsAdjustingArms = KeyCode.Alpha6;
        public static KeyCode IncBodyInterpolationSp = KeyCode.J;
        public static KeyCode DecBodyInterpoationSp = KeyCode.K ;
        public static KeyCode HideSegmentFlagPanel = KeyCode.H;
        public static KeyCode SwitchToRecordingFromLive = KeyCode.Escape;
        public static KeyCode SkipToLiveViewFromRecordingView = KeyCode.Return;
        public static KeyCode EnableTimerKey = KeyCode.PageUp;
        public static KeyCode ReturnToActivitySelection = KeyCode.DownArrow;
        public static KeyCode GoToFootballView = KeyCode.UpArrow;
  }
}
