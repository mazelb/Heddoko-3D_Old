/** 
* @file RecordingPanelView.cs
* @brief Contains the RecordingPanelView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.MainScene.Model;
using Assets.Scripts.Utils;
 
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Scene_3d.View
{
    public class RecordingPanelView : MonoBehaviour
    {
        public Button UpArrow;
        public Button DownArrow;
        public Button LeftSelectionRecording;
        public Button RightSelectionRecording;

        public GameObject AvailableRecordingButtonPrefab;
        private bool mImportCompleted;
        private bool mBottomButDisabled;

        private int mCurrentIndex = 0;
        private bool mRecordingSelected; 
        public Scrollbar Scrollbar;

        private PlayerStreamManager mPlayerStreamManager;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }

        void Awake()
        {
            Scrollbar.onValueChanged.AddListener(OnScrollValueChanged);
            LeftSelectionRecording.onClick.AddListener(LeftButtonEngaged);
            RightSelectionRecording.onClick.AddListener(RightButtonEngaged);
            UpArrow.onClick.AddListener(UpButtonEngaged);
            DownArrow.onClick.AddListener(DownButtonEngaged);
        }

        public void Show()
        {
            if (!mImportCompleted)
            {
                Transform vContentPanel = SceneObjectFinder<Transform>.FindObjectByName(transform, "ScrollView") as Transform;
                string[] vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                Button vFirstInsertedButton = null;
                if (vRecordingsFiles == null)
                {
                    BodyRecordingsMgr.Instance.ScanRecordings(FilePathReferences.RecordingsDirectory);
                    vRecordingsFiles = BodyRecordingsMgr.Instance.FilePaths;
                }
                if (vContentPanel != null)
                {
                    vContentPanel = vContentPanel.GetChild(0);
                    for (int i = 0; i < vRecordingsFiles.Length; i++)
                    {

                        GameObject vNewAvRecButton = Instantiate(AvailableRecordingButtonPrefab);
                        Button vAvRecButton = vNewAvRecButton.GetComponentInChildren<Button>();
                        if (i == 0)
                        {
                            vFirstInsertedButton = vAvRecButton;
                        }
                        string vCleanedName = vRecordingsFiles[i].Replace(FilePathReferences.sCsvDirectory + "\\", null);
                        vNewAvRecButton.GetComponentInChildren<Text>().text = vCleanedName;
                        int vTemp = i; //copy the variable i and pass it into the listener
                        vAvRecButton.onClick.AddListener(() => ChooseAndPlayRecording(vTemp));
                        vNewAvRecButton.transform.SetParent(vContentPanel, false);
                    }
                }
                //check if the panel isn't completely filled, disable the down arrow button if that is the case.
                //get the height of the content panel

                if (vFirstInsertedButton != null)
                {
                    float vContentPanelHeight = vContentPanel.GetComponent<RectTransform>().rect.height;
                    float vButtonHeight = vFirstInsertedButton.GetComponent<RectTransform>().rect.height;
                    if (CheckIfBotButtonNeedsRemoval(vContentPanelHeight, vButtonHeight, vRecordingsFiles.Length))
                    {
                        DownArrow.gameObject.SetActive(false);
                        mBottomButDisabled = true;
                    }
                }


                if (vFirstInsertedButton == null)
                {
                    UpArrow.gameObject.SetActive(false);
                    DownArrow.gameObject.SetActive(false);
                    mBottomButDisabled = true;
                }

                mImportCompleted = true;
            }
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Checks if the bottom button needs to be removed. 
        /// </summary>
        /// <param name="vPanelHeight"></param>
        /// <param name="vButtonHeight"></param>
        /// <param name="vNumberOfButtons"></param>
        /// <returns></returns>
        private bool CheckIfBotButtonNeedsRemoval(float vPanelHeight, float vButtonHeight, int vNumberOfButtons)
        {
            float vTotalH = vNumberOfButtons * vButtonHeight;
            if (vTotalH >= vPanelHeight)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetInitialFrame();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Chooses and plays the recording
        /// </summary>
        /// <param name="vRecordingIndex"></param>
        private void ChooseAndPlayRecording(int vRecordingIndex)
        {
            PlayerStreamManager.Stop();
            try
            {
                PlayerStreamManager.ResetInitialFrame();
            }
            catch (Exception)
            {
                
             
            }
            
            BodySelectedInfo.Instance.UpdateSelectedRecording(vRecordingIndex);
            PlayerStreamManager.Play();
            mCurrentIndex = vRecordingIndex;
        }

        /// <summary>
        /// Listener to scrollbar, listens to when the value has changed
        /// </summary>
        private void OnScrollValueChanged(float vNewValue)
        {
           /* if (vNewValue <= 0)
            {
                UpArrow.gameObject.SetActive(false);
            }
            if (vNewValue >= 1)
            {
                DownArrow.gameObject.SetActive(false);
            }
            if (vNewValue > 0 && Scrollbar.value < 1)
            {
                if (!mBottomButDisabled)
                {
                    DownArrow.gameObject.SetActive(true);
                }
                UpArrow.gameObject.SetActive(true);
            }*/
        }

        /// <summary>
        /// The right button has been pressed
        /// </summary>
        private void RightButtonEngaged()
        {
            mCurrentIndex++;
            if (mCurrentIndex > BodyRecordingsMgr.Instance.FilePaths.Length)
            {
                mCurrentIndex = 0;
            }
            ChooseAndPlayRecording(mCurrentIndex);
        }

        /// <summary>
        /// the left button has been pressed
        /// </summary>
        private void LeftButtonEngaged()
        {
            mCurrentIndex--;
            if (mCurrentIndex < 0)
            {
                mCurrentIndex = BodyRecordingsMgr.Instance.FilePaths.Length;
            }
            ChooseAndPlayRecording(mCurrentIndex);
        }

        /// <summary>
        /// the up arrow button has been engaged
        /// </summary>
        private void UpButtonEngaged()
        {
            float vNewVal = Scrollbar.value + 0.1f;
            Scrollbar.value = vNewVal;
        }

        /// <summary>
        /// the down arrow button has been engaged
        /// </summary>
        private void DownButtonEngaged()
        {
            float vNewVal = Scrollbar.value - 0.1f;
            Scrollbar.value = vNewVal;
        }
    }
}
