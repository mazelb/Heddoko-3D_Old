using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Body_Pipeline.Analysis.Legs
{
   public abstract class LegAnalysis : SegmentAnalysis
    {
        internal float[,] HipOrientation { get; set; }
        internal float[,] KneeOrientation { get; set; }
    }
}
