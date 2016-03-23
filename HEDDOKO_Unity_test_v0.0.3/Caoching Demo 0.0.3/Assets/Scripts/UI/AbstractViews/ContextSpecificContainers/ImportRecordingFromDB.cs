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
using Assets.Scripts.UI.AbstractViews.SelectableGridList;
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UnityEngine;
using Random = System.Random;


namespace Assets.Scripts.UI.AbstractViews.ContextSpecificContainers
{
    /// <summary>
    /// A container view for loading items from a database
    /// </summary>
    public class ImportRecordingFromDB : AbstractView
    {
        public ImportViewSelectableGridList GridList;

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
            GridList.Initialize();
            string vPath = "Assets/Resources/english-words.dict";
            string vFileContent = "";
            string[] vDictionaryContent= new string[1];
            
              using (StreamReader vStreamReader = new StreamReader(File.OpenRead(vPath)))
              {
                  vFileContent = vStreamReader.ReadToEnd();
                  vDictionaryContent = vFileContent.Split(new string[] {Environment.NewLine}, StringSplitOptions.None);
              }
            List<ImportItemDescriptor> vItemList = new List<ImportItemDescriptor>();
            for (int i = 0; i < NumberOfItems; i++)
            {
                int vRand = gen.Next(0, NumberOfItems);
                ImportItemDescriptor vItem = new ImportItemDescriptor();
                vItem.MovementTitle = vDictionaryContent[vRand];
                vItem.CreatedAtTime = RandomDay();
               
                vItemList.Add(vItem);
            }
            ImportItemDescriptor vTodayItem = new ImportItemDescriptor();
            vTodayItem.MovementTitle = "0a_Todays_recording";
            vTodayItem.CreatedAtTime = DateTime.Now;
            ImportItemDescriptor vYesterdayItem = new ImportItemDescriptor();
            vYesterdayItem.MovementTitle = "0b_Yesterdays_recording";
            vYesterdayItem.CreatedAtTime = DateTime.Now.AddDays(-1);
            vItemList.Add(vTodayItem);
            vItemList.Add(vYesterdayItem);
            GridList.LoadData(vItemList); 
        }

        public override void CreateDefaultLayout()
        {

        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
        }



    }
}
