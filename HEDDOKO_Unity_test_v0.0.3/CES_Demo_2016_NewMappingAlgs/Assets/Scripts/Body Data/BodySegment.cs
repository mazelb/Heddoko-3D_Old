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

        float[,] vTrackedTorsoOrientation = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].OrientationMatrix);
        float[,] vTrackedSpineOrientation = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_LowerSpine].OrientationMatrix);
        
        float[,] vaFinalTorsoOrientation = new float[3, 3];
        float[,] vTorsoTemp1 = new float[3, 3];
        float[,] vTempOrientation = new float[3, 3];

        //Map the sensors orientation to the body T-POSE orientation
        Vector3 u = new Vector3(0, 1, 0);
        vTempOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vTorsoTemp1 = MatrixTools.MultiplyMatrix(vTempOrientation, vTrackedTorsoOrientation);

        u.Set(0, 1, 0);
        vTempOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vaFinalTorsoOrientation = MatrixTools.MultiplyMatrix(vTorsoTemp1, vTempOrientation);

        //Update the analysis inputs
        TorsoAnalysis vTorsoAnalysis = (TorsoAnalysis)mCurrentAnalysisSegment;
        vTorsoAnalysis.TorsoOrientation = vaFinalTorsoOrientation;
        vTorsoAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vUSSubsegment.UpdateSubsegmentOrientation(vaFinalTorsoOrientation);
        //vLSSubsegment.UpdateSubsegmentOrientation(vaTorsoOrientation);

        //Update vertical position
        if(IsTrackingHeight)
        {
            UpdateHipHeight(vLSSubsegment);
        }
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
        //UL : Upper Leg
        //LL : Lower Leg
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];

        float[,] vHipOrientation = new float[3, 3];
        float[,] vKneeOrientation = new float[3, 3];
        float[,] vCurrentOrientation = new float[3, 3];

        //Intermediate arrays until achieve final orientation for hip and knee, they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
        float[,] vHipB2 = new float[3, 3];
        float[,] vHipB2T = new float[3, 3];
        float[,] vHipB5 = new float[3, 3];
        float[,] vHipB6 = new float[3, 3];
        float[,] vHipB7 = new float[3, 3];

        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3D = new float[3, 3];
        float[,] vKneeB3ID = new float[3, 3];
        float[,] vKneeB3DT = new float[3, 3];
        float[,] vKneeB3IDT = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];

        /////////// Initial Frame Adjustments /////////////////// 
        vHipB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].OrientationMatrix);
        vKneeB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].OrientationMatrix);

        vHipB2T = MatrixTools.MatrixTranspose(vHipB2);
        vKneeB3D = MatrixTools.MultiplyMatrix(vHipB2T, vKneeB2);
        vKneeB3DT = MatrixTools.MatrixTranspose(vKneeB3D);
        vKneeB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf].InitGlobalMatrix);
        vKneeB3IDT = MatrixTools.MatrixTranspose(vKneeB3ID);
        vKneeB4 = MatrixTools.MultiplyMatrix(vKneeB3IDT, MatrixTools.MultiplyMatrix(vKneeB3ID, vKneeB3D));

        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

        ////////////////// setting Hip to Final Body orientation ///////////////////////////////

        Vector3 u = new Vector3(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vHipB5 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vHipB2);

        u.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI);
        vHipB6 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vHipB5);

        u.Set(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vHipB7 = MatrixTools.MultiplyMatrix(vHipB6, vCurrentOrientation);

        u.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI);
        vHipOrientation = MatrixTools.MultiplyMatrix(vHipB7, vCurrentOrientation);

        ////////////////// setting Knee to Final Body orientation ///////////////////////////////

        Vector3 u2 = new Vector3(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vKneeB5 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vKneeB4);

        u2.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI);
        vKneeB6 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vKneeB5);

        u2.Set(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vKneeB7 = MatrixTools.MultiplyMatrix(vKneeB6, vCurrentOrientation);

        u2.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI);
        vKneeOrientation = MatrixTools.MultiplyMatrix(vKneeB7, vCurrentOrientation);

        //Update the analysis inputs
        RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
        vRightLegAnalysis.HipOrientation = vHipOrientation;
        vRightLegAnalysis.KneeOrientation = vKneeOrientation;
        vRightLegAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);

        //Update Leg height
        Vector3 vThighVec = new Vector3(vHipOrientation[0, 1], vHipOrientation[1, 1], vHipOrientation[2, 1]);
        vThighVec.Normalize();
        Vector3 vTibiaVec = new Vector3(vKneeOrientation[0, 1], vKneeOrientation[1, 1], vKneeOrientation[2, 1]);
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
        //UL: upper leg
        //LL: lower leg 
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];

        float[,] vHipOrientation = new float[3, 3];
        float[,] vKneeOrientation = new float[3, 3];
        float[,] vCurrentOrientation = new float[3, 3];

        //Intermediate arrays until achieve final orientation for hip and knee, they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
        float[,] vHipB2 = new float[3, 3];
        float[,] vHipB2T = new float[3, 3];
        float[,] vHipB5 = new float[3, 3];
        float[,] vHipB6 = new float[3, 3];
        float[,] vHipB7 = new float[3, 3];

        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3D = new float[3, 3];
        float[,] vKneeB3ID = new float[3, 3];
        float[,] vKneeB3DT = new float[3, 3];
        float[,] vKneeB3IDT = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];

        /////////// Initial Frame Adjustments /////////////////// 
        vHipB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].OrientationMatrix);
        vKneeB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].OrientationMatrix);

        vHipB2T = MatrixTools.MatrixTranspose(vHipB2);
        vKneeB3D = MatrixTools.MultiplyMatrix(vHipB2T, vKneeB2);
        vKneeB3DT = MatrixTools.MatrixTranspose(vKneeB3D);

        vKneeB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf].InitGlobalMatrix);
        vKneeB3IDT = MatrixTools.MatrixTranspose(vKneeB3ID);

        vKneeB4 = MatrixTools.MultiplyMatrix(vKneeB3IDT, MatrixTools.MultiplyMatrix(vKneeB3ID, vKneeB3D));

        ///////////////////////////////////////
        /////////// Mapping ///////////////////
        /////////////////////////////////////// 

        ////////////////// setting Hip to Final Body orientation ///////////////////////////////

        Vector3 u = new Vector3(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vHipB5 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vHipB2);

        u.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI);
        vHipB6 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vHipB5);

        u.Set(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vHipB7 = MatrixTools.MultiplyMatrix(vHipB6, vCurrentOrientation);

        u.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI);
        vHipOrientation = MatrixTools.MultiplyMatrix(vHipB7, vCurrentOrientation);

        ////////////////// setting Knee to Final Body orientation ///////////////////////////////

        Vector3 u2 = new Vector3(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vKneeB5 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vKneeB4);

        u2.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI);
        vKneeB6 = MatrixTools.MultiplyMatrix(vCurrentOrientation, vKneeB5);

        u2.Set(0, 1, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vKneeB7 = MatrixTools.MultiplyMatrix(vKneeB6, vCurrentOrientation);

        u2.Set(1, 0, 0);
        vCurrentOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI);
        vKneeOrientation = MatrixTools.MultiplyMatrix(vKneeB7, vCurrentOrientation);

        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.HipOrientation = vHipOrientation;
        vLeftLegAnalysis.KneeOrientation = vKneeOrientation;
        vLeftLegAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);

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
        //current Upperarm (shoulder) and lower arm (Elbow) joints orientation, 
        //UpAr stands for upper arm and LoAr stands for Lower arm (forearm) in this code
        float[,] vUpArOrientation = new float[3, 3];
        float[,] vLoArOrientation = new float[3, 3];

        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];

        float[,] vTorsoB2 = new float[3, 3];
        float[,] vTorsoB2T = new float[3, 3];

        float[,] vUpArB2 = new float[3, 3];
        float[,] vUpArB2T = new float[3, 3];
        float[,] vUpArB3D = new float[3, 3];
        float[,] vUpArB3ID = new float[3, 3];
        float[,] vUpArB3DT = new float[3, 3];
        float[,] vUpArB3IDT = new float[3, 3];
        float[,] vUpArB4 = new float[3, 3];
        float[,] vUpArB5 = new float[3, 3];
        float[,] vUpArB7 = new float[3, 3];

        float[,] vLoArB2 = new float[3, 3];
        float[,] vLoArB3D = new float[3, 3];
        float[,] vLoArB3ID = new float[3, 3];
        float[,] vLoArB3DT = new float[3, 3];
        float[,] vLoArB3IDT = new float[3, 3];
        float[,] vLoArB4 = new float[3, 3];
        float[,] vLoArB5 = new float[3, 3];
        float[,] vLoArB6 = new float[3, 3];
        float[,] vLoArB8 = new float[3, 3];

        float[,] vCurrentArOrientation = new float[3, 3];

        vUpArB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].OrientationMatrix);
        vLoArB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].OrientationMatrix);
        vTorsoB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].OrientationMatrix);

        //vUpArB2 = GravityRefArm(UpArB22, vNodInitAcc0);
        //vLoArB2 = GravityRefArm(LoArB22, vNodInitAcc1);

        //Upper arm transforms

        vTorsoB2T = MatrixTools.MatrixTranspose(vTorsoB2);
        vUpArB3D = MatrixTools.MultiplyMatrix(vTorsoB2T, vUpArB2);
        vUpArB3DT = MatrixTools.MatrixTranspose(vUpArB3D);
        vUpArB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitGlobalMatrix);
        vUpArB3IDT = MatrixTools.MatrixTranspose(vUpArB3ID);
        vUpArB2 = MatrixTools.MultiplyMatrix(vUpArB3IDT, MatrixTools.MultiplyMatrix(vUpArB3ID, vUpArB3D));

        //lower arm transforms
        vUpArB2T = MatrixTools.MatrixTranspose(vUpArB2);
        vLoArB3D = MatrixTools.MultiplyMatrix(vUpArB2T, vLoArB2);
        vLoArB3DT = MatrixTools.MatrixTranspose(vLoArB3D);
        vLoArB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm].InitGlobalMatrix);
        vLoArB3IDT = MatrixTools.MatrixTranspose(vLoArB3ID);

        vLoArB4 = MatrixTools.MultiplyMatrix(vLoArB3IDT, MatrixTools.MultiplyMatrix(vLoArB3ID, vLoArB3D));

        ////////////////// setting to Final Body orientation lower arm ///////////////////////////////
        Vector3 u = new Vector3(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vLoArB5 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vLoArB4);

        u.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vLoArB6 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vLoArB5);

        u.Set(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vLoArB8 = MatrixTools.MultiplyMatrix(vLoArB6, vCurrentArOrientation);

        u.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vLoArOrientation = MatrixTools.MultiplyMatrix(vLoArB8, vCurrentArOrientation);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vUpArB4 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vUpArB2);

        u2.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vUpArB5 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vUpArB4);

        u2.Set(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vUpArB7 = MatrixTools.MultiplyMatrix(vUpArB5, vCurrentArOrientation);

        u2.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vUpArOrientation = MatrixTools.MultiplyMatrix(vUpArB7, vCurrentArOrientation);//*/

        RightArmAnalysis vRightArmAnalysis = (RightArmAnalysis)mCurrentAnalysisSegment;
        vRightArmAnalysis.LoArOrientation = vLoArOrientation;
        vRightArmAnalysis.UpArOrientation = vUpArOrientation;
        vRightArmAnalysis.AngleExtraction();

        vUASubsegment.UpdateSubsegmentOrientation(vUpArOrientation);
        vLASubsegment.UpdateSubsegmentOrientation(vLoArOrientation);
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

        //Intermediate arrays until achieve final orientation for shoulder and elbow
        //they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
        //UpAr stands for upper arm sensor orientation and lower arm stands for lower arm (forearm) orientation
        float[,] vUpArOrientation = new float[3, 3];
        float[,] vLoArOrientation = new float[3, 3];

        float[,] vTorsoB2 = new float[3, 3];
        float[,] vTorsoB2T = new float[3, 3];

        float[,] vUpArB2 = new float[3, 3];
        float[,] vUpArB2T = new float[3, 3];
        float[,] vUpArB3D = new float[3, 3];
        float[,] vUpArB3ID = new float[3, 3];
        float[,] vUpArB3DT = new float[3, 3];
        float[,] vUpArB3IDT = new float[3, 3];
        float[,] vUpArB4 = new float[3, 3];
        float[,] vUpArB5 = new float[3, 3];
        float[,] vUpArB6 = new float[3, 3];

        float[,] vLoArB2 = new float[3, 3];
        float[,] vLoArB3D = new float[3, 3];
        float[,] vLoArB3ID = new float[3, 3];
        float[,] vLoArB3DT = new float[3, 3];
        float[,] vLoArB3IDT = new float[3, 3];
        float[,] vLoArB4 = new float[3, 3];
        float[,] vLoArB5 = new float[3, 3];
        float[,] vLoArB6 = new float[3, 3];
        float[,] vLoArB7 = new float[3, 3];

        float[,] vCurrentArOrientation = new float[3, 3];

        /////////// Initial Frame Adjustments ///////////////////
        vUpArB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].OrientationMatrix);
        vLoArB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].OrientationMatrix);
        vTorsoB2 = SensorsTiltCorrection(vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].OrientationMatrix);

        //vUpArB2 = vUpArB22;// GravityRefArm(UpArB22, vNodInitAcc0);
        //vLoArB2 = vLoArB22;// GravityRefArm(LoArB22, vNodInitAcc1);

        vTorsoB2T = MatrixTools.MatrixTranspose(vTorsoB2);
        vUpArB3D = MatrixTools.MultiplyMatrix(vTorsoB2T, vUpArB2);
        vUpArB3DT = MatrixTools.MatrixTranspose(vUpArB3D);
        vUpArB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitGlobalMatrix);
        vUpArB3IDT = MatrixTools.MatrixTranspose(vUpArB3ID);
        vUpArB2 = MatrixTools.MultiplyMatrix(vUpArB3IDT, MatrixTools.MultiplyMatrix(vUpArB3ID, vUpArB3D));

        vUpArB2T = MatrixTools.MatrixTranspose(vUpArB2);
        vLoArB3D = MatrixTools.MultiplyMatrix(vUpArB2T, vLoArB2);
        vLoArB3DT = MatrixTools.MatrixTranspose(vLoArB3D);

        vLoArB3ID = MatrixTools.MultiplyMatrix(MatrixTools.MatrixTranspose(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm].InitGlobalMatrix), 
                                                    vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm].InitGlobalMatrix);
        vLoArB3IDT = MatrixTools.MatrixTranspose(vLoArB3ID);

        vLoArB4 = MatrixTools.MultiplyMatrix(vLoArB3IDT, MatrixTools.MultiplyMatrix(vLoArB3ID, vLoArB3D));

        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

        ////////////////// setting to Final Body orientation lower arm///////////////////////////////
        Vector3 u = new Vector3(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vLoArB5 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vLoArB4);

        u.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vLoArB6 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vLoArB5);

        u.Set(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, (float)Math.PI / 2);
        vLoArB7 = MatrixTools.MultiplyMatrix(vLoArB6, vCurrentArOrientation);

        u.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u.normalized, -(float)Math.PI / 2);
        vLoArOrientation = MatrixTools.MultiplyMatrix(vLoArB7, vCurrentArOrientation);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vUpArB4 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vUpArB2);

        u2.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vUpArB5 = MatrixTools.MultiplyMatrix(vCurrentArOrientation, vUpArB4);

        u2.Set(1, 0, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, (float)Math.PI / 2);
        vUpArB6 = MatrixTools.MultiplyMatrix(vUpArB5, vCurrentArOrientation);

        u2.Set(0, 1, 0);
        vCurrentArOrientation = MatrixTools.RVector(u2.normalized, -(float)Math.PI / 2);
        vUpArOrientation = MatrixTools.MultiplyMatrix(vUpArB6, vCurrentArOrientation);

        LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis) mCurrentAnalysisSegment;
        vLeftArmAnalysis.LoArOrientation = vLoArOrientation;
        vLeftArmAnalysis.UpArOrientation = vUpArOrientation;
        vLeftArmAnalysis.AngleExtraction();

        vUASubsegment.UpdateSubsegmentOrientation(vUpArOrientation);
        vLASubsegment.UpdateSubsegmentOrientation(vLoArOrientation);
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

        Vector3 u1 = new Vector3(1, 0, 0);// (B2[0, 0], B2[1, 0], B2[2, 0]);

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
