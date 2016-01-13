
/** 
* @file DualPurposeMetricsView.cs
* @brief Contains the DualPurposeMetricsView class
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
    /// Displays both metrics for squats and biking
    /// </summary>
    public class DualPurposeMetricsView : MonoBehaviour
    {
        
        //
        public float MaxHipFlexion = 175f;
        public float MaxKneeFlexion = 175f;
        public Image HipFlexFill;
        public Image KneeFlexFill;
        public Text HipAngleText;
        public Text KneeFlexAngleText;
        public PlayerStreamManager PlayerStreamManager;
   
        public bool DisplayRightLegAnalysis { get; set; }
        void Update()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vCurrentBody != null)
            {
                if (DisplayRightLegAnalysis)
                {
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                    {
                        RightLegAnalysis vRightLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                                    RightLegAnalysis;
                        if (vRightLegAnalysis != null)
                        {
                            HipFlexFill.fillAmount = Mathf.Abs(vRightLegAnalysis.AngleRightHipFlexion) / MaxHipFlexion;
                            HipAngleText.text = (int)Mathf.Abs(vRightLegAnalysis.AngleRightHipFlexion)+ "°";
                            KneeFlexFill.fillAmount = Mathf.Abs(vRightLegAnalysis.AngleKneeFlexion ) / MaxKneeFlexion;
                            KneeFlexAngleText.text = (int)Mathf.Abs(vRightLegAnalysis.AngleKneeFlexion)  + "°";
                        }
                    }
                }
                else
                {
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                    {
                        LeftLegAnalysis vLeftLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                                    LeftLegAnalysis;
                        if (vLeftLegAnalysis != null)
                        {
                            HipFlexFill.fillAmount = Mathf.Abs(vLeftLegAnalysis.AngleHipFlexion )  / MaxHipFlexion;
                            HipAngleText.text = (int)Mathf.Abs(vLeftLegAnalysis.AngleHipFlexion)  + "°";
                            KneeFlexFill.fillAmount = Mathf.Abs(vLeftLegAnalysis.AngleKneeFlexion)  / MaxKneeFlexion;
                            KneeFlexAngleText.text = (int)Mathf.Abs(vLeftLegAnalysis.AngleKneeFlexion)  + "°";
                        }
                    }
                }
               
            }
        }

        public void ResetValues()
        {
            HipFlexFill.fillAmount = 0;
            KneeFlexFill.fillAmount = 0;
            HipAngleText.text = 0 + "°";
            KneeFlexAngleText.text = 0 + "°";

        }

    }
}
