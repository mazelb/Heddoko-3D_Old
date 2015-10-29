/** 
* @file FilePathReferences.cs
* @brief Contains the FilePathReferences class
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
 
using UnityEngine;
namespace Assets.Scripts.Utils
{
    /**
    * FilePathReferences class 
    * @brief Class contains reference to repos where key files are located
    */
    public static class FilePathReferences
    {
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

        public static string sCsvDirectory = Application.dataPath + "/Resources/Recordings";

    }
}
