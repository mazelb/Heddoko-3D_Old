/** 
* @file bodySubsegmentView.cs
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
        //member transform needed to update orientation
        public Transform JointTransform;   

        #endregion
        #region properties
		/**
		* mJointTransform
		* @param Transform
		* @brief class property 
		* @return 
		*/ 
        public Transform Transform
        {
            get
            {
                if (JointTransform == null)

                {
                    LookForAssociatedTransform();
                }
                return JointTransform;
            }
            set { JointTransform = value; }
        }
  
        #endregion

		/**
		*  LookForAssociatedTransform()
		* @param 
		* @brief Finds the associated transform in the scene by searching for tags that are equal to the this current's view gameObject.name  
		* @return 
		*/ 
        public void LookForAssociatedTransform()
        {
            //find the object in the scene with the tag
            GameObject go = GameObject.FindWithTag(gameObject.name);
            Transform = go.transform;
        }

		/**
		* UpdateOrientation(Quaternion vRotation))
		* @param Quaternion vRotation: Orientation in the form of a quaternion
		* @brief Sets the orientation of the mTransforms rotation the passed in parameter
		* @return  
		*/
        internal void UpdateOrientation(Quaternion vRotation)
        {  
			Transform.rotation= vRotation;
        } 
 

    }
}
