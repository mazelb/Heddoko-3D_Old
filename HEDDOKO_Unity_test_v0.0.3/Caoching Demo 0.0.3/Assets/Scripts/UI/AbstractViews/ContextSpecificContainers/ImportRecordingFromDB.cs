/** 
* @file ImportRecordingFromDB.cs
* @brief Contains the ImportRecordingFromDB class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers
{
    /// <summary>
    /// A container view for loading items from a database
    /// </summary>
    public class ImportRecordingFromDB : AbstractView, IDatabaseConsumer
    {
        public ImportViewSelectableGridList GridList;
        public Button OpenFolderButton;
        private FolderScanner mFolderScanner = new FolderScanner();
        //private ImportToDatabase6+++++++++++++++++++++++++
        public int NumberOfItems = 200;

        private Random gen = new Random();
        DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;
            return start.AddDays(gen.Next(range));
        }
        void Awake()
        {
            CreateDefaultLayout();
        }

        public Database Database { get; set; }
     

        public override void CreateDefaultLayout()
        {
            GridList.Initialize();
            OpenFolderButton.onClick.AddListener(OpenSelectFolderDialog);

        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }

        public void OpenSelectFolderDialog()
        {
            mFolderScanner.DirectoryScanCompleted += OnScanCompletion;
            // to work around uni file to only filter folders , add a nonsensical file extension
            UniFileBrowser.use.filterFileExtensions = new[] { "som3rAN66Ddhum3cks2Tension38F" };
            UniFileBrowser.use.filterFiles = true;
            UniFileBrowser.use.showVolumes = true;
            UniFileBrowser.use.volumesAreSeparate = true;
            UniFileBrowser.use.OpenFolderWindow(true, mFolderScanner.ScanDirectory);
        }

        private void OnScanCompletion(List<ImportItemDescriptor> vDescriptors)
        {
            mFolderScanner.DirectoryScanCompleted -= OnScanCompletion;
            GridList.LoadData(vDescriptors);
        }



    /*    void RecordingAddTest()
        {
            Debug.Log("scanning recordings from " + ApplicationSettings.PreferedRecordingsFolder);
            int value = BodyRecordingsMgr.Instance.ScanRecordings(ApplicationSettings.PreferedRecordingsFolder);
            Debug.Log("found " + value + " recordings");

            bool[] vAddedRec = new bool[value];
            for (int j = 0; j < vAddedRec.Length; j++)
            {
                vAddedRec[j] = false;

            }
            int i = 0;
            int max = 15;
            while (i < max)
            {
                int vRand = Random.Range(0, value);

                if (!vAddedRec[vRand])
                {
                    CurrentRecordingPath = BodyRecordingsMgr.Instance.FilePaths[i];
                    vAddedRec[vRand] = true;
                    i++;
                    BodyRecordingsMgr.Instance.ReadRecordingFile(CurrentRecordingPath, RecordingAddCallback);
                }
            }



        }*/
        void RecordingAddCallback(BodyFramesRecording vBfRec)
        {
            Debug.Log("found " + vBfRec.BodyRecordingGuid + "... now adding to DB");
            Database.Connection.CreateRecording(vBfRec);

        }
    }
}
