﻿
/** 
* @file LearnFromRecordingView.cs
* @brief Contains the LearnFromRecordingView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Cameras;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.RecordingLoading;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.ActivitiesContext.View
{
    /// <summary>
    /// Represents the learn from recording view
    /// </summary>
    public class LearnFromRecordingView : MonoBehaviour, IActivitiesContextViewSubcomponent
    {
        /// <summary>
        /// When the model needs to be shown, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModel3DEnabledAnchor; 
        public Transform HeddokoModel2DEnabledAnchor;

        /// <summary>
        /// When the model needs to be hidden, it will be placed on this anchor
        /// </summary>
        public Transform HeddokoModelHiddenAnchor;
         public Camera TrainingAndLearningCam;
       // public Camera BikesOrthoCam;
        public Button CancelButton;
        public Button TrainButton;
        public Button BackButton; 
        private PlayerStreamManager mPlayerStreamManager;
        public Model2D3DSwitch ModelSwitcher;
        public ActivitiesContextController ActivitiesContextController;
 

        public GameObject SquatsMetrics;
        public GameObject BikingMetrics;
        public GameObject DualPurposeMetrics;
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
        /// Enables and displays the view
        /// </summary>
        public void Show()
        {
           
            ModelSwitcher.TransformInview3DLocation = HeddokoModel3DEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = HeddokoModel2DEnabledAnchor;

            //HeddokoModel.transform.position = HeddokoModel3DEnabledAnchor.position;
            //  HeddokoModel.transform.rotation = HeddokoModelEnabledAnchor.rotation;
            gameObject.SetActive(true);

            //  BetaHighLimbsGeo.GetComponent<Renderer>().material = TransparentMaterial;
            //    BetaHighTorsoGeo.GetComponent<Renderer>().material = TransparentMaterial;
            // BetaHighJointsGeo.GetComponent<Renderer>().material = TrasparentJointsMaterial;
            ModelSwitcher.Show();
            TrainingAndLearningCam.gameObject.SetActive(true);
            DualPurposeMetrics.SetActive(true);
            if (ActivitiesContextController.UsingSquats)
            {
                
               // BikesOrthoCam.gameObject.SetActive(false);
                 SquatsMetrics.SetActive(true);
                BikingMetrics.SetActive(false);
                
            }
            else
            {
              //  TrainingAndLearningCam.gameObject.SetActive(false);
              //  BikesOrthoCam.gameObject.SetActive(true);
                SquatsMetrics.SetActive(false);
                SquatsMetrics.SetActive(false);
                BikingMetrics.SetActive(true);
            
            }
            PlayerStreamManager.ResetPlayer();
            PlayerStreamManager.StickTorsoToHips(ActivitiesContextController.UsingSquats);
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 0;

        }

        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            Application.targetFrameRate = -1;
            gameObject.SetActive(false); 
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetPlayer();
            ModelSwitcher.Hide();
            TrainingAndLearningCam.gameObject.SetActive(false);
            SquatsMetrics.SetActive(false);
            BikingMetrics.SetActive(false);
            DualPurposeMetrics.SetActive(false);
           // BikesOrthoCam.gameObject.SetActive(false);
        }
    }
}
