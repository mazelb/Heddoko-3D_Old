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
        public Transform HipTransform { get; set; }
        public Transform KneeTransform { get; set; }
        public TorsoAnalysis TorsoAnalysisSegment { get; set; }
        public Transform TorsoTransform { get { return TorsoAnalysisSegment.TorsoTransform; } }
        public Transform HipGlobalTransform { get { return TorsoAnalysisSegment.HipGlobalTransform; } }
    }
}
