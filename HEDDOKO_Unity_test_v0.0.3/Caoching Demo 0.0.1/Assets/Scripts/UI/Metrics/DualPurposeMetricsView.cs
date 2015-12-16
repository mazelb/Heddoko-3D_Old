
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
        public float HipFlexmultiplier=1;
        public float KneeFlexMultiplier=1;
        void Update()
        {
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vCurrentBody != null)
            {
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                {
                    RightLegAnalysis vRightLegAnalysis = vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                                RightLegAnalysis;
                    if (vRightLegAnalysis != null)
                    {
                        HipFlexFill.fillAmount = vRightLegAnalysis.AngleRightHipFlexion * HipFlexmultiplier / MaxHipFlexion;
                        HipAngleText.text = (int)vRightLegAnalysis.AngleRightHipFlexion*HipFlexmultiplier + "°";
                        KneeFlexFill.fillAmount = vRightLegAnalysis.AngleKneeFlexion* KneeFlexMultiplier / MaxKneeFlexion;
                        KneeFlexAngleText.text = (int) vRightLegAnalysis.AngleKneeFlexion * KneeFlexMultiplier + "°";
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
