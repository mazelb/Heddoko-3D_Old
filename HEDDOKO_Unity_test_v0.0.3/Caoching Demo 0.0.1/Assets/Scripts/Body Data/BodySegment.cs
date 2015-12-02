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

    private List<SensorTuple> SensorsTuple = new List<SensorTuple>();
    public BodySegmentView AssociatedView;
    public SegmentAnalysis mCurrentAnalysisSegment;

    //TODO: extract this where appropriate
    private static float mRightLegHeight = 1.0f;
    private static float mLeftLegHeight = 1.0f; 

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
    internal void UpdateSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary)
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
                Vector3 vRawEuler = Vector3.zero; //vFrame.FrameData[vPos];
                vSensTuple.InitSensor.SensorData.PositionalData = Vector3.zero; //vRawEuler;
                int vKey = (int)vSensTuple.InitSensor.SensorPosition;
                if (vSensTuple.InitSensor.SensorType == BodyStructureMap.SensorTypes.ST_Biomech)
                {
                    //get the subsegment and update its  inverse initial orientation 
                    if (BodySubSegmentsDictionary.ContainsKey(vKey))
                    {
                        //update these subsegments only with their parent's initial position
                        if (vKey == (int)BodyStructureMap.SensorPositions.SP_LowerSpine)
                        {
                            BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                            BodySubSegmentsDictionary[(int)BodyStructureMap.SensorPositions.SP_UpperSpine].UpdateInverseQuaternion(vRawEuler);
                        }
                        if (vKey == (int)BodyStructureMap.SensorPositions.SP_RightUpperArm)
                        {
                            BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                            BodySubSegmentsDictionary[(int)BodyStructureMap.SensorPositions.SP_RightForeArm].UpdateInverseQuaternion(vRawEuler);
                        }
                        if (vKey == (int)BodyStructureMap.SensorPositions.SP_LeftUpperArm)
                        {
                            BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                            BodySubSegmentsDictionary[(int)BodyStructureMap.SensorPositions.SP_LeftForeArm].UpdateInverseQuaternion(vRawEuler);
                        }
                        if (vKey == (int)BodyStructureMap.SensorPositions.SP_RightCalf)
                        {
                            BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                            BodySubSegmentsDictionary[(int)BodyStructureMap.SensorPositions.SP_RightThigh].UpdateInverseQuaternion(vRawEuler);
                        }
                        if (vKey == (int)BodyStructureMap.SensorPositions.SP_LeftCalf)
                        {
                            BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                            BodySubSegmentsDictionary[(int)BodyStructureMap.SensorPositions.SP_LeftThigh].UpdateInverseQuaternion(vRawEuler);
                        }
                    }
                }
            }
        }
    }

    /**
    * MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the torso subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapTorsoSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    {
        BodySubSegment vUSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment vLSSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        float[,] vaTorsoOrientation = new float[3, 3];
        float[,] vaSpineOrientation = new float[3, 3];

        float[,] vTrackedTorsoOrientation = (vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine]);
        float[,] vTrackedSpineOrientation = (vTransformatricies[BodyStructureMap.SensorPositions.SP_LowerSpine]);

        float[,] TorsoB3 = new float[3, 3];
        float[,] TorsoB4 = new float[3, 3];

        float[,] vCurrentTorsoOrientation = new float[3, 3];

        Vector3 u = new Vector3(vTrackedTorsoOrientation[0, 1], vTrackedTorsoOrientation[1, 1], vTrackedTorsoOrientation[2, 1]);
        vCurrentTorsoOrientation = MatrixTools.RVector(u, 0f);
        //vCurrentTorsoOrientation = MatrixTools.RVector(u, (float)Math.PI);
        TorsoB3 = MatrixTools.multi(vCurrentTorsoOrientation, vTrackedTorsoOrientation);

        u.Set(0, 1, 0);
        //vCurrentTorsoOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vCurrentTorsoOrientation = MatrixTools.RVector(u, 0f);
        vaTorsoOrientation = MatrixTools.multi(vCurrentTorsoOrientation, TorsoB3);

        //Update the analysis inputs
        TorsoAnalysis vTorsoAnalysis = (TorsoAnalysis)mCurrentAnalysisSegment;
        vTorsoAnalysis.TorsoOrientation = vCurrentTorsoOrientation;
        vTorsoAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vUSSubsegment.UpdateSubsegmentOrientation(vaTorsoOrientation);
        //vLSSubsegment.UpdateSubsegmentOrientation(vaSpineOrientation);

        //Update body height
        float vLegMovement;
        float vFootHeight = 0.0f;

        if (mRightLegHeight > mLeftLegHeight)
        {
            vLegMovement = mRightLegHeight;
        }
        else
        {
            vLegMovement = mLeftLegHeight;
        }
        Debug.Log("LegHeights: " + mRightLegHeight + " , " + mLeftLegHeight);
        vLSSubsegment.UpdateSubsegmentPosition(vLegMovement + vFootHeight);
    }

    /**
    * MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the left leg subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapRightLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    {
        //UL : Upper Leg
        //LL : Lower Leg
        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightCalf];

        float[,] vHipOrientation = new float[3, 3];
        float[,] vKneeOrientation = new float[3, 3];

        float[,] vCompensationRotationKnee = new float[3, 3];
        float[,] vCompensationRotationHip = new float[3, 3];
        float[,] vCurrentKneeOrientation = new float[3, 3];

        float vOrientationError = 0;
        float vCompensationAngle = 0;

        //Intermediate arrays until achieve final orientation for hip and knee, they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
        float[,] vHipOrientationMatrix = new float[3, 3];
        float[,] vHipB2 = new float[3, 3];
        float[,] vHipB3 = new float[3, 3];
        float[,] vHipB4 = new float[3, 3];
        float[,] vHipB5 = new float[3, 3];
        float[,] vHipB6 = new float[3, 3];
        float[,] vHipB7 = new float[3, 3];
        float[,] HipB22 = new float[3, 3];

        float[,] vKneeOrientationMatrix = new float[3, 3];
        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3 = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];
        float[,] KneeB22 = new float[3, 3];

        bool fusion = false;

        /////////// Initial Frame Adjustments /////////////////// 
        //vHipOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh];
        //vKneeOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf];

        //vHipB2 = vHipOrientationMatrix;
        //vKneeB2 = vKneeOrientationMatrix;

        HipB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh]);
        KneeB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf]);

        vHipOrientationMatrix = HipB22;// vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh];
        vKneeOrientationMatrix = KneeB22;// vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf];

        vHipB2 = HipB22;// vHipOrientationMatrix;
        vKneeB2 = KneeB22;// vKneeOrientationMatrix;

        ///////////////////////////////////////
        /////////// Fusion  ///////////////////
        /////////////////////////////////////// 
        if (fusion)
        {
            ///////////// Pitch Compensation StepForward ///////////////////
            Vector3 pitchHip = new Vector3(vHipB2[0, 2], vHipB2[1, 2], vHipB2[2, 2]);
            Vector3 pitchKnee = new Vector3(vKneeB2[0, 2], vKneeB2[1, 2], vKneeB2[2, 2]);
            Vector3 NcrossHipKnee = new Vector3(0, 0, 0);

            // rotation axis for pitch compensation
            NcrossHipKnee = Vector3.Cross(pitchHip, pitchKnee).normalized;
            vOrientationError = vHipB2[0, 2] * vKneeB2[0, 2] + vHipB2[1, 2] * vKneeB2[1, 2] + vHipB2[2, 2] * vKneeB2[2, 2];

            // Finding Pitch compensation Angle
            vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

            // Building Pitch compensation rotation matrices
            vCompensationRotationHip = MatrixTools.RVector(NcrossHipKnee, -0.5f * vCompensationAngle);
            vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKnee, +0.5f * vCompensationAngle);

            // Applying Pitch Compensation 
            vKneeB3 = MatrixTools.multi(vCompensationRotationHip, vKneeB2);
            vHipB3 = MatrixTools.multi(vCompensationRotationKnee, vHipB2);

            // this step applies the knee 180 constraint and can be used also for fusion of knee stretch sensors and nods in future
            ///////////// Knee 180 degree Constraint ///////////////////

            Vector3 RollHip = new Vector3(vHipB3[0, 0], vHipB3[1, 0], vHipB3[2, 0]);
            Vector3 YawKnee = new Vector3(vKneeB3[0, 1], vKneeB3[1, 1], vKneeB3[2, 1]);
            Vector3 YawHip = new Vector3(vHipB3[0, 1], vHipB3[1, 1], vHipB3[2, 1]);

            Vector3 NcrossHipKneeRoll = Vector3.Cross(YawHip, YawKnee).normalized;

            if (Vector3.Dot(RollHip, YawKnee) > 0) /// this case when not obey 180 degree constraint
            {
                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];

                vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);
            }
            else  /// this case when obey 180 degree constraint just to improve knee angle estimation
            {
                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];

                // Finding Pitch compensation Angle
                // stretch sensor should be added
                vCompensationAngle = 0;

                // Building yaw compensation rotation matrices
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);
            }
        }//*/
        else
        {
            vKneeB4 = vKneeB2;
            vHipB4 = vHipB2;
        }

        /// /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

        ////////////////// setting Hip to Final Body orientation ///////////////////////////////

        Vector3 u = new Vector3(vHipB4[0, 1], vHipB4[1, 1], vHipB4[2, 1]);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u, 0f);
        vHipB5 = MatrixTools.multi(vCurrentKneeOrientation, vHipB4);

        u.Set(vHipB5[0, 2], vHipB5[1, 2], vHipB5[2, 2]);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vHipB6 = MatrixTools.multi(vCurrentKneeOrientation, vHipB5);

        u.Set(0, 0, 1);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vHipB7 = MatrixTools.multi(vCurrentKneeOrientation, vHipB6);

        u.Set(0, 1, 0);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u, 0f);
        vHipOrientation = MatrixTools.multi(vCurrentKneeOrientation, vHipB7);

        ////////////////// setting Knee to Final Body orientation ///////////////////////////////

        Vector3 u2 = new Vector3(vKneeB4[0, 1], vKneeB4[1, 1], vKneeB4[2, 1]);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u2, 0);
        vKneeB5 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB4);

        u2.Set(vKneeB5[0, 2], vKneeB5[1, 2], vKneeB5[2, 2]);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vKneeB6 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB5);

        u2.Set(0, 0, 1);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vKneeB7 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB6);

        u2.Set(0, 1, 0);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u2, 0);
        vKneeOrientation = MatrixTools.multi(vCurrentKneeOrientation, vKneeB7);

        //Update the analysis inputs
        RightLegAnalysis vRightLegAnalysis = (RightLegAnalysis)mCurrentAnalysisSegment;
        vRightLegAnalysis.HipOrientation = vHipOrientation;
        vRightLegAnalysis.KneeOrientation = vKneeOrientation;
        vRightLegAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);

        //Update Leg height 
        mRightLegHeight = vKneeOrientation[1, 1] * 0.5f + vHipOrientation[1, 1] * 0.5f;
        Debug.Log("mRightLegHeight: " + mRightLegHeight);
    }

    /**
    * MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Performs mapping on the left leg subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapLeftLegSegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    {
        //UL: upper leg
        //LL: lower leg 

        BodySubSegment vULSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftThigh];
        BodySubSegment vLLSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftCalf];

        float[,] vHipOrientation = new float[3, 3];
        float[,] vKneeOrientation = new float[3, 3];

        float[,] vCompensationRotationKnee = new float[3, 3];
        float[,] vCompensationRotationHip = new float[3, 3];
        float[,] vCurrentKneeOrientation = new float[3, 3];

        float vOrientationError = 0;
        float vCompensationAngle = 0;

        //Intermediate arrays until achieve final orientation for hip and knee, they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively

        float[,] vHipOrientationMatrix = new float[3, 3];
        float[,] vHipB2 = new float[3, 3];
        float[,] vHipB3 = new float[3, 3];
        float[,] vHipB4 = new float[3, 3];
        float[,] vHipB5 = new float[3, 3];
        float[,] vHipB6 = new float[3, 3];
        float[,] vHipB7 = new float[3, 3];
        float[,] HipB21 = new float[3, 3];
        float[,] HipB22 = new float[3, 3];

        float[,] vKneeOrientationMatrix = new float[3, 3];
        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3 = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];
        float[,] KneeB21 = new float[3, 3];
        float[,] KneeB22 = new float[3, 3];

        /////////// Initial Frame Adjustments /////////////////// 
        HipB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh]);
        KneeB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf]);

        //vHipOrientationMatrix = HipB22;// vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh];
        //vKneeOrientationMatrix = KneeB22;// vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf];

        vHipB2 = HipB22;// vHipOrientationMatrix;
        vKneeB2 = KneeB22;// vKneeOrientationMatrix;

        ///////////////////////////////////////
        /////////// Fusion  ///////////////////
        /////////////////////////////////////// 
        bool fusion = false;

        if (fusion)
        {
            ///////////// Pitch Compensation StepForward ///////////////////
            Vector3 pitchHip = new Vector3(vHipB2[0, 2], vHipB2[1, 2], vHipB2[2, 2]);
            Vector3 pitchKnee = new Vector3(vKneeB2[0, 2], vKneeB2[1, 2], vKneeB2[2, 2]);
            Vector3 NcrossHipKnee = new Vector3(0, 0, 0);


            // rotation axis for pitch compensation
            NcrossHipKnee = Vector3.Cross(pitchHip, pitchKnee).normalized;
            vOrientationError = vHipB2[0, 2] * vKneeB2[0, 2] + vHipB2[1, 2] * vKneeB2[1, 2] + vHipB2[2, 2] * vKneeB2[2, 2];

            // Finding Pitch compensation Angle
            vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

            // Building Pitch compensation rotation matrices
            vCompensationRotationHip = MatrixTools.RVector(NcrossHipKnee, -0.5f * vCompensationAngle);
            vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKnee, +0.5f * vCompensationAngle);

            // Applying Pitch Compensation 
            vKneeB3 = MatrixTools.multi(vCompensationRotationHip, vKneeB2);
            vHipB3 = MatrixTools.multi(vCompensationRotationKnee, vHipB2);

            // this step applies the knee 180 constraint and can be used also for fusion of knee stretch sensors and nods in future
            ///////////// Knee 180 degree Constraint ///////////////////

            Vector3 RollHip = new Vector3(vHipB3[0, 0], vHipB3[1, 0], vHipB3[2, 0]);
            Vector3 YawKnee = new Vector3(vKneeB3[0, 1], vKneeB3[1, 1], vKneeB3[2, 1]);
            Vector3 YawHip = new Vector3(vHipB3[0, 1], vHipB3[1, 1], vHipB3[2, 1]);

            Vector3 NcrossHipKneeRoll = Vector3.Cross(YawHip, YawKnee).normalized;

            if (Vector3.Dot(RollHip, YawKnee) > 0) /// this case when not obey 180 degree constraint
            {
                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];

                vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);
            }
            else  /// this case when obey 180 degree constraint just to improve knee angle estimation
            {
                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];

                // Finding Pitch compensation Angle
                // stretch sensor should be added
                vCompensationAngle = 0;

                // Building yaw compensation rotation matrices
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);
            }
        }
        else
        {
            vKneeB4 = vKneeB2;
            vHipB4 = vHipB2;
        }

        ///////////////////////////////////////
        /////////// Mapping ///////////////////
        /////////////////////////////////////// 

        ////////////////// setting Hip to Final Body orientation ///////////////////////////////

        Vector3 u = new Vector3(vHipB4[0, 1], vHipB4[1, 1], vHipB4[2, 1]);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u, 0);
        vHipB5 = MatrixTools.multi(vCurrentKneeOrientation, vHipB4);

        u.Set(vHipB5[0, 2], vHipB5[1, 2], vHipB5[2, 2]);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vHipB6 = MatrixTools.multi(vCurrentKneeOrientation, vHipB5);

        u.Set(0, 0, 1);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vHipB7 = MatrixTools.multi(vCurrentKneeOrientation, vHipB6);

        u.Set(0, 1, 0);
        vCurrentKneeOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u, 0f);
        vHipOrientation = MatrixTools.multi(vCurrentKneeOrientation, vHipB7);

        ////////////////// setting Knee to Final Body orientation ///////////////////////////////

        Vector3 u2 = new Vector3(vKneeB4[0, 1], vKneeB4[1, 1], vKneeB4[2, 1]);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u2, 0);
        vKneeB5 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB4);

        u2.Set(vKneeB5[0, 2], vKneeB5[1, 2], vKneeB5[2, 2]);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vKneeB6 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB5);

        u2.Set(0, 0, 1);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vKneeB7 = MatrixTools.multi(vCurrentKneeOrientation, vKneeB6);

        u2.Set(0, 1, 0);
        vCurrentKneeOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        //vCurrentKneeOrientation = MatrixTools.RVector(u2, 0f);
        vKneeOrientation = MatrixTools.multi(vCurrentKneeOrientation, vKneeB7);

        //Update the analysis inputs
        LeftLegAnalysis vLeftLegAnalysis = (LeftLegAnalysis)mCurrentAnalysisSegment;
        vLeftLegAnalysis.HipOrientation = vHipOrientation;
        vLeftLegAnalysis.KneeOrientation = vKneeOrientation;
        vLeftLegAnalysis.AngleExtraction();

        //Update the segment's and segment's view orientations
        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);

        //Update leg height 
        mLeftLegHeight = vKneeOrientation[1, 1] * 0.5f + vHipOrientation[1, 1] * 0.5f;
        Debug.Log("mLeftLegHeight: " + mLeftLegHeight);
    }

    /**
    * MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    * @brief  Updates the right arm subsegment from the available sensor data
    * @param Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies : transformation matrices mapped to sensor positions
    */
    internal void MapRightArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    {
        // current Upperarm (shoulder) and lower arm (Elbow) joints orientation, 
        //UpAr stands for upper arm and LoAr stands for Lower arm (forearm) in this code
        float[,] vUpArOrientation = new float[3, 3];
        float[,] vLoArOrientation = new float[3, 3];

        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];

        float[,] vUpArB2 = new float[3, 3];
        float[,] vUpArB3 = new float[3, 3];
        float[,] vUpArB4 = new float[3, 3];
        float[,] vUpArB5 = new float[3, 3];
        float[,] vUpArB6 = new float[3, 3];
        float[,] vUpArB21 = new float[3, 3];
        float[,] vUpArB22 = new float[3, 3];

        float[,] vLoArB2 = new float[3, 3];
        float[,] vLoArB3 = new float[3, 3];
        float[,] vLoArB4 = new float[3, 3];
        float[,] vLoArB5 = new float[3, 3];
        float[,] vLoArB6 = new float[3, 3];
        float[,] vLoArB7 = new float[3, 3];
        float[,] vLoArB21 = new float[3, 3];
        float[,] vLoArB22 = new float[3, 3];

        float[,] vCompensationRotationLoAr = new float[3, 3];
        float[,] vCompensationRotationUpAr = new float[3, 3];
        float[,] vCurrentLoArOrientation = new float[3, 3];

        float vOrientationError = 0;
        float vCompensationAngle = 0;

        //Get the orientation matricies
        //UpArB21 = multi(UpArFi, UpArB1);
        //LoArB21 = multi(LoArFi, LoArB1);

        vUpArB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm]);
        vLoArB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm]);

        vUpArB2 = vUpArB22;// GravityRefArm(UpArB22, vNodInitAcc0);
        vLoArB2 = vLoArB22;// GravityRefArm(LoArB22, vNodInitAcc1);

        //vUpArB2 = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm];
        //vLoArB2 = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm];

        bool fusion = false;

        if (fusion)
        {
            ///////////// Yaw Compensation StepForward ///////////////////constraint

            // finding compensated yaw axis of lower arm in one plane with upper arm
            float temp1, temp2, temp3, temp4;
            temp1 = vLoArB2[0, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[0, 2];
            //Debug.Log("temp1 " + temp1);
            temp2 = vLoArB2[1, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[1, 2];
            //Debug.Log("temp2 " + temp2);
            temp3 = vLoArB2[2, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[2, 2];
            //Debug.Log("temp3 " + temp3);
            temp4 = (float)Math.Sqrt((float)(temp1 * temp1 + temp2 * temp2 + temp3 * temp3));
            //Debug.Log("temp4 " + temp4);
            Vector3 yawLoAr = new Vector3(temp1 / temp4, temp2 / temp4, temp3 / temp4);
            //Debug.Log("yawLoAr " + yawLoAr);

            // Finding yaw compensation Angle
            vOrientationError = yawLoAr.x * vLoArB2[0, 1] + yawLoAr.y * vLoArB2[1, 1] + yawLoAr.z * vLoArB2[2, 1];
            //Debug.Log("vOrientationError " + vOrientationError);

            vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);
                      
            //Debug.Log("CompensationAngle "+ vCompensationAngle);
            
            //CompensationAngle = 0;
            
            // Finding yaw compensation axis
            Vector3 yawLoAr2 = new Vector3(vLoArB2[0, 1], vLoArB2[1, 1], vLoArB2[2, 1]);

            Vector3 NcrossUpArLoAr = new Vector3(0, 0, 0);
            NcrossUpArLoAr = Vector3.Cross(yawLoAr2, yawLoAr).normalized;

            vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoAr, vCompensationAngle);
            vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoAr, -vCompensationAngle);

            // Applying yaw Compensation 
            vLoArB3 = MatrixTools.multi(vCompensationRotationUpAr, vLoArB2);

            // to remove the constraint un comment this line
            //		LoArB3=LoArB2;

            // this step applies the elbow 180 constraint and can be used also for fusion of elbow stretch sensors and nods in future
            ///////////// Elbow 180 degree Constraint ///////////////////

            Vector3 RollUpAr = new Vector3(vUpArB2[0, 0], vUpArB2[1, 0], vUpArB2[2, 0]);
            yawLoAr = new Vector3(vLoArB3[0, 1], vLoArB3[1, 1], vLoArB3[2, 1]);
            yawLoAr2 = new Vector3(vUpArB2[0, 1], vUpArB2[1, 1], vUpArB2[2, 1]);

            Vector3 NcrossUpArLoArRoll = new Vector3(0, 0, 0);
            NcrossUpArLoArRoll = Vector3.Cross(yawLoAr2, yawLoAr).normalized;

            if (Vector3.Dot(RollUpAr, yawLoAr) > 0) /// this case when not obey 180 degree constraint
            {
                vOrientationError = vUpArB2[0, 1] * vLoArB3[0, 1] + vUpArB2[1, 1] * vLoArB3[1, 1] + vUpArB2[2, 1] * vLoArB3[2, 1];

                //Finding yaw compensation Angle
                vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices
                vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoArRoll, +0.5f * vCompensationAngle);
                vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoArRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vLoArB4 = MatrixTools.multi(vCompensationRotationLoAr, vLoArB3);
                vUpArB3 = MatrixTools.multi(vCompensationRotationUpAr, vUpArB2);

            }
            else //*/ /// this case when obey 180 degree constraint just to improve LoAr angle estimation
            {
                vOrientationError = vUpArB2[0, 1] * vLoArB3[0, 1] + vUpArB2[1, 1] * vLoArB3[1, 1] + vUpArB2[2, 1] * vLoArB3[2, 1];
                vCompensationAngle = 0;

                // Building yaw compensation rotation matrices
                vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoArRoll, +0.5f * vCompensationAngle);
                vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoArRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vLoArB4 = MatrixTools.multi(vCompensationRotationLoAr, vLoArB3);
                vUpArB3 = MatrixTools.multi(vCompensationRotationUpAr, vUpArB2);
            }
        }
        else
        {
            vUpArB3 = vUpArB2;
            vLoArB4 = vLoArB2;
        }

        ////////////////// setting to Final Body orientation lower arm ///////////////////////////////
        Vector3 u = new Vector3(vLoArB4[0, 1], vLoArB4[1, 1], vLoArB4[2, 1]);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        vLoArB5 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB4);

        u.Set(vLoArB5[0, 0], vLoArB5[1, 0], vLoArB5[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI / 2);
        vLoArB6 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB5);

        u.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u, -(float)Math.PI / 2);
        vLoArB7 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB6);

        u.Set(0, 0, 1);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vLoArOrientation = MatrixTools.multi(vCurrentLoArOrientation, vLoArB7);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(vUpArB3[0, 1], vUpArB3[1, 1], vUpArB3[2, 1]);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, 0f);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArB4 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB3);

        u2.Set(vUpArB4[0, 0], vUpArB4[1, 0], vUpArB4[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI / 2);
        vUpArB5 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB4);

        u2.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, -(float)Math.PI / 2);
        vUpArB6 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB5);

        u2.Set(0, 0, 1);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, 0f);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArOrientation = MatrixTools.multi(vCurrentLoArOrientation, vUpArB6);

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
    internal void MapLeftArmSubsegment(Dictionary<BodyStructureMap.SensorPositions, float[,]> vTransformatricies)
    {
        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];

        //Intermediate arrays until achieve final orientation for shoulder and elbow
        //they are Tagged with F (forward rotation) and B (Backward rotation) and are numbered consecutively
        // UpAr stands for upper arm sensor orientation and lower arm stands for lower arm (forearm) orientation
        float[,] vUpArOrientation = new float[3, 3];
        float[,] vLoArOrientation = new float[3, 3];


        float[,] vUpArB2 = new float[3, 3];
        float[,] vUpArB3 = new float[3, 3];
        float[,] vUpArB4 = new float[3, 3];
        float[,] vUpArB5 = new float[3, 3];
        float[,] vUpArB6 = new float[3, 3];
        float[,] vUpArB7 = new float[3, 3];
        float[,] vUpArB21 = new float[3, 3];
        float[,] vUpArB22 = new float[3, 3];

        float[,] vLoArB2 = new float[3, 3];
        float[,] vLoArB3 = new float[3, 3];
        float[,] vLoArB4 = new float[3, 3];
        float[,] vLoArB5 = new float[3, 3];
        float[,] vLoArB6 = new float[3, 3];
        float[,] vLoArB7 = new float[3, 3];
        float[,] vLoArB8 = new float[3, 3];
        float[,] vLoArB21 = new float[3, 3];
        float[,] vLoArB22 = new float[3, 3];

        float[,] vCompensationRotationLoAr = new float[3, 3];
        float[,] vCompensationRotationUpAr = new float[3, 3];
        float[,] vCurrentLoArOrientation = new float[3, 3];

        float vOrientationError = 0;
        float vCompensationAngle = 0;

        /////////// Initial Frame Adjustments ///////////////////
        //No Gravity
        //UpArB2 = TiltNod(UpArB21);
        //LoArB2 = TiltNod(LoArB21);

        //No tilt
        //UpArB22 = UpArB21;
        //LoArB22 = LoArB21;
        vUpArB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm]);
        vLoArB22 = TiltNod(vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm]);

        vUpArB2 = vUpArB22;// GravityRefArm(UpArB22, vNodInitAcc0);
        vLoArB2 = vLoArB22;// GravityRefArm(LoArB22, vNodInitAcc1);

        //vUpArB2 = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm];
        //vLoArB2 = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm];

        /// /////////////////////////////////////////////////////  Fusion /////////////////////////////////////////////////////////////////////
        /// /////////////////////////////////////////////////////  Fusion /////////////////////////////////////////////////////////////////////
        bool fusion = false;

        if (fusion)
        {
            // finding compensated yaw in one plane with upperarm
            float temp1, temp2, temp3, temp4;
            temp1 = vLoArB2[0, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[0, 2];
            temp2 = vLoArB2[1, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[1, 2];
            temp3 = vLoArB2[2, 1] - (vUpArB2[0, 2] * vLoArB2[0, 1] + vUpArB2[1, 2] * vLoArB2[1, 1] + vUpArB2[2, 2] * vLoArB2[2, 1]) * vUpArB2[2, 2];
            temp4 = (float)Math.Sqrt((float)(temp1 * temp1 + temp2 * temp2 + temp3 * temp3));
            Vector3 yawLoAr = new Vector3(temp1 / temp4, temp2 / temp4, temp3 / temp4);

            // Finding yaw compensation Angle
            vOrientationError = yawLoAr.x * vLoArB2[0, 1] + yawLoAr.y * vLoArB2[1, 1] + yawLoAr.z * vLoArB2[2, 1];
            vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);
            //Debug.Log("LEFT CompensationAngle " + vCompensationAngle);

            // Finding yaw compensation axis
            Vector3 yawLoAr2 = new Vector3(vLoArB2[0, 1], vLoArB2[1, 1], vLoArB2[2, 1]);

            Vector3 NcrossUpArLoAr = new Vector3(0, 0, 0);
            NcrossUpArLoAr = Vector3.Cross(yawLoAr2, yawLoAr).normalized;

            vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoAr, vCompensationAngle);
            vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoAr, -vCompensationAngle);

            // Applying yaw Compensation 
            vLoArB3 = MatrixTools.multi(vCompensationRotationUpAr, vLoArB2);

            //LoArB3=LoArB2;

            // this step applies the elbow 180 constraint and can be used also for fusion of elbow stretch sensors and nods in future
            ///////////// LoAr 180 degree Constraint ///////////////////
            Vector3 RollUpAr = new Vector3(vUpArB2[0, 0], vUpArB2[1, 0], vUpArB2[2, 0]);

            yawLoAr = new Vector3(vLoArB3[0, 1], vLoArB3[1, 1], vLoArB3[2, 1]);
            yawLoAr2 = new Vector3(vUpArB2[0, 1], vUpArB2[1, 1], vUpArB2[2, 1]);

            Vector3 NcrossUpArLoArRoll = new Vector3(0, 0, 0);
            NcrossUpArLoArRoll = Vector3.Cross(yawLoAr2, yawLoAr).normalized;

            if (Vector3.Dot(RollUpAr, yawLoAr) > 0) /// this case when not obey 180 degree constraint
            {

                vOrientationError = vUpArB2[0, 1] * vLoArB3[0, 1] + vUpArB2[1, 1] * vLoArB3[1, 1] + vUpArB2[2, 1] * vLoArB3[2, 1];

                // Finding yaw compensation Angle
                vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices
                vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoArRoll, +0.5f * vCompensationAngle);
                vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoArRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vLoArB4 = MatrixTools.multi(vCompensationRotationLoAr, vLoArB3);
                vUpArB3 = MatrixTools.multi(vCompensationRotationUpAr, vUpArB2);

            }
            else//*/  /// this case when obey 180 degree constraint just to improve LoAr angle estimation
            {
                vOrientationError = vUpArB2[0, 1] * vLoArB3[0, 1] + vUpArB2[1, 1] * vLoArB3[1, 1] + vUpArB2[2, 1] * vLoArB3[2, 1];

                // Finding yaw compensation Angle
                vCompensationAngle = 0;

                // Building yaw compensation rotation matrices
                vCompensationRotationUpAr = MatrixTools.RVector(NcrossUpArLoArRoll, +0.5f * vCompensationAngle);
                vCompensationRotationLoAr = MatrixTools.RVector(NcrossUpArLoArRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vLoArB4 = MatrixTools.multi(vCompensationRotationLoAr, vLoArB3);
                vUpArB3 = MatrixTools.multi(vCompensationRotationUpAr, vUpArB2);
            }
        }
        else
        {
            vLoArB4 = vLoArB2;
            vUpArB3 = vUpArB2;
        }

        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////

        ////////////////// setting to Final Body orientation lower arm///////////////////////////////
        Vector3 u = new Vector3(vLoArB4[0, 0], vLoArB4[1, 0], vLoArB4[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI / 2);
        vLoArB5 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB4);


        u.Set(vLoArB5[0, 1], vLoArB5[1, 1], vLoArB5[2, 1]);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vLoArB6 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB5);

        u.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI / 2);
        vLoArB7 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB6);

        u.Set(0, 0, 1);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vLoArOrientation = MatrixTools.multi(vCurrentLoArOrientation, vLoArB7);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(vUpArB3[0, 0], vUpArB3[1, 0], vUpArB3[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI / 2);
        vUpArB4 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB3);

        u2.Set(vUpArB4[0, 1], vUpArB4[1, 1], vUpArB4[2, 1]);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArB5 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB4);

        u2.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI / 2);
        vUpArB6 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB5);


        u2.Set(0, 0, 1);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArOrientation = MatrixTools.multi(vCurrentLoArOrientation, vUpArB6);

        LeftArmAnalysis vLeftArmAnalysis = (LeftArmAnalysis) mCurrentAnalysisSegment;
        vLeftArmAnalysis.LoArOrientation = vLoArOrientation;
        vLeftArmAnalysis.UpArOrientation = vUpArOrientation;
        vLeftArmAnalysis.AngleExtraction();
        vUASubsegment.UpdateSubsegmentOrientation(vUpArOrientation);
        vLASubsegment.UpdateSubsegmentOrientation(vLoArOrientation);
    }

    /**
    *  TiltNod()
    * @param  
    * @brief 
    */
    public float[,] TiltNod(float[,] B2)
    {
        float[,] B3 = new float[3, 3];
        float[,] B4 = new float[3, 3];

        Vector3 u1 = new Vector3(B2[0, 0], B2[1, 0], B2[2, 0]);

        float vAngleNew = -(float)Math.PI * (7.6f / 180f);

        float[,] CurrentCompensation = new float[3, 3];
        CurrentCompensation = MatrixTools.RVector(u1, vAngleNew);

        B3 = MatrixTools.multi(CurrentCompensation, B2);

        Vector3 roll = new Vector3(1, 0, 0);

        float[,] Compensation = new float[3, 3];
        Compensation = MatrixTools.RVector(roll, -vAngleNew);

        B4 = MatrixTools.multi(Compensation, B3);

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
    private void MapSubSegments(Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary)
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
