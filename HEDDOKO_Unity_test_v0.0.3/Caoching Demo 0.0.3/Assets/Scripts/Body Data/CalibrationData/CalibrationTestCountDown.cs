
/** 
* @file GlobalCalibrationSettings.cs
* @brief Contains the GlobalCalibrationSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;

namespace Assets.Scripts.Body_Data.CalibrationData
{

 public class CalibrationTestCountDown: AbstractSuitsSubControl
    {

     private SubControlType mSubControl = SubControlType.AddCommentSubControl;
     private SuitState mPreviousSuitState = SuitState.Undefined;

    
     void Awake()
     {
         
     }
     public override SubControlType SubControlType
     {
         get { return mSubControl; }
     }

     public override void Disable()
     {
         gameObject.SetActive(false);
     }

     public override void Enable()
     {
            gameObject.SetActive(true);
        }

     public override void OnStatusUpdate(SuitState vSuitState)
     {
        //check what the previous state the suit was in
     }

     public override void OnConnection()
     {
         throw new System.NotImplementedException();
     }
    }
}
