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
  
        public  Transform jointTransform;
        private NodJoint joint;
        public Quaternion initialRot;


        void Awake()
        {
            initialRot = JointTransform.rotation;
        }

        #endregion
        #region properties
        public Transform JointTransform
        {
            get
            {
                if (jointTransform == null)
                {
                    AssignTransform();
                }
                return jointTransform;
            }
            set { jointTransform = value; }
        }
  
        #endregion

    /**
    * View
    * @param 
    * @brief View associated with this body
    * @note: a new gameobject is created and this Body is added into it as a compnent
    * @return returns the view associated with this body
    */

        public void AssignTransform()
        {
            //find the object in the scene with the tag
            GameObject go = GameObject.FindWithTag(gameObject.name);
            jointTransform = go.transform;
        }

        //updates the orientation of the joint transform
        internal void UpdateOrientation(Quaternion newRot)
        { 
           /* eulerRotation *= Mathf.Rad2Deg;
            Quaternion q = Quaternion.Euler(eulerRotation);
            JointTransform.rotation = q*initialRot;*/
            jointTransform.rotation = newRot;
        }

       

        internal void ResetOrientation(Vector3 eulerRotation)
        {
            initialRot = Quaternion.Euler(eulerRotation*Mathf.Rad2Deg);
        }
    }
}
