/** 
* @file BodySegment.cs
* @brief Contains the BodySegment class and the functionalities required to initialize, update its subsegments
* @author Mohammed Haider(Mohammed@Heddoko.com)
* @date October 2015
*/
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Utils;

/**
* BodySegment class 
* @brief BodySegment class (represents one Body segment )
*/
public class BodySegment
{
    //Segment Type 
    public BodyStructureMap.SegmentTypes mSegmentType;

    //Body SubSegments  

    public Dictionary<int, BodySubSegment> IdToBodySegmentDictionary = new Dictionary<int, BodySubSegment>();

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;


    public List<Sensor> SensorList;

    private List<SensorTuple> mSensorTuple = new List<SensorTuple>();
    public BodySegmentView AssociatedView; 
    public void UpdateSensorsData()
    {


    }

    /**
    * UpdateSensorsData(BodyFrame vFrame)
    * @param BodyFrame vFrame: a body frame from which we update the current sensors
    * @brief From the passed in Parameters, update the sensors list with the frame data
    * @return 
    */
    public void UpdateSensorsData(BodyFrame vFrame)
    {
        //get the sensor 
        List<BodyStructureMap.SensorPositions> sensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[mSegmentType];
        //get subframes of data relevant to this body segment 
        foreach (BodyStructureMap.SensorPositions sensorsPos in sensorPos)
        {
            //find a suitable sensor to update
            SensorTuple vSensTuple = mSensorTuple.First(a => a.CurrentSensor.mSensorPosition == sensorsPos);
            //get the relevant data from vFrame 
            if (vFrame.MapSensorPosToValue.ContainsKey(sensorsPos))
            {
                Vector3 vDataFromFrame = vFrame.MapSensorPosToValue[sensorsPos];
                vSensTuple.CurrentSensor.SensorData.PositionalData = vDataFromFrame;
            }
        }
        UpdateSubsegments(); //update the subsegments related to this segment
    }


    /**
    *  UpdateInitialSensorsData(BodyFrame vFrame)
    * @param BodyFrame vFrame: a body frame from which we update the initial sensors
    * @brief From the passed in Parameters, update the initial sensors list with the frame data
    * @return 
    */
    public void UpdateInitialSensorsData(BodyFrame vFrame)
    {
        //get the sensor 
        List<BodyStructureMap.SensorPositions> vSensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[mSegmentType];
        //get subframes of data relevant to this body segment
        Dictionary<BodyStructureMap.SensorPositions, Vector3> vRelevantSubFrames = new Dictionary<BodyStructureMap.SensorPositions, Vector3>(2);
        foreach (BodyStructureMap.SensorPositions vSensors in vSensorPos)
        {
            //find a suitable sensor to update
            SensorTuple vSensTuple = mSensorTuple.First(a => a.InitSensor.mSensorPosition == vSensors);

            //get the relevant data from vFrame 
            if (vFrame.MapSensorPosToValue.ContainsKey(vSensors))
            {
                Vector3 vFrameData = vFrame.MapSensorPosToValue[vSensors];
                vSensTuple.InitSensor.SensorData.PositionalData = vFrameData;
            }
        }
    }
    /**
    *  UpdateTorsoSubsegment()
    * @param  
    * @brief If the current Segment is a Torso, update its respective subsegments
    * @return 
    */
    internal void UpdateTorsoSubsegment()
    {
        Quaternion vQuaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
        SensorTuple vUpperSpineTuple = mSensorTuple.First(x => x.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_UpperSpine);
        SensorTuple vLowerSpineSensorTuple = mSensorTuple.First(x => x.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_LowerSpine);

        Vector3 vUpperSpOrientation = vUpperSpineTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg;
        Vector3 vLowerSpOrientation = vLowerSpineSensorTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg;

        BodySubSegment vUpperSpSubSegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment vLowerSpSubSegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];

        //get the sensor tuple for upperspine and lowerspine    

        Quaternion vInitialUpSpOrientation = Quaternion.Euler(vUpperSpineTuple.InitSensor.SensorData.PositionalData * Mathf.Rad2Deg);
        Quaternion vInitLowerSpOrientation = Quaternion.Euler(vLowerSpineSensorTuple.InitSensor.SensorData.PositionalData * Mathf.Rad2Deg);
        Quaternion vInitInverseUpSpOrientation = Quaternion.Inverse(vInitialUpSpOrientation);
        Quaternion vInitInverseLowSpOrientation = Quaternion.Inverse(vInitLowerSpOrientation);


        Quaternion vNodQuat = Quaternion.Euler(vUpperSpOrientation);
        Quaternion vNodQuat2 = Quaternion.Euler(vLowerSpOrientation);

        Quaternion vFinalUpSpOrientation = vInitInverseUpSpOrientation * vNodQuat * Quaternion.Inverse(vQuaternionFactor);
        Quaternion vFinalLowerSpOrientation = vInitInverseLowSpOrientation * vNodQuat2 *
                                             Quaternion.Inverse(vQuaternionFactor);
        vUpperSpSubSegment.UpdateSubsegmentOrientation(vFinalUpSpOrientation);
        vLowerSpSubSegment.UpdateSubsegmentOrientation(vFinalLowerSpOrientation);
         
    }

    /**
    *  UpdateTorsoSubsegment()
    * @param  
    * @brief If the current Segment is a RightArm, update its respective subsegments
    * @return 
    */
    internal void UpdateRightArmSubsegment()
    {


        Quaternion vQuaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
        Vector3 vEulerFactor = Vector3.one;
        SensorTuple vUpperArmTuple = mSensorTuple.First(x => x.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_RightUpperArm);
        SensorTuple vLowerArmTuple = mSensorTuple.First(x => x.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_RightForeArm);


        //the current frame has been adjust by the body, but not the initial frame 

        BodySubSegment vUpperArmSubsegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];
        BodySubSegment vLowerArmSubsegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];

        //get the inverse initial rotation

        Vector3 vInitUpperArmOrientation = vUpperArmTuple.InitSensor.SensorData.PositionalData;
        vInitUpperArmOrientation.Set(vInitUpperArmOrientation.x * vEulerFactor.x, vInitUpperArmOrientation.y * vEulerFactor.y, vInitUpperArmOrientation.z * vEulerFactor.z);
        NodQuaternionOrientation vInitUANodQuat = MatrixTools.eulerToQuaternion(vInitUpperArmOrientation.x, vInitUpperArmOrientation.y, vInitUpperArmOrientation.z);
        Quaternion vInitUpperArmQuat = new Quaternion(vInitUANodQuat.x, vInitUANodQuat.y, vInitUANodQuat.z, vInitUANodQuat.w);
        Quaternion vInverseUpperArm = Quaternion.Inverse(vInitUpperArmQuat * Quaternion.Inverse(vQuaternionFactor));

        Vector3 vInitLowerArmOrientation = vLowerArmTuple.InitSensor.SensorData.PositionalData;
        NodQuaternionOrientation vInitLANodQuat = MatrixTools.eulerToQuaternion(vInitLowerArmOrientation.x, vInitLowerArmOrientation.y, vInitLowerArmOrientation.z);
        Quaternion vInitLowerArmQuat = new Quaternion(vInitLANodQuat.x, vInitLANodQuat.y, vInitLANodQuat.z, vInitLANodQuat.w);
        Quaternion vInverseLowerArm = Quaternion.Inverse(vInitUpperArmQuat * Quaternion.Inverse(vQuaternionFactor)); 

        Vector3 vCurrentUpperArmOrientation = vUpperArmTuple.CurrentSensor.SensorData.PositionalData;
        Vector3 vCurrentLowerArmOrientation = vLowerArmTuple.CurrentSensor.SensorData.PositionalData;
         
        NodQuaternionOrientation vCurrUPArmNodQuat = MatrixTools.eulerToQuaternion(vCurrentUpperArmOrientation.x, vCurrentUpperArmOrientation.y, vCurrentUpperArmOrientation.z);
        NodQuaternionOrientation vCurrLowArmNodQuat = MatrixTools.eulerToQuaternion(vCurrentLowerArmOrientation.x, vCurrentLowerArmOrientation.y, vCurrentLowerArmOrientation.z);

        Quaternion vCurrUpArmOrientation = new Quaternion(vCurrUPArmNodQuat.x, vCurrUPArmNodQuat.y, vCurrUPArmNodQuat.z, vCurrUPArmNodQuat.w);
        Quaternion vCurrLowArmOrientation = new Quaternion(vCurrLowArmNodQuat.x, vCurrLowArmNodQuat.y, vCurrLowArmNodQuat.z, vCurrLowArmNodQuat.w); 
        Quaternion vJointQuat = vInverseUpperArm * vCurrUpArmOrientation * Quaternion.Inverse(vQuaternionFactor);

        Quaternion vJointQuat2 = vInverseLowerArm * vCurrLowArmOrientation * Quaternion.Inverse(vQuaternionFactor);
        vUpperArmSubsegment.UpdateSubsegmentOrientation(vJointQuat);
        vLowerArmSubsegment.UpdateSubsegmentOrientation(vJointQuat2);
     

    }

    /**
    *  UpdateTorsoSubsegment()
    * @param  
    * @brief If the current Segment is a Left Arm, update its respective subsegments
    * @return 
    */
    internal void UpdateLeftArmSubsegment()
    {
        Quaternion vQuaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
        //get the body sub segments
        BodySubSegment vUpperLeftArmSubSegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];
        BodySubSegment vLowerLeftArmSubsegment = IdToBodySegmentDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];


        //inverse initial rotation, referencing from off NodJoint, resetJoint functions
        SensorTuple vUpperArmTuple = mSensorTuple.First(sens => sens.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_LeftUpperArm);
        SensorTuple vLowerArmTupple = mSensorTuple.First(sens => sens.InitSensor.mSensorPosition == BodyStructureMap.SensorPositions.SP_LeftForeArm);

        Vector3 vInitSensorUpArmRawEuler = vUpperArmTuple.InitSensor.SensorData.PositionalData;
        Vector3 vInitSensorForarmRawEuler = vLowerArmTupple.InitSensor.SensorData.PositionalData;

        NodQuaternionOrientation vSensorUpArmQuat = MatrixTools.eulerToQuaternion(vInitSensorUpArmRawEuler.z, vInitSensorUpArmRawEuler.x, vInitSensorUpArmRawEuler.y);
        NodQuaternionOrientation vSensorForarmQuat = MatrixTools.eulerToQuaternion(vInitSensorForarmRawEuler.z, vInitSensorForarmRawEuler.x, vInitSensorForarmRawEuler.y); 
        Quaternion vPreInitialUpArmInverseQuat = new Quaternion(vSensorUpArmQuat.x, vSensorUpArmQuat.y, vSensorUpArmQuat.z, vSensorUpArmQuat.w);
        Quaternion vInitialForearmInverseQuat = new Quaternion(vSensorForarmQuat.x, vSensorForarmQuat.y, vSensorForarmQuat.z, vSensorForarmQuat.w);

        Quaternion vInitialUpArmInverseQuat = Quaternion.Inverse(vPreInitialUpArmInverseQuat * Quaternion.Inverse(vQuaternionFactor));

        vInitialForearmInverseQuat = Quaternion.Inverse(vInitialForearmInverseQuat * Quaternion.Inverse(vQuaternionFactor));

        Vector3 vCurrentUpArmOrientation = vUpperArmTuple.CurrentSensor.SensorData.PositionalData;
        Vector3 vCurentForarmOrientation = vLowerArmTupple.CurrentSensor.SensorData.PositionalData;

        NodQuaternionOrientation vNodForArmQuat = MatrixTools.eulerToQuaternion(vCurentForarmOrientation.z, vCurentForarmOrientation.x, vCurentForarmOrientation.y);
        NodQuaternionOrientation vNodUpperArmQuat = MatrixTools.eulerToQuaternion(vCurrentUpArmOrientation.z, vCurrentUpArmOrientation.x, vCurrentUpArmOrientation.y);

        Quaternion vForeArmQuat = new Quaternion(vNodForArmQuat.x, vNodForArmQuat.y, vNodForArmQuat.z, vNodForArmQuat.w);
        Quaternion vUpArmQuat = new Quaternion(vNodUpperArmQuat.x, vNodUpperArmQuat.y, vNodUpperArmQuat.z, vNodUpperArmQuat.w);

        Quaternion vCurrentForArmOrient = vInitialForearmInverseQuat * vForeArmQuat * Quaternion.Inverse(vQuaternionFactor);
        Quaternion vCurrentUpperArmOrient = vInitialUpArmInverseQuat * vUpArmQuat * Quaternion.Inverse(vQuaternionFactor); 
        vUpperLeftArmSubSegment.UpdateSubsegmentOrientation(vCurrentUpperArmOrient);
        vLowerLeftArmSubsegment.UpdateSubsegmentOrientation(vCurentForarmOrientation);
 
    }
   
    /**
    *  InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    * @param  vSegmentType: the desired Segment Type
    * @brief Initializes a new body structure's internal properties with the desired Segment Type
    */
    internal void InitializeBodySegment(BodyStructureMap.SegmentTypes vSegmentType)
    {
        #region use of unity
        GameObject vGo = new GameObject(EnumUtil.GetName(vSegmentType));
        AssociatedView = vGo.AddComponent<BodySegmentView>();

        #endregion

        List<BodyStructureMap.SubSegmentTypes> vSubSegmentTypes =
           BodyStructureMap.Instance.SegmentToSubSegmentMap[vSegmentType];

        List<BodyStructureMap.SensorPositions> vSensorPosition =
            BodyStructureMap.Instance.SegmentToSensorPosMap[vSegmentType];

        foreach (var vSensorPos in vSensorPosition)
        {
            Sensor vNewSensor = new Sensor();
            vNewSensor.mSensorBodyID = BodyStructureMap.Instance.SensorPosToSensorIDMap[vSensorPos];
            vNewSensor.mSensorType = BodyStructureMap.Instance.SensorPosToSensorTypeMap[vSensorPos];
            vNewSensor.mSensorPosition = vSensorPos;
            //Sensors.Add(newSensor);
            SensorTuple vTuple = new SensorTuple();
            vTuple.CurrentSensor = new Sensor(vNewSensor);
            vTuple.InitSensor = new Sensor(vNewSensor);

            mSensorTuple.Add(vTuple);
            SensorList.Add(vTuple.CurrentSensor);
        }

        foreach (BodyStructureMap.SubSegmentTypes vSSTypes in vSubSegmentTypes)
        {
            BodySubSegment vSubSegment = new BodySubSegment();
            vSubSegment.SubSegmentType = vSSTypes;
            vSubSegment.InitializeBodySubsegment(vSSTypes);
            //BodySubSegments.Add(subSegment);
            IdToBodySegmentDictionary.Add((int)vSSTypes, vSubSegment);
            #region use of unity functions

            vSubSegment.AssociatedView.transform.parent = AssociatedView.transform;

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
    *  UpdateSubsegments()
    * @param 
    * @brief Updates the body segments subsegments according to the current bodysegments type
    */
    private void UpdateSubsegments()
    {
        if (mSegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso)
        {
            //    UpdateTorsoSubsegment();
        }
        if (mSegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
        {
            // UpdateRightArmSubsegment();
        }
        if (mSegmentType == BodyStructureMap.SegmentTypes.SegmentType_LeftArm)
        {
            UpdateLeftArmSubsegment();
        }
         


    }
    #endregion

}
