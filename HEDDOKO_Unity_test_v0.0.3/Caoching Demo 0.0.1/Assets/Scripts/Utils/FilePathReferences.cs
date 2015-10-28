/** 
* @file FilePathReferences.cs
* @brief Contains the FilePathReferences class
* @author Mohammed Haider (Mohammed@heddoko.com)
* @date October 2015
*/
using UnityEngine;
namespace Assets.Scripts.Utils
{
    /**
    * FilePathReferences class 
    * @brief Static class that returns paths to various directories vital to the functionality to the app
    */
   
    public static class FilePathReferences
    {
        /**
        * LocalSavedDataPath(string postPendPath)
        * @param string vSuffix: a suffix that will be added to Application.persistentDataPath
        * @brief returns a string that reflects where a file would be saved with respect to  Application.persistentDataPath
        * @note:
        * @return string of the file save path
        */
        public static string LocalSavedDataPath(string vSuffix)
        {
            return Application.persistentDataPath + "/" + vSuffix;
        }
        public static string sCSVDirectory = Application.dataPath + "/Resources/Recordings"; 
    }
}
