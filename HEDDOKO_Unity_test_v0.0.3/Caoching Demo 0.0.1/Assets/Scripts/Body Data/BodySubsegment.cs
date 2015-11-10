/** 
* @file BodySubSegment.cs
* @brief Contains the BodySubSegment  class
* @author Mohammed Haider(mohamed@heddoko.com)
* @date October 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using Assets.Scripts.Body_Data.view;
using UnityEngine;
using Assets.Scripts.Utils;

/**
* BodySubSegment class 
* @brief BodySubSegment class (represents one abstracted reprensentation of a body subsegment)
*/
[Serializable]
public class BodySubSegment
{
    public BodyStructureMap.SubSegmentTypes subsegmentType;
    //TODO: Sub Segment Orientation inverseInitRotation 
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    private Quaternion mSubsegmentOrientation;
    public float[,] OrientationMatrix = MatrixTools.Identity3X3Matrix();
    [SerializeField]
    private Quaternion mInitialInverseOrientation = Quaternion.identity;
    private SubSegmentOrientationType mSubsegmentOrientationType;
    public BodySubsegmentView AssociatedView;
    [SerializeField]
    private Vector3 mRotationFactor = new Vector3(0, 90, 0);

    /**
    * SubSegmentOrientationType enum
    * @brief The type of subsegment's orientation
    */
    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }
   
    /**
    * UpdateInverseQuaternion(Vector3 vInitRawEuler)
    * @param   Vector3 vInitRawEuler:The raw euler orientation that will be used to set the inverse initial quaternion
    * @brief  Updates and resets the inverse quaternion that will be needed for the final object's tranform. 
    */
    public void UpdateInverseQuaternion(Vector3 vInitRawEuler)
    {
        AssociatedView.ResetOrientation();
        Quaternion vQuaternionFactor = Quaternion.Euler(mRotationFactor);
        NodQuaternionOrientation vRawOrientation = MatrixTools.eulerToQuaternion(vInitRawEuler.x, vInitRawEuler.y, vInitRawEuler.z);
        Quaternion vConvertedQuaternion = new Quaternion(vRawOrientation.x, vRawOrientation.y, vRawOrientation.z, vRawOrientation.w);
        mInitialInverseOrientation = Quaternion.Inverse(vConvertedQuaternion * Quaternion.Inverse(vQuaternionFactor));

        /*
        //Reset the sensors
        if (null == mNodSensor)
			return;

		Debug.Log("Reseting nod");
		initRotation = mNodSensor.rotation;
		initRotationEuler = mNodSensor.eulerRotation;

        NodGyro vNodGyro = mNodSensor.gyro;
        initGyro = new Vector3(vNodGyro.gyroX, vNodGyro.gyroY, vNodGyro.gyroZ);
        NodAccel vNodAccel = mNodSensor.acceleration;
        initAcceleration = new Vector3(vNodAccel.accelX, vNodAccel.accelY, vNodAccel.accelZ);

        curRotation = Quaternion.identity;
		curRotationEuler = Vector3.zero;
		curRotationRawEuler = Vector3.zero;
        currentAcceleration = Vector3.zero;
        currentGyro = Vector3.zero;

        Vector3 vNodRawEuler = mNodSensors[ndx].curRotationRawEuler;
        vNodRawEuler = new Vector3(vNodRawEuler.x, vNodRawEuler.y, vNodRawEuler.z);
		//vNodRawEuler = new Vector3(vNodRawEuler.x, -vNodRawEuler.y, -vNodRawEuler.z);
        vNodIniEuler.Set(vNodRawEuler.x * eulerFactor.x, vNodRawEuler.y * eulerFactor.y, vNodRawEuler.z * eulerFactor.z);
        NodQuaternionOrientation vNodRawQuat = eulerToQuaternion(vNodRawEuler.x, vNodRawEuler.y, vNodRawEuler.z);
		//NodQuaternionOrientation vNodRawQuat = eulerToQuaternion(vNodRawEuler.x, -vNodRawEuler.y, -vNodRawEuler.z);
        Quaternion vNodQuat = new Quaternion(vNodRawQuat.x, vNodRawQuat.y, vNodRawQuat.z, vNodRawQuat.w);
        inverseInitRotation = Quaternion.Inverse(vNodQuat * Quaternion.Inverse(Quaternion.Euler(quaternionFactor)));
        */
    }

    /**
    *  UpdateSubsegmentOrientation(float[,] vOrientationMatrix)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentOrientation(float[,] vaOrientationMatrix)
    {
        Quaternion vQuaternionFactor = Quaternion.Euler(mRotationFactor);
        OrientationMatrix = vaOrientationMatrix;
        //Convert to a something that unity can understand
        NodQuaternionOrientation vNodSubsegmentQuaternion = MatrixTools.MatToQuat(OrientationMatrix);
        Quaternion vSubsegmentQuat = new Quaternion(vNodSubsegmentQuaternion.x, vNodSubsegmentQuaternion.y, vNodSubsegmentQuaternion.z, vNodSubsegmentQuaternion.w);
        Quaternion vNewOrientation = mInitialInverseOrientation * vSubsegmentQuat * Quaternion.Inverse(vQuaternionFactor);

        AssociatedView.UpdateOrientation(vNewOrientation);
    }

    /**
   *  InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes sstype)
   * @param  sstype: the desired SubSegment Type
   * @brief Initializes a new  subsegment structure's internal properties with the desired subsegment Type 
   */
    internal void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes vSubsegmentType)
    {
        #region using unity functions
        GameObject go = new GameObject(EnumUtil.GetName(vSubsegmentType));
        AssociatedView = go.AddComponent<BodySubsegmentView>();
        AssociatedView.AssociatedSubSegment = this;
        #endregion

        subsegmentType = vSubsegmentType;

    }



}
