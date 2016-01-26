/** 
* @file FilePathReferences.cs
* @brief Contains the FilePathReferences class
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/

using System;
using System.IO;
using UnityEngine;
namespace Assets.Scripts.Utils
{
    /**
    * FilePathReferences class 
    * @brief Class contains reference to repos where key files are located
    */
    public static class FilePathReferences
    {
        public static string sCsvDirectory = Application.dataPath + "/Resources/Recordings";

        /// <summary>
        /// Settings file 
        /// </summary>
        public static string gsSettingsFile = "settings.setting";

        /// <summary>
        ///returns the settings file's application path, if it doesn't exist, the folder will be  created
        /// </summary>
        public static string SettingsFolder
        {
            get
            {
                //check if the folder currently exists
                string vCurrentPath = Environment.CurrentDirectory + "/Settings";

                if (!Directory.Exists(vCurrentPath))
                {
                    Directory.CreateDirectory(vCurrentPath);
                }

                return vCurrentPath;
            } 
        }


        /**
        * LocalSavedDataPath(string vSuffix)
        * @param: the data path where a local file can be saved
        * @brief returns a path where a file can be savved locally
        * @return returns a string for a path where a file can be saved locally.
        */
        public static string LocalSavedDataPath(string vSuffixedDataPath)
        {
            return Application.persistentDataPath + "/" + vSuffixedDataPath;
        }
        /// <summary>
        /// The directory of recordings
        /// </summary>
        public static string RecordingsDirectory
        {
            get
            {
                return Application.dataPath + "/Resources/Recordings";
            }
        }

    }
}
