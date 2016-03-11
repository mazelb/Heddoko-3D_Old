﻿/** 
* @file SuitStateChangerControl.cs
* @brief Contains the SuitStateChangerControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date Macrch 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/ 
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.Enums; 
using UnityEngine.UI;

namespace Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls
{
    /// <summary>
    /// contains a button control which allows the state change of the suit
    /// </summary>
    public class SuitStateChangerControl : AbstractSuitsSubControl
    {
        public Button SuitStateControl;
        public Text ControlText;
        private string mWaitingText = "Waiting for response";
        private string mInRecordingStateTxt = "Stop Recording";
        private string mInIdleStateTxt = "Start Recording";
        private string mInErrorStateTxt = "Error: click to reset";
        private string mInResetStateTxt = "Reinitializing..";
        private string mInDisconnectStatetxt = "Disconnected state";
        public ColorBlock RecordingStateColorBlock;
        public ColorBlock IdleStateBlock;
        public ColorBlock InErrorStateBlock;
        public ColorBlock ResetStateBlock;
        public ColorBlock DisconnectedBlock;
        private static SubControlType sType = SubControlType.SuitStateModifierSubControl;

        void Awake()
        {
            SuitStateControl.onClick.AddListener(EngageControl);
            //check current state of the suit
            if (mIsConnectedToSuit)
            {
                OnStatusUpdate(SuitState);
            }
            else
            {
                OnDisconnect();
            }
        }

        /// <summary>
        /// In a disconnected state, set the controls to disconnected
        /// </summary>
        public override void OnDisconnect()
        {
            SuitStateControl.interactable = false;
            SuitStateControl.colors = DisconnectedBlock;
            ControlText.text = mInDisconnectStatetxt;
            base.OnDisconnect();
        }

        public override void OnStatusUpdate(SuitState vSuitState)
        {
            //dont change states to the current state
            if (vSuitState == SuitState)
            {
                return;
            }
            switch (vSuitState)
            {
                case SuitState.Error:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = InErrorStateBlock;
                    ControlText.text = mInErrorStateTxt;
                    break;
                case SuitState.Idle:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = IdleStateBlock;
                    ControlText.text = mInIdleStateTxt;
                    break;
                case SuitState.Reset:
                    SuitStateControl.interactable = false;
                    SuitStateControl.colors = ResetStateBlock;
                    ControlText.text = mInResetStateTxt;
                    break;
                case SuitState.Recording:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = RecordingStateColorBlock;
                    ControlText.text = mInRecordingStateTxt;
                    break;
                case SuitState.Undefined:
                    //todo
                    break;

            }
            SuitState = vSuitState;
        }
        

        
        /// <summary>
        /// 
        /// </summary>
        public override void OnConnection()
        {
            SuitStateControl.interactable = true;
            WaitForStatusResponse();
        }
 

        private void EngageControl()
        {
            switch (SuitState)
            {
                case SuitState.Error:
                    WaitForStatusResponse();
                    SuitConnection.InitiateSuitReset();
                    break;
                case SuitState.Idle:
                    WaitForStatusResponse();
                    SuitConnection.InitiateSuitRecording();
                    break;
                case SuitState.Reset:
                    SuitStateControl.interactable = false;
                    SuitStateControl.colors = ResetStateBlock;
                    ControlText.text = mInResetStateTxt;
                    break;
                case SuitState.Recording:
                    WaitForStatusResponse();
                    SuitConnection.InitateStopRecordingReq();
                    break;
                case SuitState.Undefined:
                    //todo
                    break;
            }
        }

        /// <summary>
        /// Waiting on status change update
        /// </summary>
        private void WaitForStatusResponse()
        {
            ControlText.text = mWaitingText;
            SuitStateControl.interactable = false;
        }

        public override SubControlType SubControlType
        {
            get { return sType; }
        }

        public override void Disable()
        {
           
        }

        public override void Enable()
        {
            
        }

        private void OnDisable()
        {
            SuitState = SuitState.Start;
        }
    }


}
