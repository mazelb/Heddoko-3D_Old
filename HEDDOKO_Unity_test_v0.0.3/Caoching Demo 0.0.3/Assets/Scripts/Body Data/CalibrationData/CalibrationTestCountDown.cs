
/** 
* @file GlobalCalibrationSettings.cs
* @brief Contains the GlobalCalibrationSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
 
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Communication.Controller;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.AbstractSuitSubControls;
using Assets.Scripts.UI.AbstractViews.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Body_Data.CalibrationData
{

    public class CalibrationTestCountDown : AbstractSuitsSubControl
    {

        private SubControlType mSubControl = SubControlType.AddCommentSubControl;
        private SuitState mPreviousSuitState = SuitState.Undefined;
        private Dictionary<CalibrationType, CalibrationTestDescriptor> mCalibrationImageMap = new Dictionary<CalibrationType, CalibrationTestDescriptor>(3);
        public Sprite TPoseSprite;
        public Sprite ArmsFrontSprite;
        public Sprite ArmsDownSprite;
        public Sprite TposToArmsFrontSprite;
        public Sprite ArmsFrontToDownSprite;
        public Image CalibrationImage;
        public Text Info;
        public Text CountDowntext;
        public Animator DebugTextAnimator;

        void EnableCountdownTextAnim(bool vFlag)
        {
            DebugTextAnimator.enabled = vFlag;
            CountDowntext.gameObject.SetActive(vFlag);
        }
        private void Awake()
        {
            EnableCountdownTextAnim(false);
            EnableInfo(false);

            CalibrationTestDescriptor vTposeDescritor = new CalibrationTestDescriptor
            {
                Info = "T-Pose",
                Sprite = TPoseSprite
            };
            CalibrationTestDescriptor vArmsFrontDescriptor = new CalibrationTestDescriptor
            {
                Info = "Arms out front",
                Sprite = ArmsFrontSprite
            };
            CalibrationTestDescriptor vArmsDownSprite = new CalibrationTestDescriptor
            {
                Info = "Arms down",
                Sprite = ArmsDownSprite
            };
            CalibrationTestDescriptor vTposeToArmFrontTestDescriptor = new CalibrationTestDescriptor()
            {
                Info = "Next calibration: Arms front",
                Sprite = TposToArmsFrontSprite,
                UsesAnimation = true
            };
            CalibrationTestDescriptor vArmsFrontToArmsDownDescriptor = new CalibrationTestDescriptor()
            {
                Info = "Next calibration: Arms down",
                Sprite = ArmsFrontToDownSprite,
                UsesAnimation = true
            };

            mCalibrationImageMap.Add(CalibrationType.Tpose, vTposeDescritor);
            mCalibrationImageMap.Add(CalibrationType.TposeToZombieTransition, vTposeToArmFrontTestDescriptor);
            mCalibrationImageMap.Add(CalibrationType.ArmsForward, vArmsFrontDescriptor);
            mCalibrationImageMap.Add(CalibrationType.ZombieToSoldierTransition, vArmsFrontToArmsDownDescriptor);
            mCalibrationImageMap.Add(CalibrationType.SoldierPose, vArmsDownSprite);
        }
        /// <summary>
        /// Starts the countdown routine
        /// </summary>
        private void StartCountDownRoutine()
        {
            EnableInfo(true);
            StartCoroutine(CountdownRoutine());
        }

        void EnableInfo(bool vFlag)
        {
            CalibrationImage.gameObject.SetActive(vFlag);
            Info.gameObject.SetActive(vFlag);

        }

        IEnumerator CountdownRoutine()
        {
            float vWaitTime = GlobalCalibrationSettings.CalibrationTimer;
            foreach (KeyValuePair<CalibrationType, CalibrationTestDescriptor> vKvPair in mCalibrationImageMap)
            {

                CalibrationImage.sprite = vKvPair.Value.Sprite;
                Info.text = vKvPair.Value.Info;
                if (!vKvPair.Value.UsesAnimation)
                {
                    EnableCountdownTextAnim(false);
                    yield return new WaitForSeconds(vWaitTime);
                }
                else
                {
                    EnableCountdownTextAnim(true);
                    yield return StartCoroutine(CountDownNum((int)GlobalCalibrationSettings.TransitionTimer));
                }
            }
            StopCountdownRoutine();

        }

        private void StopCountdownRoutine()
        {
            EnableCountdownTextAnim(false);
            StopAllCoroutines();
            EnableInfo(false);
        }


        public override SubControlType SubControlType
        {
            get { return mSubControl; }
        }

        public override void Disable()
        {
            StopCountdownRoutine();
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            SuitStateChangerControl.StopRecordingEvent += StopCountdownRoutine;
            SuitStateChangerControl.StartRecordingEvent += StartCountDownRoutine;
        }

        void OnDisable()
        {
            SuitStateChangerControl.StopRecordingEvent -= StopCountdownRoutine;
            SuitStateChangerControl.StartRecordingEvent -= StartCountDownRoutine;
        }
        public override void Enable()
        {
            gameObject.SetActive(true);
        }

        public override void OnStatusUpdate(SuitState vSuitState)
        {

        }

        public override void OnConnection()
        {
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCountDownRoutine();
            }
        }
        private IEnumerator CountDownNum(int vCountDownFrom)
        {
            int vCountdown = vCountDownFrom;
            while (vCountdown >= 0)
            {
                CountDowntext.text = "" + vCountdown;
                DebugTextAnimator.Play("CountdownAnimator", -1, 0f);
                yield return new WaitForSeconds(1);
                vCountdown--;
            }

        }

    }

    public class CalibrationTestDescriptor
    {
        public Sprite Sprite;
        public string Info;
        public bool UsesAnimation;

    }

}
