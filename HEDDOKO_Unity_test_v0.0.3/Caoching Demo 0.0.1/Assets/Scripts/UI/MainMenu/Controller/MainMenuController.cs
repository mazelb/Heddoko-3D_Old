
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Communication.View;
using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.UI.MainScene.View;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;
namespace Assets.Scripts.UI.MainScene.Controller
{
    /// <summary>
    /// Represents the controller for the main menu. This class allows for the changing of context
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        public Button ImportButton;
        public Button AnalyzeButton;
        public Button Visualize3DButton;
        public BrainpackConnectionView BrainpackInfoPanel;
        private List<Button> mRegisteredButtons = new List<Button>(3);
        private InformationPanel mInfoPanel;
        private bool vImportCompleted;
        public GameObject LoadingScreen;
        public GameObject AvailableRecordingsPanelPrefab;
        public GameObject AvailableRecordingButtonPrefab;
        private bool mStart3D; //indicates if the user is using 3D to load the scene or starting the scene with a brainpack feed
        private GameObject mCurrentRecordingPanel;

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

        #region unity functions
        /**
        * Awake 
        * @brief On awake, initialize all the control UI elements in the scene 
        */
        /// <summary>
        /// On  awake, initialize all the control UI elements in the scene
        /// </summary>
        void Awake()
        {
            //find the import button in the scene
            ImportRecordingsButton vImportRecButton = (ImportRecordingsButton)FindObjectOfType(typeof(ImportRecordingsButton));
            vImportRecButton.AssignAction(ImportButtonClick);
            ImportButton = vImportRecButton.CurrentButton;
            mCurrentRecordingPanel = Instantiate(AvailableRecordingsPanelPrefab);
            mCurrentRecordingPanel.SetActive(false);
            GameObject vGo = GameObject.FindGameObjectWithTag("UiCanvas");
            mCurrentRecordingPanel.transform.SetParent(vGo.transform, false);
            mRegisteredButtons.Add(ImportButton);
            mRegisteredButtons.Add(AnalyzeButton);
            mRegisteredButtons.Add(Visualize3DButton);
            Visualize3DButton.onClick.AddListener(Visualize3DButtonClicked);

        }
        #endregion
        /**
        * ChangeTo3DSceneView() 
        * @brief Switches scenes to the 3d scene view
        */
        /// <summary>
        /// Switches scenes to the 3d view 
        /// </summary>
        private void ChangeTo3DSceneView()
        {
            LoadingScreen.SetActive(true);
            StartCoroutine(LoadMainScene());

        }
        /**
        * LoadMainScene() 
        * @brief Coroutine to load the main scene asychronously
        */
        /// <summary>
        /// Coroutine to load the main scene asychronously
        /// </summary>
        /// <returns></returns>
        private IEnumerator LoadMainScene()
        {
            AsyncOperation async = Application.LoadLevelAsync(1);
            yield return async;
            LoadingScreen.SetActive(false); 
        }
        /**
        * ImportButtonClick() 
        * @brief Import button in scene has been clicked
        */
        /// <summary>
        /// Import button in scene has been clicked
        /// </summary>
        private void ImportButtonClick()
        {
            BrainpackInfoPanel.Hide();
            EnableAllButLeaveoneInactive(ImportButton);


            BodySelectedInfo.Instance.UpdateNumberOfRecordings();
            InfoPanel.UpdateInformationPanel("");
            int vNumberOfRec = BodySelectedInfo.Instance.TotalRecordingsAvailable;
            if (vNumberOfRec == 0)
            {
                InfoPanel.UpdateInformationPanel("There are no recordings to import");
                InfoPanel.FreezeText(1.25f);
                ReactivateAllButtons();
            }
            else
            {
                DisplayAllAvailableRecordings();
            }
            if (!vImportCompleted)
            {
                //start creating buttons
                //first find the Scroll view from the available panels recording
                Transform vContentPanel = SceneObjectFinder<Transform>.FindObjectByName(mCurrentRecordingPanel.transform, "ScrollView") as Transform;
                string[] vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                if (vContentPanel != null)
                {
                    vContentPanel = vContentPanel.GetChild(0);
                    for (int i = 0; i < vRecordingsFiles.Length; i++)
                    {

                        GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                        Button vAvRecButton = vNewAvRecButton.GetComponent<Button>();
                        string vCleanedName = vRecordingsFiles[i].Replace(FilePathReferences.sCsvDirectory + "\\", null);
                        vAvRecButton.GetComponentInChildren<Text>().text = vCleanedName;
                        int vTemp = i; //copy the variable i and pass it into the listener
                        vAvRecButton.onClick.AddListener(() => ChooseRecording(vTemp));
                        vNewAvRecButton.transform.SetParent(vContentPanel, false);
                    }
                }
                vImportCompleted = true;
            }
          
        }
        /// <summary>
        /// The visualize3d button has been clicked
        /// </summary>
        private void Visualize3DButtonClicked()
        {
            mCurrentRecordingPanel.GetComponent<AvailableRecordingPanelView>().Hide();
            EnableAllButLeaveoneInactive(Visualize3DButton);
            BrainpackInfoPanel.Show();
        }
        /**
        * DeactivateAllButtons() 
        * @brief deactivate all buttons in the scene
        */
        /// <summary>
        /// Deactivate all buttons in the scene
        /// </summary>
        private void DeactivateAllButtons()
        {
            foreach (var vButton in mRegisteredButtons)
            {
                vButton.interactable = false;
            }
        }
        /// <summary>
        /// Sets all the buttons to interactive except for the passed in one.
        /// </summary>
        /// <param name="vB"></param>
        private void EnableAllButLeaveoneInactive(Button vB)
        {
            foreach (var vButton in mRegisteredButtons)
            {
                vButton.interactable = true;
            }
            vB.interactable = false;
        }
        /**
        * DeactivateAllButtons() 
        * @brief reactivate all buttons in the scene
        */
        /// <summary>
        /// Reactivate all buttons
        /// </summary>
        private void ReactivateAllButtons()
        {

            foreach (var vButton in mRegisteredButtons)
            {
                vButton.interactable = true;
            }
        }
        /**
        * DisplayAllAvailableRecordings() 
        * @brief Display all available recordings 
        */
        /// <summary>
        /// Display all available recordings 
        /// </summary>
        private void DisplayAllAvailableRecordings()
        {
            mCurrentRecordingPanel.GetComponent<AvailableRecordingPanelView>().Show();
        }
        /**
        * HideAllAvailableRecordings() 
        * @brief Hides all available recordings 
        */
        /// <summary>
        /// Hides all available recordings 
        /// </summary>
        private void HideAllAvailableRecordings()
        {
            mCurrentRecordingPanel.GetComponent<AvailableRecordingPanelView>().Hide();
        }
        /**
        * ChooseRecording(int vRecordingIndex)
        * @brief he in scene recording button selects the wanted recording. 
        * @param int vRecordingIndex: The recording index
        */
        /// <summary>
        /// The in scene recording button selects the wanted recording. 
        /// </summary>
        /// <param name="vRecordingIndex">The recording index</param>
        private void ChooseRecording(int vRecordingIndex)
        {
            HideAllAvailableRecordings();
            ReactivateAllButtons();
            BodySelectedInfo.Instance.UpdateSelectedRecording(vRecordingIndex);
            ChangeTo3DSceneView(); 
        }
    }
}
