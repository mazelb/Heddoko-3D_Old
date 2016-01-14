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
    public bool IsTrackingHeight = true;
    public bool IsTrackingHipsY = false;
    public bool IsUsingInterpolation = true;
    public float InterpolationSpeed = 0.3f;

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
                        //if (vKey != (int)BodyStructureMap.SensorPositions.SP_LowerSpine)
                        {
                            BodySubSegmentsDictionary[vKey].ResetViewOrientation();
                        }
                    }
                }
            }
        }
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
        BodySubSegment vUSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment vLSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

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

        ////////////////////////////////////////////////////////  Apply Results /////////////////////////////////////////////////////////////////////
        Quaternion vTorsoQuat;
        Quaternion vHipQuat;

        if (IsUsingInterpolation)
        {
            vTorsoQuat = Quaternion.Slerp(vUSSubsegment.SubsegmentOrientation, vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ, InterpolationSpeed);
            vHipQuat = Quaternion.Slerp(vLSSubsegment.SubsegmentOrientation, Quaternion.Euler(0, vTorsoQuat.eulerAngles.y, 0), InterpolationSpeed);
        }
        else
        {
            vTorsoQuat = vTorsoQuatY * vTorsoQuatX * vTorsoQuatZ;
            vHipQuat = Quaternion.Euler(0, vTorsoQuat.eulerAngles.y, 0);
        }

        if (IsTrackingHipsY)
        {
            vLSSubsegment.UpdateSubsegmentOrientation(vHipQuat, 0, true);
            vUSSubsegment.UpdateSubsegmentOrientation(Quaternion.Inverse(vHipQuat) * vTorsoQuat, 0, true);
        }
        else
        {
            vUSSubsegment.UpdateSubsegmentOrientation(vTorsoQuat, 0, true);
        }

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        TorsoAnalysis vTorsoAnalysis = (TorsoAnalysis)mCurrentAnalysisSegment;
        vTorsoAnalysis.TorsoTransform = vUSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.HipGlobalTransform = vLSSubsegment.AssociatedView.SubsegmentTransform;
        vTorsoAnalysis.AngleExtraction();

        //Update vertical position
        if (IsTrackingHeight)
        {
            UpdateHipHeight(vLSSubsegment);
        }//*/
    }

    /// <summary>
    /// MapRightLegSegment: Performs mapping on the right leg subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapRightLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];
        BodySubSegment vHipsSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vHipInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vHipCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawEuler * 180f / Mathf.PI;

        //Upper Leg
        Quaternion vHipInitQuat = Quaternion.Euler(0, -vHipInitialRawEuler.z, 0);
        Quaternion vHipQuatY = Quaternion.Euler(0, -vHipCurrentRawEuler.z, 0);
        vHipQuatY = Quaternion.Inverse(vHipInitQuat) * vHipQuatY;

        vHipInitQuat = Quaternion.Euler(-vHipInitialRawEuler.x, 0, 0);
        Quaternion vHipQuatX = Quaternion.Euler(-vHipCurrentRawEuler.x, 0, 0);
        vHipQuatX = Quaternion.Inverse(vHipInitQuat) * vHipQuatX;

        vHipInitQuat = Quaternion.Euler(0, 0, vHipInitialRawEuler.y);
        Quaternion vHipQuatZ = Quaternion.Euler(0, 0, vHipCurrentRawEuler.y);
        vHipQuatZ = Quaternion.Inverse(vHipInitQuat) * vHipQuatZ;

        //Apply results
        Quaternion vHipQuat;

        if (IsUsingInterpolation)
        {
            vHipQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vHipQuatY * vHipQuatX * vHipQuatZ, InterpolationSpeed);
        }
        else
        {
            vHipQuat = Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vHipQuatY * vHipQuatX * vHipQuatZ;
        }

        vULSubsegment.UpdateSubsegmentOrientation(vHipQuat, 0, true);

        //Lower leg
        Quaternion vKneeInitQuat = Quaternion.Euler(0, -vKneeInitialRawEuler.z, 0);
        Quaternion vKneeQuatY = Quaternion.Euler(0, -vKneeCurrentRawEuler.z, 0);
        vKneeQuatY = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatY;

        vKneeInitQuat = Quaternion.Euler(-vKneeInitialRawEuler.x, 0, 0);
        Quaternion vKneeQuatX = Quaternion.Euler(-vKneeCurrentRawEuler.x, 0, 0);
        vKneeQuatX = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatX;

        vKneeInitQuat = Quaternion.Euler(0, 0, vKneeInitialRawEuler.y);
        Quaternion vKneeQuatZ = Quaternion.Euler(0, 0, vKneeCurrentRawEuler.y);
        vKneeQuatZ = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatZ;

        //Apply results
        Quaternion vKneeQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;

        if (IsUsingInterpolation)
        {
            vKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vKneeQuat, InterpolationSpeed);
        }
        else
        {
            vKneeQuat = Quaternion.Inverse(vHipQuat) * vKneeQuat;
        }
        
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeQuat, 0, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
        vRightLegAnalysis.HipTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vRightLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
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
        BodySubSegment vHipsSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vHipInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawEuler * 180f / Mathf.PI;
        Vector3 vHipCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawEuler * 180f / Mathf.PI;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawEuler * 180f / Mathf.PI;

        //Upper Leg
        Quaternion vHipInitQuat = Quaternion.Euler(0, -vHipInitialRawEuler.z, 0);
        Quaternion vHipQuatY = Quaternion.Euler(0, -vHipCurrentRawEuler.z, 0);
        vHipQuatY = Quaternion.Inverse(vHipInitQuat) * vHipQuatY;

        vHipInitQuat = Quaternion.Euler(-vHipInitialRawEuler.x, 0, 0);
        Quaternion vHipQuatX = Quaternion.Euler(-vHipCurrentRawEuler.x, 0, 0);
        vHipQuatX = Quaternion.Inverse(vHipInitQuat) * vHipQuatX;

        vHipInitQuat = Quaternion.Euler(0, 0, vHipInitialRawEuler.y);
        Quaternion vHipQuatZ = Quaternion.Euler(0, 0, vHipCurrentRawEuler.y);
        vHipQuatZ = Quaternion.Inverse(vHipInitQuat) * vHipQuatZ;

        //Apply results
        Quaternion vHipQuat;

        if (IsUsingInterpolation)
        {
            vHipQuat = Quaternion.Slerp(vULSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vHipQuatY * vHipQuatX * vHipQuatZ, InterpolationSpeed);
        }
        else
        {
            vHipQuat = Quaternion.Inverse(vHipsSubsegment.SubsegmentOrientation) * vHipQuatY * vHipQuatX * vHipQuatZ;
        }

        vULSubsegment.UpdateSubsegmentOrientation(vHipQuat, 0, true);

        //Lower leg
        Quaternion vKneeInitQuat = Quaternion.Euler(0, -vKneeInitialRawEuler.z, 0);
        Quaternion vKneeQuatY = Quaternion.Euler(0, -vKneeCurrentRawEuler.z, 0);
        vKneeQuatY = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatY;

        vKneeInitQuat = Quaternion.Euler(-vKneeInitialRawEuler.x, 0, 0);
        Quaternion vKneeQuatX = Quaternion.Euler(-vKneeCurrentRawEuler.x, 0, 0);
        vKneeQuatX = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatX;

        vKneeInitQuat = Quaternion.Euler(0, 0, vKneeInitialRawEuler.y);
        Quaternion vKneeQuatZ = Quaternion.Euler(0, 0, vKneeCurrentRawEuler.y);
        vKneeQuatZ = Quaternion.Inverse(vKneeInitQuat) * vKneeQuatZ;

        //Apply results
        Quaternion vKneeQuat = vKneeQuatY * vKneeQuatX * vKneeQuatZ;

        if (IsUsingInterpolation)
        {
            vKneeQuat = Quaternion.Slerp(vLLSubsegment.SubsegmentOrientation, Quaternion.Inverse(vHipQuat) * vKneeQuat, InterpolationSpeed);
        }
        else
        {
            vKneeQuat = Quaternion.Inverse(vHipQuat) * vKneeQuat;
        }

        vLLSubsegment.UpdateSubsegmentOrientation(vKneeQuat, 0, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.HipTransform = vULSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.KneeTransform = vLLSubsegment.AssociatedView.SubsegmentTransform;
        vLeftLegAnalysis.AngleExtraction();
        mLeftLegHeight = vLeftLegAnalysis.LegHeight;
    }

    /// <summary>
    /// MapRightArmSubsegment: Updates the right arm subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];
        BodySubSegment vTORSOSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].CurrRawEuler * 180f / Mathf.PI;

        //Upper arm
        Quaternion vUpArmInitQuat = Quaternion.Euler(0, -vUpArmInitialRawEuler.z, 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, -vUpArmCurrentRawEuler.z, 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatY;

        vUpArmInitQuat = Quaternion.Euler(vUpArmInitialRawEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUpArmCurrentRawEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatX;

        vUpArmInitQuat = Quaternion.Euler(0, 0, -vUpArmInitialRawEuler.y);
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, -vUpArmCurrentRawEuler.y);
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatZ;

        //Apply results
        Quaternion vUpArmQuat;

        if (IsUsingInterpolation)
        {
            vUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ, InterpolationSpeed);
        }
        else
        {
            vUpArmQuat = Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
        }

        vUASubsegment.UpdateSubsegmentOrientation(vUpArmQuat, 0, true);

        //Lower arm
        Quaternion vLoArmInitQuat = Quaternion.Euler(0, -vLoArmInitialRawEuler.z, 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, -vLoArmCurrentRawEuler.z, 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatY;

        vLoArmInitQuat = Quaternion.Euler(vLoArmInitialRawEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLoArmCurrentRawEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatX;

        vLoArmInitQuat = Quaternion.Euler(0, 0, -vLoArmInitialRawEuler.y);
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, -vLoArmCurrentRawEuler.y);
        vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatZ;

        //Apply results
        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;

        if (IsUsingInterpolation)
        {
            vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuat) * vLoArmQuat, InterpolationSpeed);
        }
        else
        {
            vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * vLoArmQuat;
        }
        
        vLASubsegment.UpdateSubsegmentOrientation(vLoArmQuat, 0, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;
        vRightArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vRightArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
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
        BodySubSegment vTORSOSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];

        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vUpArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitRawEuler * 180f / Mathf.PI;
        Vector3 vUpArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].CurrRawEuler * 180f / Mathf.PI;
        Vector3 vLoArmInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitRawEuler  * 180f / Mathf.PI;
        Vector3 vLoArmCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].CurrRawEuler  * 180f / Mathf.PI;

        //Upper arm
        Quaternion vUpArmInitQuat = Quaternion.Euler(0, -vUpArmInitialRawEuler.z, 0);
        Quaternion vUpArmQuatY = Quaternion.Euler(0, -vUpArmCurrentRawEuler.z, 0);
        vUpArmQuatY = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatY;

        vUpArmInitQuat = Quaternion.Euler(vUpArmInitialRawEuler.x, 0, 0);
        Quaternion vUpArmQuatX = Quaternion.Euler(vUpArmCurrentRawEuler.x, 0, 0);
        vUpArmQuatX = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatX;

        vUpArmInitQuat = Quaternion.Euler(0, 0, -vUpArmInitialRawEuler.y);
        Quaternion vUpArmQuatZ = Quaternion.Euler(0, 0, -vUpArmCurrentRawEuler.y);
        vUpArmQuatZ = Quaternion.Inverse(vUpArmInitQuat) * vUpArmQuatZ;

        //Apply results
        Quaternion vUpArmQuat;

        if (IsUsingInterpolation)
        {
            vUpArmQuat = Quaternion.Slerp(vUASubsegment.SubsegmentOrientation, Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ, InterpolationSpeed);
        }
        else
        {
            vUpArmQuat = Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
        }

        vUASubsegment.UpdateSubsegmentOrientation(vUpArmQuat, 0, true);

        //Lower arm
        Quaternion vLoArmInitQuat = Quaternion.Euler(0, -vLoArmInitialRawEuler.z, 0);
        Quaternion vLoArmQuatY = Quaternion.Euler(0, -vLoArmCurrentRawEuler.z, 0);
        vLoArmQuatY = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatY;

        vLoArmInitQuat = Quaternion.Euler(vLoArmInitialRawEuler.x, 0, 0);
        Quaternion vLoArmQuatX = Quaternion.Euler(vLoArmCurrentRawEuler.x, 0, 0);
        vLoArmQuatX = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatX;

        vLoArmInitQuat = Quaternion.Euler(0, 0, -vLoArmInitialRawEuler.y);
        Quaternion vLoArmQuatZ = Quaternion.Euler(0, 0, -vLoArmCurrentRawEuler.y);
        vLoArmQuatZ = Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuatZ;

        //Apply results
        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;

        if (IsUsingInterpolation)
        {
            vLoArmQuat = Quaternion.Slerp(vLASubsegment.SubsegmentOrientation, Quaternion.Inverse(vUpArmQuat) * vLoArmQuat, InterpolationSpeed);
        }
        else
        {
            vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * vLoArmQuat;
        }

        vLASubsegment.UpdateSubsegmentOrientation(vLoArmQuat, 0, true);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis)mCurrentAnalysisSegment;
        vLeftArmAnalysis.UpArTransform = vUASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.LoArTransform = vLASubsegment.AssociatedView.SubsegmentTransform;
        vLeftArmAnalysis.AngleExtraction();//*/
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