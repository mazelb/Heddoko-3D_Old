using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// Parent class to the specific subsegment. 
    /// </summary>
    public abstract class SegmentAnalysis
    {
        public virtual float[,] TorsoOrientation { get; set; }
        internal float DeltaTime;
        internal BodyStructureMap.SegmentTypes SegmentType;
        internal float mLastTimeCalled;
        /// <summary>
        /// Listener that is notified when the torso orientation has been updated
        /// </summary>
        /// <param name="vNewTorsoOrientation"></param>
        internal void UpdateTorsoOrientationListener(float[,] vNewTorsoOrientation)
        {
            TorsoOrientation = vNewTorsoOrientation;
        }
        /// <summary>
        /// Extraction of angles. The parent class Updates Delta time
        /// </summary>
        public virtual void AngleExtraction()
        {
          
            
        }
    }
}
