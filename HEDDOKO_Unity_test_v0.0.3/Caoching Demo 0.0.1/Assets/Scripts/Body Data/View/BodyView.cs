/** 
* @file BodyView.cs
* @brief Contains the BodyView class and functionalities required to execute it The view for a body . 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodyView class 
    * @brief Contains the view for a body and allows the body to fetch data from a pipeline fed buffer on a frame by frame basis
    */
    public class BodyView : MonoBehaviour
    {
        public Button ResetButton;
        private BodyFrameBuffer mBuffer;
        private Body mAssociatedBody;
        private BodyFrame mCurreBodyFrame;

        /**
        * StartUpdating
        * @param bool: sets the property 
        * @brief StartUpdating allows the  needed to start pulling data from the buffer in order to update the associated body 
        * @return returns the property's value
        */
        public bool StartUpdating
        {
            get;
            set;
        }
        /**
        * Init(Body vAssociatedBody, BodyFrameBuffer vBuffer)
        * @param bool: Body vAssociatedBody: the associated body for this view, BodyFrameBuffer vBuffer: the buffer to pull bodyframe data  from
        * @brief StartUpdating allows the  needed to start pulling data from the buffer in order to update the associated body 
        * @return returns the property's value
        */
        public void Init(Body vAssociatedBody, BodyFrameBuffer vBuffer)
        {
            this.mBuffer = vBuffer;
            this.mAssociatedBody = vAssociatedBody;

        }
        /**
        * ResetJoint()
        * @brief sets the current frame to be the initial body frame
        */
        public void ResetJoint()
        {
            if (mCurreBodyFrame != null)
            {
                mAssociatedBody.SetInitialFrame(mAssociatedBody.CurrentBodyFrame);
            }
        }
        /**
        * OnDisable()
        * @brief Automatically called by Unity when the app is exited. Tells the associated body to stop its tasks
        */
        void OnApplicationQuit()
        {
            if (mAssociatedBody != null)
            {
                mAssociatedBody.StopThread();
            }
        }

        /**
        * Update()
        * @brief Automatically called by Unity and if conditions are set, will update the associated body with body frame data fetched from mBuffer.
        */
        private void Update()
        {
            if (StartUpdating)
            {
                if (mBuffer != null)
                {
                    mCurreBodyFrame = mBuffer.Dequeue();
                    mAssociatedBody.UpdateBody(mCurreBodyFrame);
                }
            }

        }


    }
}
