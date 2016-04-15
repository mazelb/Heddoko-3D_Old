﻿
/** 
* @file DemoKitDoublePlayerPlayback .cs
* @brief Contains the DemoKitDoublePlayerPlayback   
* @author Mohammed Haider (mohammed@heddoko.com)
* @date April 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using Assets.Scripts.UI.AbstractViews;
using Assets.Scripts.UI.AbstractViews.Enums;
using Assets.Scripts.UI.AbstractViews.Layouts;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.View.Anaylsis;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.AbstractSubControls.cameraSubControls;
using Assets.Scripts.UI.AbstractViews.camera;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.DemoKit
{
    /// <summary>
    /// Demokit player playback
    /// </summary>
    public class DemoKitDoublePlayerPlayback : AbstractView
    {
        public BezierCurve PathToTop;
        public BezierCurve PathToSide;
        
        private PanelCamera mLeftCamera;
        private PanelCamera mRightCamera;
        public Transform LookAtTarget;
        public Body DefaultBody;
        public BodyFrameRecordingAsset AssetTo;
        public Layout CurrentLayout;
        private LayoutType mLayoutType = LayoutType.OneRightByTwoLeft;
        private PanelNode[] mPanelNodes;
        public List<List<ControlPanelType>> ControlPanelTypeList = new List<List<ControlPanelType>>(2);
        private List<ControlPanelType> mLeftControlPanel = new List<ControlPanelType>();
        private List<ControlPanelType> mMainControlPanel = new List<ControlPanelType>();
        private DemoPlayer mPlayer;
        private RebaDemoPointCounting mDemoPointCounting;
        public RebaDemoPointCounting Prefab;
        void Start()
        {
            CreateDefaultLayout();
        }

        public override void CreateDefaultLayout()
        {
            BodiesManager.Instance.CreateNewBody("DefaultBody");
            DefaultBody = BodiesManager.Instance.GetBodyFromUUID("DefaultBody");
            mMainControlPanel.Add(ControlPanelType.DemoKit);

            CurrentLayout = new Layout(mLayoutType, this);
            mPanelNodes = CurrentLayout.ContainerStructure.RenderingPanelNodes;
            mPanelNodes[0].name = "Main";
            mPanelNodes[1].name = "LeftParent";
            mPanelNodes[2].name = "RightParent";
            mPanelNodes[3].name = "LeftTopChild";
            mPanelNodes[4].name = "LeftBotChild";
            mPanelNodes[1].GetComponent<LayoutElement>().preferredWidth += 150f;

            //one panel only to have a playback
            //the other to just render data but, set the arc's layer to be something completely different from another
            BodySegment.IsTrackingHeight = false;
            mPanelNodes[2].PanelSettings.Init(mMainControlPanel, true, DefaultBody);
            mPanelNodes[3].PanelSettings.Init(mLeftControlPanel, false, DefaultBody);

            mPanelNodes[3].PanelCamUpdated += UpdateLayerMasksAfterPanelCamUpdate;

            mDemoPointCounting = Instantiate(Prefab);
            mPanelNodes[4].gameObject.AddComponent<VerticalLayoutGroup>();
            mDemoPointCounting.transform.SetParent(mPanelNodes[4].transform, false);
        }

        private RulaVisualAngleAnalysis vTrunkFlexionExtension;
        private RulaVisualAngleAnalysis vTrunkRotation;
        void UpdateLayerMasksAfterPanelCamUpdate(PanelNode vNode)
        {
            mPlayer = mPanelNodes[2].GetComponentInChildren<DemoPlayer>();
             mRightCamera = mPanelNodes[2].PanelSettings.CameraToBodyPair.PanelCamera;
            mLeftCamera = mPanelNodes[3].PanelSettings.CameraToBodyPair.PanelCamera;
            int vRotMask = LayerMask.NameToLayer("DemoArcAngle");
            int vCoronalMask = LayerMask.NameToLayer("DemoArcModel2");
            int vFinalMask = (1 << vRotMask) | (1 << DefaultBody.RenderedBody.CurrentLayerMask.value);
            int vFinalCorMask = (1 << vCoronalMask) | (1 << DefaultBody.RenderedBody.CurrentLayerMask.value);


            vTrunkFlexionExtension = DefaultBody.RenderedBody.GetRulaVisualAngleAnalysis(
                  AnaylsisFeedBackContainer.PosturePosition.TrunkFlexionExtension);
            vTrunkFlexionExtension.UpdateMask(vCoronalMask);
            vTrunkRotation =
              DefaultBody.RenderedBody.GetRulaVisualAngleAnalysis(
                  AnaylsisFeedBackContainer.PosturePosition.TrunkRotation);
            vTrunkRotation.UpdateMask(vRotMask);
            mLeftCamera.UpdateLayerMask(vFinalMask);
            mRightCamera.UpdateLayerMask(vFinalCorMask);
            mLeftCamera.PanelRenderingCamera.GetComponent<CameraOrbitter>().IsEnabled = false;
            mRightCamera.PanelRenderingCamera.GetComponent<CameraOrbitter>().IsEnabled = false;
            LookAtTarget = mRightCamera.PanelRenderingCamera.GetComponent<CameraOrbitter>().Target;

        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                StartPlayingRecording();
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                MoveTowards = true;
                MoveBack = false;

            }
            if (Input.GetKeyDown(KeyCode.O))
            {
                MoveTowards = false;
                MoveBack = true;

            }
            if (mPlayer.DemoBody.View.Buffer.Count > 0)
            {
                vTrunkFlexionExtension.IsCountingPoints = true;
                vTrunkRotation.IsCountingPoints = true;
                mDemoPointCounting.UpdateScore(vTrunkFlexionExtension.Point, vTrunkRotation.Point);
            }
            else
            {
                vTrunkFlexionExtension.IsCountingPoints = false;
                vTrunkRotation.IsCountingPoints = false;
            }
        }

        public float Speed = 2f;
        private float mPercentage = 0f;
        public bool MoveTowards = false;
        public bool MoveBack = false;
        private void MovePanelCameraTowardsView()
        {
            mPercentage += Time.fixedDeltaTime / Speed;
            if (mPercentage < 1f)
            {

                Vector3 vNewPos1 = PathToTop.GetPointAt(mPercentage);
                Vector3 vNewPos2 = PathToSide.GetPointAt(mPercentage);
                float vNewOrthoSize1 = Mathf.Lerp(1.6f, 1.2f, mPercentage);

                mLeftCamera.PanelRenderingCamera.transform.position = vNewPos1;
                mRightCamera.PanelRenderingCamera.transform.position = vNewPos2;
                mLeftCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.orthographicSize =
                    mLeftCamera.PanelRenderingCamera.orthographicSize = vNewOrthoSize1;
            }
            else
            {
                mPercentage = 1;
                Vector3 vNewPos1 = PathToTop.GetPointAt(1);
                Vector3 vNewPos2 = PathToSide.GetPointAt(1);
                mLeftCamera.PanelRenderingCamera.transform.position = vNewPos1;
                mRightCamera.PanelRenderingCamera.transform.position = vNewPos2;
                mLeftCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                if (!Animated)
                {
                    vTrunkRotation.Show();
                    vTrunkFlexionExtension.Show();
                    Animated = true;
                }

                mRightCamera.PanelRenderingCamera.orthographicSize =
                    mLeftCamera.PanelRenderingCamera.orthographicSize = 1.2f;
            }
        }

        private bool Animated = false;
        private void MovePanelCameraAwayView()
        {
            mPercentage -= Time.fixedDeltaTime / Speed;
            if (mPercentage > 0f)
            {
                float vNewOrthoSize1 = Mathf.Lerp(1.6f, 1.2f, mPercentage);
                Vector3 vNewPos1 = PathToTop.GetPointAt(mPercentage);
                Vector3 vNewPos2 = PathToSide.GetPointAt(mPercentage);
                mLeftCamera.PanelRenderingCamera.transform.position = vNewPos1;
                mRightCamera.PanelRenderingCamera.transform.position = vNewPos2;
                mLeftCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.orthographicSize =
                    mLeftCamera.PanelRenderingCamera.orthographicSize = vNewOrthoSize1;
            }
            else
            {
                mPercentage = 0;
                Vector3 vNewPos1 = PathToTop.GetPointAt(0);
                Vector3 vNewPos2 = PathToSide.GetPointAt(0);
                mLeftCamera.PanelRenderingCamera.transform.position = vNewPos1;
                mRightCamera.PanelRenderingCamera.transform.position = vNewPos2;
                mLeftCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                mRightCamera.PanelRenderingCamera.transform.LookAt(LookAtTarget);
                if (Animated)
                {
                    vTrunkRotation.Hide();
                    vTrunkFlexionExtension.Hide();
                    vTrunkFlexionExtension.ResetPoints();
                    vTrunkFlexionExtension.ResetPoints();
                    Animated = false;
                }

                mRightCamera.PanelRenderingCamera.orthographicSize =
                    mLeftCamera.PanelRenderingCamera.orthographicSize = 1.6f;
            }
        }

        void FixedUpdate()
        {

            if (MoveTowards)
            {
                try
                {
                    MovePanelCameraTowardsView();

                }
                catch (Exception)
                {
                    Debug.Log(mPercentage);
                }
            }
            if (MoveBack)
            {
                try
                {
                    MovePanelCameraAwayView();

                }
                catch (Exception)
                {
                    Debug.Log(mPercentage);
                }

            }

        }
        void StartPlayingRecording()
        {
            mPlayer.InitBodyPlayback(AssetTo.Lines);

        }

        void OnApplicationQuit()
        {

            DefaultBody.StopThread();

        }
    }
}
