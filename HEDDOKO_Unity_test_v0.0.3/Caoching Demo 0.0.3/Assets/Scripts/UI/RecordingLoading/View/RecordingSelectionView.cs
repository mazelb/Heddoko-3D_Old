/** 
* @file RecordingSelectionView.cs
* @brief Contains the RecordingSelectionView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Scene_3d.View;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.RecordingLoading.View
{
    /// <summary>
    /// the recording selection view in the main menu
    /// </summary>
    public class RecordingSelectionView : MonoBehaviour
    {

        // back button, return to main menu
        public Button BackButton;
        public RecordingPanelView RecordingPanelView;
        public PlayerStreamManager PlayerManager;
        public Camera LoadRecordingsCamera;
        public Camera TrainAndLearningCamera; 
        public Transform InviewAnchor;
        public Transform OutOfViewAnchor;

        public Model2D3DSwitch ModelSwitcher;

        /// <summary>
        /// shows the recording selection view
        /// </summary>
        public void Show()
        {
            Application.targetFrameRate = 50; 
            gameObject.SetActive(true);
            RecordingPanelView.Show();
            ModelSwitcher.Show();
            LoadRecordingsCamera.gameObject.SetActive(true);
            TrainAndLearningCamera.gameObject.SetActive(false);
            PlayerManager.StickTorsoToHips(true);
        }

        /// <summary>
        /// hides the recording selection view
        /// </summary>
        public void Hide()
        {

            Application.targetFrameRate = -1;
            gameObject.SetActive(false);
            RecordingPanelView.Hide();
            ModelSwitcher.Hide();
            LoadRecordingsCamera.gameObject.SetActive(false);
            TrainAndLearningCamera.gameObject.SetActive(true);
        }


    }
}
