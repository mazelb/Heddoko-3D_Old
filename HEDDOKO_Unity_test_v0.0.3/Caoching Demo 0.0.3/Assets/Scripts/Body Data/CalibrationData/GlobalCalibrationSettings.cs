
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
        private static float sTransitionPeriod = 0.5f;

        /// <summary>
        /// Calibration timing
        /// </summary>
        private static float sCalibrationTimer = 2f;

        

        private static List<CalibrationType> sCalibrationTypes= new List<CalibrationType>();


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
            sCalibrationTypes.Add(CalibrationType.Tpose);
            sCalibrationTypes.Add(CalibrationType.ArmsForward);
            sCalibrationTypes.Add(CalibrationType.ArmsDown);

        }
    }

    /// <summary>
    /// Calibration types
    /// </summary>
    public enum CalibrationType
    {
        Tpose,
        ArmsForward,
        ArmsDown,
        SlightSquat
    }
}
