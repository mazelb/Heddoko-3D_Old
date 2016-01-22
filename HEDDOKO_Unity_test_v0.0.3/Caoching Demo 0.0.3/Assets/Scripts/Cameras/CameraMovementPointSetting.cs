
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Data model which holds settings for different view points, from which a camera controller will reference as a camera approaches a LookAtTarget
    /// </summary>
    [Serializable]
    public class CameraMovementPointSetting : MonoBehaviour
    {
        private static Dictionary<FeedbackTextCategory, Dictionary<PlaneNormalFromTransformType, string>> sgFeedbackTextMapping =
          new Dictionary<FeedbackTextCategory, Dictionary<PlaneNormalFromTransformType, string>>();

        private static bool sgFeedbackTextMapComplete=false;

        //Offset from the transform
        public Vector3 Offset;
        public float OrthographicSize = 1f;
        public Transform LookAtTarget;
        public bool FollowLookAtTarget;

        //the Arcsprite's angle at this position.
        public Sprite ArcSprite;

        /// <summary>
        /// A transform from which a plane will orient itself with.
        /// </summary>
        public Transform PlaneOrientationTransform;

        //How far is the object away from the 
        public Vector3 TransformOffset;
        [SerializeField]
        private PlaneNormalFromTransformType PlaneNormalType;

        void Awake()
        {
            if (!sgFeedbackTextMapComplete)
            {
                Dictionary<PlaneNormalFromTransformType, string> vLessThanFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, string>(3);
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Right, "Bad position on sagital plane ");
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Up , "MOVE ELBOW BACKWARDS");
                vLessThanFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, "LOWER ELBOW");

                Dictionary<PlaneNormalFromTransformType, string> vGreaterThanFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, string>(3);
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Right, "Bad position on sagital plane ");
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Up , "MOVE ELBOW FORWARD");
                vGreaterThanFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, "RAISE ELBOW");

                Dictionary<PlaneNormalFromTransformType, string> vGoodFeedbackMapping = new Dictionary<PlaneNormalFromTransformType, string>(3);
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Right, "GOOD");
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Forward, "GOOD");
                vGoodFeedbackMapping.Add(PlaneNormalFromTransformType.Up, "GOOD");

                sgFeedbackTextMapping.Add(FeedbackTextCategory.LessThan, vLessThanFeedbackMapping);
                sgFeedbackTextMapping.Add(FeedbackTextCategory.GreaterThan, vGreaterThanFeedbackMapping);
                sgFeedbackTextMapping.Add(FeedbackTextCategory.Good, vGoodFeedbackMapping);
                sgFeedbackTextMapComplete = true;
            }
           
        }

        /// <summary>
        /// Getter that returns feedback text at current point
        /// </summary>
        public string GetFeedBackTextAtCurrentPoint(FeedbackTextCategory vTextCategory)
        {
            return sgFeedbackTextMapping[vTextCategory][PlaneNormalType];
        }


        public Vector3 PlaneNormal
        {
            get
            {
                Vector3 vToBeReturned = Vector3.zero;
                switch (PlaneNormalType)
                {
                    case (PlaneNormalFromTransformType.Up):
                        vToBeReturned = -PlaneOrientationTransform.up;
                        break;
                    case (PlaneNormalFromTransformType.Forward):
                        vToBeReturned = PlaneOrientationTransform.forward;
                        break;
                    case (PlaneNormalFromTransformType.Right):
                        vToBeReturned = PlaneOrientationTransform.right;
                        break;
                }
                return vToBeReturned;
            }
        }

        public float multi=1f;

        void LateUpdate()
        {
            if (FollowLookAtTarget)
            {
                transform.position = LookAtTarget.position  + (((LookAtTarget.forward  )*  5f * multi ))  ;
            }
        }
        public enum PlaneNormalFromTransformType
        {
            Up,
            Forward,
            Right
        }

        public enum FeedbackTextCategory
        {
            LessThan,
            Good,
            GreaterThan
        }


    }
}
