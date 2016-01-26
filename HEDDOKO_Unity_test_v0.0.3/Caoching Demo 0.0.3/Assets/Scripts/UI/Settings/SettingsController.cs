/** 
* @file SettingsController.cs
* @brief Contains the SettingsController class
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System.IO;
using Assets.Scripts.Utils;
using UnityEngine;
using Newtonsoft.Json;
namespace Assets.Scripts.UI.Settings
{

    /// <summary>
    /// Settings controller:  sets, saves and creates settings 
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        private SettingsModel mModel;

        private void Awake()
        {
            LoadSettingsFile();
        }

        /// <summary>
        /// Attempts to load the settings file, if it doesn't exist, create it.
        /// </summary>
        private void LoadSettingsFile()
        {
            string vPath = FilePathReferences.SettingsFolder + "/" + FilePathReferences.gsSettingsFile;
            try
            {
                mModel = JsonUtilities.JsonFileToObject<SettingsModel>(vPath);
            }

                //in case the file isn't found, write a new one with generic values
            catch (FileNotFoundException)
            {
                mModel = new SettingsModel();
                SettingsModel.RecordingsFolder = FilePathReferences.RecordingsDirectory;
                mModel.SetComport(0);
                JsonUtilities.ConvertObjectToJson(vPath, mModel);
            }


        }

        /// <summary>
        /// Attempts to save the settings file
        /// </summary>
        private void SaveSettingsFile()
        {
            string vPath = FilePathReferences.SettingsFolder + "/" + FilePathReferences.gsSettingsFile;
            JsonUtilities.ConvertObjectToJson(vPath, mModel);
        }

    }
}
