 
using UnityEngine;
using UnityEngine.UI;
/**
* @file bodyView.cs:
* @brief This file contains all of the functionality required
* to execute the containment of the Body's view in unity
* @author: Mohammed Haider(Mohammed@heddoko.com)
* @date October 2015
*/
namespace Assets.Scripts.Body_Data.view
{
	/**
	* BodyView  class 
	* @brief BodyView class (represents one Body view)
	*/    
	public class BodyView : MonoBehaviour
    {
        #region member fields 
        private BodyFrameBuffer mBuffer;
        private Body mAssociatedBody;
        public  bool mStartUpdating;
        public BodyFrame CurrentFrame;
        #endregion

        /**
        * Init(Body mAssociatedBody, BodyFrameBuffer vBuffer)
        * @param Body vAssociatedBody, the  Body object associated with this view. BodyFrameBuffer vBuffer: the buffer needed to pull bodyframe data from
        * @brief Initializes the class object with passed in parameters.
        * @note 
        * @return 
        */
        public void Init(Body vAssociatedBody, BodyFrameBuffer vBuffer)
        { 
            this.mBuffer = vBuffer;
            this.mAssociatedBody = vAssociatedBody; 
        }
        /**
        * ResetJoint()
        * @param 
        * @brief Sets the initial frame of the associated body with the current frame. 
        * @note 
        * @return 
        */
        public void ResetJoint()
        {
            if (mAssociatedBody.CurrentBodyFrame != null)
            {
                mAssociatedBody.SetInitialFrame(mAssociatedBody.CurrentBodyFrame);
            }
        }

        #region Unity function calls
        /**
        * OnDisable()
        * @param  
        * @brief OnDisable is automatically initiated in the event that the current script is disabled. 
        * @note 
        * @return 
        */
        void OnApplicationQuit()
        {
            if (mAssociatedBody != null)

            {
                mAssociatedBody.StopThread(); //stop threads
            }
        }


        /**
        * Update()
        * @param  
        * @brief Update is called on every frame. In this case, dequeue body frame data from the buffer and update the associated body 
        * @note 
        * @return 
        */
        private void Update()
        {
			if (mStartUpdating)
            {
                if (mBuffer != null)

                {
                   CurrentFrame = mBuffer.Dequeue();
                    mAssociatedBody.UpdateBody(CurrentFrame); 
                }
            }
          
        }
        #endregion
  
    }
}
