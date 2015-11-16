/** 
* @file BodySubsegmentView.cs
* @brief The view for a body's subsegment. Application of transforms happen in here. 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine; 
namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodySubsgmentView  class 
    * @brief BodySubsegmentView class (represents one subsegment view)
    */
    public class BodySubsegmentView : MonoBehaviour
    {
        #region fields
        //the transforms required to transform
  
        public Transform vSubSegmentTransform;
      //  public Transform vSubSegmentInitialTransform;
        private Quaternion vSubSegmentInitialOrientation;

        public BodySubSegment AssociatedSubSegment;
        #endregion
        #region properties
        /**
        * JointTransform
        * @param 
        * @brief The joint transform associated with this subsegment view, if not assigned, calls on the AssignTransform helper function
        * @return returns the Transform associated with this body
        */
        public Transform SubSegmentTransform
        {
            get
            {
                if (vSubSegmentTransform == null)
                {
                    AssignTransform();
                }
                return vSubSegmentTransform;
            }
            set { vSubSegmentTransform = value; }
        }
  
        #endregion

        /**
        * View
        * @param 
        * @brief View associated with this body
        * @note: a new gameobject is created and this Body is added into it as a component
        * @return returns the view associated with this body
        */
        public void AssignTransform()
        {
            //find the object in the scene with the tag
            GameObject vGameObject = GameObject.FindWithTag(gameObject.name);
            vSubSegmentTransform = vGameObject.transform;
            //vSubSegmentInitialTransform = vSubSegmentTransform;
        }

        /**
       * UpdateOrientation(Quaternion vNewOrientation)
       * @param Quaternion vNewOrientation: the new orientation of the subsegment
       * @brief Updates the current orientation with the passed in parameter
       */
        internal void UpdateOrientation(Quaternion vNewOrientation)
        {
            SubSegmentTransform.rotation = vNewOrientation; 
        }

        /**
        * ResetOrientation()
        * @brief Resets the current orientation to its initial value
        */
        internal void ResetOrientation()
        {
            SubSegmentTransform.rotation = vSubSegmentInitialOrientation;
                // vSubSegmentInitialTransform.rotation;// vNewOrientation;
        }

        #region Unity functions
        /**
        * Awake()
        * @brief On Awake: set the segment's initial orientation .
        */
        internal void Awake()
        {
            vSubSegmentInitialOrientation = SubSegmentTransform.rotation;
        }
        #endregion

    }
}
