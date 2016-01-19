
/** 
* @file BodySegmentSettingsKeyMap.cs
* @brief BodySegmentSettingsKeyMap class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utils.DebugContext;
using UnityEngine;

namespace Assets.Scripts.UI.NFLDemo
{
    /// <summary>
    /// Sets the TradingHeight, IsHipFollowingTorsoRotation, IsUsingInterpolation flags, as well as increments and decrements
    /// interpolation speed
    /// </summary>
   public class BodySegmentSettingsKeyMap : MonoBehaviour
    {
        public PlayerStreamManager PlayerStreamManager;
        private Body mBody;

        void Awake()
        {
            PlayerStreamManager.BodyChangedEvent += SetBody;
        }

        void OnDisable()
        {
            PlayerStreamManager.BodyChangedEvent -= SetBody;
        }

        /// <summary>
        /// Setts the new body
        /// </summary>
        /// <param name="vNewBody"></param>
        private void SetBody(Body vNewBody)
        {
            mBody = vNewBody;
        }

        void Update()
        {
            if (mBody != null)
            {
                InputHandler();
            }
        }
        

        /// <summary>
        /// Handles input key events
        /// </summary>
        void InputHandler()
        {
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.DecBodyInterpoationSp))
            {
                ChangeInterpolationValue(-0.05f);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IncBodyInterpolationSp))
            {
                ChangeInterpolationValue(0.05f);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipFollowingTorsoRotation))
            {
                for (int i = 0; i < mBody.BodySegments.Count; i++)
                {
                    mBody.BodySegments[i].IsHipsFollowingTorsoRotation = !mBody.BodySegments[i].IsHipsFollowingTorsoRotation;
                }
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsTrackingHeight))
            {
                for (int i = 0; i < mBody.BodySegments.Count; i++)
                {
                    mBody.BodySegments[i].IsTrackingHeight = !mBody.BodySegments[i].IsTrackingHeight;
                }
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolationForBody))
            {
                for (int i = 0; i < mBody.BodySegments.Count; i++)
                {
                    mBody.BodySegments[i].IsUsingInterpolation = !mBody.BodySegments[i].IsUsingInterpolation;
                }
            }
        }

        /// <summary>
        /// increments/decrements interpolation value of the current body in play. The interpolation is clamped between 0-1
        /// </summary>
        /// <param name="vIncValue"></param>
        private void ChangeInterpolationValue(float vIncValue)
        {
            for (int i = 0; i < mBody.BodySegments.Count; i++)
            {
                mBody.BodySegments[i].InterpolationSpeed += vIncValue;
                Mathf.Clamp01(mBody.BodySegments[i].InterpolationSpeed);
            }
        }


    }
}
