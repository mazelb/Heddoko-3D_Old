﻿/** 
* @file ImportViewSelectableGridLost.cs
* @brief Contains the ImportViewSelectableGridLost class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System;
using System.Collections.Generic; 
using Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors;
using UIWidgets;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList
{
    /// <summary>

    /// </summary>
    public class ImportViewSelectableGridList : ListViewCustom<ImportRecordingGridSelectionComponent, ImportItemDescriptor>
    {
        private bool mInitialized = false;
        private Dictionary<SortingType, Comparison<ImportItemDescriptor>> mSortingComparisons = new Dictionary<SortingType, Comparison<ImportItemDescriptor>>();
        [SerializeField]
        private SortingType mCurrentSortingType = SortingType.ByTitle;
        protected override void Awake()
        {
             
        }

     public void Initialize()
        {
            if (mInitialized)
            {
                return;
            }
            mInitialized = true;
            base.Start();
            SetupSortingComparisons();
        }

        /// <summary>
        /// Sets up sorting comparisons
        /// </summary>
        void SetupSortingComparisons()
        {
            mSortingComparisons.Add(SortingType.ByTitle, TitleComparison);
            mSortingComparisons.Add(SortingType.ByDate, DateTimeComparison);
            mSortingComparisons.Add(SortingType.ByDuration, RecordingDurationComparison);
        }

        /// <summary>
        /// Loads data for the current list
        /// </summary>
        /// <param name="vItemDescriptors"></param>
        public void LoadData(List<ImportItemDescriptor> vItemDescriptors)
        { 
            DataSource.BeginUpdate();
            DataSource.Clear(); 
            for (int i = 0; i < vItemDescriptors.Count; i++)
            {
                DataSource.Add(vItemDescriptors[i]); 
            } 
            DataSource.EndUpdate(); 
            
        }
        /// <summary>
        /// Compare titles
        /// </summary>
        /// <param name="vItemA"></param>
        /// <param name="vItemY"></param>
        /// <returns></returns>
        private int TitleComparison(ImportItemDescriptor vItemA, ImportItemDescriptor vItemY)
        {
            return vItemA.MovementTitle.CompareTo(vItemY.MovementTitle);
        }

        /// <summary>
        /// Compare date times
        /// </summary>
        /// <param name="vItemA"></param>
        /// <param name="vItemY"></param>
        /// <returns></returns>
        private int DateTimeComparison(ImportItemDescriptor vItemA, ImportItemDescriptor vItemY)
        {
            return vItemA.CreatedAtTime.CompareTo(vItemY.CreatedAtTime);
        }

        /// <summary>
        /// comparison between recording duration
        /// </summary>
        /// <param name="vItemA"></param>
        /// <param name="vItemY"></param>
        /// <returns></returns>
        private int RecordingDurationComparison(ImportItemDescriptor vItemA, ImportItemDescriptor vItemY)
        {
            return vItemA.RecordingDuration.CompareTo(vItemY.RecordingDuration);
        }
        /// <summary>
        /// Sets the component with spefied vItem
        /// </summary>
        /// <param name="vComponenent"></param>
        /// <param name="vItem"></param>
        protected override void SetData(ImportRecordingGridSelectionComponent vComponenent, ImportItemDescriptor vItem)
        {
            vComponenent.SetData(vItem);
        }

        protected override void HighlightColoring(ImportRecordingGridSelectionComponent vComponent)
        {
            base.HighlightColoring(vComponent);
            vComponent.MovementTitle.color = HighlightedColor;
            vComponent.CreatedAtDescription.color = HighlightedColor;
        }

        protected override void SelectColoring(ImportRecordingGridSelectionComponent vComponent)
        {
            base.SelectColoring(vComponent);
            vComponent.MovementTitle.color = SelectedColor;
            vComponent.CreatedAtDescription.color = SelectedColor;
        }

        protected override void DefaultColoring(ImportRecordingGridSelectionComponent vComponent)
        {
            base.DefaultColoring(vComponent);
            vComponent.MovementTitle.color = DefaultColor;
            vComponent.CreatedAtDescription.color = DefaultColor;
        }
        


        /// <summary>
        /// Returns the number of current selected items
        /// </summary>
        /// <returns>The total number of counted items</returns>
        public int GetSelectedCount
        {
            get
            {
                int vTotalCount = SelectedItems.Count;
                return vTotalCount;
            }
        }

        

        /// <summary>
        /// toggle sort according to the sort
        /// </summary>
        /// <param name="vSortingType"></param>
        public void ToggleSort(SortingType vSortingType)
        {
            if (vSortingType == mCurrentSortingType)
            {
                DataSource.Reverse();
            }
            else if (mSortingComparisons.ContainsKey(vSortingType))
            {
                mCurrentSortingType = vSortingType;
                DataSource.Sort(mSortingComparisons[vSortingType]);
            }
        }


        /// <summary>
        /// Sort the grid list by title
        /// </summary>
        public void SortByTitle()
        {
            ToggleSort(SortingType.ByTitle);
        }

        /// <summary>
        /// Sort the grid by duration
        /// </summary>
        public void SortByDuration()
        {
            ToggleSort(SortingType.ByDuration);
        }

        /// <summary>
        /// Sort the grid by date
        /// </summary>
        public void SortByDate()
        {
            ToggleSort(SortingType.ByDate);
        }
        public enum SortingType
        {
            ByTitle,
            ByDate,
            ByDuration
        }

    }
}
