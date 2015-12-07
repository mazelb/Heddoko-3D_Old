/** 
* @file RecordingSelectionView.cs
* @brief Contains the RecordingSelectionView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/


 
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

        public Camera LoadRecordingsCamera;
        public Camera TrainAndLearningCamera;

        //Gameobject that will play back the recording
        public GameObject HeddokoModel;

        public Transform InviewAnchor;
        public Transform OutOfViewAnchor;

        public Model2D3DSwitch ModelSwitcher;

        /// <summary>
        /// shows the recording selection view
        /// </summary>
        public void Show()
        {
            HeddokoModel.transform.position = InviewAnchor.position;
            HeddokoModel.transform.rotation = InviewAnchor.rotation;
            gameObject.SetActive(true);
            RecordingPanelView.Show();
            ModelSwitcher.Show();   
          LoadRecordingsCamera.gameObject.SetActive(true);
          TrainAndLearningCamera.gameObject.SetActive(false);
    }

        /// <summary>
        /// hides the recording selection view
        /// </summary>
        public void Hide()
        {
            HeddokoModel.transform.position = OutOfViewAnchor.position;
            HeddokoModel.transform.rotation = OutOfViewAnchor.rotation;
            gameObject.SetActive(false);
            RecordingPanelView.Hide();
            ModelSwitcher.Hide();
            LoadRecordingsCamera.gameObject.SetActive(false);
            TrainAndLearningCamera.gameObject.SetActive(true);
        }

        
    }
}
