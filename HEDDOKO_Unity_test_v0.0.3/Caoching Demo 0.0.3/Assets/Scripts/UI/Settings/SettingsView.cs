//using System;
//using UnityEngine;
//using System.Windows.Forms;
//
//public class SettingsView : MonoBehaviour {
//
//    private FolderBrowserDialog folderBrowserDialog1;
//    private string vStoredDirectory;
//    // Use this for initialization
//    void Start () {
//        folderBrowserDialog1 = new FolderBrowserDialog();
//        folderBrowserDialog1.Description =
//              "Select the directory that you want to use as the default.";
//        folderBrowserDialog1.ShowNewFolderButton = false;
//        folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Personal;
//    }
//	
//	// Update is called once per frame
//	void Update () {
//	    if (Input.GetKeyDown(KeyCode.O))
//	    {
//
//	        DialogResult vRe= folderBrowserDialog1.ShowDialog();
//	    }
//    }
//}
