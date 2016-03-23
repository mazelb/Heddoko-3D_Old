/** 
* @file ImportItemDescriptor.cs
* @brief Contains the ImportItemDescriptor class  
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.SelectableGridList.Descriptors
{
    /// <summary>
    /// Describes an item that is held by the import view selectable grid list
    /// </summary>
    [Serializable]
    public class ImportItemDescriptor
    {
        [SerializeField]
        public string MovementTitle;
        [SerializeField]
        public string CreatedAtDescription;
        [SerializeField]
        public string Tag;
        [SerializeField]
        private DateTime mCreatedAtTime;

        public DateTime CreatedAtTime
        {
            get
            {
                return mCreatedAtTime;
            }
            set
            {
                mCreatedAtTime = value;
                //check if the created time is today or yesterday
                bool vIsYesterday = DateTime.Today - mCreatedAtTime.Date == TimeSpan.FromDays(1);
                bool vIsToday= DateTime.Today - mCreatedAtTime.Date == TimeSpan.FromDays(0);
                string vPrefix = mCreatedAtTime.ToLongDateString();
                if (vIsToday)
                {
                    vPrefix = "Today";
                }
                else if (vIsYesterday)
                {
                    vPrefix = "Yesterday";
                }
                CreatedAtDescription = vPrefix + ", " +
                                       mCreatedAtTime.ToString("hh:mm tt",
                                           System.Globalization.DateTimeFormatInfo.InvariantInfo);
            }
        }


    }
}