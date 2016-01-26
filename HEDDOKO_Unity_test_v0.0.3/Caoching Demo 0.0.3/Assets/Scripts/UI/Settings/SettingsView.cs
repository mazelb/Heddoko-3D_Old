using System.Windows.Forms;
using UnityEngine;

namespace Assets.Scripts.UI.Settings
{
    public class SettingsView : MonoBehaviour {
 
        private FolderBrowserDialog folderBrowserDialog1;
        private string vStoredDirectory;
        // Use this for initialization
        void Start () {
            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.Description =
                "Select the default recordings directory";
            folderBrowserDialog1.ShowNewFolderButton = true;
            // folderBrowserDialog1.RootFolder = Environment.SpecialFolder.ProgramFiles;
        }
 	
        // Update is called once per frame
        void Update () {
            if (Input.GetKeyDown(KeyCode.O))
            {
  
                DialogResult vRe= folderBrowserDialog1.ShowDialog();
            }
        }
    }
}
