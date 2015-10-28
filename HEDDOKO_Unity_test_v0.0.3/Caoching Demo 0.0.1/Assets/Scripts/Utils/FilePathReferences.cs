using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace Assets.Scripts.Utils
{
    /// <summary>
    /// This class acts like a repository of locally saved data pathss
    /// </summary>
    public static class FilePathReferences
    {
         /// <summary>
         /// Returns a string whose path reflects the directory of local saved data. 
         /// </summary>
         /// <param name="postPendPath"></param>
         /// <returns></returns>
        public static string LocalSavedDataPath(string postPendPath)
        {
            return Application.persistentDataPath + "/" + postPendPath;
        }

        public static string CSVDirectory = Application.dataPath + "/Resources/Recordings";

    }
}
