
/** 
* @file HeddokoDebugKeyMappings.cs
* @brief Contains the HeddokoDebugKeyMappings class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
namespace Assets.Scripts.Utils.DebugContext
{
    /// <summary>
    /// keycodes for different functions
    /// </summary>
  public  static class HeddokoDebugKeyMappings
  {
        public static KeyCode ResetFrame = KeyCode.Home;
        public static KeyCode Pause = KeyCode.P;
        public static KeyCode MoveNext = KeyCode.RightArrow;
        public static KeyCode MoveToPrev = KeyCode.LeftArrow;
        public static KeyCode SkipToLiveViewFromRecordingView = KeyCode.S;
        public static KeyCode IsTrackingHeight = KeyCode.Alpha0;
        public static KeyCode IsHipsEstimateForward = KeyCode.Alpha9;
        public static KeyCode IsHipsEstimateUp = KeyCode.Alpha8;
        public static KeyCode IsUsingInterpolationForBody = KeyCode.Alpha7;
        public static KeyCode IncBodyInterpolationSp = KeyCode.Alpha2;
        public static KeyCode DecBodyInterpoationSp = KeyCode.Alpha1;
    

    
    }
}
