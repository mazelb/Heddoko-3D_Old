using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// Parent class to the specific abstracted segment type(leg or arm). 
    /// </summary>
    public abstract class SegmentAnalysis
    {
        internal float DeltaTime;
        internal BodyStructureMap.SegmentTypes SegmentType;
        internal float mLastTimeCalled;

        /// <summary>
        /// Extraction of angles. The parent class Updates Delta time
        /// </summary>
        public virtual void AngleExtraction()
        {
          
            
        }
    }
}
