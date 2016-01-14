
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// this camera controller is to be used solely for the NFL tech crunch demo
    /// </summary>
    public class NFLCameraController : MonoBehaviour
    {
        public Camera Camera;
        public BezierCurve Curve;
        public CameraLookAt CamLookAt;
        private int mIndex;
        public float MovementSpeed = 2f;
        public delegate void FinishedRepositioningCam();

        public event FinishedRepositioningCam CamFinishPosEvent;



        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                StartCoroutine(MoveNext());
            }
        }
        IEnumerator MoveNext()
        {
            float vStartTime = 0;
            float vCurrOrthoCam = Camera.orthographicSize;
            int vIndexLength = Curve.pointCount;
            Vector3 vCurrentLookAtPos = CamLookAt.Target.position;
            while (true)
            {

                if (mIndex >= Curve.pointCount)
                {
                    mIndex = 0;
                }

                int vNextPoint = mIndex + 1;

                if (mIndex == Curve.pointCount - 1)
                {
                    vNextPoint = 0;
                }

                CameraMovementPointSetting vCurrentPointSetting =
                  Curve[mIndex].gameObject.GetComponent<CameraMovementPointSetting>();
                CameraMovementPointSetting vNextPointSetting =
                  Curve[vNextPoint].gameObject.GetComponent<CameraMovementPointSetting>();

                vStartTime += Time.deltaTime;
                float vPercentage = vStartTime / MovementSpeed;
                vPercentage = vPercentage*vPercentage*vPercentage*(vPercentage*(6f* vPercentage - 15f) + 10f);
                Vector3 vNewPosition = BezierCurve.GetPoint(Curve[mIndex], Curve[vNextPoint], vPercentage);
                float vNextOrthoSize = Mathf.Lerp(vCurrOrthoCam, vNextPointSetting.OrthographicSize, vPercentage);
                Vector3 vNextLookAtPos = Vector3.Lerp(vCurrentLookAtPos, vNextPointSetting.LookAtTarget.position,
                    vPercentage);

                Camera.transform.position = vNewPosition;
                Camera.orthographicSize = vNextOrthoSize; 
                CamLookAt.TargetPos = vNextLookAtPos; 
                if (vPercentage >= 1)
                {

                    mIndex++;
                    if (CamFinishPosEvent != null)
                    {
                        CamFinishPosEvent();
                    }
                    break;

                }

                yield return null;
            }
        }
    }
}
