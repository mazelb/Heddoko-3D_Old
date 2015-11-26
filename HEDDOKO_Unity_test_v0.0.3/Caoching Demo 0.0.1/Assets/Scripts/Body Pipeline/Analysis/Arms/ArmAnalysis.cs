
/** 
* @file ArmAnalysis.cs
* @brief ArmAnalysis class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Analysis.Torso;

namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    public abstract class ArmAnalysis: SegmentAnalysis
    {
        internal float[,] UpArOrientation { get; set; }
        internal float[,] LoArOrientation { get; set; }

        public TorsoAnalysis TorsoAnalysisSegment { get; set; }
        public float[,] TorsoOrientation { get { return TorsoAnalysisSegment.TorsoOrientation; } }


    }
}
