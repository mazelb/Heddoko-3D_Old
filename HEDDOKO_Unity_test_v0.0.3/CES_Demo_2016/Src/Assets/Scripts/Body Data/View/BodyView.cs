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
using System.Net;
using Assets.Scripts.Utils.UnityUtilities;

namespace Assets.Scripts.Body_Data.view
{
    /**
    * BodyView class 
    * @brief Contains the view for a body and allows the body to fetch data from a pipeline fed buffer on a frame by frame basis
    */
    public class BodyView : MonoBehaviour
    {

        //private BodyFrameBuffer mBuffer;
        private BodyFrameBuffer mBuffer;
        [SerializeField]
        private Body mAssociatedBody;
        private BodyFrame mCurreBodyFrame;
        [SerializeField]
        private bool mIsPaused;
        [SerializeField]
        private bool mStartUpdating;
        OutterThreadToUnityTrigger InitialFrameSetTrigger = new OutterThreadToUnityTrigger();

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
            get { return mStartUpdating; }
            set
            {
                if (value)
                {
                    mStartUpdating = false;
                }
                mStartUpdating = value;
            }
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

        /// <summary>
        /// Reset the initial frame
        /// </summary>
        public void ResetInitialFrame()
        {
            if (mAssociatedBody != null)
            {
                AssociatedBody.SetInitialFrame(mAssociatedBody.CurrentBodyFrame);
            }
        }

        /// <summary>
        /// Will be called on by an external thread in the case that the initial frame needs to be set 
        /// </summary>
        /// <param name="vInitialBodyframe">The initial bodyframe</param>
        internal void SetInitialBodyFrame(BodyFrame vInitialBodyframe)
        {
            InitialFrameSetTrigger.Triggered = true;
            InitialFrameSetTrigger.Args = vInitialBodyframe;
        }

        /// <summary>
        /// pause the current frame
        /// </summary>
        public void PauseFrame()
        {
            if (mAssociatedBody != null)
            {
                AssociatedBody.PauseThread();
                mIsPaused = !mIsPaused;
            }
        }

        /**
        * OnDisable()
        * @brief Automatically called by Unity when the app is exited. Cleans up tasks and unhooks event listeners  
        */
        void OnApplicationQuit()
        {
            if (AssociatedBody != null)
            {
                AssociatedBody.StopThread();
                AssociatedBody.UnhookBrainpackListeners();
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
                if (mIsPaused)
                {
                    return;
                }
                if (InitialFrameSetTrigger.Triggered)
                {
                    InitialFrameSetTrigger.Reset();
                    BodyFrame vInitBodyFrame = (BodyFrame) InitialFrameSetTrigger.Args;
                    AssociatedBody.SetInitialFrame(vInitBodyFrame);
                }
                if (mBuffer != null && mBuffer.Count > 0)
                {
                    BodyFrame vBodyFrame = mBuffer.Dequeue();

                    if (AssociatedBody.InitialBodyFrame == null)
                    {
                        AssociatedBody.SetInitialFrame(vBodyFrame);
                    }

                    AssociatedBody.UpdateBody(vBodyFrame);
                    Dictionary<BodyStructureMap.SensorPositions, float[,]> vDic = Body.GetTracking(AssociatedBody); 
                   
                    if (vDic != null)
                    {
                        Body.ApplyTracking(AssociatedBody, vDic);
                        //todo: extract this from the view and place it in its own module
                    } 
                }
            }
        }

        /**
        * Awake()
        * @brief Automatically called by Unity when the game object awakes. In this case, look for the debug gameobject in the scene 
        * and set the body view to this.
        */
        private void Awake()
        {
            if (name != "body view guid: e75115c356218d84fa35dbd8a3159284")
            {
                GameObject vGo = GameObject.FindGameObjectWithTag("debug");
                if (vGo)
                {
                    Application.targetFrameRate = 10;
                    Debugger vDebugger = vGo.GetComponent<Debugger>();
                    vDebugger.View = this;
                }
            }
        }
    }
}
