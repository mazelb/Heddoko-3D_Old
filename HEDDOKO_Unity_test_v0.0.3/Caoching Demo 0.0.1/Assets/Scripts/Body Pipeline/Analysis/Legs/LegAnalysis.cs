/** 
* @file LegAnalysis.cs
* @brief LegAnalysis  
* @author Mohammed Haider(mohamed@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved

*/

using Assets.Scripts.Body_Pipeline.Analysis.Torso;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
   public abstract class LegAnalysis : SegmentAnalysis
    {
        internal float[,] HipOrientation { get; set; }
        internal float[,] KneeOrientation { get; set; }
        public TorsoAnalysis TorsoAnalysisSegment { get; set; }
        public float[,] TorsoOrientation { get { return TorsoAnalysisSegment.TorsoOrientation; } }
    }
}
