using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BodySegment : MonoBehaviour
{
    //Segment Type 
    public BodyStructureMap.SegmentTypes SegmentType; 

    //Body SubSegments 
    public List<BodySubSegment> BodySubSegments = new List<BodySubSegment>();

    //Is segment tracked (based on body type) 
    public bool IsTracked = true;

    //TODO: List of sensors to get data access
    private List<SensorTuple> Sensors = new List<SensorTuple>();

    public void UpdateSensorsData()
    {

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

}
