

using Assets.Scripts.Body_Data.view;
using UnityEngine;

public class BodySubSegment
{
    public BodyStructureMap.SubSegmentTypes subsegmentType;
    //TODO: Sub Segment Orientation 
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    private Quaternion mSubsegmentOrientation;
    private SubSegmentOrientationType mSubsegmentOrientationType;
    public BodySubsegmentView associatedView;


    public Quaternion SubsegmentOrientation
    {
        get { return mSubsegmentOrientation; }
        set { mSubsegmentOrientation = value; }
    }

    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }
    /**
   *  UpdateSubsegmentOrientation(Vector3 vRawEuler)
   * @param   vRawEuler: the raw euler in Rads that will update the subsegment's orientation
   * @brief  Updates the subsegments orientation
   */
    public void UpdateSubsegmentOrientation(Vector3 vRawEuler)
    {
        SubsegmentOrientation = Quaternion.Euler(vRawEuler* Mathf.Rad2Deg);
        associatedView.UpdateOrientation(SubsegmentOrientation);
    }
    /**
    *  UpdateSubsegmentOrientation(Quaternion vOrientation)
    * @param  vOrientation: the orientation that will update the subsegment's orientation
    * @brief  Updates the subsegments orientation
    */
    public void UpdateSubsegmentOrientation(Quaternion vOrientation)
    {
        SubsegmentOrientation =vOrientation;
        associatedView.UpdateOrientation(SubsegmentOrientation);
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
        associatedView = go.AddComponent<BodySubsegmentView>();
        #endregion

        subsegmentType = vSubsegmentType;

    }

 
 
}
