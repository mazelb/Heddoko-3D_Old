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
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    public BodyStructureMap.SubSegmentTypes subsegmentType;
    private Quaternion mSubsegmentOrientation;
    public float[,] OrientationMatrix = MatrixTools.Identity3X3Matrix();
    [SerializeField]
    private Quaternion mInitialInverseOrientation = Quaternion.identity;
    private SubSegmentOrientationType mSubsegmentOrientationType;
    public BodySubsegmentView AssociatedView;
    [SerializeField]
    private Vector3 mRotationFactor = new Vector3(0, 0, 0);

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

    /// <summary>
    /// Resets the orientations of the associated view
    /// </summary>
    public void ResetViewOrientation()
    {
        AssociatedView.ResetOrientation();
    }


    /**
    * UpdateInverseQuaternion(Vector3 vInitRawEuler)
    * @param   Vector3 vInitRawEuler:The raw euler orientation that will be used to set the inverse initial quaternion
    * @brief  Updates and resets the inverse quaternion that will be needed for the final object's tranform. 
    */
    public void UpdateInverseQuaternion(Vector3 vInitRawEuler)
    {
        //update the view
        ResetViewOrientation();
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
        Quaternion vNewOrientation = vSubsegmentQuat;

        //update the view
        AssociatedView.UpdateOrientation(vNewOrientation);
    }

    /**
    * UpdateSubsegmentPosition(Vector3 vNewPosition)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentPosition(float vNewDisplacement)
    {
        //update the view
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
