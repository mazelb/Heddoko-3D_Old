﻿/** 
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
    private Vector3 mRotationFactor = new Vector3(0, -90, 0);

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
        //Debug.Log(vInitRawEuler);
        Quaternion vQuaternionFactor = Quaternion.Euler(mRotationFactor);

        //IMUQuaternionOrientation vRawOrientation = MatrixTools.eulerToQuaternion(vInitRawEuler.x, vInitRawEuler.y, vInitRawEuler.z);
        //Quaternion vConvertedQuaternion = new Quaternion(vRawOrientation.x, vRawOrientation.y, vRawOrientation.z, vRawOrientation.w);
        //mInitialInverseOrientation = Quaternion.Inverse(vConvertedQuaternion);// * Quaternion.Inverse(vQuaternionFactor));

        IMUQuaternionOrientation vIMUSubsegmentQuaternion = MatrixTools.MatToQuat(OrientationMatrix);
        Quaternion vNewInitialOrientation = new Quaternion(vIMUSubsegmentQuaternion.x, vIMUSubsegmentQuaternion.y, vIMUSubsegmentQuaternion.z, vIMUSubsegmentQuaternion.w);
        mInitialInverseOrientation = Quaternion.Inverse(vNewInitialOrientation * Quaternion.Inverse(vQuaternionFactor));

        //Debug.Log(mInitialInverseOrientation);
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
        IMUQuaternionOrientation vIMUSubsegmentQuaternion = MatrixTools.MatToQuat(OrientationMatrix);
        Quaternion vSubsegmentQuat = new Quaternion(vIMUSubsegmentQuaternion.x, vIMUSubsegmentQuaternion.y, vIMUSubsegmentQuaternion.z, vIMUSubsegmentQuaternion.w);
        Quaternion vNewOrientation = mInitialInverseOrientation * vSubsegmentQuat * Quaternion.Inverse(vQuaternionFactor);

        AssociatedView.UpdateOrientation(vNewOrientation);
    }

    /**
    * UpdateSubsegmentPosition(Vector3 vNewPosition)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentPosition(float vNewDisplacement)
    {
        AssociatedView.UpdatePosition(vNewDisplacement);
    }

    /**
    *  InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes sstype)
    * @param  sstype: the desired SubSegment Type
    * @brief Initializes a new  subsegment structure's internal properties with the desired subsegment Type 
    */
    internal void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes vSubsegmentType)
    {
        GameObject go = new GameObject(EnumUtil.GetName(vSubsegmentType));
        AssociatedView = go.AddComponent<BodySubsegmentView>();
        AssociatedView.AssociatedSubSegment = this;
        subsegmentType = vSubsegmentType;
    }
}
