﻿
/** 
* @file ArmAnalysis.cs
* @brief ArmAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using UnityEngine;
using System;

namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    [Serializable]
    public abstract class ArmAnalysis: SegmentAnalysis
    {
        public Quaternion UpArOrientation { get; set; }
        public Quaternion LoArOrientation { get; set; }
        public TorsoAnalysis TorsoAnalysisSegment { get; set; }
        public Quaternion TorsoOrientation { get { return TorsoAnalysisSegment.TorsoOrientation; } }
    }
}
