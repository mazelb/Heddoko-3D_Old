﻿/** 
* @file SuitStateChangerControl.cs
* @brief Contains the SuitStateChangerControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date Macrch 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections;
using Assets.Scripts.Communication.Controller;
using UnityEngine;
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

        void Awake()
        {
            SuitStateControl.onClick.AddListener(EngageControl);
            //check current state of the suit
            if (mIsConnectedToSuit)
            {
                OnStatusUpdate(mCurrentSuitState);
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

        public override void OnStatusUpdate(CurrentSuitState vCurrentSuitState)
        {
            //dont change states to the current state
            if (vCurrentSuitState == mCurrentSuitState)
            {
                return;
            }
            switch (vCurrentSuitState)
            {
                case CurrentSuitState.Error:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = InErrorStateBlock;
                    ControlText.text = mInErrorStateTxt;
                    break;
                case CurrentSuitState.Idle:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = IdleStateBlock;
                    ControlText.text = mInIdleStateTxt;
                    break;
                case CurrentSuitState.Reset:
                    SuitStateControl.interactable = false;
                    SuitStateControl.colors = ResetStateBlock;
                    ControlText.text = mInResetStateTxt;
                    break;
                case CurrentSuitState.Recording:
                    SuitStateControl.interactable = true;
                    SuitStateControl.colors = RecordingStateColorBlock;
                    ControlText.text = mInRecordingStateTxt;
                    break;
                case CurrentSuitState.Undefined:
                    //todo
                    break;

            }
            mCurrentSuitState = vCurrentSuitState;
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
            switch (mCurrentSuitState)
            {
                case CurrentSuitState.Error:
                    WaitForStatusResponse();
                    SuitConnection.InitiateSuitReset();
                    break;
                case CurrentSuitState.Idle:
                    WaitForStatusResponse();
                    SuitConnection.InitiateSuitRecording();
                    break;
                case CurrentSuitState.Reset:
                    SuitStateControl.interactable = false;
                    SuitStateControl.colors = ResetStateBlock;
                    ControlText.text = mInResetStateTxt;
                    break;
                case CurrentSuitState.Recording:
                    WaitForStatusResponse();
                    SuitConnection.InitateStopRecordingReq();
                    break;
                case CurrentSuitState.Undefined:
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

    }


}