
/** 
* @file SquatMetricsView.cs
* @brief Contains the SquatMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Squats
    /// </summary>
    public class SquatMetricsView : MonoBehaviour
    {
    
        private PlayerStreamManager mPlayerStreamManager;
        public float UpperRangeOfSquatMotion = 110f;
        public float LowerRangeOfSquatMotion = 90f;
        public float LargestAngleSum = 170f;
        [SerializeField]
        private float mMaxKneeFlexion = 150f;
      //  public SquatColoredFeedback SquatColoredFeedback;
       // public SquatCounter SquatCounter;
        private int mPreviousSquatValue = 0;
        public Text NumberOfSquats;
        public Image VisualSquatFeedback;
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
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    RightLegAnalysis vRightLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                            RightLegAnalysis;
                    vRightLegAnalysis.StartCountingSquats(true);
                    //NumberSquatsOfText.text = "Total number = " + vRightLegAnalysis.NumberofRightSquats;
                    float vAngleKneeflex = Mathf.Abs(vRightLegAnalysis.AngleKneeFlexion);

                    //are we still counting?
                    /*   if (vAngleSum > 0.1f && vAngleSum <= LargestAngleSum)
                       {*/
                    float vPositionOfSquat = vAngleKneeflex / mMaxKneeFlexion;
                    if (vPositionOfSquat > 1)
                    {
                        VisualSquatFeedback.fillAmount = 1;
                        // SquatColoredFeedback.SetScrollValue(1);
                    }
                    else
                    {
                        VisualSquatFeedback.fillAmount = vPositionOfSquat;
                        //SquatColoredFeedback.SetScrollValue(vPositionOfSquat);
                    }
                    int vCurrentSquats = Mathf.FloorToInt(vRightLegAnalysis.NumberofRightSquats/2);
                   UpdateSquatsCount(vCurrentSquats);
                    /*

                                            if (vRightLegAnalysis.AngleKneeFlexion < LowerRangeOfSquatMotion)
                                            { 
                                               // GoHigherLowerText.text = "Increase depth";
                                            }
                                            if (vRightLegAnalysis.AngleKneeFlexion >= LowerRangeOfSquatMotion &&
                                                vRightLegAnalysis.AngleKneeFlexion < UpperRangeOfSquatMotion)
                                            {
                                                //GoHigherLowerText.text = " Good! ";
                                            }
                                            else if(vRightLegAnalysis.AngleKneeFlexion >= UpperRangeOfSquatMotion)
                                            {
                                              //  GoHigherLowerText.text = "Decrease depth";
                                            }
                                            //GoHigherLowerText.text = "go lower";*/

                    // }
                    //check if the sum is less than 170 degree, 
                    /*else if (vAngleSum > 70 && vAngleSum < 170)
                    {
                        GoHigherLowerText.text = "go higher";
                    }*/
                }
             
        }

        /// <summary>
        /// Brings the view into screen
        /// </summary>
        public void Show()
        {
          
            // float vPerfectSquatNormalVal = UpperRangeOfSquatMotion + LowerRangeOfSquatMotion;
            //   vPerfectSquatNormalVal /= 2f;
           /* //  HeddokoMathTools.Map(vPerfectSquatNormalVal, 0, LargestAngleSum, 0, 1);
            SquatCounter.gameObject.SetActive(true);
            SquatColoredFeedback.gameObject.SetActive(true);
            SquatColoredFeedback.SetGradient(15f / mMaxKneeFlexion, LowerRangeOfSquatMotion / mMaxKneeFlexion, UpperRangeOfSquatMotion / mMaxKneeFlexion);*/
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hides the view out of screen 
        /// </summary>
        public void Hide()
        {
           /* SquatCounter.gameObject.SetActive(false);
            SquatColoredFeedback.gameObject.SetActive(false);*/
 
            gameObject.SetActive(false);
        }

        private void UpdateSquatsCount(int vNewVal)
        {
            //SquatCounter.SetSquatNumber(vNewVal);
            NumberOfSquats.text = ""+vNewVal;
        }
 
        /// <summary>
        /// Rests the values
        /// </summary>
        public void ResetValues()
        {
            NumberOfSquats.text = 0+"";
            VisualSquatFeedback.fillAmount = 0;
        }
    }
}
