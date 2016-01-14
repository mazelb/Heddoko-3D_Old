using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Body_Pipeline.Analysis
{
    /// <summary>
    /// Parent class to the specific abstracted segment type(leg or arm). 
<<<<<<< HEAD
    /// </summary>\
=======
    /// </summary>
>>>>>>> 553f1b2ab299babdca98f7191091449b4d1e54a2
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

        /*float SignedAngle(Vector3 a, Vector3 b, Vector3 n)
        {
            // angle in [0,180]
            float angle = Vector3.Angle(a,b);
            float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));

            // angle in [-179,180]
            float signed_angle = angle * sign;

            // angle in [0,360] (not used but included here for completeness)
            float angle360 =  (signed_angle + 180) % 360;

            return angle360;    
        }//*/
    }
}
