
/** 
* @file BodyPlayerButtons.cs
* @brief Contains the BodyPlayerButtons class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date January 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using Assets.Scripts.Utils.DebugContext;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Demos
{
    /// <summary>
    /// 
    /// </summary>
    public class BodyPlayerButtons : MonoBehaviour
    {
        public BodyPlayer BodyPlayer;
        public Button TrackingHeight;
        public Text TrackHeightTxt;

        public Button ArmAdjustment;
        public Text ArmAdjustmentTxt;

        public Button HipsEstForward;
        public Text HipsEstForwardTxt;

        public Button HipsEstUp;
        public Text HipsEstUpTxt;

        public Button Interpolation;
        public Text InterpolationTxt;

        public Text HideText;

        private bool mBodySet;
        public Animator Animator;
        [SerializeField] private bool mIsHidden;

        void Update()
        {
            if (BodyPlayer.CurrentBodyInPlay != null)
            {

                if (!mBodySet)
                {
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
                    ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.IsAdjustingArms);
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);
                }
                InputHandler();
            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.HideSegmentFlagPanel))
            {
                mIsHidden = !mIsHidden;
                Animator.SetBool("mIsHidden", mIsHidden);
            }
        }

        void Awake()
        {
            HideText.text = "Press " + HeddokoDebugKeyMappings.HideSegmentFlagPanel + "  to hide/show ";
            TrackingHeight.onClick.AddListener(() =>
            {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsTrackingHeight = !BodySegment.IsTrackingHeight;
                    TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
                }
            }
                );
            ArmAdjustment.onClick.AddListener( ()=> { 
                 if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsAdjustingArms = !BodySegment.IsAdjustingArms;
                    ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.IsAdjustingArms);
                }
            }
                );
            HipsEstForward.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsHipsEstimateForward = !BodySegment.IsHipsEstimateForward;
                    HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
                }
            }
               );

            HipsEstUp.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsHipsEstimateUp = !BodySegment.IsHipsEstimateUp;
                    HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
                }
            }
               );
            Interpolation.onClick.AddListener(() => {
                if (BodyPlayer.CurrentBodyInPlay != null)
                {
                    BodySegment.IsUsingInterpolation = !BodySegment.IsUsingInterpolation;
                    InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);
                }
            }
              );

        }
        /// <summary>
        /// Handles input from keyboard
        /// </summary>
        void InputHandler()
        {
          
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsTrackingHeight))
            {
                BodySegment.IsTrackingHeight = !BodySegment.IsTrackingHeight;
                TrackHeightTxt.text = ReturnOnOffFromBool(BodySegment.IsTrackingHeight);
            }

            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsAdjustingArms))
            {
                BodySegment.IsAdjustingArms = !BodySegment.IsAdjustingArms;
                ArmAdjustmentTxt.text = ReturnOnOffFromBool(BodySegment.IsAdjustingArms);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateForward))
            {
                BodySegment.IsHipsEstimateForward = !BodySegment.IsHipsEstimateForward;
                HipsEstForwardTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateForward);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsHipsEstimateUp))
            {
                BodySegment.IsHipsEstimateUp = !BodySegment.IsHipsEstimateUp;
                HipsEstUpTxt.text = ReturnOnOffFromBool(BodySegment.IsHipsEstimateUp);
            }
            if (Input.GetKeyDown(HeddokoDebugKeyMappings.IsUsingInterpolationForBody))
            {
                BodySegment.IsUsingInterpolation = !BodySegment.IsUsingInterpolation;
                InterpolationTxt.text = ReturnOnOffFromBool(BodySegment.IsUsingInterpolation);
            }
        }

        /// <summary>
        ///from the bool passed in, returns either On or Off
        /// </summary>
        /// <returns></returns>
        private string ReturnOnOffFromBool(bool vValue)
        {
            return vValue ? "ON" : "OFF";
        }


    }
}
