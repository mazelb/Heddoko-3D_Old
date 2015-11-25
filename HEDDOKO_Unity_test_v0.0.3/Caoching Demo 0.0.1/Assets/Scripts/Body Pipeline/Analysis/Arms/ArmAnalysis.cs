using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Body_Pipeline.Analysis.Arms
{
    public abstract class ArmAnalysis: SegmentAnalysis
    {
        internal float[,] UpArOrientation { get; set; }
        internal float[,] LoArOrientation { get; set; } 
    
        /// <summary>
        /// Listens to the event that the torso has updated its orientation
        /// </summary>
        /// <param name="vNewTorsoOrientation"></param>
        public void TorsoOrientationUpdated(float[,] vNewTorsoOrientation)
        {
            TorsoOrientation = vNewTorsoOrientation;
        }


    }
}
