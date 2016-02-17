
using System;
using System.Collections.Generic;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Scene_3d.View;
using Assets.Scripts.UI.Settings;
using UnityEngine;

namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// HeddokoAppStart:  This class is the starting point of the application, performing a variety of checks before the application can be
    /// launched.
    /// </summary>
    public class HeddokoAppStart : MonoBehaviour
    {
        private DBAccess mDbAccess;
        public GameObject[] GOtoReEnable;
        public ScrollablePanel ContentPanel; 

  
        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            BodySegment.IsTrackingHeight = false;
            bool vAppSafelyLaunched;


            EnableObjects(false);
            //Start loading animation
            LoadingBoard.StartLoadingAnimation();

            mDbAccess = new DBAccess();

            bool vApplicationSettingsFound = mDbAccess.SetApplicationSettings();
/*
#if UNITY_EDITOR
            vApplicationSettingsFound = true;
#endif
*/

            if (vApplicationSettingsFound)
            {
                vAppSafelyLaunched = ApplicationSettings.AppLaunchedSafely;
                if (vAppSafelyLaunched)
                {
                    string vGet = ApplicationSettings.PreferedConnName;
                    LauncherBrainpackSearchResults.MapResults();
                    if (LauncherBrainpackSearchResults.BrainpackToComPortMappings.ContainsKey(vGet))
                    {
                        vGet = LauncherBrainpackSearchResults.BrainpackToComPortMappings[vGet];
                    }
                    else
                    {
                        vGet = string.Empty;
                    }
                   
                    BrainpackConnectionController.Instance.BrainpackComPort = vGet;
                    LoadingBoard.StopLoadingAnimation();
                    List<ScrollableContent> vContentList = new List<ScrollableContent>();

                    foreach (KeyValuePair<string, string> vKV in LauncherBrainpackSearchResults.BrainpackToComPortMappings)
                    {

                        ScrollableContent vContent = new ScrollableContent();
                        vContent.ContentValue = vKV.Value;
                        if (vContent.ContentValue.Contains("COM") || vContent.ContentValue.Contains("com"))
                        {
                            vContent.Key = vKV.Key;
                            vContent.CallbackAction = new Action(() =>
                            {
                                BrainpackConnectionController.Instance.BrainpackComPort = vContent.ContentValue;
                            });
                            vContentList.Add(vContent);
                            ContentPanel.CurrentlySelectedContent = vContent;
                        }

                    }
                    ContentPanel.Contents = vContentList;
                    EnableObjects(true);
                }
                else
                {
                    AppNotLaunchedThroughLauncher();
                }
            }


        }


        /// <summary>
        /// Enables or disable the array of gameobjects 
        /// </summary>
        /// <param name="vFlag"></param>
        void EnableObjects(bool vFlag)
        {
            foreach (var vGo in GOtoReEnable)
            {
                vGo.SetActive(vFlag);
            }
        }

        /// <summary>
        /// The application wasn't started with the launcher. Display a message then quit the application
        /// </summary>
        private void AppNotLaunchedThroughLauncher()
        {
            LoadingBoard.StopLoadingAnimation();
            ModalPanel.SingleChoice("The application wasn't started with the Launcher. Press Ok to exit and try again. dbloc ", Application.Quit);
        }
    }
}
