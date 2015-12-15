﻿
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// This class controls a camera's behaviour in scene
    /// </summary>
    [RequireComponent(typeof(MoveCameraToPositon))]
    public class CameraController : MonoBehaviour
    {
        public LayerMask ModelMask;
        private RaycastHit mHitInfo;
        private Camera mCurrentCam;
        private CameraOrbit mCameraOrbit;
        private bool mCommenceMove;
        public float ReturnSpeed = 10f;
        public bool ReturnToPositionEnabled=true;
        private MoveCameraToPositon mAutoCamMover;
        private Vector3 mOriginalPos;
        private Quaternion mOriginalRotation;
        private bool mIn2DMode = false;
        void Awake()
        {
            mCurrentCam = GetComponent<Camera>();
            mCameraOrbit = GetComponent<CameraOrbit>();
            mAutoCamMover = GetComponent<MoveCameraToPositon>();
            mAutoCamMover.Cam = mCurrentCam;
            mOriginalPos = transform.position;
            mOriginalRotation = transform.rotation;
        }
        
        void Update()
        {
            if (!mIn2DMode)
            {
                //left click
                if (Input.GetMouseButton(0) && !mCommenceMove)
                {
                    //check if the mouse has landed on the model in view
                    mHitInfo = new RaycastHit();

                    //check if the mouse is right above the model
                    if (Physics.Raycast(mCurrentCam.ScreenPointToRay(Input.mousePosition), 250f, ModelMask))
                    {
                        mCommenceMove = true;
                    }
                }
                if (mCommenceMove)
                {
                    if (Input.GetMouseButton(0))
                    {
                        if (mCameraOrbit != null)
                        {
                            mCameraOrbit.enabled = true;
                        }
                        mAutoCamMover.enabled = false;
                    }
                    else
                    {
                        if (mCameraOrbit != null)
                        {
                            mCameraOrbit.enabled = false;
                        }
                        mCommenceMove = false;
                        if (ReturnToPositionEnabled)
                        {
                            if (!mAutoCamMover.enabled)
                            {
                                mAutoCamMover.enabled = true;
                            }
                        }

                    }

                }
                 
            }
            InputHandler();
        }

        /// <summary>
        /// handles input
        /// </summary>
        private void InputHandler()
        {
            //move camera
            if (Input.GetKeyDown(KeyCode.Space))
            {
                mAutoCamMover.MovetoNextPos();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                mAutoCamMover.MoveToPos(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                mAutoCamMover.MoveToPos(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                mAutoCamMover.MoveToPos(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                mAutoCamMover.MoveToPos(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                mAutoCamMover.MoveToPos(4);
            }
        }

        /// <summary>
        /// Disable camera orbit and move to camera scripts
        /// </summary>
        public void PrepFor2DView()
        {
            if (mCameraOrbit)
            {
                mCameraOrbit.enabled = false;
            }
            mAutoCamMover.enabled = false;
            mIn2DMode = true;
            Reset();
        }
        /// <summary>
        /// Enables camera orbit and move to camera scripts
        /// </summary>
        public void PrepFor3DView()
        {
            if (mCameraOrbit)
            {
                mCameraOrbit.enabled = true;
            }
            mAutoCamMover.enabled = true;
            mIn2DMode = false;
        }
        /// <summary>
        /// Resets the tranformation
        /// </summary>
       public void Reset()
        {
            transform.position = mOriginalPos;
            transform.rotation = mOriginalRotation;
        }

    }
}
