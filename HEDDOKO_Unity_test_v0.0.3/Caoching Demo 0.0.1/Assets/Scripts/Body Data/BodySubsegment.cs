
/** 
* @file BodySubSegment.cs
* @brief Contains the BodySubSegment class, and enum SubSegmentOrientationType as wellthe functionalities required to produce an abstract representation of a subsegment  object
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
using Assets.Scripts.Body_Data.view;
using UnityEngine;
	/**
    * BodySubSegment  class 
    * @brief BodySubSegment class (represents one subsegment)
    */
public class BodySubSegment
{
    public BodyStructureMap.SubSegmentTypes SubSegmentType;
    //TODO: Sub Segment Orientation 
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    private Quaternion mSubSementOrient;
    private SubSegmentOrientationType mSubSegmentOrientationType;
    public BodySubsegmentView AssociatedView;


    public Quaternion SubsegmentOrientation
    {
        get { return mSubSementOrient; }
        set { mSubSementOrient = value; }
    }
    /**
    * SubSegmentOrientationType  enum 
    * @brief Represents a type of Subsegment orientation type
    */
    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }
 
	/**
    *  UpdateSubsegmentOrientation(Vector3 vInput)
    * @param  vInput: a orientation in Euler form whose components are in Radian format
    * @brief Updates the subsegments orientation with the passed in parameter. 
    */
    public void UpdateSubsegmentOrientation(Vector3 vInput)
    {
        SubsegmentOrientation = Quaternion.Euler(vInput* Mathf.Rad2Deg);
        AssociatedView.UpdateOrientation(SubsegmentOrientation);
    }
     /**
    *  UpdateSubsegmentOrientation(Quaternion vQuaternion)
    * @param  vInput: a orientation in Quaternion form
    * @brief Updates the subsegments orientation with the passed in parameter. 
    */
    public void UpdateSubsegmentOrientation(Quaternion vQuaternion)
    {
		SubsegmentOrientation =vQuaternion;
        AssociatedView.UpdateOrientation(SubsegmentOrientation);
    }
    
    /**
    *  InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes sstype)
    * @param  sstype: the desired SubSegment Type
    * @brief Initializes a new  subsegment structure's internal properties with the desired subsegment Type 
    */
    internal void InitializeBodySubsegment(BodyStructureMap.SubSegmentTypes sstype)
    {
        #region using unity functions
        GameObject vGo = new GameObject(EnumUtil.GetName(sstype));
        AssociatedView = vGo.AddComponent<BodySubsegmentView>();
        #endregion

        SubSegmentType = sstype;

    }

 
 
}
