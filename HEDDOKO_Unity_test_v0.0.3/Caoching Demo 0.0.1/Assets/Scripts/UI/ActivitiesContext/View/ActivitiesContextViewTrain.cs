
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
       // public GameObject HeddokoModel; 
        public GameObject BetaHighLimbsGeo;
        public GameObject BetaHighJointsGeo;
        public GameObject BetaHighTorsoGeo;
        public Material RegularTrainingMaterial;
        public Material RegularJointstTrainingMaterial;
     //   public Text NumberSquatsOfText;
        public SquatMetricsView SquatsMetricsView;
        public BikingMetricsView BikingMetricsView;
        public ActivitiesContextController ActivitiesContextController;

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
            BetaHighLimbsGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighTorsoGeo.GetComponent<Renderer>().material = RegularTrainingMaterial;
            BetaHighJointsGeo.GetComponent<Renderer>().material = RegularJointstTrainingMaterial;
            ModelSwitcher.TransformInview3DLocation = Heddoko3DModelEnabledAnchor;
            ModelSwitcher.TransformInview2DLocation = Heddoko2DModelEnabledAnchor;
            ModelSwitcher.Show();
            //check if using squats or bike
            if (ActivitiesContextController.UsingSquats)
            {
                BikingMetricsView.Hide();
                SquatsMetricsView.Show();
            }
            else
            {
                SquatsMetricsView.Hide();
                BikingMetricsView.Show(); 
            }
             


        }
        /// <summary>
        /// Hides the view
        /// </summary>
        public void Hide()
        {
            SquatsMetricsView.Hide();
            BikingMetricsView.Hide();
            ModelSwitcher.Hide();

            //  HeddokoModel.SetActive(false);
            //  HeddokoModel.transform.position = HeddokoModelHiddenAnchor.position;
            PlayerStreamManager.Stop();
            PlayerStreamManager.ResetInitialFrame(); 
            gameObject.SetActive(false);
            mIsActive = false;
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    PlayerStreamManager.ResetInitialFrame();
                    PlayerStreamManager.CurrentBodyInPlay.MBodyFrameThread.CreateNewFile = true;
                    Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                    if (vCurrentBody != null)
                    {
                        RightLegAnalysis vRightLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as RightLegAnalysis;
                         vRightLegAnalysis.NumberofRightSquats = 0;
                        BikingMetricsView.ResetValues();
                    }
                
                }
            }
           /* if (mIsActive)
            {
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    RightLegAnalysis vRightLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as RightLegAnalysis;
                    vRightLegAnalysis.StartCountingSquats(true);
                    NumberSquatsOfText.text = "number of squats = " + vRightLegAnalysis.NumberofRightSquats;
                }
            }*/
            else
            {
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    RightLegAnalysis vRightLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as RightLegAnalysis;
                    vRightLegAnalysis.StartCountingSquats(false);
                
                }
            }
        }
    }
}
