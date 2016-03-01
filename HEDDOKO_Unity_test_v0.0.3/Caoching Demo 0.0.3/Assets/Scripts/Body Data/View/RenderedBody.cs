/** 
* @file RenderedBody.cs
* @brief The rendering component of a Body
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Runtime.Serialization;
using Assets.Scripts.Body_Data.view;
using UnityEngine;

namespace Assets.Scripts.Body_Data
{
    /// <summary>
    /// The class that models the in-scene avatar, referencing it's visual and movement components. 
    /// </summary>
    public class RenderedBody: MonoBehaviour
    {
        public BodyView AssociatedBodyView;
        public SkinnedMeshRenderer Joints;
        public SkinnedMeshRenderer Torso;
        public SkinnedMeshRenderer Limbs;

        public Shader NormalShader;
        public Shader XRayShader;
        public Transform UpperLeftArm;
        public Transform LowerLeftArm;
        public Transform UpperRightArm;
        public Transform LowerRightArm;
        public Transform UpperLeftLeg;
        public Transform LowerLeftLeg;
        public Transform UpperRightLeg;
        public Transform LowerRightLeg;
        public Transform Hips;
        public Transform UpperSpine;
        


        [SerializeField]
        private BodyStructureMap.BodyTypes mCurrentBodyType;
        /// <summary>
        /// on awake register transformations
        /// </summary>
        void Awake()
        {
            
        }

        /// <summary>
        /// Applies a transformation to the skin based on the body type
        /// </summary>
        /// <param name="vTypes"></param>
        public void Init(BodyStructureMap.BodyTypes vType)
        {
            mCurrentBodyType = vType;
        }

        /// <summary>
        /// Updates the current body type to the passed in body type
        /// </summary>
        /// <param name="vType"></param>
        public void UpdateBodyType(BodyStructureMap.BodyTypes vType)
        {
            mCurrentBodyType = vType;
        }

        /// <summary>
        /// Assign a new rendering layer to the gameobject
        /// </summary>
        /// <param name="vNewLayer">layer mask</param>
        public void AssignLayer(int vNewLayer)
        {
            
        }

        /// <summary>
        /// Assign a new rendering layer to the gameobject
        /// </summary>
        /// <param name="vNewLayer">layer name</param>
        public void AssignLayer(string vNewLayer)
        {
            
        }
        /// <summary>
        /// Hides the segment based on the segment passed in
        /// </summary>
        /// <param name="vSegment"></param>
        public void HideSegment(BodyStructureMap.SegmentTypes vSegment)
        {
            
        }

        /// <summary>
        /// Changes a segment's color based on the segment
        /// </summary>
        /// <param name="vSegment"></param>
        /// <param name="vNewColor"></param>
        public void ChangeSegmentColor(BodyStructureMap.SegmentTypes vSegment, Color32 vNewColor)
        {
            
        }

        /// <summary>
        /// Valides the segment change based on the current body type
        /// </summary>
        /// <param name="vSegment"></param>
        /// <returns></returns>
        private bool ValidateSegmentChange(BodyStructureMap.SegmentTypes vSegment)
        {
            return true;
        }

        /// <summary>
        /// deactivates the current rendered body and cleans up resources
        /// </summary>
        public void Cleanup()
        {
            
        }

    }
   

    [Serializable]
    public class InvalidSegmentChangeRequestException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidSegmentChangeRequestException()
        {
        }

        public InvalidSegmentChangeRequestException(string message) : base(message)
        {
        }

        public InvalidSegmentChangeRequestException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidSegmentChangeRequestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
