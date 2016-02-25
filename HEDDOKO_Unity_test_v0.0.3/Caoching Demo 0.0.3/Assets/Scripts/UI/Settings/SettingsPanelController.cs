﻿/** 
* @file SettingsPanelController.cs
* @brief Contains the SettingsPanelController  class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.Collections;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.UI.Settings
{
    public class SettingsPanelController : AbstractView
    {
        public SettingsView SettingsView;
        public Camera UiCamera;
        private float mSettingsButtonTimer = 0.5f;
        private bool mSettingsButtonPressed = false;

 

 

        void Awake()
        {
            SettingsView.SettingsButton.onClick.AddListener(SettingsButtonPressed);
            InputHandler.RegisterActions(HeddokoDebugKeyMappings.SettingsButton, SettingsButtonPressed);
            BrainpackConnectionController.BrainpackStatusResponse += UpdateTextBox;
            BrainpackConnectionController.BrainpackTimeSetResp += GenericAckMsg;
            BrainpackConnectionController.ResetBrainpackResp += GenericAckMsg;
            BrainpackConnectionController.BrainpackShutdown += GenericAckMsg;
        

        }

        /// <summary>
        /// Generic response that prints out Ack
        /// </summary>
        private void GenericAckMsg()
        {
            SettingsView.UpdateStatusText("Ack"); 
        }
        /// <summary>
        ///   response that prints out the message passed in
        /// </summary>
        private void UpdateTextBox(string vMsg)
        {
            SettingsView. UpdateStatusText(vMsg); 
        }
        private void SettingsButtonPressed()
        {
            if (!mSettingsButtonPressed)
            {
                mSettingsButtonPressed = true;
                if (SettingsView.Panel.gameObject.activeSelf)
                {
                    HidePanel();
                }
                else
                {
                    ShowPanel();
                }

                StartCoroutine(EnableSettingsStateChange());
            }
        }

        public void ChangeBrainpackRecordingPrefix()
        {
          
        }

        private IEnumerator EnableSettingsStateChange()
        {
            yield return new WaitForSeconds(mSettingsButtonTimer);
            mSettingsButtonPressed = false;
        }
        public void HidePanel()
        {
            SettingsView.Hide();
        }

        public void ShowPanel()
        {
            SettingsView.Show();
        }

        public bool flag;
        /// <summary>
        /// Triggered by an in-game event, checks if the mouse was clicked outside the panel
        /// </summary>
        public void CheckIfMouseOutOfBounds()
        {
           
            bool vMousePointerInBounds = RectTransformUtility.RectangleContainsScreenPoint(
                 SettingsView.Panel.GetComponent<RectTransform>(),
                 Input.mousePosition,
                 UiCamera); 
            if (!vMousePointerInBounds)
            {
                HidePanel();
            }

        }
    }
}