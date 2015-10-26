using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Body_Data.view;

public class BodySegment
{
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType;

    //Body SubSegments 
    //  public List<BodySubSegment> BodySubSegments = new List<BodySubSegment>();

    public Dictionary<int, BodySubSegment> BodySubSegmentsDictionary = new Dictionary<int, BodySubSegment>();

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;

   
    public List<Sensor> sensorList; 
 
    private List<SensorTuple> SensorsTuple = new List<SensorTuple>();
    public BodySegmentView AssociatedView;
     

    public void UpdateSensorsData()
    {
       

    }
    /// <summary>
    /// Update sensors information with reference to the given frame of data
    /// </summary>
    /// <param name="vFrame">Body frame of data </param>
    public void UpdateSensorsData(BodyFrame vFrame)
    {
        //get the sensor 
        List<BodyStructureMap.SensorPositions> sensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];
        //get subframes of data relevant to this body segment 
        foreach (BodyStructureMap.SensorPositions sensorsPos in sensorPos)
        {
            //find a suitable sensor to update
            SensorTuple sensTuple = SensorsTuple.First(a => a.CurrentSensor.SensorPosition == sensorsPos); 
            //get the relevant data from vFrame 
            if (vFrame.FrameData.ContainsKey(sensorsPos))
            {
                Vector3 data = vFrame.FrameData[sensorsPos];
                sensTuple.CurrentSensor.SensorData.PositionalData = data;
            }
        }
        UpdateSubsegments(); //update the subsegments related to this segment
    }


    /// <summary>
    /// Update the initial sensors information 
    /// </summary>
    /// <param name="vFrame">a BodyFrame to set initial sensors</param>
    public void UpdateInitialSensorsData(BodyFrame vFrame)
    {
        //get the sensor 
        List<BodyStructureMap.SensorPositions> sensorPos = BodyStructureMap.Instance.SegmentToSensorPosMap[SegmentType];
        //get subframes of data relevant to this body segment
        Dictionary<BodyStructureMap.SensorPositions, Vector3> relevantSubframes = new Dictionary<BodyStructureMap.SensorPositions, Vector3>(2);
        foreach (BodyStructureMap.SensorPositions sensorsPos in sensorPos)
        {
            //find a suitable sensor to update
            SensorTuple sensTuple = SensorsTuple.First(a => a.InitSensor.SensorPosition == sensorsPos);

            //get the relevant data from vFrame 
            if (vFrame.FrameData.ContainsKey(sensorsPos))
            {
                Vector3 data = vFrame.FrameData[sensorsPos];

                sensTuple.InitSensor.SensorData.PositionalData = data;
            } 
        }
    }
    /// <summary>
    /// updates torso orientation
    /// </summary>
    internal void UpdateTorsoSubsegment()
    {
        Quaternion quaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
 
 
        SensorTuple upperSpineTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_UpperSpine);
        SensorTuple lowerSpineTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_LowerSpine);
 
        Vector3 upperSpOrientation = upperSpineTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg;
        Vector3 lowerSpOrientation = lowerSpineTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg;
    
        BodySubSegment upperSpineSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_UpperSpine];
        BodySubSegment lowerSpineSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LowerSpine];
      
        //get the sensor tuple for upperspine and lowerspine    

        Quaternion initUpperSpineQuaternion = Quaternion.Euler(upperSpineTuple.InitSensor.SensorData.PositionalData * Mathf.Rad2Deg);
        Quaternion initLowerSpineQuaternion = Quaternion.Euler(lowerSpineTuple.InitSensor.SensorData.PositionalData * Mathf.Rad2Deg);
        Quaternion initInverseUpperSpineRotation = Quaternion.Inverse(initUpperSpineQuaternion);
        Quaternion initialInverseLowerSpineRotation = Quaternion.Inverse(initLowerSpineQuaternion);


        Quaternion vNodQuat = Quaternion.Euler(upperSpOrientation);
        Quaternion vNodQuat2 = Quaternion.Euler(lowerSpOrientation);

        Quaternion finalUpperSpOrientation = initInverseUpperSpineRotation*vNodQuat*Quaternion.Inverse(quaternionFactor);
        Quaternion finalLowerSpOrientation = initialInverseLowerSpineRotation*vNodQuat2*
                                             Quaternion.Inverse(quaternionFactor);
        upperSpineSubsegment.UpdateSubsegmentOrientation(finalUpperSpOrientation);
        lowerSpineSubsegment.UpdateSubsegmentOrientation(finalLowerSpOrientation);
  

        //todo update legs
/*
        float vUpperLegLength = 1f;
        float vLowerLegLength = 1f;
        float vFootHeight = 0.1f;

        float vRightLegHeight = NodJointLegRight.RightLegMovement(vUpperLegLength, vLowerLegLength);
        float vLeftLegHeight = NodJointLegLeft.LeftLegMovement(vUpperLegLength, vLowerLegLength);
        float vLegMovement;

        if (vRightLegHeight > vLeftLegHeight)
        {
            vLegMovement = vRightLegHeight;
        }
        else
        {
            vLegMovement = vLeftLegHeight;
        }
        Vector3 v3 = lowerSpineSubsegment.AssociatedView.transform.localPosition;
        //*****To move the body down when sitting uncomment next line****#1#/
        //v3.y = vLegMovement+vFootHeight;
        lowerSpineSubsegment.AssociatedView.transform.localPosition = v3;*/
 

    }
    /// <summary>
    /// Updates the right arm subsegment from the available sensor data
    /// </summary>

    internal void UpdateRightArmSubsegment()
    {
        Quaternion quaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
        SensorTuple upperArmTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_RightUpperArm);
        SensorTuple lowerArmTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_RightForeArm);

        BodySubSegment upperRightArmSubSegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightUpperArm];

        BodySubSegment lowerRightArmSubSegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_RightForeArm];
        //todo: angle arm extraction
        Quaternion initUpperArmQuat = Quaternion.Euler(upperArmTuple.InitSensor.SensorData.PositionalData * Mathf.Rad2Deg); //initial upper arm quaternion  
        Quaternion initLowerArmQuat = Quaternion.Euler(lowerArmTuple.InitSensor.SensorData.PositionalData); //initial lower arm quaternion 

        Quaternion upperArmOrientation = Quaternion.Euler(upperArmTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg); //current upper arm quaternion
        Quaternion lowerArmOrientation = Quaternion.Euler(lowerArmTuple.CurrentSensor.SensorData.PositionalData * Mathf.Rad2Deg); //current lower arm quaternion 

        Quaternion inverseInitRotation = Quaternion.Inverse(initUpperArmQuat * Quaternion.Inverse(quaternionFactor));


        //get the current torso orientation for shoulder angle extraction, get from the list of sensors
        Vector3 torsoEulerOrientation =
            sensorList.Find(x => x.SensorPosition == BodyStructureMap.SensorPositions.SP_UpperSpine).SensorData.PositionalData;
        Quaternion torsoOrientation = Quaternion.Euler(torsoEulerOrientation);

        Quaternion vJointQuat = inverseInitRotation * upperArmOrientation * Quaternion.Inverse(quaternionFactor);

        Quaternion vJointQuat2 = inverseInitRotation * lowerArmOrientation * Quaternion.Inverse(quaternionFactor);
        upperRightArmSubSegment.UpdateSubsegmentOrientation(vJointQuat);
        lowerRightArmSubSegment.UpdateSubsegmentOrientation(vJointQuat2);
        //update current body segments

    }
    /// <summary>
    /// Updates the left arm subsegment from the available sensor data
    /// </summary>

    internal void UpdateLeftArmSubsegment()
    {
        Quaternion quaternionFactor = Quaternion.Euler(new Vector3(0, 90, 0));
        SensorTuple upperArmTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_LeftUpperArm);
        SensorTuple lowerArmTuple = SensorsTuple.First(x => x.InitSensor.SensorPosition == BodyStructureMap.SensorPositions.SP_LeftForeArm);

        BodySubSegment upperleftArmSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftUpperArm];

        BodySubSegment lowerleftArmSubsegment = BodySubSegmentsDictionary[(int)BodyStructureMap.SubSegmentTypes.SubsegmentType_LeftForeArm];
        //todo: angle arm extraction
        Quaternion initUpperArmQuat = Quaternion.Euler(upperArmTuple.InitSensor.SensorData.PositionalData); //initial upper arm quaternion  
        Quaternion initLowerArmQuat = Quaternion.Euler(lowerArmTuple.InitSensor.SensorData.PositionalData); //initial lower arm quaternion 

        Quaternion upperArmOrientation = Quaternion.Euler(upperArmTuple.CurrentSensor.SensorData.PositionalData); //current upper arm quaternion
        Quaternion lowerArmOrientation = Quaternion.Euler(lowerArmTuple.CurrentSensor.SensorData.PositionalData); //current lower arm quaternion 

        Quaternion inverseInitRotation = Quaternion.Inverse(initUpperArmQuat * Quaternion.Inverse(quaternionFactor));


        //get the current torso orientation for shoulder angle extraction, get from the list of sensors
        Vector3 torsoEulerOrientation =
            sensorList.Find(x => x.SensorPosition == BodyStructureMap.SensorPositions.SP_UpperSpine).SensorData.PositionalData;
        Quaternion torsoOrientation = Quaternion.Euler(torsoEulerOrientation);

        Quaternion vJointQuat = inverseInitRotation * upperArmOrientation * Quaternion.Inverse(quaternionFactor);

        Quaternion vJointQuat2 = inverseInitRotation * lowerArmOrientation * Quaternion.Inverse(quaternionFactor);
        upperleftArmSubsegment.UpdateSubsegmentOrientation(vJointQuat);
        lowerleftArmSubsegment.UpdateSubsegmentOrientation(vJointQuat2);
        //update current body segments

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
            sensorList.Add(tuple.CurrentSensor);
        }

        foreach (BodyStructureMap.SubSegmentTypes sstype in subsegmentTypes)
        {
            BodySubSegment subSegment = new BodySubSegment();
            subSegment.SubSegmentType = sstype;
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
    /// <summary>
    /// Updates the subsegments
    /// </summary>
    private void UpdateSubsegments()
    {
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_Torso)
        {
        //    UpdateTorsoSubsegment();
        }
        if (SegmentType == BodyStructureMap.SegmentTypes.SegmentType_RightArm)
        {
            UpdateRightArmSubsegment();
        }
        //from the list of current subsegments we have, find the subsegments that correspond to the current sensor from the sensor tuples list
        //get the integer value of the keyvalue pair in the subsegment dictionary
        /*foreach (KeyValuePair<int, BodySubSegment> segmentPair in BodySubSegmentsDictionary)
        {
            //find the corresponding sensor to this subsegment by looking up the integer value of its segment type
            int potentialKey = (int)segmentPair.Value.SubSegmentType;
            //get the potential value from the list of sensortuples
            SensorTuple sensTuple = SensorsTuple.First(a => a.CurrentSensor.SensorPosition == (BodyStructureMap.SensorPositions)potentialKey);
            //update the orientation of this subsegment
            BodySubSegmentsDictionary[segmentPair.Key].UpdateSubsegmentOrientation(sensTuple.CurrentSensor.SensorData.PositionalData);
        }*/


    }
    #endregion

}
