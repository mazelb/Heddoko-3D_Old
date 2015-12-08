
/** 
* @file SquatMetricsView.cs
* @brief Contains the SquatMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Squats
    /// </summary>
    public class SquatMetricsView : MonoBehaviour
    {
        [SerializeField]
        private bool mIsActive;
        public Text NumberSquatsOfText;
        public Text GoHigherLowerText;
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

        void Update()
        {
            if (mIsActive)
            {
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    RightLegAnalysis vRightLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                            RightLegAnalysis;
                    vRightLegAnalysis.StartCountingSquats(true);
                    NumberSquatsOfText.text = "Total number = " + vRightLegAnalysis.NumberofRightSquats;
                    float vAngleSum = vRightLegAnalysis.AngleSumRight;
                    if (vAngleSum >= 15 && vAngleSum <= 70)
                    {
                        GoHigherLowerText.text = "go lower";
                    }
                    else if (vAngleSum > 70 && vAngleSum <= 140)
                    {
                        GoHigherLowerText.text = "go higher"; 
                    }
            }
            }
        }

        /// <summary>
        /// Brings the view into screen
        /// </summary>
        public void Show()
        {
            mIsActive = true;
         gameObject.SetActive(true);   
        }

        /// <summary>
        /// Hides the view out of screen 
        /// </summary>
        public void Hide()
        {
            mIsActive = false;
            gameObject.SetActive(false);
        }
    }
}
