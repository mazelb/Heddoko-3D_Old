/** 
* @file AngleInfoMetrics.cs
* @brief Contains the AngleInfoMetrics class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.UI.MainScene.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    public class AngleInfoMetrics : MonoBehaviour
    {
        public Text AngleInfoDisplayText;
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
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
            Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
            if (vCurrentBody != null)
            {
                string vText = "Angle Extractions + \n";
                LeftLegAnalysis vLeftLegAnalysis;
                RightLegAnalysis vRightLegAnalysis;
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                {
                    vLeftLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                            LeftLegAnalysis;
                    double vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipFlexion * 100) / 100;
                    vText = "Left Hip Flexion / Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipAbduction * 100) / 100;
               
                    vText += "Left Hip Abduction/Adduction: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleLeftHipRotation * 100) / 100;
                    vText += "Left Hip Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Knee Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vLeftLegAnalysis.AngleKneeRotation * 100) / 100;
                    vText += "Tibial Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                }
                if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                {
                    vRightLegAnalysis =
                        vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                            RightLegAnalysis;

                    double vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleRightHipFlexion * 100) / 100;
                    vText += "Right Hip Flexion/Extension: " +  string.Format("{0:N2}", vTruncatedVal) + "\n";

                      vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleRightHipAbduction * 100) / 100;
                    vText += "Right Hip Abduction/Adduction: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleRightHipRotation * 100) / 100;
                    vText += "Right Hip Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Knee Flexion/Extension: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                    vTruncatedVal = Math.Truncate(vRightLegAnalysis.AngleKneeFlexion * 100) / 100;
                    vText += "Tibial Internal/External Rotation: " + string.Format("{0:N2}", vTruncatedVal) + "\n";

                }
                AngleInfoDisplayText.text = vText;
            }
        }
    }
}
