

using Assets.Scripts.Body_Data.view;
using UnityEngine;

public class BodySubSegment
{
    public BodyStructureMap.SubSegmentTypes SubSegmentType;
    //TODO: Sub Segment Orientation 
    //TODO: Sub Segment Orientation Type (Raw-Tracked-fused-mapped)
    private Quaternion subsegmentOrientation;
    private SubSegmentOrientationType subSegmentOrientationType;
    public BodySubsegmentView AssociatedView;


    public Quaternion SubsegmentOrientation
    {
        get { return subsegmentOrientation; }
        set { subsegmentOrientation = value; }
    }

    public enum SubSegmentOrientationType
    {
        Fused = 0,
        NonFused = 1,
        MappedTransformation
    }
    /// <summary>
    /// Updates the Subsegment's orientation from the given input
    /// </summary>
    /// <param name="input"></param>
    public void UpdateSubsegmentOrientation(Vector3 input)
    {
        SubsegmentOrientation = Quaternion.Euler(input* Mathf.Rad2Deg);
        AssociatedView.UpdateOrientation(SubsegmentOrientation);
    }

    public void UpdateSubsegmentOrientation(Quaternion q)
    {
        SubsegmentOrientation =q;
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
        GameObject go = new GameObject(EnumUtil.GetName(sstype));
        AssociatedView = go.AddComponent<BodySubsegmentView>();
        #endregion

        SubSegmentType = sstype;

    }

 
 
}
