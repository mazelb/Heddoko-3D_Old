/** 
* @file BodySegment.cs
* @brief Contains the BodySegment  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Utils;
using System;
using Assets.Scripts.Body_Pipeline.Analysis;
using Assets.Scripts.Body_Pipeline.Analysis.Arms;
using Assets.Scripts.Body_Pipeline.Analysis.Legs;
using Assets.Scripts.Body_Pipeline.Analysis.Torso;

/// <summary>
/// BodySegment class: represents one abstracted reprensentation of a body segment.
/// </summary>
public class BodySegment
{
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType;

    //Body SubSegments 
    public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();

    public Body ParentBody;

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;
    static public bool IsTrackingHeight = true;
    static public bool IsAdjustingArms = true;
    static public bool IsHipsEstimateForward = true;
    static public bool IsHipsEstimateUp = true;
    static public bool IsUsingInterpolation = true;
    static public float InterpolationSpeed = 0.3f;

    //Extract the delta time of the frames
    public float LastFrameTime = 0.0f;
    public float CurrentFrameTime = 0.0f;
    public float DeltaTime = 0.0f;

    //Sensor data tuples
    private List<SensorTuple> SensorsTuple = new List<SensorTuple>();

    //Associated view for the segment
    public BodySegmentView AssociatedView;

    //Analysis pipeline of the segment data
    public SegmentAnalysis mCurrentAnalysisSegment;

    //Detection of vertical Hip position
    private static float mHipHeight = 0.95f;
    private static float mRightLegHeight = 0.95f;
    private static float mLeftLegHeight = 0.95f;

    /// <summary>
    /// The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the current sensor's information
    /// </summary>
    /// <param name="vFrame"></param>
    public void UpdateSensorsData(BodyFrame vFrame)
    {
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
                        BodySubSegmentsDictionary[vKey].ResetViewOrientation();
                    }
                }
            }
        }
        mCurrentAnalysisSegment.ResetMetrics();
    }

    /// <summary>
    /// Updates the vertical position of the Hips. TODO: move this to appropriate place
    /// </summary>
    internal void UpdateHipHeight(BodySubSegment vSegment)
    {
        //Update body height
        float vHipHeight = mHipHeight;

        if (mRightLegHeight > mLeftLegHeight)
        {
            vHipHeight = mRightLegHeight;
        }
        else
        {
            vHipHeight = mLeftLegHeight;
        }

        vSegment.UpdateSubsegmentPosition(Mathf.Lerp(mHipHeight, vHipHeight, InterpolationSpeed));
        mHipHeight = vHipHeight;
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

        vTorsoInitQuat = Quaternion.Euler(0, 0, vTorsoInitialRawEuler.y);
        Quaternion vTorsoQuatZ = Quaternion.Euler(0, 0, vTorsoCurrentRawEuler.y);
        vTorsoQuatZ = Quaternion.Inverse(vTorsoInitQuat) * vTorsoQuatZ;

        ////////////////////////////////////////////////////////  Apply Results To Torso /////////////////////////////////////////////////////////////////////
        Quaternion vTorsoQuat;
        Quaternion vHipQuat;
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
            vUpAngle = -EstimateHipsUpAngle(vLSSubsegment.AssociatedView.SubsegmentTransform, vUSSubsegment.AssociatedView.SubsegmentTransform,
                                           vRULSubSegment.AssociatedView.SubsegmentTransform, vLULSubSegment.AssociatedView.SubsegmentTransform,
                                           vRLLSubsegment.AssociatedView.SubsegmentTransform, vLLLSubsegment.AssociatedView.SubsegmentTransform);
        }

        if (IsUsingInterpolation)
        {
            vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(vUpAngle, 0, 0) * Quaternion.Euler(0, vForwardAngle, 0), InterpolationSpeed);
            vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation,  Quaternion.Inverse(vHipQuat) * vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
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

        //Update vertical position
        if (IsTrackingHeight)
        {
            UpdateHipHeight(vLSSubsegment);
        }//*/
    }

    internal float EstimateHipsForwardAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
    {
        //Estimate Hips forward orientation
        Vector3 vHipsForwardDirection = (Vector3.ProjectOnPlane(vUSTransform.forward, Vector3.up) + Vector3.ProjectOnPlane(vRULTransform.forward, Vector3.up) + 
                                         Vector3.ProjectOnPlane(vLULTransform.forward, Vector3.up) + Vector3.ProjectOnPlane(vRLLTransform.forward, Vector3.up) + 
                                         Vector3.ProjectOnPlane(vLLLTransform.forward, Vector3.up)) / 5;

        return SegmentAnalysis.GetSignedAngle(vHipsForwardDirection, Vector3.forward, Vector3.up);
    }

    internal float EstimateHipsUpAngle(Transform vLSTransform, Transform vUSTransform, Transform vRULTransform, Transform vLULTransform, Transform vRLLTransform, Transform vLLLTransform)
    {
        //Estimate Hips forward orientation
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
        vRightLegAnalysis.HipTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vRightLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
        vRightLegAnalysis.DeltaTime = DeltaTime;
        vRightLegAnalysis.AngleExtraction();
        mRightLegHeight = vRightLegAnalysis.LegHeight;
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
        MapLegsOrientations(vThighInitialRawEuler, vThighCurrentRawEuler, vKneeInitialRawEuler, vKneeCurrentRawEuler, vULSubsegment, vLLSubsegment, vHipsSubsegment);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.HipTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.DeltaTime = DeltaTime;
        vLeftLegAnalysis.AngleExtraction();
        mLeftLegHeight = vLeftLegAnalysis.LegHeight;
    }

    /// <summary>
    /// MapLegsOrientations: Updates the legs orientations from the initial and current eulerangles.
    /// </summary>
    public void MapLegsOrientations(Vector3 vULInitEuler, Vector3 vULCurEuler, Vector3 vLLInitEuler, Vector3 vLLCurEuler,
                                    BodySubSegment vULSubsegment, BodySubSegment vLLSubsegment, BodySubSegment vHipsSubsegment)
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
        Quaternion vThighQuat;
        Quaternion vKneeQuat;

        if (IsUsingInterpolation)
        {
            if (vHipsSubsegment != null)
            {
                vThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vThighQuatY * vThighQuatX * vThighQuatZ, InterpolationSpeed);
                vKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, Quaternion.Inverse(vThighQuat) * Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vKneeQuatY * vKneeQuatX * vKneeQuatZ, InterpolationSpeed);
            }
            else
            {
                vThighQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, vThighQuatY * vThighQuatX * vThighQuatZ, InterpolationSpeed);
                vKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, Quaternion.Inverse(vThighQuat) * vKneeQuatY * vKneeQuatX * vKneeQuatZ, InterpolationSpeed);
            }
        }
        else
        {
            if (vHipsSubsegment != null)
            {
                vThighQuat = Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vThighQuatY * vThighQuatX * vThighQuatZ;
                vKneeQuat = Quaternion.Inverse(vThighQuat) * Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vKneeQuatY * vKneeQuatX * vKneeQuatZ;
            }
            else
            {
                vThighQuat = vThighQuatY * vThighQuatX * vThighQuatZ;
                vKneeQuat = Quaternion.Inverse(vThighQuat) * vKneeQuatY * vKneeQuatX * vKneeQuatZ;
            }
        }
        
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeQuat, 0, true);
        vULSubsegment.UpdateSubsegmentOrientation(vThighQuat, 0, true);
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
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawEuler * 180f / Mathf.PI;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        vRightArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        Debug.Log(DeltaTime);
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
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawEuler  * 180f / Mathf.PI;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler  * 180f / Mathf.PI;
        MapArmsOrientations(vUpArmInitialRawEuler, vUpArmCurrentRawEuler, vLoArmInitialRawEuler, vLoArmCurrentRawEuler, vUASubsegment, vLASubsegment, vTorsoSubSegment, vHipsSubsegment);

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
    public void MapArmsOrientations(Vector3 vUAInitEuler, Vector3 vUACurEuler, Vector3 vLAInitEuler, Vector3 vLACurEuler, 
                                    BodySubSegment vUASubsegment, BodySubSegment vLASubsegment, BodySubSegment vTorsoSubSegment, BodySubSegment vHipsSubsegment)
    {
        //Upper arm
        Quaternion vUpArmInitQuat = Quaternion.Euler(0, -vUAInitEuler.z, 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, -vUACurEuler.z, 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatY;

        vUpArmInitQuat = Quaternion.Euler(vUAInitEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUACurEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatX;

        vUpArmInitQuat = Quaternion.Euler(0, 0, -vUAInitEuler.y);
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, -vUACurEuler.y);
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatZ;

        //Lower arm
        Quaternion vLoArmInitQuat = Quaternion.Euler(0, -vLAInitEuler.z, 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, -vLACurEuler.z, 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatY;

        vLoArmInitQuat = Quaternion.Euler(vLAInitEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLACurEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatX;

        vLoArmInitQuat = Quaternion.Euler(0, 0, -vLAInitEuler.y);
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, -vLACurEuler.y);
        vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatZ;

        //Apply results
        Quaternion vUpArmQuat;
        Quaternion vLoArmQuat;

        if (IsUsingInterpolation)
        {
            if (vTorsoSubSegment != null && vHipsSubsegment != null)
            {
                vUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) *
                                              Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ, InterpolationSpeed);
                if (IsAdjustingArms)
                {
                    vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuatX) * Quaternion.Inverse(vUpArmQuat) * Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) *
                                                  Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ, InterpolationSpeed);
                }
                else
                {
                    vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuat) * Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) *
                                                  Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ, InterpolationSpeed);
                }
            }
            else
            {
                vUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ, InterpolationSpeed);
                if (IsAdjustingArms)
                {
                    vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuatX) * Quaternion.Inverse(vUpArmQuat) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ, InterpolationSpeed);
                }
                else
                {
                    vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuat) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ, InterpolationSpeed);
                }
            }
        }
        else
        {
            if (vTorsoSubSegment != null && vHipsSubsegment != null)
            {
                vUpArmQuat = Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) * Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
                if (IsAdjustingArms)
                {
                    vLoArmQuat = Quaternion.Inverse(vUpArmQuatX) * Quaternion.Inverse(vUpArmQuat) * Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) * Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
                }
                else
                {
                    vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * Quaternion.Inverse(vTorsoSubSegment.SubsegmentOrientation) * Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
                }
            }
            else
            {
                vUpArmQuat = vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
                if (IsAdjustingArms)
                {
                    vLoArmQuat = Quaternion.Inverse(vUpArmQuatX) * Quaternion.Inverse(vUpArmQuat) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
                }
                else
                {
                    vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
                }
            }
        }

        vLASubsegment.UpdateSubsegmentOrientation(vLoArmQuat, 0, true);
        vUASubsegment.UpdateSubsegmentOrientation(vUpArmQuat, 0, true);
    }

    /// <summary>
    /// SensorsTiltCorrection: Adjusts for possible 7.6 degrees tilt in sensors (as per manufacturing process). (DEPRECATED)
    /// </summary>
    public float[,] SensorsTiltCorrection(float[,] B2)
    {
        float[,] B3 = new float[3, 3];
        float[,] B4 = new float[3, 3];

        Vector3 u1 = new Vector3(B2[0, 0], B2[1, 0], B2[2, 0]);
        //Vector3 u1 = new Vector3(1, 0, 0);

        float vAngleNew = (float)Math.PI * (7.6f / 180f);
        //float vAngleNew = (float)Math.PI * (0.0f / 180f);

        float[,] CurrentCompensation = new float[3, 3];
        CurrentCompensation = MatrixTools.RVector(u1.normalized, vAngleNew);

        B3 = MatrixTools.MultiplyMatrix(CurrentCompensation, B2);

        Vector3 roll = new Vector3(1, 0, 0);

        float[,] Compensation = new float[3, 3];
        Compensation = MatrixTools.RVector(roll.normalized, -vAngleNew);

        B4 = MatrixTools.MultiplyMatrix(B3, Compensation);

        return B4;
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
            MapTorsoSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
        {
            MapRightArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
        {
            MapLeftArmSubsegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso)
        {
            MapTorsoSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightLeg)
        {
            MapRightLegSegment(vFilteredDictionary);
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftLeg)
        {
            MapLeftLegSegment(vFilteredDictionary);
        }
    }
}