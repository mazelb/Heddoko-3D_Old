/** 
* @file ImportRecordingFromDB.cs
* @brief Contains the ImportRecordingFromDB class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Communication.DatabaseConnectionPipe;
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UIWidgets;
using UnityEngine;
using UnityEngine.UI; 


namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers.Importation
{
    /// <summary>
    /// A container view for loading items from a database
    /// </summary>
    public class ImportRecordingFromDB : AbstractView, IDatabaseConsumer
    {
        public ImportViewSelectableGridList GridList;
        public Button OpenFolderButton;
        private FolderScanner mFolderScanner = new FolderScanner();
        public ImportItemsToDb ImportItem;
        //private ImportToDatabase6+++++++++++++++++++++++++
        public int NumberOfItems = 200; 
      
        void Awake()
        {
            CreateDefaultLayout();
        }

        public Database Database { get; set; }


        public override void CreateDefaultLayout()
        {
            GridList.Initialize();
            OpenFolderButton.onClick.AddListener(OpenSelectFolderDialog);
          //  Notify.Template.

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
            UniFileBrowser.use.volumesAreSeparate = false;
            UniFileBrowser.use.OpenFolderWindow(true, mFolderScanner.ScanDirectory);
        }

        private void OnScanCompletion(List<ImportItemDescriptor> vDescriptors)
        {
            mFolderScanner.DirectoryScanCompleted -= OnScanCompletion;
            GridList.LoadData(vDescriptors);
        }

        public void ImportItems()
        {
             if (GridList.SelectedItems.Count > 0)
            {
                GameObject vDialog = Dialog.Template(ImportItem.TemplateName).Show("Import File Progress");
                
                ImportItem = vDialog.GetComponent<ImportItemsToDb>();
                ImportItem.Database = Database;
                ImportItem.InitiateImport(GridList.SelectedItems);
                ImportItem.GridList = GridList;
            }



        }



    }
}
