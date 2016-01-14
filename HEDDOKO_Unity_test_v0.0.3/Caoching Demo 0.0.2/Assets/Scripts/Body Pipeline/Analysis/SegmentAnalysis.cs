using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// Parent class to the specific abstracted segment type(leg or arm). 
    /// </summary>\
    [Serializable]
    public abstract class SegmentAnalysis
    {
        [SerializeField]
        internal float DeltaTime;
        [SerializeField]
        internal BodyStructureMap.SegmentTypes SegmentType;
        [SerializeField]
        internal float mLastTimeCalled;

        /// <summary>
        /// Extraction of angles. The parent class Updates Delta time
        /// </summary>
        public virtual void AngleExtraction()
        {
          
            
        }
    }
}
