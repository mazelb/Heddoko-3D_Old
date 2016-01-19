
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
    /// Data model which holds settings for different view points, from which a camera controller will reference as a camera approaches a LookAtTarget
    /// </summary>
    [Serializable]
   public class CameraMovementPointSetting: MonoBehaviour
    {
        //Offset from the transform
        public Vector3 Offset;
        public float OrthographicSize = 1f;
        public Transform LookAtTarget;
        public bool FollowLookAtTarget;
         
        //the Arcsprite's angle at this position.
        public Sprite ArcSprite;

        public Vector3 PlaneNormal;
        //How far is the object away from the 
        public Vector3 TransformOffset;
        
        void LateUpdate()
        {
            if (FollowLookAtTarget)
            {
                transform.position = LookAtTarget.position + Offset;
            }
        }
 
    }
}
