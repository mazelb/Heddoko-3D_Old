﻿/** 
* @file HeddokoAppStart.cs
* @brief Contains the HeddokoAppStart class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic; 
using Assets.Scripts.Body_Data.View;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.camera;
using Assets.Scripts.UI.Loading;
using Assets.Scripts.UI.ModalWindow;
using Assets.Scripts.UI.Scene_3d.View;
using Assets.Scripts.UI.Settings;
using Assets.Scripts.UI.Tagging;
using Assets.Scripts.Utils.DebugContext.logging;
using UnityEngine;
using Application = UnityEngine.Application;

namespace Assets.Scripts.Utils.DatabaseAccess
{
    /// <summary>
    /// HeddokoAppStart:  This class is the starting point of the application, 
    /// performing a variety of checks before the application can be launched.
    /// </summary>
    public class HeddokoAppStart : MonoBehaviour
    {
        private LocalDBAccess mDbAccess;
        private Database mDatabase;
        private TaggingManager mTaggingManager;
        public GameObject[] GOtoReEnable;
        public GameObject[] DatabaseConsumers;
        public GameObject[] TaggingManagerConsumers;
        public ScrollablePanel ContentPanel;

        // ReSharper disable once UnusedMember.Local
        void Awake()
        {
            BodySegment.IsTrackingHeight = false;
            OutterThreadToUnityThreadIntermediary.Instance.Init();
            mTaggingManager =new TaggingManager();
            InitiliazePools();
            InitializeDatabase();
            InjectDatabaseDependents();
            InjectTaggingManagerDependents(); 
            InitializeLoggers();

            bool vAppSafelyLaunched;
            EnableObjects(false);
            //Start loading animation
            LoadingBoard.StartLoadingAnimation();

            mDbAccess = new LocalDBAccess();

            bool vApplicationSettingsFound = mDbAccess.SetApplicationSettings();


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
        /// Injects the single database component into interested consumers
        /// </summary>
        private void InjectDatabaseDependents()
        {
            mTaggingManager.Database = mDatabase;
            foreach (var vDbConsumer in DatabaseConsumers)
            {
                //attempt to grab the database consumer interface from the gameobject
                IDatabaseConsumer vConsumer = vDbConsumer.GetComponent<IDatabaseConsumer>();
                if (vConsumer != null)
                {
                    vConsumer.Database = mDatabase;
                }
            }
        }
        /// <summary>
        ///Injects tagging manager dependents with a tagging manager object
        /// </summary>
        private void InjectTaggingManagerDependents()
        {
            foreach (var vDependent in TaggingManagerConsumers)
            {
                ITaggingManagerConsumer vConsumer = vDependent.GetComponent<ITaggingManagerConsumer>();
                vConsumer.TaggingManager = mTaggingManager;
            }
        }

        private void InitializeLoggers()
        {
            DebugLogger.Instance.Start();

#if !DEBUG
            DebugLogger.Settings.AllFalse();
#endif

        }

        /// <summary>
        /// Sets up internal pools
        /// </summary>
        private void InitiliazePools()
        {
            GameObject vRenderedBodyGroup = GameObject.FindWithTag("RenderedBodyGroup");
            GameObject vPanelCameraGroup = GameObject.FindWithTag("PanelCameraGroup");

            RenderedBodyPool.ParentGroupTransform = vRenderedBodyGroup.transform;
            PanelCameraPool.CameraParent = vPanelCameraGroup.transform;
        }

        void Start()
        {
            UniFileBrowser.use.SetPath(ApplicationSettings.PreferedRecordingsFolder);
        }
        /// <summary>
        /// Enables or disable the array of gameobjects 
        /// </summary>
        /// <param name="vFlag"></param>
        void EnableObjects(bool vFlag)
        {
            foreach (var vGo in GOtoReEnable)
            {
                if (vGo != null)
                {
                    vGo.SetActive(vFlag);
                }
            }
        }

        /// <summary>
        /// The application wasn't started with the launcher. Display a message then quit the application
        /// </summary>
        private void AppNotLaunchedThroughLauncher()
        {
            LoadingBoard.StopLoadingAnimation();
            ModalPanel.SingleChoice("The application wasn't started with the Launcher. Press Ok to exit and try again. ", Application.Quit);
        }

        void OnApplicationQuit()
        {
            // mDatabase.CleanUp();
            DebugLogger.Instance.Stop();
        }

        /// <summary>
        /// Initialize the database 
        /// </summary>
        private void InitializeDatabase()
        {
            mDatabase = new Database(DatabaseConnectionType.Local);
            mDatabase.Init();
           
        }


    }
}
