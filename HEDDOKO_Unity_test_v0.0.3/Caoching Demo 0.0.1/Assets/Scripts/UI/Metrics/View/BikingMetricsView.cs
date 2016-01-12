﻿
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
using Assets.Scripts.UI.Metrics.View;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Animations;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Metrics
{
    /// <summary>
    /// Shows the metrics for Biking
    /// </summary>
    public class BikingMetricsView : NonSquatWrapperView, IResettableMetricView
    {
        private bool mDisplayRightLegAnalysis;

        public int NumberOfFrameToCount = 5;
        [SerializeField]
        private int mCurrentCountOfFrames = 0;

        private float mRightKneeAvg;
        private float mLeftKneeVelAvg;
        [SerializeField]
        private bool mIsActive;

        public float mInitialFlexion = 70;
        [SerializeField]
        private float mPreviousMeanVel = 0;
        [SerializeField]
        private float mMostUpToDateMeanVel = 0;
        private List<float> mOldFrames;
        public float LerpSpeed = 50;
        private bool mContinueAnimation;
        public float MaxKneeFlexVel = 999;

        public Text VelocityText;
        private float mInitialTime;

        [SerializeField]
        private float mTimeAccumulator;
        [SerializeField]
        private int mRevolution;

        private AverageKneeFlexVelocity KneeFlexVelocityAvg;
        private PlayerStreamManager mPlayerStreamManager;
        public Image VelocityPowerbar;
        public FillAnimationComponent FillAnimationComponent;

        private LegMetricsView mLegMetricsView;
        public LegMetricsView LegMetricsView
        {
            get
            {
                if (mLegMetricsView == null)
                {
                    mLegMetricsView = FindObjectOfType<LegMetricsView>();
                }
                return mLegMetricsView;
            }
        }

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

        void Awake()
        {
            PlayerStreamManager.ResettableViews.Add(this);
        }

        /// <summary>
        /// Brings the view into screen
        /// </summary>
        public override void Show()
        {
            gameObject.SetActive(true);
            mIsActive = true;
            LegMetricsView.gameObject.SetActive(true);
            ResetValues();
        }

        /// <summary>
        /// Takes the view out of screen
        /// </summary>
        public override  void Hide()
        {
            gameObject.SetActive(false);
            mIsActive = false;
            LegMetricsView.gameObject.SetActive(false);
            ResetValues();
        }


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
                            FillAnimationComponent.StartAnimation(mRightKneeAvg);
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
                            FillAnimationComponent.StartAnimation(mLeftKneeVelAvg);
                            mRightKneeAvg = 0;
                        }
                    }
                    mCurrentCountOfFrames++;
                }
            }
        }
         

        /// <summary>
        /// ResetValues the biking metrics variables
        /// </summary>
        public void ResetValues()
        {
            mRevolution = 0;
            mOldFrames = null; 
            mPreviousMeanVel = 0;
            mMostUpToDateMeanVel = 0;

        }
 

    }
}

