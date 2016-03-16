
/** 
* @file CameraVerticalPosition.cs
* @brief Contains the CameraVerticalPosition interface
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved 
*/

using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI.AbstractViews.camera
{
    /// <summary>
    /// Maintains the same position as a target then disables itself after a X seconds
    /// </summary>
    public class CameraVerticalPosition : MonoBehaviour
    {
        /// <summary>
        /// duration in seconds
        /// </summary>
        public float Duration = 1f;

        public Transform Target;

        void OnEnable()
        {
            if (!isActiveAndEnabled)
            {
                this.enabled = true;
                StartCoroutine(RepositionAndDisableScript());
            }
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }
        IEnumerator RepositionAndDisableScript()
        {
            float vStartTime = Time.time;
            float vRemainingTime = Duration;
            while (vRemainingTime > 0)
            {
                if (Target == null)
                {
                    continue;
                }
                transform.position = Target.position;
                float vDeltaTime = Time.time - vStartTime;
                vRemainingTime -= vDeltaTime;
                yield return null;
            }
            this.enabled = false;
        }

}
}