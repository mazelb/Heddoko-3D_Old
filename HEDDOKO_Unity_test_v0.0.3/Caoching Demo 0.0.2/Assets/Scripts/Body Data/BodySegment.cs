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

/**
* BodySegment class 
* @brief BodySegment class (represents one abstracted reprensentation of a body segment)
*/
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

    //Sensor data tuples
    private List<SensorTuple> SensorsTuple = new List<SensorTuple>();

    //Associated view for the segment
    public BodySegmentView AssociatedView;

    //Analysis pipeline of the segment data
    public SegmentAnalysis mCurrentAnalysisSegment;

    //TODO: extract this where appropriate
    //Detection of vertical Hip position
    private static float mInitThighHeight = 0.42f;
    private static float mInitTibiaHeight = 0.39f;
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

    /**
    * UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary)
    * @param UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary): A filtered list of transformation matrices. 
    * @brief  Depending on the segment type, apply transformation matrices 
    */
    internal void UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vFilteredDictionary)
    {
        MapSubSegments(vFilteredDictionary);
    }

    /**
    * UpdateInitialSensorsData(BodyFrame vFrame)
    * @param BodyFrame vFrame: the body frame whose subframes will updates to initial sensors
    * @brief  The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the initial sensor's information
    */
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
                        if (vKey != (int)BodyStructureMap.SensorPositions.SP_LowerSpine)
                        {
                            BodySubSegmentsDictionary[vKey].ResetViewOrientation();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// MapTorsoSegment: Performs mapping on the torso subsegment from the available sensor data.
    /// </summary>
    /// <param name="vTransformatricies">transformation matrices mapped to sensor positions.</param>
    internal void MapTorsoSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vUSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment vLSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        //TODO: keeping this ONLY for the analysis part... awaiting changing it

        Vector3 vTorsoInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitRawEuler;
        Vector3 vTorsoCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].CurrRawEuler;

        Vector3 vTorsoInitRawEuler = new Vector3(vTorsoInitialRawEuler.x, vTorsoInitialRawEuler.y, vTorsoInitialRawEuler.z);
        Vector3 vTorsoCurrRawEuler = new Vector3(vTorsoCurrentRawEuler.x, vTorsoCurrentRawEuler.y, vTorsoCurrentRawEuler.z);

        float[,] vTorsoInitGlobalMatrix = MatrixTools.RotationLocal(vTorsoInitRawEuler.z, vTorsoInitRawEuler.x, vTorsoInitRawEuler.y);
        float[,] vTorsoCurrentLocalMatrix = MatrixTools.RotationLocal(vTorsoCurrRawEuler.z, vTorsoCurrRawEuler.x, vTorsoCurrRawEuler.y);
        float[,] vTorsoOrientationMatrix = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTorsoInitGlobalMatrix), vTorsoCurrentLocalMatrix);
        
        //Convert to something that unity can understand
        IMUQuaternionOrientation vTorsoQuaternion = MatrixTools.MatToQuat(vTorsoOrientationMatrix);
        Quaternion vTorsoQuat = new Quaternion(vTorsoQuaternion.x, vTorsoQuaternion.y, vTorsoQuaternion.z, vTorsoQuaternion.w);
        Vector3 vTorsoEulers = vTorsoQuat.eulerAngles;
        vTorsoQuat = Quaternion.Euler(0, 0, vTorsoEulers.x) * Quaternion.Euler(0, vTorsoEulers.y, 0) * Quaternion.Euler(-vTorsoEulers.z, 0, 0);
        Quaternion vHipQuat = Quaternion.Euler(0, vTorsoEulers.y, 0);
        
        //vUSSubsegment.UpdateSubsegmentOrientation(vTorsoQuat, 0, true);
        //vLSSubsegment.UpdateSubsegmentOrientation(vHipQuat, 0, true);

        //Update vertical position
        if (IsTrackingHeight)
        {
            UpdateHipHeight(vLSSubsegment);
        }//*/
    }

    /// <summary>
    /// Updates the vertical position of the Hips. TODO: move this to appropriate place
    /// </summary>
    internal void UpdateHipHeight(BodySubSegment vSegment)
    {
        //Update body height
        float vHipHeight;

        if (mRightLegHeight > mLeftLegHeight)
        {
            vHipHeight = mRightLegHeight;
        }
        else
        {
            vHipHeight = mLeftLegHeight;
        }

        vSegment.UpdateSubsegmentPosition(Mathf.Lerp(mHipHeight, vHipHeight, Time.time));
        mHipHeight = vHipHeight;
    }

    /**
    * MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the left leg subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapRightLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vHipInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitRawEuler;
        Vector3 vHipCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].CurrRawEuler;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitRawEuler;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].CurrRawEuler;

        Vector3 vHipInitRawEuler = new Vector3(vHipInitialRawEuler.x, vHipInitialRawEuler.y, vHipInitialRawEuler.z);
        Vector3 vHipCurrRawEuler = new Vector3(vHipCurrentRawEuler.x, vHipCurrentRawEuler.y, vHipCurrentRawEuler.z);

        Vector3 vKneeInitRawEuler = new Vector3(vKneeInitialRawEuler.x, vKneeInitialRawEuler.y, vKneeInitialRawEuler.z);
        Vector3 vKneeCurrRawEuler = new Vector3(vKneeCurrentRawEuler.x, vKneeCurrentRawEuler.y, vKneeCurrentRawEuler.z);

        float[,] vHipInitGlobalMatrix = MatrixTools.RotationLocal(vHipInitRawEuler.z, vHipInitRawEuler.x, vHipInitRawEuler.y);
        float[,] vHipCurrentLocalMatrix = MatrixTools.RotationLocal(vHipCurrRawEuler.z, vHipCurrRawEuler.x, vHipCurrRawEuler.y);
        float[,] vHipOrientationMatrix = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vHipInitGlobalMatrix), vHipCurrentLocalMatrix);

        float[,] vKneeInitGlobalMatrix = MatrixTools.RotationLocal(vKneeInitRawEuler.z, vKneeInitRawEuler.x, vKneeInitRawEuler.y);
        float[,] vKneeCurrentLocalMatrix = MatrixTools.RotationLocal(vKneeCurrRawEuler.z, vKneeCurrRawEuler.x, vKneeCurrRawEuler.y);
        float[,] vKneeOrientationMatrix = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vKneeInitGlobalMatrix), vKneeCurrentLocalMatrix);
 
        //Convert to something that unity can understand
        IMUQuaternionOrientation vHipQuaternion = MatrixTools.MatToQuat(vHipOrientationMatrix);
        Quaternion vHipQuat = new Quaternion(vHipQuaternion.x, vHipQuaternion.y, vHipQuaternion.z, vHipQuaternion.w);
        Vector3 vHipEulers = vHipQuat.eulerAngles;
        vHipQuat = Quaternion.Euler(0, 0, vHipEulers.x) * Quaternion.Euler(0, -vHipEulers.y, 0) * Quaternion.Euler(vHipEulers.z, 0, 0);

        IMUQuaternionOrientation vKneeQuaternion = MatrixTools.MatToQuat(vKneeOrientationMatrix);
        Quaternion vKneeQuat = new Quaternion(vKneeQuaternion.x, vKneeQuaternion.y, vKneeQuaternion.z, vKneeQuaternion.w);
        Vector3 vKneeEulers = vKneeQuat.eulerAngles;
        vKneeQuat = Quaternion.Euler(0, 0, vKneeEulers.x) * Quaternion.Euler(0, -vHipEulers.y, 0) * Quaternion.Euler(vKneeEulers.z, 0, 0);

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipQuat, 1);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeQuat, 1);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
        vRightLegAnalysis.HipOrientation = vHipOrientationMatrix;
        vRightLegAnalysis.KneeOrientation = vKneeOrientationMatrix;
        vRightLegAnalysis.AngleExtraction();

        //Update Leg height
        Vector3 vThighVec = new Vector3(vHipOrientationMatrix[0, 1], vHipOrientationMatrix[1, 1], vHipOrientationMatrix[2, 1]);
        vThighVec.Normalize();
        Vector3 vTibiaVec = new Vector3(vKneeOrientationMatrix[0, 1], vKneeOrientationMatrix[1, 1], vKneeOrientationMatrix[2, 1]);
        vTibiaVec.Normalize();
        float vThighHeight = mInitThighHeight * Vector3.Dot(vThighVec, new Vector3(0, 1, 0));
        float vTibiaHeight = mInitTibiaHeight/* * Vector3.Dot(vTibiaVec, vThighVec)*/;
        mRightLegHeight = vThighHeight + vTibiaHeight;
    }

    /**
    * MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the left leg subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, BodyStructureMap.TrackingStructure> vTransformatricies)
    {
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];

        float[,] vHipOrientation = new float[3, 3];
        float[,] vKneeOrientation = new float[3, 3];

        ////////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        Vector3 vHipInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitRawEuler;
        Vector3 vHipCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].CurrRawEuler;
        Vector3 vKneeInitialRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitRawEuler;
        Vector3 vKneeCurrentRawEuler = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].CurrRawEuler;

        Vector3 vHipInitRawEuler = new Vector3(vHipInitialRawEuler.x, vHipInitialRawEuler.y, vHipInitialRawEuler.z);
        Vector3 vHipCurrRawEuler = new Vector3(vHipCurrentRawEuler.x, vHipCurrentRawEuler.y, vHipCurrentRawEuler.z);

        Vector3 vKneeInitRawEuler = new Vector3(vKneeInitialRawEuler.x, vKneeInitialRawEuler.y, vKneeInitialRawEuler.z);
        Vector3 vKneeCurrRawEuler = new Vector3(vKneeCurrentRawEuler.x, vKneeCurrentRawEuler.y, vKneeCurrentRawEuler.z);

        float[,] vHipInitGlobalMatrix = MatrixTools.RotationLocal(vHipInitRawEuler.z, vHipInitRawEuler.x, vHipInitRawEuler.y);
        float[,] vHipCurrentLocalMatrix = MatrixTools.RotationLocal(vHipCurrRawEuler.z, vHipCurrRawEuler.x, vHipCurrRawEuler.y);
        float[,] vHipOrientationMatrix = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vHipInitGlobalMatrix), vHipCurrentLocalMatrix);

        float[,] vKneeInitGlobalMatrix = MatrixTools.RotationLocal(vKneeInitRawEuler.z, vKneeInitRawEuler.x, vKneeInitRawEuler.y);
        float[,] vKneeCurrentLocalMatrix = MatrixTools.RotationLocal(vKneeCurrRawEuler.z, vKneeCurrRawEuler.x, vKneeCurrRawEuler.y);
        float[,] vKneeOrientationMatrix = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vKneeInitGlobalMatrix), vKneeCurrentLocalMatrix);

        vHipOrientation = (vHipOrientationMatrix);
        vKneeOrientation = (vKneeOrientationMatrix);//*/

        //Convert to something that unity can understand
        IMUQuaternionOrientation vHipQuaternion = MatrixTools.MatToQuat(vHipOrientation);
        Quaternion vHipQuat = new Quaternion(vHipQuaternion.x, vHipQuaternion.y, vHipQuaternion.z, vHipQuaternion.w);
        Vector3 vHipEulers = vHipQuat.eulerAngles;
        vHipQuat = Quaternion.Euler(vHipEulers.z, 0, 0) * Quaternion.Euler(0, -vHipEulers.y, 0) * Quaternion.Euler(0, 0, vHipEulers.x);

        IMUQuaternionOrientation vKneeQuaternion = MatrixTools.MatToQuat(vKneeOrientation);
        Quaternion vKneeQuat = new Quaternion(vKneeQuaternion.x, vKneeQuaternion.y, vKneeQuaternion.z, vKneeQuaternion.w);
        Vector3 vKneeEulers = vKneeQuat.eulerAngles;
        vKneeQuat = Quaternion.Euler(vKneeEulers.z, 0, 0) * Quaternion.Euler(0, -vHipEulers.y, 0) * Quaternion.Euler(0, 0, vKneeEulers.x);

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipQuat, 1);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeQuat, 1);

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.HipOrientation = vHipOrientation;
        vLeftLegAnalysis.KneeOrientation = vKneeOrientation;
        vLeftLegAnalysis.AngleExtraction();

        //Update leg height 
        Vector3 vThighVec = new Vector3(vHipOrientation[0, 1], vHipOrientation[1, 1], vHipOrientation[2, 1]);
        vThighVec.Normalize();
        Vector3 vTibiaVec = new Vector3(vKneeOrientation[0, 1], vKneeOrientation[1, 1], vKneeOrientation[2, 1]);
        vTibiaVec.Normalize();
        float vThighHeight = mInitThighHeight * Vector3.Dot(vThighVec, new Vector3(0, 1, 0));
        float vTibiaHeight = mInitTibiaHeight/* * Vector3.Dot(vTibiaVec, vThighVec)*/;
        mLeftLegHeight = vThighHeight + vTibiaHeight;
    }

    /**
    * MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Updates the right arm subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
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
       
        Quaternion vUpArmQuat = Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
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

        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
        vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * vLoArmQuat;
        vLASubsegment.UpdateSubsegmentOrientation(vLoArmQuat, 0, true);

        //Vector3 vLoArmInitRawEuler = new Vector3(0, -vLoArmInitialRawEuler.z, 0);
        //Vector3 vLoArmCurrRawEuler = new Vector3(0, -vLoArmCurrentRawEuler.z, 0);
        //Quaternion vLoArmInitQuat = Quaternion.Euler(vLoArmInitRawEuler);
        //Quaternion vLoArmQuat = Quaternion.Euler(vLoArmCurrRawEuler);
        //vLoArmQuat = Quaternion.Inverse(Quaternion.Euler(0, vUpArmQuat.eulerAngles.y, 0)) * Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuat;

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        /*RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;
        vRightArmAnalysis.LoArOrientation = vLoArOrientation;
        vRightArmAnalysis.UpArOrientation = vUpArOrientation;
        vRightArmAnalysis.AngleExtraction();//*/
    }

    /**
    * MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the left arm subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor position
    */
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

        Quaternion vUpArmQuat = Quaternion.Inverse(vTORSOSubsegment.SubsegmentOrientation) * vUpArmQuatY * vUpArmQuatX * vUpArmQuatZ;
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

        Quaternion vLoArmQuat = vLoArmQuatY * vLoArmQuatX * vLoArmQuatZ;
        vLoArmQuat = Quaternion.Inverse(vUpArmQuat) * vLoArmQuat;
        vLASubsegment.UpdateSubsegmentOrientation(vLoArmQuat, 0, true);

        //Vector3 vLoArmInitRawEuler = new Vector3(0, -vLoArmInitialRawEuler.z, 0);
        //Vector3 vLoArmCurrRawEuler = new Vector3(0, -vLoArmCurrentRawEuler.z, 0);
        //Quaternion vLoArmInitQuat = Quaternion.Euler(vLoArmInitRawEuler);
        //Quaternion vLoArmQuat = Quaternion.Euler(vLoArmCurrRawEuler);
        //vLoArmQuat = Quaternion.Inverse(Quaternion.Euler(0, vUpArmQuat.eulerAngles.y, 0)) * Quaternion.Inverse(vLoArmInitQuat) * vLoArmQuat;

        ////////////////////////////////////////////////////////  Analysis /////////////////////////////////////////////////////////////////////
        /*RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;
        vRightArmAnalysis.LoArOrientation = vLoArOrientation;
        vRightArmAnalysis.UpArOrientation = vUpArOrientation;
        vRightArmAnalysis.AngleExtraction();//*/
    }

    /**
    *  SensorsTiltCorrection()
    * @param  
    * @brief 
    */
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

    /**
    *  InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    * @param  vSegmentType: the desired Segment Type
    * @brief Initializes a new body structure's internal properties with the desired Segment Type
    */
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

    /**
    * private void MapSubSegments(Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary)
    * @brief Perform mapping on the current segments and its respective subsegments 
    * @param 
    */
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