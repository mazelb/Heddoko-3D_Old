using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.MainScene
{
    /// <summary>
    /// Represents the import button view in the main menu. Hooks events into recording select button's listeners
    /// </summary>
    public class ImportRecordingsButton: MonoBehaviour
    {
        private bool mImportCompleted;
        private bool ShowRecodingPanelPrefab;
        private InformationPanel mInfoPanel;
        private List<string> mRecordingsFiles = new List<string>(1);
        public GameObject AvailableRecordingsPanelPrefab;
        private GameObject mCurrentRecordingPanel;
        public GameObject AvailableRecordingButtonPrefab;
        private List<GameObject> mButtonsList= new List<GameObject>(1); //instantiate with a minimum of one
        private Button mCurrentButton;
        int mTotalRecordings;
        int mCurrentSelectedRecordingIndex=-1;
       // private BodyFramesRecording mCurrentlySelectedRecording;
        public InformationPanel InfoPanel
        {
            get
            {
                if (mInfoPanel == null)
                {
                    GameObject vGo = SceneObjectFinder<InformationPanel>.FindObjecInSceneWithObjectAttached("InformationPanel");
                    mInfoPanel = vGo.GetComponent<InformationPanel>();
                }
                return mInfoPanel;
            } 
        }
        /// <summary>
        /// On Awake: set the current recording prefab by instantiating the available recording panels prefab
        /// and disable it right away. Set the button's callback action to StartImportProcess
        /// </summary>
        void Awake()
        {
            mCurrentRecordingPanel =Instantiate(AvailableRecordingsPanelPrefab);
            //find the canvas in the scene
            GameObject vCanvas = GameObject.FindGameObjectWithTag("UiCanvas");
            //make the recording panel the child gameobject
            mCurrentRecordingPanel.transform.SetParent(vCanvas.transform,false);
            mCurrentRecordingPanel.SetActive(false);
            mCurrentButton = GetComponentInChildren<Button>();
            mCurrentButton.onClick.AddListener(StartImportProcess);
            mTotalRecordings = BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.sCsvDirectory); 
           
        }

        
        /// <summary>
        /// Starts the import process. 
        /// </summary>
        private void StartImportProcess()
        { 
             
            UpdateView(mTotalRecordings);
        }

        private void UpdateView(int vTotalRecordings)
        {
            if (vTotalRecordings == 0) //no recordings found
            {
                NoRecordingsFound();
            }
            else
            {
                if (!mImportCompleted) //check if already imported
                {
                    mImportCompleted = true;
                   string[] vRecordingPaths = BodyRecordingsMgr.Instance.FilePaths;
                    //trim 
                    for (int i = 0; i < vRecordingPaths.Length; i++)
                    {
                        if (vRecordingPaths[i].Contains(".meta"))
                        { 
                            continue;
                        }
                        
                       mRecordingsFiles.Add(vRecordingPaths[i]);
                    }
                    CreateButtons();//create buttons once
                }
                ShowPanel();
            }
        }

        private void NoRecordingsFound()
        {
            mInfoPanel.UpdateInformationPanel("No recordings found");
        }
        /**
        * ShowPanel()
        * @brief  Helper function that shows the panel 
        */
        private void ShowPanel( )
        {
            mCurrentRecordingPanel.SetActive(true);
        }
        /**
        * HidePanel()
        * @brief  Helper function that hides the panel
        */
        private void HidePanel()
        {
            mCurrentRecordingPanel.SetActive(false);
        }
        /**
        * CreateButtons()
        * @brief  Helper function that instantiates a list of buttons, depending on the number of recordings found 
        */
        private void CreateButtons()
        {
            //first find the Scroll view from the available panels recording
            Transform vContentPanel = SceneObjectFinder<Transform>.FindObjectByName(mCurrentRecordingPanel.transform, "ScrollView")  as Transform;
            vContentPanel = vContentPanel.GetChild(0);
            if (vContentPanel != null)//if game object found
            {
                for (int i = 0; i < mRecordingsFiles.Count; i++)
                {
                    GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                    Button vAvRecButton = vNewAvRecButton.GetComponent<Button>();
                    string vCleanedName = mRecordingsFiles[i].Replace(FilePathReferences.sCsvDirectory + "\\", null);
                    vAvRecButton.GetComponentInChildren<Text>().text =vCleanedName;
                    int vTemp = i; //copy the variable i and pass it into the listener
                    vAvRecButton.onClick.AddListener(() => SelectButton(vTemp));
                    vNewAvRecButton.transform.SetParent(vContentPanel,false);
                    mButtonsList.Add(vNewAvRecButton);
                } 
            }
            else
            {
                Debug.Log("scroll view not found in scene");
            }
        
        }
        /// <summary>
        /// A Registered button has been pressed, the currently selected button is now mRecordings[vRegisteredIndex]
        /// </summary>
        /// <param name="vRegisteredIndex">the index of the recording</param>
        private void SelectButton(int vRegisteredIndex)
        {
            mCurrentSelectedRecordingIndex = vRegisteredIndex;
            HidePanel();
        }
    }
}
