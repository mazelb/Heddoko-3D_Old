﻿
/** 
* @file BodySegment.cs
* @brief Containsthe  BodySegment  class 
* @author  
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Body_Data;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;
using System.IO;
using Assets.Scripts.Utils;
public partial class BodySegment
    {
#if SEGMENTS_DEBUG
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType;

    //Body SubSegments 
    public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();

    public Body ParentBody;

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;
    public bool IsReseting = false;
    public int ResetCounter = 0;
    static public bool IsTrackingHeight = true;
    static public bool IsTrackingGait = false;
    static public bool IsTrackingHips = false;
    static public bool IsAdjustingSegmentAxis = false;
    static public bool IsAdjustingArms;
    static public bool IsFusingSubSegments = false;
    static public bool IsProjectingXZ = false;
    static public bool IsProjectingXY = false;
    static public bool IsProjectingYZ = false;
    static public bool IsHipsEstimateForward = true;
    static public bool IsHipsEstimateUp = false;
    static public bool IsUsingInterpolation = true;
    static public float InterpolationSpeed = 0.3f;
    static public bool IsCalibrating = true;
    //Extract the delta time of the frames
    public float LastFrameTime = 0.0f;
    public float CurrentFrameTime = 0.0f;
    public float DeltaTime = 0.0f;

    //Sensor data tuples
    private List<SensorTuple> SensorsTuple = new List<SensorTuple>();


    public BodySegmentView AssociatedView;
    //Analysis pipeline of the segment data
    public SegmentAnalysis mCurrentAnalysisSegment;

    //Detection of vertical Hip position
    private static Vector3 mHipDisplacement = new Vector3(0, 0.95f, 0);
    private static float mRightLegHeight = 0.95f;
    private static float mLeftLegHeight = 0.95f;
    private static float mInitialLegHeight = 0.95f;
    private static Vector3 mRightLegStride = Vector3.zero;
    private static Vector3 mLeftLegStride = Vector3.zero;
    private Vector3 mUACurInitRotation = Vector3.zero;
    private Vector3 mLACurInitRotation = Vector3.zero;
    private Vector3 mULCurInitRotation = Vector3.zero;
    private Vector3 mLLCurInitRotation = Vector3.zero;
    private Vector3 mUTCurInitRotation = Vector3.zero;
    
    // *** Definitions for calibration methods
   //************ 5th try ************
    float vAngle1 = 0;
    float vAvgAngle1 = 0;
    float vSumAngle1 = 0;
    Vector3 vAxis1 = new Vector3(0f, 0f, 0f);
    Vector3 vAvgAxis1 = new Vector3(0f, 0f, 0f);
    Vector3 vSumAxis1 = new Vector3(0f, 0f, 0f);

    float vAngle2 = 0;
    float vAvgAngle2 = 0;
    float vSumAngle2 = 0;
    Vector3 vAxis2 = new Vector3(0f, 0f, 0f);
    Vector3 vAvgAxis2 = new Vector3(0f, 0f, 0f);
    Vector3 vSumAxis2 = new Vector3(0f, 0f, 0f);
    int counter = 0;
    float vAngleofArmsSum = 0;
    //   ******7th try
    Quaternion vQuatAllignUp=Quaternion.identity;
    Quaternion vQuatAllignLo=Quaternion.identity;
    Vector3 vRotAxisUp = new Vector3(0f, 0f, 0f);
    Vector3 vRotAxisLo = new Vector3(0f, 0f, 0f);
    float vRotAngleUp = 0;
    float vRotAngleLo = 0;
    float vAlignAngleLo = 0;
    Vector3 vAlignAxisLo = new Vector3(0f, 0f, 0f);
    float vAlignAngleUp = 0;
    Vector3 vAlignAxisUp = new Vector3(0f, 0f, 0f);
    //   ******** 7-0 try ****
    Vector3 vRLAAxisRightTPose = new Vector3(0f, 0f, 0f);
    Vector3 vRLAAxisRightZombie = new Vector3(0f, 0f, 0f);
    Vector3 vRLAAxisUpZombie = new Vector3(0f, 0f, 0f);
    Quaternion vRAQuatRights = Quaternion.identity;
    Quaternion vRAQuatUps = Quaternion.identity;

    Vector3 vLLAAxisRightTPose = new Vector3(0f, 0f, 0f);
    Vector3 vLLAAxisRightZombie = new Vector3(0f, 0f, 0f);
    Vector3 vLLAAxisUpZombie = new Vector3(0f, 0f, 0f);
    Quaternion vLAQuatRights = Quaternion.identity;
    Quaternion vLAQuatUps = Quaternion.identity;

    ////////************  6th try  ////************
    public Quaternion vUpArmQuatSaved= Quaternion.identity;
    public Quaternion vLoArmQuatSaved = Quaternion.identity;
    public Quaternion vUpArmQuatSaved1 = Quaternion.identity;
    public Quaternion vLoArmQuatSaved1 = Quaternion.identity;
    public Quaternion vUpArmQuatSavedNew = Quaternion.identity;
    public Quaternion vLoArmQuatSavedNew = Quaternion.identity;
    int counterR = 0;
    int counterL = 0;
   

    /// <summary>
    /// The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the current sensor's information
    /// </summary>
    /// <param name="vFrame"></param>
    public void UpdateSensorsData(BodyFrame vFrame)
    {
		Debug.Log("in segments non active");
        //Update the delta time
        CurrentFrameTime = vFrame.Timestamp;
        DeltaTime = CurrentFrameTime - LastFrameTime;
        LastFrameTime = CurrentFrameTime;

        //get the sensor 
        List<BodyStructureMap.SensorPositions> vSensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];

        //get subframes of data relevant to this body segment 
        foreach (BodyStructureMap.SensorPositions vSensorPosKey in vSensorPos)
        {
            //find a suitable sensor to update
            SensorTuple vSensTuple = SensorsTuple.First(a => a.CurrentSensor.SensorPosition == vSensorPosKey);

            //get the relevant data from vFrame 
            if (vFrame.FrameData.ContainsKey(vSensorPosKey))
            {
                Vector3 vFrameData = vFrame.FrameData[vSensorPosKey];
                vSensTuple.CurrentSensor.SensorData.PositionalData = vFrameData;
            }
        }
    }

    /// <summary>
    /// UpdateSegment: Depending on the segment type, apply transformation matrices.
    /// </summary>
    /// <param name="vFilteredDictionary">Dictionnary of tracked segments and their transformations.</param>
    internal void UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary)
    {
        MapSubSegments(vFilteredDictionary);
    }

    /// <summary>
    /// UpdateInitialSensorsData: The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the initial sensor's information.
    /// </summary>
    /// <param name="vFrame">the body frame whose subframes will updates to initial sensors.</param>
    public void UpdateInitialSensorsData(BodyFrame vFrame)
    {
        IsReseting = true;
        List<BodyStructureMap.SensorPositions> vSensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];
        foreach (BodyStructureMap.SensorPositions vPos in vSensorPos)
        {
            //find a suitable sensor to update
            SensorTuple vSensTuple = SensorsTuple.First(a => a.InitSensor.SensorPosition == vPos);

            //get the relevant data from vFrame 
            if (vFrame.FrameData.ContainsKey(vPos))
            {
                vSensTuple.InitSensor.SensorData.PositionalData = vFrame.FrameData[vPos];
                int vKey = (int)vSensTuple.InitSensor.SensorPosition;

                if (vSensTuple.InitSensor.SensorType == BodyStructureMap.SensorTypes.ST_Biomech)
                {
                    //get the subsegment and update its  inverse initial orientation 
                    if (BodySubSegmentsDictionary.ContainsKey(vKey))
                    {
                        BodySubSegmentsDictionary[vKey].ResetViewTransforms();
                    }
                }
            }
        }
        mCurrentAnalysisSegment.ResetMetrics();
    }

    /// <summary>
    /// Resets metrics to their initial values
    /// </summary>
    public void ResetMetrics()
    {
        mCurrentAnalysisSegment.ResetMetrics();
    }

    static int svRightCounter = 0;
    static int svLeftCounter = 0;
    static int svStandCounter = 0;

    /// <summary>
    /// Updates the vertical position of the Hips. TODO: move this to appropriate place
    /// </summary>
    internal void UpdateHipPosition(BodySubSegment vSegment)
    {
        //Update body positions
        Vector3 vDisplacement = Vector3.zero;// mHipDisplacement;
        float vHeightDiff = mRightLegHeight - mLeftLegHeight;

        if (Mathf.Abs(vHeightDiff) < 0.1f)
        {
            //Debug.Log("!!!!!  STAND !!!!!!! " + svStandCounter);
            svStandCounter++;

            //Standing position
            if(IsTrackingHeight)
            {
                vDisplacement.y = mRightLegHeight;
            }
            if(IsTrackingHips)
            {
                //vDisplacement.x = mRightLegStride.x;
                vDisplacement.z = -mRightLegStride.z;
            }
        }
        else
        {
            if(vHeightDiff > 0)
            {
                //Debug.Log("!!!!!!!!!!!!  RIGHT " + svRightCounter);
                svRightCounter++;

                //Right leg height is taller = Standing on the right leg
                if (IsTrackingHeight)
                {
                    vDisplacement.y = mRightLegHeight;
                }
                if (IsTrackingHips)
                {
                    //vDisplacement.x = mRightLegStride.x;
                    vDisplacement.z = -mRightLegStride.z;
                }
            }
            else
            {
                //Left leg height is taller = Standing on the left leg
                //Debug.Log("LEFT !!!!!!!!!!!! " + svLeftCounter);
                svLeftCounter++;

                if (IsTrackingHeight)
                {
                    vDisplacement.y = mLeftLegHeight;
                }
                if (IsTrackingHips)
                {
                    //vDisplacement.x = mLeftLegStride.x;
                    vDisplacement.z = -mLeftLegStride.z;
                }
            }
        }

        /*
        //Hips position is based on an circular motion
        X := originX + sin(angle)*radius;
        Y := originY + cos(angle)*radius;
        OriginX = OriginY = 0 (at the base of the feet) 
        Radius = Full Leg length
        Angle = Angle between the X coordinate vector and the new caluclated hips position
        */
        Vector3 vNewDisplacement = Vector3.zero;
        vNewDisplacement.x = mInitialLegHeight * Mathf.Cos(Mathf.Acos(Vector3.Dot(new Vector3(vDisplacement.x, vDisplacement.y, 0), Vector3.right)));
        vNewDisplacement.y = vDisplacement.y;
        vNewDisplacement.z = mInitialLegHeight * Mathf.Cos(Mathf.Acos(Vector3.Dot(new Vector3(0, vDisplacement.y, vDisplacement.z), Vector3.forward)));

        mHipDisplacement = Vector3.Lerp(mHipDisplacement, vNewDisplacement, 0.5f);
        vSegment.UpdateSubsegmentPosition(mHipDisplacement);
    }

    /// <summary>
    /// MapTorsoSegment: Performs mapping on the torso subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapTorsoSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        TorsoAnalysis vTorsoAnalysis = (TorsoAnalysis)mCurrentAnalysisSegment;

        BodySubSegment vUSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment vLSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        //This is for estimation of the hips orientation (general body orientation)
        BodySubSegment vRLLSubsegment, vRULSubSegment;
        BodySubSegment vLLLSubsegment, vLULSubSegment;
        BodySegment vRightLegSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_RightLeg);
        BodySegment vLeftLegSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_LeftLeg);
        vRLLSubsegment = vRightLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf).Value;
        vRULSubSegment = vRightLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh).Value;
        vLLLSubsegment = vLeftLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf).Value;
        vLULSubSegment = vLeftLegSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * 180f / Mathf.PI;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * 180f / Mathf.PI;

        //Upper torso
        Quaternion vTorsoInitQuat = Quaternion.Euler(0, -vTorsoInitialRawEuler.z, 0);
        Quaternion vTorsoQuatY = Quaternion.Euler(0, -vTorsoCurrentRawEuler.z, 0);
        vTorsoQuatY = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatY;

        vTorsoInitQuat = Quaternion.Euler(-vTorsoInitialRawEuler.x, 0, 0);
        Quaternion vTorsoQuatX = Quaternion.Euler(-vTorsoCurrentRawEuler.x, 0, 0);
        vTorsoQuatX = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatX;
        //Quaternion vTorsoQuatX = Quaternion.Euler(-Mathf.DeltaAngle(vTorsoInitialRawEuler.x, vTorsoCurrentRawEuler.x), 0, 0);

        vTorsoInitQuat = Quaternion.Euler(0, 0, vTorsoInitialRawEuler.y);
        Quaternion vTorsoQuatZ = Quaternion.Euler(0, 0, vTorsoCurrentRawEuler.y);
        vTorsoQuatZ = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatZ;

        //vUSSubsegment.UpdateSubsegmentOrientation(Quaternion.identity, 0, true);
        //vUSSubsegment.AssociatedView.SubsegmentTransform.RotateAround(vUSSubsegment.AssociatedView.SubsegmentTransform.position, vUSSubsegment.AssociatedView.SubsegmentTransform.right, testAngle++);
        //vUSSubsegment.AssociatedView.SubsegmentTransform.RotateAround(vUSSubsegment.AssociatedView.SubsegmentTransform.position, vUSSubsegment.AssociatedView.SubsegmentTransform.right, -Mathf.DeltaAngle(vTorsoInitialRawEuler.x, vTorsoCurrentRawEuler.x));

        ////////////////////////////////////////////////////////  Apply Results To Torso /////////////////////////////////////////////////////////////////////
        Quaternion vTorsoQuat = Quaternion.identity;
        Quaternion vHipQuat = Quaternion.identity;
        float vForwardAngle = 0;
        float vUpAngle = 0;

        if (IsHipsEstimateForward)
        {
            vForwardAngle = -EstimateHipsForwardAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
                                                      vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
                                                      vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
        }

        if (IsHipsEstimateUp)
        {
            vUpAngle = EstimateHipsUpAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
                                           vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
                                           vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
        }

        if (IsUsingInterpolation)
        {
            vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
            vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
        }
        else
        {
            vHipQuat = Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0);
            vTorsoQuat = Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ;
        }

        //Apply results
        vUSSubsegment.UpdateSubsegmentOrientation(vTorsoQuat, 0, true);
        vLSSubsegment.UpdateSubsegmentOrientation(vHipQuat, 3, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        vTorsoAnalysis.TorsoTransform = vUSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.HipGlobalTransform = vLSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.DeltaTime = DeltaTime;
        vTorsoAnalysis.AngleExtraction();

        //UpdateHipPosition(vLSSubsegment);//*/
    }

    internal float EstimateHipsForwardAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
    {
        //Estimate Hips forward orientation
        Vector3 vGlobalVectorUp = Vector3.ProjectOnPlane(Vector3.up, vLSTransform.forward);
        Vector3 vHipsForwardDirection = (Vector3.ProjectOnPlane(vUSTransform.forward, vGlobalVectorUp) + Vector3.ProjectOnPlane(vRULTransform.forward, vGlobalVectorUp) + 
                                         Vector3.ProjectOnPlane(vLULTransform.forward, vGlobalVectorUp) + Vector3.ProjectOnPlane(vRLLTransform.forward, vGlobalVectorUp) + 
                                         Vector3.ProjectOnPlane(vLLLTransform.forward, vGlobalVectorUp)) / 5;

        return SegmentAnalysis.GetSignedAngle(vHipsForwardDirection, Vector3.ProjectOnPlane(Vector3.forward, vLSTransform.up), vGlobalVectorUp);
    }

    internal float EstimateHipsUpAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
    {
        //Estimate Hips forward orientation
        //Vector3 vGlobalVectorRight = Vector3.ProjectOnPlane(Vector3.right, vLSTransform.forward);
        Vector3 vHipsUpDirection = (Vector3.ProjectOnPlane(vUSTransform.up, Vector3.right) + Vector3.ProjectOnPlane(vRULTransform.up, Vector3.right) +
                                    Vector3.ProjectOnPlane(vLULTransform.up, Vector3.right) + Vector3.ProjectOnPlane(vRLLTransform.up, Vector3.right) +
                                    Vector3.ProjectOnPlane(vLLLTransform.up, Vector3.right)) / 5;

        return SegmentAnalysis.GetSignedAngle(vHipsUpDirection, Vector3.up, Vector3.right);
    }

    /// <summary>
    /// MapRightLegSegment: Performs mapping on the right leg subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapRightLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];
        BodySegment vHipsSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vHipsSubsegment = vHipsSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vThighInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vThighCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawEuler * 180f / Mathf.PI;
        MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
        vRightLegAnalysis.ThighTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vRightLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
        vRightLegAnalysis.DeltaTime = DeltaTime;
        vRightLegAnalysis.AngleExtraction();
        mRightLegHeight = vRightLegAnalysis.LegHeight;
        mRightLegStride = vRightLegAnalysis.RightLegStride;
        //UpdateHipPosition(vHipsSubsegment);
    }

    /// <summary>
    /// MapLeftLegSegment: Performs mapping on the left leg subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];
        BodySegment vHipsSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vHipsSubsegment = vHipsSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vThighInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vThighCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawEuler * 180f / Mathf.PI;
        MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment, false);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.ThighTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.DeltaTime = DeltaTime;
        vLeftLegAnalysis.AngleExtraction();
        mLeftLegHeight = vLeftLegAnalysis.LegHeight;
        mLeftLegStride = vLeftLegAnalysis.LeftLegStride;
        UpdateHipPosition(vHipsSubsegment);
    }

    /// <summary>
    /// MapLegsOrientations: Updates the legs orientations from the initial and current eulerangles.
    /// </summary>
    public void MapLegsOrientations(Vector3 vULInitEuler, Vector3 vULCurEuler, Vector3 vLLInitEuler, Vector3 vLLCurEuler,
                                    BodySubSegment vULSubsegment, BodySubSegment vLLSubsegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
    {
        //Upper Leg
        Quaternion vThighInitQuat = Quaternion.Euler(0, -vULInitEuler.z, 0);
        Quaternion vThighQuatY = Quaternion.Euler(0, -vULCurEuler.z, 0);
        vThighQuatY = Quaternion.Inverse(vThighInitQuat) * vThighQuatY;

        vThighInitQuat = Quaternion.Euler(-vULInitEuler.x, 0, 0);
        Quaternion vThighQuatX = Quaternion.Euler(-vULCurEuler.x, 0, 0);
        vThighQuatX = Quaternion.Inverse(vThighInitQuat) * vThighQuatX;

        vThighInitQuat = Quaternion.Euler(0, 0, vULInitEuler.y);
        Quaternion vThighQuatZ = Quaternion.Euler(0, 0, vULCurEuler.y);
        vThighQuatZ = Quaternion.Inverse(vThighInitQuat) * vThighQuatZ;

        //Lower leg
        Quaternion vKneeInitQuat = Quaternion.Euler(0, -vLLInitEuler.z, 0);
        Quaternion vKneeQuatY = Quaternion.Euler(0, -vLLCurEuler.z, 0);
        vKneeQuatY = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatY;

        vKneeInitQuat = Quaternion.Euler(-vLLInitEuler.x, 0, 0);
        Quaternion vKneeQuatX = Quaternion.Euler(-vLLCurEuler.x, 0, 0);
        vKneeQuatX = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatX;

        vKneeInitQuat = Quaternion.Euler(0, 0, vLLInitEuler.y);
        Quaternion vKneeQuatZ = Quaternion.Euler(0, 0, vLLCurEuler.y);
        vKneeQuatZ = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatZ;

        //Apply results
        Quaternion vThighQuat = vThighQuatY * vThighQuatX * vThighQuatZ;
        Quaternion vKneeQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;


        //Get necessary Axis info
        Vector3 vULAxisUp, vULAxisRight, vULAxisForward;
        Vector3 vLLAxisUp, vLLAxisRight, vLLAxisForward;
        Vector3 vNewRight1 = Vector3.up;
        Vector3 vNewRight2 = Vector3.up;
        Vector3 vULNewForward = Vector3.forward;
        Vector3 vLLNewForward = Vector3.forward;

        vULAxisUp = vThighQuat * Vector3.up;
        vULAxisRight = vThighQuat * Vector3.right;
        vULAxisForward = vThighQuat * Vector3.forward;
        Vector3 vNewULAxisUp = vULAxisUp;

        vLLAxisUp = vKneeQuat * Vector3.up;
        vLLAxisRight = vKneeQuat * Vector3.right;
        vLLAxisForward = vKneeQuat * Vector3.forward;

        vNewRight1 = Vector3.Cross(vULAxisUp, vLLAxisUp).normalized;

        if (Mathf.Abs(Vector3.Angle(vLLAxisUp, vULAxisUp)) < 10)
        {
            vNewRight1 = vULAxisRight;
        }

        //if (IsFusingSubSegments)
        //{
        //    Vector3 vULAxisUpProjected = Vector3.ProjectOnPlane(vULAxisUp, vULAxisRight);
        //    Vector3 vLLAxisUpProjected = Vector3.ProjectOnPlane(vULAxisUp, vULAxisRight);

        //    float vULAdjustAngle = SegmentAnalysis.GetSignedAngle(vULAxisUpProjected, vLLAxisUpProjected, vULAxisRight);

        //    vULNewForward = Vector3.Cross(vNewRight1, vNewULAxisUp).normalized;
        //    vLLNewForward = Vector3.Cross(vNewRight1, vLLAxisUp).normalized;

        //    vThighQuat = Quaternion.LookRotation(vULNewForward, vNewULAxisUp);
        //    vKneeQuat = Quaternion.LookRotation(vLLNewForward, vLLAxisUp);
        //}

        Quaternion vNewThighQuat = Quaternion.identity;
        Quaternion vNewKneeQuat = Quaternion.identity;

        if (IsUsingInterpolation)
        {
            //if (IsAdjustingSegmentAxis)
            //{
            //    vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat * vThighQuatY, InterpolationSpeed);
            //    vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat * vKneeQuatY, InterpolationSpeed);
            //}
            //else
            {
                vNewThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuat, InterpolationSpeed);
                vNewKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, vKneeQuat, InterpolationSpeed);
            }
        }
        else
        {
            //if (IsAdjustingSegmentAxis)
            //{
            //    vNewThighQuat = vThighQuat * vThighQuatY;
            //    vNewKneeQuat = vKneeQuat * vKneeQuatY;
            //}
            //else
            {
                vNewThighQuat = vThighQuat;
                vNewKneeQuat = vKneeQuat;
            }
        }

        vULSubsegment.UpdateSubsegmentOrientation(vNewThighQuat, 1, true);
        vLLSubsegment.UpdateSubsegmentOrientation(vNewKneeQuat, 1, true);
    }

    /// <summary>
    /// MapRightArmSubsegment: Updates the right arm subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;

        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];
        BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
        BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value; 

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * Mathf.Rad2Deg;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        vRightArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.DeltaTime = DeltaTime;
        vRightArmAnalysis.ReferenceVector = Vector3.one;
        vRightArmAnalysis.AngleExtraction();//*/
    }

    /// <summary>
    /// MapLeftArmSubsegment: Updates the left arm subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
        BodySegment vTorsoSegment = ParentBody.GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes.SegmentType_Torso);
        BodySubSegment vTorsoSubSegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine).Value;
        BodySubSegment vHipsSubsegment = vTorsoSegment.BodySubSegmentsDictionary.FirstOrDefault(x => x.Key == (int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine).Value;

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawEuler * Mathf.Rad2Deg;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawEuler  * Mathf.Rad2Deg;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler  * Mathf.Rad2Deg;
        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler * Mathf.Rad2Deg;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler * Mathf.Rad2Deg;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vTorsoInitialRawEuler, vTorsoCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment, false);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis)mCurrentAnalysisSegment;
        vLeftArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.DeltaTime = DeltaTime;
        vLeftArmAnalysis.AngleExtraction();//*/
    }

    /// <summary>
    /// MapArmsOrientations: Updates the arm orientations from the initial and current eulerangles.
    /// </summary>
    float vCurrentAngle = 0;
    bool vIsIncreasingAngle = true;
	
    public void MapArmsOrientations(Vector3 vUAInitEuler, Vector3 vUACurEuler, Vector3 vLAInitEuler, Vector3 vLACurEuler, Vector3 vTorsoInitEuler, Vector3 vTorsoCurEuler,
                                    BodySubSegment vUASubsegment, BodySubSegment vLASubsegment, BodySubSegment vTorsoSubSegment, BodySubSegment vHipsSubsegment, bool vIsRight = true)
    {
        //Upper arm
        //Quaternion vUpArmInitQuatY = Quaternion.Euler(0, 0, 0);
        //Quaternion vUpArmQuatY = Quaternion.Euler(0, (vIsRight) ? 100 * Mathf.Sin(vCurrentAngle) : -100 * Mathf.Sin(vCurrentAngle), 0);
        //vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuatY) * vUpArmQuatY;
        //vUpArmQuatY = Quaternion.Inverse(vUpArmQuatY);
		
        Quaternion vUpArmInitQuatY = Quaternion.Euler(0, (vIsRight) ? vUAInitEuler.z : (vUAInitEuler.z), 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, (vIsRight) ? vUACurEuler.z : (vUACurEuler.z), 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuatY) * vUpArmQuatY;
        vUpArmQuatY = Quaternion.Inverse(vUpArmQuatY);

        Quaternion vUpArmInitQuatX = Quaternion.Euler(vUAInitEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUACurEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuatX) * vUpArmQuatX;

        Quaternion vUpArmInitQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vUAInitEuler.y : (vUAInitEuler.y));
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vUACurEuler.y : (vUACurEuler.y));
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuatZ) * vUpArmQuatZ;
        vUpArmQuatZ = Quaternion.Inverse(vUpArmQuatZ);
		
        //Lower arm
        //Quaternion vLoArmInitQuatY = Quaternion.Euler(0, 0, 0);
        //Quaternion vLoArmQuatY = Quaternion.Euler(0, (vIsRight) ? 100 * Mathf.Sin(vCurrentAngle) : -100 * Mathf.Sin(vCurrentAngle), 0);
        //vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuatY) * vLoArmQuatY;
        //vLoArmQuatY = Quaternion.Inverse(vLoArmQuatY);

        Quaternion vLoArmInitQuatY = Quaternion.Euler(0, (vIsRight) ? vLAInitEuler.z : (vLAInitEuler.z), 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, (vIsRight) ? vLACurEuler.z : (vLACurEuler.z), 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuatY) * vLoArmQuatY;
        vLoArmQuatY = Quaternion.Inverse(vLoArmQuatY);

                Quaternion vLoArmInitQuatX = Quaternion.Euler(vLAInitEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLACurEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuatX) * vLoArmQuatX;

        Quaternion vLoArmInitQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vLAInitEuler.y : (vLAInitEuler.y));
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, (vIsRight) ? vLACurEuler.y : (vLACurEuler.y));
        vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuatZ) * vLoArmQuatZ;
        vLoArmQuatZ = Quaternion.Inverse(vLoArmQuatZ);

        Quaternion vUpArmQuat = vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;

        vCurrentAngle += 0.01f;
		 
       //Get necessary Axis info
        Vector3 vUAAxisUp, vUAAxisRight, vUAAxisForward;
        Vector3 vLAAxisUp, vLAAxisRight, vLAAxisForward;
		
        Vector3 vUANewRight = Vector3.right;
        Vector3 vLANewRight = Vector3.right;
        Vector3 vUANewUp = Vector3.up;
        Vector3 vLANewUp = Vector3.up;
        Vector3 vUANewForward = Vector3.forward;
        Vector3 vLANewForward = Vector3.forward;
		
       vUAAxisUp = vUpArmQuat * Vector3.up;
        vUAAxisRight = vUpArmQuat * Vector3.right;
        vUAAxisForward = vUpArmQuat * Vector3.forward;
		
        vLAAxisUp = vLoArmQuat * Vector3.up;
        vLAAxisRight = vLoArmQuat * Vector3.right;
        vLAAxisForward = vLoArmQuat * Vector3.forward;

        // **** Calibration process for arms *****
        if (IsCalibrating)
        {
            Quaternion vLoArmQuatNew;
            Quaternion vUpArmQuatNew;

            //////---***********6th try  ************
            //// calculate the axis of rotation between two consequtive frames

            //Quaternion vDiffCalibrateUp2 = Quaternion.Inverse(vUpArmQuatSaved1) * vUpArmQuat;
            //Quaternion vDiffCalibrateLo2 = Quaternion.Inverse(vLoArmQuatSaved1) * vLoArmQuat;


            //float angleUp = 0.0F;
            //Vector3 axisUp = Vector3.zero;
            //vDiffCalibrateUp2.ToAngleAxis(out angleUp, out axisUp);
            //axisUp = axisUp.normalized;

            //float angleLo = 0.0F;
            //Vector3 axisLo = Vector3.zero;
            //vDiffCalibrateLo2.ToAngleAxis(out angleLo, out axisLo);
            //axisLo = axisLo.normalized;

            ////Debug.Log("6th" + " ;" + axisLo + " ;" + axisUp);

            //////****** write *******
            //int vFileNameIncrement = 10;
            ////Make sure we have a directory to write to.
            //string vDataPath = "../RecordMar9";
            //if (!Directory.Exists(vDataPath))
            //{
            //    Directory.CreateDirectory(vDataPath);
            //}
            //string mRecordingFileNameR = string.Format("{0}/{1}_{2}.csv", vDataPath, "6th_ElbowRight", vFileNameIncrement);
            //string mRecordingFileNameL = string.Format("{0}/{1}_{2}.csv", vDataPath, "6th_ElbowLeft", vFileNameIncrement);
            //StreamWriter writeRightArm = new StreamWriter(mRecordingFileNameR, true);
            //StreamWriter writeLeftArm = new StreamWriter(mRecordingFileNameL, true);

            //if (vIsRight)
            //{
            //    writeRightArm.WriteLine(angleLo + "," + axisLo.x + "," + axisLo.y + "," + axisLo.z + "," + angleUp + "," + axisUp.x + "," + axisUp.y + "," + axisUp.z);
            //}
            //else
            //{
            //    writeLeftArm.WriteLine(angleLo + "," + axisLo.x + "," + axisLo.y + "," + axisLo.z + "," + angleUp + "," + axisUp.x + "," + axisUp.y + "," + axisUp.z);
            //}

            //writeRightArm.Close();
            //writeLeftArm.Close();

            ////save data for next frame
            //vUpArmQuatSaved1 = vUpArmQuat;
            //vLoArmQuatSaved1 = vLoArmQuat;

            /////// ********** 7th try ****** cross product of rights is the rotation axis in 1st and last frame
            //if (vIsRight)
            //{
            //    counterR++;
            //}
            //else
            //{
            //    counterL++;
            //}
            ////Debug.Log(counterR+ ";" + counterL);
            //if (counterR< 330 && counterR>220) //end of the desired movement counter<150 && counter>140 -for right arm only
            //{
            //    vUAAxisRight = vUpArmQuat * Vector3.right;
            //    vLAAxisRight = vLoArmQuat * Vector3.right;
            //    vUAAxisUp = vUpArmQuat * Vector3.up;
            //    vLAAxisUp = vLoArmQuat * Vector3.up;
            //    ////Initial frame right axis -....
            //    Vector3 vUAAxisRightInit = Vector3.right;
            //    Vector3 vLAAxisRightInit = Vector3.right;

            //    //    find the rotation axis with sensor coordinates - current and initial frame right axis cross product is the rotation axis
            //     vRotAxisUp = Vector3.Cross(vUAAxisRightInit,- vUAAxisRight).normalized; //rot axis btw start and end of movement
            //     vRotAxisLo = Vector3.Cross(vLAAxisRightInit, -vLAAxisRight).normalized;
            //     vRotAngleUp = Vector3.Angle(vUAAxisRightInit, -vUAAxisRight);
            //     vRotAngleLo = Vector3.Angle(vLAAxisRightInit, -vLAAxisRight); // felxion angle
            //     //Debug.Log(counterR+"vRotAxisUp" + vRotAxisUp + "vRotAxisLo" + vRotAxisLo + "LoArmUp" + vLAAxisUp + "UpperUp" + vUAAxisUp + "vRotAngleLo" + vRotAngleLo + "vRotAngleUp" + vRotAngleUp);

            //     vAlignAngleLo = Vector3.Angle(vRotAxisUp, Vector3.up); 
            //     vAlignAxisLo = Vector3.Cross(vRotAxisUp, Vector3.up).normalized;
            //     vAlignAngleUp = Vector3.Angle(vRotAxisLo,Vector3.up);
            //     vAlignAxisUp = Vector3.Cross(vRotAxisLo, Vector3.up);

            //     vQuatAllignUp = Quaternion.AngleAxis(vAlignAngleUp,vAlignAxisUp);
            //     vQuatAllignLo = Quaternion.AngleAxis(vAlignAngleLo, vAlignAxisLo);

            //    //////   ***write * ******to chek the amounts
            //    //int vFileNameIncrement7 = 7;
            //    ////Make sure we have a directory to write to.
            //    //string vDataPath7 = "../RecordFati2";
            //    //if (!Directory.Exists(vDataPath7))
            //    //{
            //    //    Directory.CreateDirectory(vDataPath7);
            //    //}
            //    //string mRecordingFileNameR7 = string.Format("{0}/{1}_{2}.csv", vDataPath7, "_RightArm", vFileNameIncrement7);
            //    //string mRecordingFileNameL7 = string.Format("{0}/{1}_{2}.csv", vDataPath7, "_LeftArm", vFileNameIncrement7);
            //    //StreamWriter writeRightArm7 = new StreamWriter(mRecordingFileNameR7, true);
            //    //StreamWriter writeLeftArm7 = new StreamWriter(mRecordingFileNameL7, true);

            //    //if (vIsRight)
            //    //{
            //    //    writeRightArm7.WriteLine(angleLo + "," + vRotAxisLo.x + "," + vRotAxisLo.y + "," + vRotAxisLo.z + "," + angleUp + "," + vRotAxisUp.x + "," + vRotAxisUp.y + "," + vRotAxisUp.z);
            //    //}

            //    //else
            //    //{
            //    //    writeLeftArm7.WriteLine(angleLo + "," + vRotAxisLo.x + "," + vRotAxisLo.y + "," + vRotAxisLo.z + "," + angleUp + "," + vRotAxisUp.x + "," + vRotAxisUp.y + "," + vRotAxisUp.z);
            //    //}
            //    //writeRightArm7.Close();
            //    //writeLeftArm7.Close();
            //}   
            //else if(counterR>150)
            //{
            //    vUpArmQuatNew = vQuatAllignUp * vUpArmQuat;
            //    vLoArmQuatNew = vQuatAllignLo * vLoArmQuat;
            //    //vLoArmQuat = vLoArmQuatNew;
            //    //vUpArmQuat = vUpArmQuatNew;
            //}

            ///// ********** 8th  try ****** from T-Pose to zombie pose (90 degree rotation of the upper arm to front)

            vUAAxisRight = vUpArmQuat * Vector3.right;
            vLAAxisRight = vLoArmQuat * Vector3.right;
            vUAAxisUp = vUpArmQuat * Vector3.up;
            vLAAxisUp = vLoArmQuat * Vector3.up;
            if (vIsRight)
            {
                //if (counterR < 35) //T-pose-horizontal shoulder movement
                //{
                //    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                //}
                //if (counterR < 202 && counterR > 195)
                //{
                //    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                //}

                if (counterR < 20) //T-pose-horizontal elbow movement
                {
                    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                    vRLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal =(1,0,0)
                }
                if (counterR < 297 && counterR > 293) // zombie pose-horizontal elbow movement (90 degree rotation frames)
                {
                    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                    vRLAAxisRightZombie = vLAAxisRight; //right direction in zombie pose the ideal is (0,0,1)
                    vRLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at t-pose
                }
                vRAQuatRights = Quaternion.FromToRotation(vRLAAxisRightZombie, Vector3.forward);
                vRAQuatUps = Quaternion.FromToRotation(vRLAAxisUpZombie, Vector3.up);
                Debug.Log(counterR + ";" + vRAQuatRights + ";" + vRAQuatUps);
                if (counterR > 296) // apply calibration from now on
                {
                    vLoArmQuat = vRAQuatUps * vLoArmQuat;
                    if (IsFusingSubSegments)// apply 
                    {
                        vLoArmQuat = vRAQuatRights * vLoArmQuat;
                    }
                }
                //if (counter < 35) //T-pose-horizontal shoulder
                //{
                //    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                //}
                //if (counterR < 202 && counterR > 195)
                //{
                //    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                //}
            }

            else //left arm
            {
             
                if (counterL < 20) //T-pose-horizontal elbow
                {
                    Debug.Log(counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
                    vLLAAxisRightTPose = vLAAxisRight; //right direction in T-pose ,it is ideal(1,0,0)
                }
                if (counterL < 297 && counterR > 293)
                {
                    Debug.Log(counterR + ";" + vUAAxisRight + ";" + vLAAxisRight);
                    vLLAAxisRightZombie = vLAAxisRight; //right direction in zombie pose the ideal is (0,0,1)
                    vLLAAxisUpZombie = vLAAxisUp;  //up direction in zombie pose ideal is (1,0,0) we suppose ideal movement around up initial axis at t-pose
                }
                vLAQuatRights = Quaternion.FromToRotation(vLLAAxisRightZombie, Vector3.forward);
                vLAQuatUps = Quaternion.FromToRotation(vLLAAxisUpZombie, Vector3.up);
                Debug.Log(counterL + ";" + vLAQuatRights + ";" + vLAQuatUps);
                if (counterL > 296)
                {
                    vLoArmQuat = vLAQuatRights * vLoArmQuat;
                }
            }


            //else
            //{
            //    if (counterL < 35) //T-pose-horizontal shoulder
            //    {
            //        Debug.Log(counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //    }
            //    if (counterL < 202 && counterL > 195)
            //    {
            //        Debug.Log(counterL + ";" + vUAAxisRight + ";" + vLAAxisRight);
            //    }
            //}
        }

        /////////////////////////// Arms Fusion ////////////////////////   
        //{

        //    /////////// Step1: keeping lower arm in the plane with upper arm .Up axis as the normal /////////////                                                              
        //    Vector3 vLAAxisRightProjected = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisUp); //projected on palne with upper arm up axis as the normal
        //    float vCopmAngle = Vector3.Angle(vLAAxisRight, vLAAxisRightProjected);
        //    Vector3 vRotAxis = Vector3.Cross(vLAAxisRight, vLAAxisRightProjected);
        //    Quaternion vQuatRot = Quaternion.AngleAxis(vCopmAngle, vRotAxis);
        //    //QuatRotL = Quaternion.identity; //to cancel step1

        //    ///////////// Step2: Omit rotation of lower arm  //////////////
        //    Vector3 vLAAxisUp1 = (vQuatRot * vLoArmQuat) * Vector3.up;
        //    float vCopmAngleLARot = Vector3.Angle(vLAAxisUp1, vUAAxisUp);
        //    Vector3 vRotAxisLARot = Vector3.Cross(vLAAxisUp1, vUAAxisUp);
        //    Quaternion vQuatLARot = Quaternion.AngleAxis(vCopmAngleLARot, vRotAxisLARot);

        //    ////////////// Step3: Fix arm flexion angle /////////////
        //    //Current lower arm right axis
        //    Vector3 vLoArmRightAxisCurr = (vQuatLARot * vQuatRot * vLoArmQuat) * Vector3.right;
        //    //Current angle of  lower arm and upper arm right axes      
        //    float vCopmAngleCurr = Vector3.Angle(vLoArmRightAxisCurr, vUAAxisRight);
        //    //Initial angle of  lower arm and upper arm right axes 
        //    float vCopmAngleInit = Vector3.Angle(vLAAxisRight, vUAAxisRight);
        //    float vMeanAngle = (vCopmAngleCurr + vCopmAngleInit) / 2; //To Do: check using this angle for creating the quaternion
        //    Vector3 vRotAxis180Init = Vector3.Cross(vLoArmRightAxisCurr, vUAAxisRight);
        //    Quaternion vQuatRot180Init = Quaternion.AngleAxis(vCopmAngleCurr - vCopmAngleInit, vRotAxis180Init);

        //    ////////////// Step4: Arm 180 degree constraint /////////////
        //    Vector3 vLoArmRightAxis180 = (vQuatRot180Init * vQuatLARot * vQuatRot * vLoArmQuat) * Vector3.right; //right lower arm axis in world
        //    float vCopmAngle180 = Vector3.Angle(vLoArmRightAxis180, vUAAxisRight);
        //    //Setting the limit of arm flexion to 160 degree
        //    vCopmAngle180 = vCopmAngle180 > 90 ? vCopmAngle180 - 200 : vCopmAngle180; // To Do: fix it later for 90 degree, it may jump @90 deg //if (vCopmAngle180>90) {vCopmAngle180 = 180 - vCopmAngle180;}      
        //    Vector3 vRotAxis180 = Vector3.Cross(vLoArmRightAxis180, vUAAxisRight);
        //    Quaternion vQuatRot180 = Quaternion.identity;

        //    bool Is180;
        //    if (vIsRight)
        //    {
        //        Is180 = (Vector3.Dot(vUAAxisForward, vLoArmRightAxis180) < 0 ? true : false);
        //    }
        //    else
        //    {
        //        Is180 = (Vector3.Dot(vUAAxisForward, vLoArmRightAxis180) > 0 ? true : false);
        //    }

        //    if (Is180) // When arm is bended backwards
        //    {
        //        vQuatRot180 = Quaternion.AngleAxis(vCopmAngle180, vRotAxis180);
        //    }

        //    /////////////// Step5: Rotate Upper arm within lower arm ////////////////// 
        //    //To Do: check for the cases that we already have upper arm rotation
        //    //difference between current and initial rotaion of lower arm
        //    Vector3 vLoArmUpAxisInit = vLAAxisUp;
        //    Vector3 vLoArmUpAxisCurr = vQuatRot180 * vQuatLARot * vQuatRot * vLoArmQuat * Vector3.up;
        //    float vCompAngleUARot = Vector3.Angle(vLoArmUpAxisInit, vLoArmUpAxisCurr);
        //    Vector3 vAxisUARot = vUpArmQuat * Vector3.right;
        //    Quaternion vQuatUARot = Quaternion.AngleAxis(-vCompAngleUARot, vAxisUARot);

        //    //////////// Update and apply results ////////////
        //    Quaternion vNewUpArmQuat = Quaternion.identity;
        //    Quaternion vNewLoArmQuat = Quaternion.identity;

        //    if (IsUsingInterpolation)
        //    {
        //        vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vQuatUARot * vUpArmQuat, InterpolationSpeed);

        //        if (IsAdjustingArms)
        //        {
        //            vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vQuatUARot * vQuatRot180 * vQuatRot180Init * vQuatLARot * vQuatRot * vLoArmQuat * vLoArmQuatX, InterpolationSpeed);
        //        }
        //        else
        //        {   
        //            vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vQuatUARot * vQuatRot180 * vQuatRot180Init * vQuatLARot * vQuatRot * vLoArmQuat, InterpolationSpeed);
        //        }
        //    }
        //    else
        //    {
        //        vNewUpArmQuat = vQuatUARot *  vUpArmQuat;

        //        if (IsAdjustingArms)
        //        {
        //            vNewLoArmQuat = vQuatUARot *  vQuatRot180 * vQuatRot180Init * vQuatLARot * vQuatRot * vLoArmQuat * vLoArmQuatX;
        //        }
        //        else
        //        {
        //            vNewLoArmQuat = vQuatUARot * vQuatRot180 * vQuatRot180Init * vQuatLARot * vQuatRot * vLoArmQuat;
        //        }
        //    }

        //    vUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
        //    vLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
        //}

        if (IsProjectingXY)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisForward);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }
        else if(IsProjectingXZ)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisUp);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }
        else if(IsProjectingYZ)
        {
            vLANewRight = Vector3.ProjectOnPlane(vLAAxisRight, vUAAxisRight);
            vLANewUp = Vector3.Cross(vLAAxisForward, vLANewRight);
            vLANewForward = Vector3.Cross(vLANewRight, vLANewUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vLANewUp);
        }

        vUAAxisUp = vUpArmQuat * Vector3.up;
        vUAAxisRight = vUpArmQuat * Vector3.right;
        vUAAxisForward = vUpArmQuat * Vector3.forward;

        vLAAxisUp = vLoArmQuat * Vector3.up;
        vLAAxisRight = vLoArmQuat * Vector3.right;
        vLAAxisForward = vLoArmQuat * Vector3.forward;

        if (IsFusingSubSegments)
        {
            Vector3 vNewAxisUp = Vector3.up;
            Vector3 vTempUp = Vector3.up;

            vNewAxisUp = Vector3.Cross(vLAAxisRight, vUAAxisRight).normalized;
            vTempUp = Vector3.Cross(vUAAxisRight, vLAAxisRight).normalized;

            if (Mathf.Abs(Vector3.Angle(vLAAxisRight, vUAAxisRight)) < 10)
            {
                vNewAxisUp = vUAAxisUp;
            }
            else
            {
                if (!vIsRight)
                {
                    vNewAxisUp = vTempUp;
                }
            }

            vUANewForward = Vector3.Cross(vUAAxisRight, vNewAxisUp).normalized;
            vLANewForward = Vector3.Cross(vLAAxisRight, vNewAxisUp).normalized;

            vUpArmQuat = Quaternion.LookRotation(vUANewForward, vNewAxisUp);
            vLoArmQuat = Quaternion.LookRotation(vLANewForward, vNewAxisUp);

            //Vector3 vNewUAAxisRight = vUpArmQuat * Vector3.right;
            //Vector3 vNewLAAxisRight = vLoArmQuat * Vector3.right;
            //if(vIsRight) Debug.Log(SegmentAnalysis.GetSignedAngle(vNewUAAxisRight, vNewLAAxisRight, vNewAxisUp));
            //vLoArmQuat = Quaternion.AngleAxis(SegmentAnalysis.GetSignedAngle(vNewLAAxisRight, vNewUAAxisRight, vNewAxisUp), vNewAxisUp);
         }

        Quaternion vNewUpArmQuat = Quaternion.identity;
        Quaternion vNewLoArmQuat = Quaternion.identity;

        if (IsUsingInterpolation)
        {
            if (IsAdjustingSegmentAxis)
            {
                vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat * Quaternion.Inverse(vUpArmQuatX), InterpolationSpeed);
                vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat * Quaternion.Inverse(vLoArmQuatX), InterpolationSpeed);
            }
            else
            {
                vNewUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuat, InterpolationSpeed);
                vNewLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, vLoArmQuat, InterpolationSpeed);
            }
        }
        else
        {
            if (IsAdjustingSegmentAxis)
            {
                vNewUpArmQuat = vUpArmQuat * Quaternion.Inverse(vUpArmQuatX);
                vNewLoArmQuat = vLoArmQuat * Quaternion.Inverse(vLoArmQuatX);
            }
            else
            {
                vNewUpArmQuat = vUpArmQuat;
                vNewLoArmQuat = vLoArmQuat;
            }
        }

        vUASubsegment.UpdateSubsegmentOrientation(vNewUpArmQuat, 1, true);
        vLASubsegment.UpdateSubsegmentOrientation(vNewLoArmQuat, 1, true);
    }

    public float getAngleDelta(float vCurAngle, float vInitAngle)
    {
        float vResult = 0;

        if (vCurAngle > 0)
        {
            if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
            {
                vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
            }
            else
            {
                vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
            }
        }
        else
        {
            if (Mathf.Abs(vCurAngle) > Mathf.Abs(vInitAngle))
            {
                vResult = Mathf.Abs(vInitAngle) - Mathf.Abs(vCurAngle);
            }
            else
            {
                vResult = Mathf.Abs(vCurAngle) - Mathf.Abs(vInitAngle);
            }
        }

        return vResult;
    }

    /// <summary>
    /// InitializeBodySegment: Initializes a new body structure's internal properties with the desired Segment Type.
    /// </summary>
    /// <param name="vSegmentType">The segment type to initialize it to.</param>
    internal void InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    {
        GameObject go = new GameObject(EnumUtil.GetName(vSegmentType));
        AssociatedView = go.AddComponent<BodySegmentView>();

        List<BodyStructureMap.SubSegmentTypes> subsegmentTypes =
           BodyStructureMap.Instance.SegmentToSubSegmentMap[vSegmentType];

        List<BodyStructureMap.SensorPositions> sensorPositions =
            BodyStructureMap.Instance.SegmentToSensorPosMap[vSegmentType];

        foreach (var sensorPos in sensorPositions)
        {
            Sensor newSensor = new Sensor();
            newSensor.SensorBodyId = BodyStructureMap.Instance.SensorPosToSensorIDMap[sensorPos];
            newSensor.SensorType = BodyStructureMap.Instance.SensorPosToSensorTypeMap[sensorPos];
            newSensor.SensorPosition = sensorPos;
            SensorTuple tuple = new SensorTuple();
            tuple.CurrentSensor = new Sensor(newSensor);
            tuple.InitSensor = new Sensor(newSensor);
            SensorsTuple.Add(tuple);
        }

        foreach (BodyStructureMap.SubSegmentTypes sstype in subsegmentTypes)
        {
            BodySubSegment subSegment = new BodySubSegment();
            subSegment.subsegmentType = sstype;
            subSegment.InitializeBodySubsegment(sstype);
            BodySubSegmentsDictionary.Add((int)sstype, subSegment);
            subSegment.AssociatedView.transform.parent = AssociatedView.transform;
        }

    }


    /// <summary>
    /// MapSubSegments: Perform mapping on the current segments and its respective subsegments.
    /// </summary>
    /// <param name="vFilteredDictionary">Dictionnary of tracked segments and their transformations.</param>
    private void MapSubSegments(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary)
    {
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso)
        {
            //Debug.Log("TORSO");
            MapTorsoSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
        {
            //Debug.Log("RIGHT ARM");
            MapRightArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
        {
            //Debug.Log("LEFT ARM");
            MapLeftArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightLeg)
        {
            //Debug.Log("RIGHT LEG");
            MapRightLegSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftLeg)
        {
            //Debug.Log("LEFT LEG");
            MapLeftLegSegment(vFilteredDictionary);
        }
    }

    /// <summary>
    /// Updates subsegments with the passed in RenderedBody
    /// </summary>
    /// <param name="vRendered"></param>
    public void UpdateRenderedSegment(RenderedBody vRendered)
    {
        foreach (var vsubSegment in BodySubSegmentsDictionary)
        {
            Transform vSubSegmentTransform = vRendered.GetSubSegment((BodyStructureMap.SubSegmentTypes)vsubSegment.Key);
            vsubSegment.Value.UpdateSubSegmentTransform(vSubSegmentTransform);
        }
    }


#endif
}

