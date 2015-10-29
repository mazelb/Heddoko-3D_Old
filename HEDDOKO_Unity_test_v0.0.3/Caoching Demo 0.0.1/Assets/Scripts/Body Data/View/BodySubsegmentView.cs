/** 
* @file BodySubsegmentView.cs
* @brief The view for a body's subsegment. Application of transforms happen in here. 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
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
  
        public  Transform vJointTransform; 

        #endregion
        #region properties
        /**
        * JointTransform
        * @param 
        * @brief The joint transform associated with this subsegment view, if not assigned, calls on the AssignTransform helper function
        * @return returns the Transform associated with this body
        */
        public Transform JointTransform
        {
            get
            {
                if (vJointTransform == null)
                {
                    AssignTransform();
                }
                return vJointTransform;
            }
            set { vJointTransform = value; }
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
            GameObject go = GameObject.FindWithTag(gameObject.name);
            vJointTransform = go.transform;
        }

        /**
       * UpdateOrientation(Quaternion vNewOrientation)
       * @param Quaternion vNewOrientation: the new orientation of the subsegment
       * @brief Updates the current orientation with the passed in parameter
       */
        internal void UpdateOrientation(Quaternion vNewOrientation)
        {  
            vJointTransform.rotation = vNewOrientation;
        }
    }
}
