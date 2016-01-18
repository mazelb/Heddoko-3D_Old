﻿
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.UI.NFLDemo;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// this camera controller is to be used solely for the NFL tech crunch demo
    /// </summary>
    [Serializable]
    public class NFLCameraController : MonoBehaviour
    {
        public Camera Camera;
        public BezierCurve Curve;
        public CameraLookAt CamLookAt;
        [SerializeField]
        private float mLerpPercentage = 0;
        public float MovementSpeed = 2f;
        [SerializeField]
        private bool mFinishedMovingCam = true;
        /// <summary>
        /// Has the camera finished moving? public getter and private setter
        /// </summary>
        public bool FinishedMovingCam
        {
            get { return mFinishedMovingCam; }
            private set { mFinishedMovingCam = value; }
        }
        [SerializeField]
        private int mCurrCamIndex = 0;
        /// <summary>
        /// Getter that provides the current camera index, private setter
        /// </summary>
        public int CurrCamIndex
        {
            get
            {
                if (mCurrCamIndex >= Curve.pointCount)
                {
                    mCurrCamIndex = 0;
                }
                return mCurrCamIndex;
            }
            private set
            {
                mCurrCamIndex = value; 
            }
        }

        private int mNextCamIndex = 1;
        /// <summary>
        /// Getter that provides the next camera index, private setter
        /// </summary>
        public int NextCamIndex
        {
            get
            {
                return mNextCamIndex;

            }
            private set
            {
                mNextCamIndex = value;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(MoveNext());
            }
        }

        /// <summary>
        /// Move to next position
        /// </summary>
        public void MoveToNextPos()
        {
            FinishedMovingCam = false;
            StopAllCoroutines();
            StartCoroutine(MoveNext());
        }
        IEnumerator MoveNext()
        {
            float vStartTime = 0;
            float vCurrOrthoCam = Camera.orthographicSize;
            Vector3 vCurrentLookAtPos = CamLookAt.Target.position;
            while (true)
            {

                if (CurrCamIndex >= Curve.pointCount)
                {
                    CurrCamIndex = 0;
                }

                NextCamIndex = CurrCamIndex + 1;

                if (CurrCamIndex == Curve.pointCount - 1)
                {
                    NextCamIndex = 0;
                }

                CameraMovementPointSetting vNextPointSetting =
                  Curve[NextCamIndex].gameObject.GetComponent<CameraMovementPointSetting>();

                vStartTime += Time.deltaTime;
                mLerpPercentage = vStartTime / MovementSpeed;
                //mLerpPercentage = mLerpPercentage * mLerpPercentage * mLerpPercentage * (mLerpPercentage * (6f * mLerpPercentage - 15f) + 10f);
                Vector3 vNewPosition = BezierCurve.GetPoint(Curve[CurrCamIndex], Curve[NextCamIndex], mLerpPercentage);
                float vNextOrthoSize = Mathf.Lerp(vCurrOrthoCam, vNextPointSetting.OrthographicSize, mLerpPercentage);
                Vector3 vNextLookAtPos = Vector3.Lerp(vCurrentLookAtPos, vNextPointSetting.LookAtTarget.position,
                    mLerpPercentage);

                Camera.transform.position = vNewPosition;
                Camera.orthographicSize = vNextOrthoSize;
                 CamLookAt.TargetPos = vNextLookAtPos;
                CamLookAt.Target = vNextPointSetting.LookAtTarget;
                if (mLerpPercentage >= 1)
                {
                    CurrCamIndex++;
                    FinishedMovingCam = true;
                    mLerpPercentage = 0;
                    break;
                }

                yield return null;
            }
        }

        /// <summary>
        /// Returns an AnalysisView object that is held by a point on the curve. Null will be returned if an invalid index is given
        /// </summary>
        /// <param name="vIndex">the index of a point on the curve referenced by the current controller</param>
        /// <returns> returns an AnalysisView object held by a point on the curve</returns>
        public AnalysisView GetAnalysisView(int vIndex)
        {
            //invalid index
            if (vIndex >= Curve.length || vIndex < 0)
            {
                return null;
            }

            return Curve[CurrCamIndex].gameObject.GetComponent<AnalysisView>();
        }

        /// <summary>
        /// Returns an CameraMovementPointSetting object that is held by a point on the curve. Null will be returned if an invalid index is given
        /// </summary>
        /// <param name="vIndex">the index of a point on the curve referenced by the current controller</param>
        /// <returns> returns an CameraMovementPointSetting object held by a point on the curve</returns>
        public CameraMovementPointSetting GetPointAt(int vIndex)
        {
            if (vIndex >= Curve.length || vIndex < 0)
            {
                return null;
            }

            return Curve[CurrCamIndex].gameObject.GetComponent<CameraMovementPointSetting>();
        }

        /// <summary>
        /// Reset the controller
        /// </summary>
        public void Reset()
        {
            StopAllCoroutines();
            mFinishedMovingCam = true;
            CamLookAt.Target = Curve[0].gameObject.GetComponent<CameraMovementPointSetting>().LookAtTarget;
            CamLookAt.TargetPos = Curve[0].gameObject.GetComponent<CameraMovementPointSetting>().LookAtTarget.position;
            for (int i = 0; i < Curve.length; i++)
            {
                AnalysisView vAnalysisView = GetAnalysisView(i);
                if (vAnalysisView != null)
                {
                    vAnalysisView.Hide();
                }
            }
            NextCamIndex = -1;
            CurrCamIndex = 0;
        }
    }
}
