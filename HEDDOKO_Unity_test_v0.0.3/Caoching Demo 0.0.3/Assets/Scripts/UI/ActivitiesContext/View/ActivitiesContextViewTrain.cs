
/** 
* @file ActivitiesContextViewTrain.cs
* @brief Contains the ActivitiesContextViewTrain class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Cameras;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Metrics;
using Assets.Scripts.UI.Metrics.View;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the training context view
    /// </summary>
    public class ActivitiesContextViewTrain : MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform Heddoko3DModelEnabledAnchor;
        public Transform Heddoko2DModelEnabledAnchor;
        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;

        public Button BackButton;
        public Camera TrainingAndLearningCam;  
 
        public ActivitiesContextController ActivitiesContextController;

        public SquatMetricsView SquatMetrics;
        public NonSquatWrapperView NonSquatMetrics; 


        private PlayerStreamManager mPlayerStreamManager;
        private bool mIsActive;
        public Model2D3DSwitch ModelSwitcher;

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
        /// <summary>
        /// Enables and shows the view
        /// </summary>
        public void Show()
        { 
            gameObject.SetActive(true); 
            ModelSwitcher.TransformInview3DLocation = Heddoko3DModelEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = Heddoko2DModelEnabledAnchor;
            ModelSwitcher.Show(); 
            TrainingAndLearningCam.gameObject.SetActive(true);
            PlayerStreamManager.ResetPlayer();

            //check if using squats or bike
            if (ActivitiesContextController.UsingSquats)
            {
                SquatMetrics.Show();
                NonSquatMetrics.Hide();
            }
            else
            {
                SquatMetrics.Hide();
                NonSquatMetrics.Show();
            }

        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            SquatMetrics.Hide();
            NonSquatMetrics.Hide();
            ModelSwitcher.Hide();
            TrainingAndLearningCam.gameObject.SetActive(false); 
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetPlayer();
            gameObject.SetActive(false);
            mIsActive = false;
 
        }
    }
}
