
/** 
* @file BikingMetricsView.cs
* @brief Contains the BikingMetricsView class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date December 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Utils;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Biking
    /// </summary>
    public class BikingMetricsView : MonoBehaviour
    {
        private bool mDisplayRightLegAnalysis;

        public int NumberOfFrameToCount = 5;
        [SerializeField] private int mCurrentCountOfFrames = 0;

        private float mRightKneeAvg;
        private float mLeftKneeVelAvg;
        [SerializeField] private bool mIsActive;

        public float mInitialFlexion = 70;
        [SerializeField] private float mPreviousMeanVel = 0;
        [SerializeField] private float mMostUpToDateMeanVel = 0;
        //  private List<float> mMostRecentXFrames;//= new float[NumberOfFrameToCount];
        private List<float> mOldFrames;
      //  [SerializeField] private float mAngleKneeFlexionPrev;



        public float LerpSpeed = 50;
        private bool mContinueAnimation;
        public float MaxKneeFlexVel = 999;

        public Text VelocityText;
        // public Text InformationPanel;
        //public float RevsPerMinute;
        private float mInitialTime;
        [SerializeField] private float mTimeAccumulator;
        [SerializeField] private int mRevolution;

     //   private RightLegAnalysis mRightLegAnalysis;
        private AverageKneeFlexVelocity KneeFlexVelocityAvg;
        private PlayerStreamManager mPlayerStreamManager;

        public Image VelocityPowerbar;

        public bool DisplayRightLegAnalysis
        {
            get { return mDisplayRightLegAnalysis; }
            set
            {
                if (value != mDisplayRightLegAnalysis)
                {
                    ResetValues();
                }

                mDisplayRightLegAnalysis = value;
            }
        }

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

  /*      private void LateUpdate()
        {
            if (mRightLegAnalysis != null)
            {
                mAngleKneeFlexionPrev = mRightLegAnalysis.AngleKneeFlexion;

            }
        }*/

        private void Update()
        {
            if (mIsActive)
            {
                Body vCurrentBody = PlayerStreamManager.CurrentBodyInPlay;
                if (vCurrentBody != null)
                {
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_RightLeg))
                    {
                        RightLegAnalysis vRightLegAnalysis =
                            vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                                RightLegAnalysis;
                        if (vRightLegAnalysis != null)
                        {
                            if (mCurrentCountOfFrames < NumberOfFrameToCount)
                            {
                                mRightKneeAvg += Mathf.Abs(vRightLegAnalysis.mAngularVelocityKneeFlexion);
                            }

                        }
                    }
                    if (vCurrentBody.AnalysisSegments.ContainsKey(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg))
                    {
                        LeftLegAnalysis vLeftLegAnalysis =
                            vCurrentBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_LeftLeg] as
                                LeftLegAnalysis;
                        if (vLeftLegAnalysis != null)
                        {
                            if (mCurrentCountOfFrames < NumberOfFrameToCount)
                            {
                                mLeftKneeVelAvg += Mathf.Abs(vLeftLegAnalysis.mAngularVelocityKneeFlexion);
                            }

                        }
                    }
                    if (DisplayRightLegAnalysis)
                    {
                        if (mCurrentCountOfFrames == NumberOfFrameToCount)
                        {
                            mRightKneeAvg /= NumberOfFrameToCount;
                            mCurrentCountOfFrames = 0;
                            StopAllCoroutines();
                            mContinueAnimation = false;
                            StartCoroutine(StartFillAnimation(mRightKneeAvg));
                            mRightKneeAvg = 0;
                        }
                    }
                    else
                    {
                        if (mCurrentCountOfFrames == NumberOfFrameToCount)
                        {
                            mLeftKneeVelAvg /= NumberOfFrameToCount;
                            mCurrentCountOfFrames = 0;
                            StopAllCoroutines();
                            mContinueAnimation = false;
                            StartCoroutine(StartFillAnimation(mLeftKneeVelAvg));
                            mRightKneeAvg = 0;
                        }
                    }
                    mCurrentCountOfFrames++;




                    // float vAvg = KneeFlexVelocityAvg.CompareAverageKneeFlexionSpeed();
                    /*  if (vAvg >= 0)
                         {*/


                    //   }
                    /* Vector3 vAxis1 = new Vector3(mRightLegAnalysis.HipOrientation[0, 1], mRightLegAnalysis.HipOrientation[1, 1], mRightLegAnalysis.HipOrientation[2, 1]);
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
                            VelocityText.text = vOutput + "rpm";
                        }
                        mTimeAccumulator += Time.deltaTime;*/
                    // UpdateKneeFlexionAverages();
                }
            }
        }
    

    /// <summary>
        /// Initialize the biking metric variables
        /// </summary>
        /// <param name="vBody"></param>
        private void Initialize(Body vBody)
        {
          /*  mRightLegAnalysis =
                      vBody.AnalysisSegments[BodyStructureMap.SegmentTypes.SegmentType_RightLeg] as
                          RightLegAnalysis;
            mInitialFlexion = 70f;// mRightLegAnalysis.AngleKneeFlexion;
            mInitialTime = 0f;
            mTimeAccumulator = 0f;
            mMostRecentXFrames = new List<float>(NumberOfFrameToCount);
            mOldFrames = new List<float>(NumberOfFrameToCount);
            KneeFlexVelocityAvg = new AverageKneeFlexVelocity(vBody);*/
        }
/*
        private void UpdatePowerBar()
        {
            float vAbsVelocityVal = Mathf.Abs(mRightLegAnalysis.mAngularVelocityKneeFlexion);
            float vPercent = HeddokoMathTools.Map(mRightLegAnalysis.mAngularVelocityKneeFlexion, 0f, MaxKneeFlexVel, 0.108f, 1f);
            Debug.Log(vPercent);
            VelocityPowerbar.fillAmount = vPercent;
        }*/
        /// <summary>
        /// ResetValues the biking metrics variables
        /// </summary>
        public void ResetValues()
        {
            mRevolution = 0;
          //  mAngleKneeFlexionPrev = 0;
         //   mRightLegAnalysis = null;
           // mMostRecentXFrames = null;
            mOldFrames = null;
            //RevsPerMinute = 0;
            mPreviousMeanVel = 0;
            mMostUpToDateMeanVel = 0;

        }

/*        private void UpdateKneeFlexionAverages()
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

            float vMostUpToDateVelSum = 0f;
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

            /*  if (mMostUpToDateMeanVel < mPreviousMeanVel)
              {
                  InformationPanel.text = "You're going faster";
              }

              if (mMostUpToDateMeanVel > mPreviousMeanVel)
              {
                  InformationPanel.text = "You're going slower";
              }
  #1#

            //pop the first element from mOldFrames and mMostRecentXFrames
            float vToGoOldFrame = mMostRecentXFrames[0];
            mMostRecentXFrames.RemoveAt(0);
            mOldFrames.RemoveAt(0);

            //push the most updated value to mMostRecentXFrames

            mMostRecentXFrames.Add(mRightLegAnalysis.mAngularVelocityKneeFlexion);

            //push the popped value from mMostRecentXFrames
            mOldFrames.Add(vToGoOldFrame);

        }*/

        private IEnumerator StartFillAnimation(float vNewVal)
        {
            mContinueAnimation = true;
            float vInitialVal = HeddokoMathTools.Map(VelocityPowerbar.fillAmount, 0.108f, 1f, 0f, MaxKneeFlexVel);
            float vDistanceInit = Mathf.Abs(vNewVal - vInitialVal);
            float vTimeStarted = Time.time;

            while (true)
            {
                if (!mContinueAnimation)
                {
                    break;
                }
                float vTimeTaken = Time.time - vTimeStarted;
                float vPercentage = LerpSpeed * vTimeTaken / vTimeStarted;// LerpSpeed * (HeddokoMathTools.Map(VelocityPowerbar.fillAmount, 0.108f, 1f, 0f, MaxKneeFlexVel) / vDistanceInit);
                float vPreMappedFillVal = Mathf.Lerp(vInitialVal, vNewVal, vPercentage);

                if (vPercentage > 1)
                {
                    VelocityPowerbar.fillAmount = HeddokoMathTools.Map(vNewVal, 0f, MaxKneeFlexVel, 0.108f, 1f);
                    VelocityText.text = (int)vNewVal + "";
                    break;
                }
                float vMappedFilledVal = HeddokoMathTools.Map(vPreMappedFillVal, 0f, MaxKneeFlexVel, 0.108f, 1f);
                VelocityPowerbar.fillAmount = vMappedFilledVal;
                VelocityText.text = (int)vNewVal + "";

                yield return null;
            }
        }

    }
}

