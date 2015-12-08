
/** 
* @file BikingMetricsView.cs
* @brief Contains the BikingMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Biking
    /// </summary>
    public class BikingMetricsView : MonoBehaviour
    {
        public int NumberOfFrameToCount = 5;
        [SerializeField]
        private int mCurrentCountOfFrames = 0;
        [SerializeField]
        private bool mIsActive;
        [SerializeField]
        private float mAngleSumRight;
        [SerializeField]
        private float mPreviousMeanVel = 0;
        [SerializeField]
        private float mMostUpToDateMeanVel = 0;
        private List<float> mMostRecentXFrames;//= new float[NumberOfFrameToCount];
        private List<float> mOldFrames;
        [SerializeField]
        private float mAngleKneeFlexionPrev;
        [SerializeField]
        private float mInitialFlexion;

        public Text RPMeter;
        public Text InformationPanel;
        public float RevsPerMinute;
        private float mInitialTime;
        [SerializeField]
        private float mTimeAccumulator;
        [SerializeField]
        private int mRevolution;

        private RightLegAnalysis mRightLegAnalysis;

        private PlayerStreamManager mPlayerStreamManager;

        public PlayerStreamManager PlayerStreamManager
        {
            get
            {
                if (mPlayerStreamManager == null)
                {
                    mPlayerStreamManager = FindObjectOfType<PlayerStreamManager>();
                }
                return mPlayerStreamManager;
            }
        }

        /// <summary>
        /// Brings the view into screen
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            mIsActive = true;
      
            ResetValues();
        }

        /// <summary>
        /// Takes the view out of screen
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
            mIsActive = false;
           ResetValues();
        }

        private void LateUpdate()
        {
            if (mRightLegAnalysis != null)
            {
                mAngleKneeFlexionPrev = mRightLegAnalysis.AngleKneeFlexion;

            }
        }
        private void Update()
        {
            if (mIsActive)
            {
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    if (mRightLegAnalysis == null)
                    {
                        mRightLegAnalysis =
                       vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                           RightLegAnalysis;
                        mInitialFlexion = mRightLegAnalysis.AngleKneeFlexion;
                        mInitialTime = 0f;
                        mTimeAccumulator = 0f;
                        mMostRecentXFrames = new List<float>(NumberOfFrameToCount);
                        mOldFrames = new List<float>(NumberOfFrameToCount);
                    }

                    if (mAngleKneeFlexionPrev <= 0)
                    {
                        return;
                    }
                    Vector3 vAxis1 = new Vector3(mRightLegAnalysis.HipOrientation[0, 1], mRightLegAnalysis.HipOrientation[1, 1], mRightLegAnalysis.HipOrientation[2, 1]);
                    Vector3 vAxis2 = new Vector3(mRightLegAnalysis.KneeOrientation[0, 1], mRightLegAnalysis.KneeOrientation[1, 1], mRightLegAnalysis.KneeOrientation[2, 1]);
                    float vAngleKneeFlexionNew = Vector3.Angle(vAxis1, vAxis2);
                    if (mAngleKneeFlexionPrev <= mInitialFlexion && mInitialFlexion <= vAngleKneeFlexionNew)
                    {
                        mRevolution++;
                    }
                    if (mTimeAccumulator > 0f)
                    {
                        RevsPerMinute = (mRevolution / mTimeAccumulator) * 60f;
                        if (mTimeAccumulator > 60)
                        {
                            mTimeAccumulator = 0;
                        }
                        double vTruncatedVal = Math.Truncate(RevsPerMinute * 100) / 100;
                        string vOutput = string.Format("{0:N2}", vTruncatedVal);
                        RPMeter.text = vOutput + "rpm";
                    }
                    mTimeAccumulator += Time.deltaTime;
                    UpdateKneeFlexionAverages();
                }
            }
        }

        public void ResetValues()
        {
            mRevolution = 0;
            mAngleKneeFlexionPrev = 0;
            mRightLegAnalysis = null;
            mMostRecentXFrames = null;
            mOldFrames = null;
            RevsPerMinute = 0;
            mPreviousMeanVel = 0;
            mMostUpToDateMeanVel = 0;
        }

        private void UpdateKneeFlexionAverages()
        {
            if (mOldFrames.Count < NumberOfFrameToCount)
            {
                mOldFrames.Add(mRightLegAnalysis.mAngularVelocityKneeFlexion);
                return;
            }
            if (mMostRecentXFrames.Count < NumberOfFrameToCount)
            {
                mMostRecentXFrames.Add(mRightLegAnalysis.mAngularVelocityKneeFlexion);
                return;
            }

            float vMostUpToDateVelSum=0f;
            float vOldVelSum = 0f;

            foreach (float val in mMostRecentXFrames)
            {
                vMostUpToDateVelSum += Mathf.Abs(val);
            }
           
            mMostUpToDateMeanVel = vMostUpToDateVelSum / NumberOfFrameToCount;

            foreach (float val in mOldFrames)
            {
                vOldVelSum += Mathf.Abs(val);
            }
            mPreviousMeanVel = vOldVelSum / NumberOfFrameToCount;

            if (mMostUpToDateMeanVel < mPreviousMeanVel)
            {
                InformationPanel.text = "You're going faster";
            }

            if (mMostUpToDateMeanVel > mPreviousMeanVel)
            {
                InformationPanel.text = "You're going slower";
            }

            //pop the first element from mOldFrames and mMostRecentXFrames
            float vToGoOldFrame = mMostRecentXFrames[0];
            mMostRecentXFrames.RemoveAt(0);
            mOldFrames.RemoveAt(0);

            //push the most updated value to mMostRecentXFrames

            mMostRecentXFrames.Add(mRightLegAnalysis.mAngularVelocityKneeFlexion);

            //push the popped value from mMostRecentXFrames
            mOldFrames.Add(vToGoOldFrame);

        }
    }
}
