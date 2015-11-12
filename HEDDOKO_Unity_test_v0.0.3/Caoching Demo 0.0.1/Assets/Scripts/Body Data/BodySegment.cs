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
    //a list of body segment neighbors

    /**
    * UpdateSensorsData(BodyFrame vFrame)
    * @param BodyFrame vFrame: the body frame containing the sensor data to be updated to the segment's sensors
    * @brief  The function will update the sensors data with the passed in BodyFrame. Iterates through the list of sensor tuples and updates the current sensor's information
    */
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
                Vector3 vRawEuler = vFrame.FrameData[vPos];
                vSensTuple.InitSensor.SensorData.PositionalData = Vector3.zero; //vRawEuler;
                int vKey = (int)vSensTuple.InitSensor.SensorPosition;
                vRawEuler = Vector3.zero;
                //if (BodySubSegmentsDictionary.ContainsKey(vKey))
                //{
                //    BodySubSegmentsDictionary[vKey].UpdateInverseQuaternion(vRawEuler);
                //}
                if (vSensTuple.InitSensor.SensorType == BodyStructureMap.SensorTypes.ST_Biomech)
                {
                    //update the initial orientation per segment
                    //get the subsegment associated with the initial sensor
                    //int vKey = (int)vSensTuple.InitSensor.SensorPosition;

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

        //        float[,] vaTorsoOrientation = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine];
        //        float[,] vaSpineOrientation = vTransformatricies[BodyStructureMap.SensorPositions.SP_LowerSpine];

        float[,] vaTorsoOrientation = new float[3, 3];
        float[,] vaSpineOrientation = new float[3, 3];

        float[,] vTrackedTorsoOrientation = vTransformatricies[BodyStructureMap.SensorPositions.SP_UpperSpine];
        float[,] vTrackedSpineOrientation = vTransformatricies[BodyStructureMap.SensorPositions.SP_LowerSpine];

        float[,] TorsoB3 = new float[3, 3];
        float[,] TorsoB4 = new float[3, 3];

        float[,] vCurrentTorsoOrientation = new float[3, 3];

        Vector3 u = new Vector3(vTrackedTorsoOrientation[0, 1], vTrackedTorsoOrientation[1, 1], vTrackedTorsoOrientation[2, 1]);
        //vCurrentTorsoOrientation = MatrixTools.RVector(u,0f);
        vCurrentTorsoOrientation = MatrixTools.RVector(u, (float)Math.PI);
        TorsoB3 = MatrixTools.multi(vCurrentTorsoOrientation, vTrackedTorsoOrientation);

        u.Set(0, 1, 0);
        vCurrentTorsoOrientation = MatrixTools.RVector(u, (float)Math.PI);
        //vCurrentTorsoOrientation = MatrixTools.RVector(u, 0f);
        vaTorsoOrientation = MatrixTools.multi(vCurrentTorsoOrientation, TorsoB3);

        vUSSubsegment.UpdateSubsegmentOrientation(vaTorsoOrientation);
        //vLSSubsegment.UpdateSubsegmentOrientation(vaSpineOrientation);
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

        float[,] vKneeOrientationMatrix = new float[3, 3];
        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3 = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];

        float vAngleRightKneeFlexionImu;
        float vOptimumAngle;
        float vOptimumAngle2;

        bool fusion = true;

        /////////// Initial Frame Adjustments /////////////////// 
        vHipOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightThigh];
        vKneeOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightCalf];

        vHipB2 = vHipOrientationMatrix;
        vKneeB2 = vKneeOrientationMatrix;

        ///////////////////////////////////////
        /////////// Fusion  ///////////////////
        /////////////////////////////////////// 
        if (fusion)
        {
            ///////////// Pitch Compensation Step ///////////////////
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
            if (Vector3.Dot(RollHip, YawKnee) < 0) /// this case when not obey 180 degree constraint
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
        }
        /*if (fusion)
        {
            ///////////// Pitch Compensation Step ///////////////////
            Vector3 vPitchHip = new Vector3(vHipB2[0, 2], vHipB2[1, 2], vHipB2[2, 2]);
            Vector3 vPitchKnee = new Vector3(vKneeB2[0, 2], vKneeB2[1, 2], vKneeB2[2, 2]);
            Vector3 vNcrossHipKnee = new Vector3(0, 0, 0);


            // rotation axis for pitch compensation
            vNcrossHipKnee = Vector3.Cross(vPitchHip, vPitchKnee).normalized;
            vOrientationError = vHipB2[0, 2] * vKneeB2[0, 2] + vHipB2[1, 2] * vKneeB2[1, 2] + vHipB2[2, 2] * vKneeB2[2, 2];

            // Finding Pitch compensation Angle
            vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

            // Building Pitch compensation rotation matrices
            vCompensationRotationHip = MatrixTools.RVector(vNcrossHipKnee, -0.5f * vCompensationAngle);
            vCompensationRotationKnee = MatrixTools.RVector(vNcrossHipKnee, +0.5f * vCompensationAngle);

            // Applying Pitch Compensation 
            vKneeB3 = MatrixTools.multi(vCompensationRotationHip, vKneeB2);
            vHipB3 = MatrixTools.multi(vCompensationRotationKnee, vHipB2);

            // this step applies the knee 180 constraint and can be used also for fusion of knee stretch sensors and nods in future
            ///////////// Knee 180 degree Constraint ///////////////////

            Vector3 vRollHip = new Vector3(vHipB3[0, 0], vHipB3[1, 0], vHipB3[2, 0]);
            Vector3 vYawKnee = new Vector3(vKneeB3[0, 1], vKneeB3[1, 1], vKneeB3[2, 1]);
            Vector3 vYawHip = new Vector3(vHipB3[0, 1], vHipB3[1, 1], vHipB3[2, 1]);
            
            Vector3 NcrossHipKneeRoll = Vector3.Cross(vYawHip, vYawKnee).normalized;
            if (Vector3.Dot(vRollHip, vYawKnee) < 0) /// this case when not obey 180 degree constraint
            {
                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];

                //****			// Finding yaw compensation Angle - pi/2
                //			if ((float)Math.Acos(OrientationError> 1.00f ? 1f : OrientationError) > (3.1415f / 2))
                //			{
                //				CompensationAngle = (float)Math.Acos(OrientationError> 1.00f ? 1f : OrientationError)  - 3.1415f;
                //			}
                //			else
                //			{
                //				//Finding yaw compensation Angle
                //				CompensationAngle = (float)Math.Acos(OrientationError> 1.00f ? 1f : OrientationError) ;
                //			}
                //****

                vCompensationAngle = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);

                // Applying yaw Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);

            }
            else  /// this case when obey 180 degree constraint just to improve knee angle estimation adding stretch sensor
            {

                vOrientationError = vHipB3[0, 1] * vKneeB3[0, 1] + vHipB3[1, 1] * vKneeB3[1, 1] + vHipB3[2, 1] * vKneeB3[2, 1];
                vAngleRightKneeFlexionImu = (float)Math.Acos(vOrientationError > 1.00f ? 1f : vOrientationError);

                // Building yaw compensation rotation matrices and fusion compensation
                vCompensationRotationHip = MatrixTools.RVector(NcrossHipKneeRoll, +0.5f * vCompensationAngle);
                vCompensationRotationKnee = MatrixTools.RVector(NcrossHipKneeRoll, -0.5f * vCompensationAngle);

                // Applying yaw and fusion Compensation 
                vKneeB4 = MatrixTools.multi(vCompensationRotationKnee, vKneeB3);
                vHipB4 = MatrixTools.multi(vCompensationRotationHip, vHipB3);

            }
        }*/
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
        
        //////////////////////////////ASSIGN TO SUBSEGMENT///////////////////////////
        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);
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

        float[,] vKneeOrientationMatrix = new float[3, 3];
        float[,] vKneeB2 = new float[3, 3];
        float[,] vKneeB3 = new float[3, 3];
        float[,] vKneeB4 = new float[3, 3];
        float[,] vKneeB5 = new float[3, 3];
        float[,] vKneeB6 = new float[3, 3];
        float[,] vKneeB7 = new float[3, 3];


        /////////// Initial Frame Adjustments /////////////////// 
        vHipOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftThigh];
        vKneeOrientationMatrix = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftCalf];

        vHipB2 = vHipOrientationMatrix;
        vKneeB2 = vKneeOrientationMatrix;

        ///////////////////////////////////////
        /////////// Fusion  ///////////////////
        /////////////////////////////////////// 
        bool fusion = true;

        if (fusion)
        {
            ///////////// Pitch Compensation Step ///////////////////
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
            if (Vector3.Dot(RollHip, YawKnee) < 0) /// this case when not obey 180 degree constraint
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
            vKneeB4 = vKneeOrientationMatrix;
            vHipB4 = vHipOrientationMatrix;
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


        vULSubsegment.UpdateSubsegmentOrientation(vHipOrientation);
        vLLSubsegment.UpdateSubsegmentOrientation(vKneeOrientation);
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
        float[,] UpArOrientation = new float[3, 3];
        float[,] LoArOrientation = new float[3, 3];

        BodySubSegment vUASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLASubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];

        float[,] UpArB3 = new float[3, 3];
        float[,] UpArB4 = new float[3, 3];
        float[,] UpArB5 = new float[3, 3];
        float[,] UpArB6 = new float[3, 3];
        float[,] LoArB4 = new float[3, 3];
        float[,] LoArB5 = new float[3, 3];
        float[,] LoArB6 = new float[3, 3];
        float[,] LoArB7 = new float[3, 3];

        float[,] CurrentLoArOrientation = new float[3, 3];

        //Get the orientation matricies
        UpArB3 = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightUpperArm];
        LoArB4 = vTransformatricies[BodyStructureMap.SensorPositions.SP_RightForeArm];

        ////////////////// setting to Final Body orientation lower arm ///////////////////////////////
        Vector3 u = new Vector3(LoArB4[0, 1], LoArB4[1, 1], LoArB4[2, 1]);
        //CurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        CurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        LoArB5 = MatrixTools.multi(CurrentLoArOrientation, LoArB4);

        u.Set(LoArB5[0, 0], LoArB5[1, 0], LoArB5[2, 0]);
        CurrentLoArOrientation = MatrixTools.RVector(u, -(float)Math.PI / 2);
        LoArB6 = MatrixTools.multi(CurrentLoArOrientation, LoArB5);

        u.Set(1, 0, 0);
        CurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI / 2);
        LoArB7 = MatrixTools.multi(CurrentLoArOrientation, LoArB6);

        u.Set(0, 0, 1);
        CurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        //CurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        LoArOrientation = MatrixTools.multi(CurrentLoArOrientation, LoArB7);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(UpArB3[0, 1], UpArB3[1, 1], UpArB3[2, 1]);
        CurrentLoArOrientation = MatrixTools.RVector(u2, 0f);
        //CurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        UpArB4 = MatrixTools.multi(CurrentLoArOrientation, UpArB3);

        u2.Set(UpArB4[0, 0], UpArB4[1, 0], UpArB4[2, 0]);
        CurrentLoArOrientation = MatrixTools.RVector(u2, -(float)Math.PI / 2);

        UpArB5 = MatrixTools.multi(CurrentLoArOrientation, UpArB4);

        u2.Set(1, 0, 0);
        CurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI / 2);

        UpArB6 = MatrixTools.multi(CurrentLoArOrientation, UpArB5);


        u2.Set(0, 0, 1);
        CurrentLoArOrientation = MatrixTools.RVector(u2, 0f);
        //CurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        UpArOrientation = MatrixTools.multi(CurrentLoArOrientation, UpArB6);

        vUASubsegment.UpdateSubsegmentOrientation(UpArOrientation);
        vLASubsegment.UpdateSubsegmentOrientation(LoArOrientation);
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
       

        float[,] vUpArB3 = new float[3, 3];
        float[,] vUpArB4 = new float[3, 3];
        float[,] vUpArB5 = new float[3, 3];
        float[,] UpArB6 = new float[3, 3];


        float[,] vLoArB4 = new float[3, 3];
        float[,] vLoArB5 = new float[3, 3];
        float[,] vLoArB6 = new float[3, 3];
        float[,] LoArB7 = new float[3, 3];


        float[,] vCurrentLoArOrientation = new float[3, 3];

        /////////// Initial Frame Adjustments ///////////////////

        vUpArB3 = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftUpperArm];
        vLoArB4 = vTransformatricies[BodyStructureMap.SensorPositions.SP_LeftForeArm];

        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////  Mapping /////////////////////////////////////////////////////////////////////
         
        ////////////////// setting to Final Body orientation lower arm///////////////////////////////
        Vector3 u = new Vector3(vLoArB4[0, 0], vLoArB4[1, 0], vLoArB4[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI / 2);
        vLoArB5 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB4);


        u.Set(vLoArB5[0, 1], vLoArB5[1, 1], vLoArB5[2, 1]);
        vCurrentLoArOrientation = MatrixTools.RVector(u, 0f);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vLoArB6 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB5);

        u.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u, -(float)Math.PI / 2);
        LoArB7 = MatrixTools.multi(vCurrentLoArOrientation, vLoArB6);

        u.Set(0, 0, 1);
        vCurrentLoArOrientation = MatrixTools.RVector(u, 0);
        //vCurrentLoArOrientation = MatrixTools.RVector(u, (float)Math.PI);
        vLoArOrientation = MatrixTools.multi(vCurrentLoArOrientation, LoArB7);

        ////////////////// setting to Final Body orientation upper arm///////////////////////////////
        Vector3 u2 = new Vector3(vUpArB3[0, 0], vUpArB3[1, 0], vUpArB3[2, 0]);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI / 2);
        vUpArB4 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB3);

        u2.Set(vUpArB4[0, 1], vUpArB4[1, 1], vUpArB4[2, 1]);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, 0);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArB5 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB4);

        u2.Set(1, 0, 0);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, -(float)Math.PI / 2);
        UpArB6 = MatrixTools.multi(vCurrentLoArOrientation, vUpArB5);


        u2.Set(0, 0, 1);
        vCurrentLoArOrientation = MatrixTools.RVector(u2, 0);
        //vCurrentLoArOrientation = MatrixTools.RVector(u2, (float)Math.PI);
        vUpArOrientation = MatrixTools.multi(vCurrentLoArOrientation, UpArB6);

        vUASubsegment.UpdateSubsegmentOrientation(vUpArOrientation);
        vLASubsegment.UpdateSubsegmentOrientation(vLoArOrientation);
    }

    /**
    *  InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    * @param  vSegmentType: the desired Segment Type
    * @brief Initializes a new body structure's internal properties with the desired Segment Type
    */
    internal void InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    {
        #region use of unity
        GameObject go = new GameObject(EnumUtil.GetName(vSegmentType));
        AssociatedView = go.AddComponent<BodySegmentView>();

        #endregion

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
            //Sensors.Add(newSensor);
            SensorTuple tuple = new SensorTuple();
            tuple.CurrentSensor = new Sensor(newSensor);
            tuple.InitSensor = new Sensor(newSensor);

            SensorsTuple.Add(tuple);
            //              sensorList.Add(tuple.CurrentSensor);
        }

        foreach (BodyStructureMap.SubSegmentTypes sstype in subsegmentTypes)
        {
            BodySubSegment subSegment = new BodySubSegment();
            subSegment.subsegmentType = sstype;
            subSegment.InitializeBodySubsegment(sstype);
            //BodySubSegments.Add(subSegment);
            BodySubSegmentsDictionary.Add((int)sstype, subSegment);
            #region use of unity functions

            subSegment.AssociatedView.transform.parent = AssociatedView.transform;

            #endregion

        }

    }


    public void InitSegment(/*SegmentTypes vSegType, bool vIsTracked = true*/)
    {
        //SegmentType = vSegType;

        //switch (SegmentType)
        //{
        //    case SegmentTypes.SegmentType_Torso:
        //        {

        //        }
        //        break;
        //    case SegmentTypes.SegmentType_RightArm:
        //        {

        //        }
        //        break;
        //    case SegmentTypes.SegmentType_LeftArm:
        //        {

        //        }
        //        break;
        //    case SegmentTypes.SegmentType_RightLeg:
        //        {

        //        }
        //        break;
        //    case SegmentTypes.SegmentType_LeftLeg:
        //        {

        //        }
        //        break;
        //    default:
        //        //TODO: Invalid Body Type
        //        break;
        //}
    }
    #region helperfunctions

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
    #endregion


}
