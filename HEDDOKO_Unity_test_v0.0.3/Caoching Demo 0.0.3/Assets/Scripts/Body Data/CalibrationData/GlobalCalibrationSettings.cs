
/** 
* @file GlobalCalibrationSettings.cs
* @brief Contains the GlobalCalibrationSettings class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/


using System.Collections.Generic;

namespace Assets.Scripts.Body_Data.CalibrationData
{
    /// <summary>
    /// Global Calibration settings 
    /// </summary>
    public static class GlobalCalibrationSettings
    {
        private static bool sIsInitialized;
        /// <summary>
        /// Transition period between the number of calibrations
        /// </summary>
        private static float sTransitionPeriod = 3f;

        /// <summary>
        /// Calibration timing
        /// </summary>
        private static float sCalibrationTimer = 3f;

        


        private static List<CalibrationType> sCalibrationTypes = new List<CalibrationType>();
        private static Dictionary<CalibrationType, float> sCalibrationTimes = new Dictionary<CalibrationType, float>();

        /// <summary>
        /// Transition period between the number of calibrations
        /// </summary>
        public static float TransitionPeriod
        {
            get
            {
                CheckIfInitialized();
                return sTransitionPeriod;
            }
        }

        /// <summary>
        /// Calibration timing
        /// </summary>
        public static float CalibrationTimer
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTimer;
            }
        }

        public static List<CalibrationType> CalibrationTypes
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTypes;
            }
        }
        public static CalibrationType FinalPose { get; private set; }
        public static Dictionary<CalibrationType, float> CalibrationTimes
        {
            get
            {
                CheckIfInitialized();
                return sCalibrationTimes;
            }
        }

        /// <summary>
        /// Checks if the Gloabal Calibration settings have been initialized
        /// </summary>
        private static void CheckIfInitialized()
        {
            if (!sIsInitialized)
            {
                sIsInitialized = true;
                Init();
            }
        }
        /// <summary>
        /// initializes global settings
        /// </summary>
        private static void Init()
        {
            sCalibrationTypes = new List<CalibrationType>();
            //set calibration times according to their current order
            float vMultipler = 1;

            sCalibrationTimes.Add(CalibrationType.Tpose, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.ArmsForward, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.SoldierPose, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.TposeToZombieTransition, CalibrationTimer * vMultipler++);
            sCalibrationTimes.Add(CalibrationType.ZombieToSoldierTransition, CalibrationTimer * vMultipler++);
            FinalPose = CalibrationType.ZombieToSoldierTransition;
        }
    }



    /// <summary>
    /// Calibration types
    /// </summary>
    public enum CalibrationType
    {
        Tpose=0,
        TposeToZombieTransition=1,
        ArmsForward = 2,
        ZombieToSoldierTransition=3,
        SoldierPose=4,
        Count=5
    }
}
