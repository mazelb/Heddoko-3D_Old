/** 
* @file DisplayAngleAnalysis.cs
* @brief Contains the DisplayAngleAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Demos;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.UI.MainScene.Model;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Displays Angle analysis
    /// </summary>
    public class DisplayAngleAnalysis : MonoBehaviour
    {
        private Body mCurrentBody;
        public BodyPlayer BodyPlayer;
        public Button LeftArmButton;
        public Button RightArmButton;
        public Button TorsoButton;
        public Button LeftLegButton;
        public Button RightLegButton;
        private CurrentAngleAnalysisView Analysis;
        public Text InformationTxtPanel;
        private enum CurrentAngleAnalysisView
        {
            LeftArm,
            RightArm, Torso, LeftLeg, RightLeg
        }

        private void Update()
        {
            mCurrentBody = BodyPlayer.CurrentBodyInPlay;
            switch (Analysis)
            {
                case (CurrentAngleAnalysisView.LeftArm):
                {
                    LeftArmAnalysis vLeftArm = mCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftArm] as LeftArmAnalysis;
                        break;
                    }
                case (CurrentAngleAnalysisView.RightArm):
                    {
                        break;
                    }
                case (CurrentAngleAnalysisView.LeftLeg):
                    {
                        break;
                    }
                case (CurrentAngleAnalysisView.RightLeg):
                    {
                        break;
                    }
                case (CurrentAngleAnalysisView.Torso):
                    {
                        break;
                    }
            }   
        }
    }
        
    }
}
