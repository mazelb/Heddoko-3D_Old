/** 
* @file LegAnalysis.cs
* @brief LegAnalysis  
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using UnityEngine;
using System;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
    [Serializable]
    public abstract class LegAnalysis : SegmentAnalysis
    {
        public Quaternion HipOrientation { get; set; }
        public Quaternion KneeOrientation { get; set; }
        public TorsoAnalysis TorsoAnalysisSegment { get; set; }
        public Quaternion TorsoOrientation { get { return TorsoAnalysisSegment.TorsoOrientation; } }
    }
}
