
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved 
*/

using System.Collections;
using Assets.Scripts.UI.RecordingLoading;
using Assets.Scripts.UI._2DSkeleton;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        public Button LeftButton;
        public Button RightButton;
        public LegMoveSwitcher LegSwitcher;
        void Awake()
        {
            mCurrentCam = GetComponent<Camera>();
            mCameraOrbit = GetComponent<CameraOrbit>();
            mAutoCamMover = GetComponent<MoveCameraToPositon>();
            mAutoCamMover.Cam = mCurrentCam;
            mOriginalPos = transform.position;
            mOriginalRotation = transform.rotation;
            if (LeftButton != null && RightButton != null)
            {
                LeftButton.onClick.AddListener(SwitchToLeftSide);
                RightButton.onClick.AddListener(SwitchToRightSide);
            }
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
        /// Switches to the left view of the body, depending on whether in 2d mode or 3d
        /// </summary>
        private void SwitchToLeftSide()
        {
            if (mIn2DMode)
            {
                if (LegSwitcher != null)
                {
                    LegSwitcher.TurnOnSprite(0);
                }
            }
            else
            {
                mAutoCamMover.MoveToPos(3);
                mAutoCamMover.enabled = true;
            }
          
        }

        /// <summary>
        /// Switches to the right  view of the body, depending on whether in 2d mode or 3d
        /// </summary>
        private void SwitchToRightSide() {
            if (mIn2DMode)
            {
                if (LegSwitcher != null)
                {
                    LegSwitcher.TurnOnSprite(1);
                }
            }
            else
            {
                mAutoCamMover.MoveToPos(1);
                mAutoCamMover.enabled = true;
            }
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
