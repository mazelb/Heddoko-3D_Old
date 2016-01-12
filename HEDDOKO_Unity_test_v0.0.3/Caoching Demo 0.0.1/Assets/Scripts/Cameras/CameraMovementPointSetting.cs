﻿
/**  
* @file CameraController.cs 
* @brief Contains the CamRotate class 
* @author Mohammed Haider(mohamed@heddoko.com) 
* @date January 2016
* Copyright Heddoko(TM) 2015, all rights reserved 
*/


using System;
using UnityEngine;

namespace Assets.Scripts.Cameras
{
    /// <summary>
    /// Class holds a setting which a camera controller will reference as a camera approaches a LookAtTarget
    /// </summary>
    [Serializable]
   public class CameraMovementPointSetting: MonoBehaviour
    {
        //Offset from the transform
        public Vector3 Offset;
        public float OrthographicSize = 1f;
        public Transform LookAtTarget;
        public bool FollowLookAtTarget;

        void LateUpdate()
        {
            if (FollowLookAtTarget)
            {
                transform.position = LookAtTarget.position + Offset;
            }
        }

    }
}