
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
        public float UpperRangeOfSquatMotion = 110f;
        public float LowerRangeOfSquatMotion = 90f;
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

                    //are we still counting?
                    if (vAngleSum > 0.1f && vAngleSum <= 170)
                    {
                        if (vRightLegAnalysis.AngleKneeFlexion < LowerRangeOfSquatMotion)
                        {
                            GoHigherLowerText.text = "Increase depth";
                        }
                        if (vRightLegAnalysis.AngleKneeFlexion >= LowerRangeOfSquatMotion &&
                            vRightLegAnalysis.AngleKneeFlexion < UpperRangeOfSquatMotion)
                        {
                            GoHigherLowerText.text = " Good! ";
                        }
                        else if(vRightLegAnalysis.AngleKneeFlexion >= UpperRangeOfSquatMotion)
                        {
                            GoHigherLowerText.text = "Decrease depth";
                        }
                        //GoHigherLowerText.text = "go lower";
                        
                    }
                    //check if the sum is less than 170 degree, 
                    /*else if (vAngleSum > 70 && vAngleSum < 170)
                    {
                        GoHigherLowerText.text = "go higher";
                    }*/
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
