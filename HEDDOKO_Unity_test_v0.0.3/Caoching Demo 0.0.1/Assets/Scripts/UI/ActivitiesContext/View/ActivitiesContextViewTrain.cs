
/** 
* @file ActivitiesContextViewTrain.cs
* @brief Contains the ActivitiesContextViewTrain class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.ActivitiesContext.Controller;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.Metrics;
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
        public Camera BikesOrthoCam;
       // public GameObject HeddokoModel; 
      //  public GameObject BetaHighLimbsGeo;
      //  public GameObject BetaHighJointsGeo;
     //   public GameObject BetaHighTorsoGeo;
        public Material RegularTrainingMaterial;
        public Material RegularJointstTrainingMaterial;
     //   public Text NumberSquatsOfText;
 
        public ActivitiesContextController ActivitiesContextController;


        public GameObject SquatsMetrics;
        public GameObject BikingMetrics;
        public GameObject DualPurposeMetrics;


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
            
           // HeddokoModel.transform.position = HeddokoModelEnabledAnchor.position;
           //   HeddokoModel.transform.rotation = HeddokoModelEnabledAnchor.rotation; 
           //HeddokoModel.SetActive(true);
            gameObject.SetActive(true);
    /*        BetaHighLimbsGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighTorsoGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighJointsGeo.GetComponent<Renderer>().material = RegularJointstTrainingMaterial;*/
            ModelSwitcher.TransformInview3DLocation = Heddoko3DModelEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = Heddoko2DModelEnabledAnchor;
            ModelSwitcher.Show();
            DualPurposeMetrics.SetActive(true);

            //check if using squats or bike
            if (ActivitiesContextController.UsingSquats)
            {
                BikesOrthoCam.gameObject.SetActive(false);
                TrainingAndLearningCam.gameObject.SetActive(true);
                SquatsMetrics.SetActive(true);
                BikingMetrics.SetActive(false); 
            }
            else
            {
                BikesOrthoCam.gameObject.SetActive(true);
                TrainingAndLearningCam.gameObject.SetActive(false);
                SquatsMetrics.SetActive(false);
                BikingMetrics.SetActive(true);
            }

        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            SquatsMetrics.SetActive(false);
            BikingMetrics.SetActive(false);
            ModelSwitcher.Hide();
            TrainingAndLearningCam.gameObject.SetActive(false);
            BikesOrthoCam.gameObject.SetActive(false);

            //  HeddokoModel.SetActive(false);
            //  HeddokoModel.transform.position = HeddokoModelHiddenAnchor.position;
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetPlayer();
            gameObject.SetActive(false);
            mIsActive = false;
 
        }
    }
}
