/** 
* @file BodySubsegmentView.cs
* @brief The view for a body's subsegment. Application of transforms happen in here. 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodySubsgmentView  class 
    * @brief BodySubsegmentView class (represents one subsegment view)
    */
    public class BodySubsegmentView : MonoBehaviour
    {
        //the transforms required to transform
        public BodySubSegment AssociatedSubSegment;
        public List<Transform> SubSegmentTransforms = new List<Transform>();

        /**
        * View
        * @param 
        * @brief View associated with this body
        * @note: a new gameobject is created and this Body is added into it as a component
        * @return returns the view associated with this body
        */
        public void AssignTransforms()
        {
            //find the object in the scene with the tag
            GameObject[] vGameObjects = GameObject.FindGameObjectsWithTag(gameObject.name);
            foreach (GameObject vObj in vGameObjects)
            {
                Transform vTransform = vObj.transform;

                SubSegmentTransforms.Add(vTransform);
            }
        }

        public void ApplyTransformations(Quaternion vNewOrientation)
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                vObjTransform.rotation = vNewOrientation;
            }
        }

        public void ResetOrientations()
        {
            foreach (Transform vObjTransform in SubSegmentTransforms)
            {
                vObjTransform.rotation = Quaternion.identity;
            }
        }

        /**
        * UpdateOrientation(Quaternion vNewOrientation)
        * @param Quaternion vNewOrientation: the new orientation of the subsegment
        * @brief Updates the current orientation with the passed in parameter
        */
        internal void UpdateOrientation(Quaternion vNewOrientation)
        {
            try
            {
                ApplyTransformations(vNewOrientation);
            }
            catch (Exception)
            {
                
            }
        }

        /**
        * ResetOrientation()
        * @brief Resets the current orientation to its initial value
        */
        internal void ResetOrientation()
        {
            ResetOrientations();
        }

        #region Unity functions

        /**
        * Awake()
        * @brief On Awake: set the segment's initial orientation .
        */
        internal void Awake()
        {
            AssignTransforms();
        }

        #endregion

    }
}
