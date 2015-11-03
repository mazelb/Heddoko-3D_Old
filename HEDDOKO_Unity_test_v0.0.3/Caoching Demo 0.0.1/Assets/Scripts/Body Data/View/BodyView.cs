/** 
* @file BodyView.cs
* @brief Contains the BodyView class and functionalities required to execute it The view for a body . 
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using Assets.Scripts.Body_Pipeline.Tracking;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodyView class 
    * @brief Contains the view for a body and allows the body to fetch data from a pipeline fed buffer on a frame by frame basis
    */
    public class BodyView : MonoBehaviour
    {
       
        //private BodyFrameBuffer mBuffer;
        private TrackingBuffer mBuffer;
        private Body mAssociatedBody;
        private BodyFrame mCurreBodyFrame;

        /**
        * AssociatedBody
        * @param Internally set the Body associated to this view
        * @brief Class property that returns the Body associated with this view
        * @return returns the property's value
        */
        public Body AssociatedBody
        {
            get
            {
                return mAssociatedBody;
            }
            internal set
            {
                mAssociatedBody = value;
            }
        }

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
        public void Init(Body vAssociatedBody, TrackingBuffer vBuffer)
        {
            this.mBuffer = vBuffer;
            this.mAssociatedBody = vAssociatedBody;

        }
        /**
        * SetInitialFrameToCurrent()
        * @brief sets the current frame to be the initial body frame
        */
<<<<<<< HEAD
        public void SetInitialFrameToCurrent()
=======
        public void ResetInitialFrame()
>>>>>>> origin/master
        {
            if (mCurreBodyFrame != null)
            {
                AssociatedBody.SetInitialFrame(mAssociatedBody.CurrentBodyFrame); 
            }
        }
        /**
        * OnDisable()
        * @brief Automatically called by Unity when the app is exited. Tells the associated body to stop its tasks
        */
        void OnApplicationQuit()
        {
            if (AssociatedBody != null)
            {
                AssociatedBody.StopThread();
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
                if (mBuffer != null && mBuffer.Count>0)
                {
                    Dictionary<BodyStructureMap.SensorPositions, float[,]> v  = mBuffer.Dequeue();
                    AssociatedBody.UpdateBody(AssociatedBody.CurrentBodyFrame);
                    Body.ApplyTracking(AssociatedBody);
                } 
            }
        }
<<<<<<< HEAD
        /**
         * Awake()
         * @brief Automatically called by Unity when the game object awakes. In this case, look for the debug gameobject in the scene 
         * and set the body view to this.
         */
        private void Awake()
        {
            if(name != "body view guid: e75115c356218d84fa35dbd8a3159284")
            {
                GameObject vGo = GameObject.FindGameObjectWithTag("debug");
                if( vGo)
                { 
                    Debugger vDebugger = vGo.GetComponent<Debugger>();
                    vDebugger.View = this;
                }
            }
          
        }

=======
>>>>>>> origin/master
    }
}
