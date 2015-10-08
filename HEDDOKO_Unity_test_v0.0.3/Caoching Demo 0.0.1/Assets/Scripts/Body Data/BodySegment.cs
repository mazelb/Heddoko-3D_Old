using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BodySegment : MonoBehaviour
{
    //Segment Types
    public enum SegmentTypes
    {
        SegmentType_Torso = 0,
        SegmentType_RightArm = 1,
        SegmentType_LeftArm = 2,
        SegmentType_RightLeg = 3,
        SegmentType_LeftLeg = 4,
        SegmentType_Count
    };

    //Segment Type 
    public SegmentTypes SegmentType = SegmentTypes.SegmentType_Count; 

    //Body SubSegments 
    public List<BodySubSegment> BodySubSegments = new List<BodySubSegment>();

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;

    //TODO: List of sensors to get data access
    private List<SensorTuple> Sensors = new List<SensorTuple>();


    public void InitSegment(SegmentTypes vSegType)
    {
        SegmentType = vSegType;

        switch (SegmentType)
        {
            case SegmentTypes.SegmentType_Torso:
                {

                }
                break;
            case SegmentTypes.SegmentType_RightArm:
                {

                }
                break;
            case SegmentTypes.SegmentType_LeftArm:
                {

                }
                break;
            case SegmentTypes.SegmentType_RightLeg:
                {

                }
                break;
            case SegmentTypes.SegmentType_LeftLeg:
                {

                }
                break;
            default:
                //TODO: Invalid Body Type
                break;
        }
    }

}
