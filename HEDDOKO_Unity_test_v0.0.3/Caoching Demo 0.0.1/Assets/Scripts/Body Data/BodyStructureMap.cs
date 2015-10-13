using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyStructureMap : MonoBehaviour
{
    #region Singleton definition
    private static readonly BodyRecordingsMgr instance = new BodyRecordingsMgr();

    // Explicit static constructor to tell C# compiler
    // not to mark type as beforefieldinit
    static BodyRecordingsMgr()
    {
    }

    private BodyRecordingsMgr()
    {
    }

    public static BodyRecordingsMgr Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    //Body Types
    public enum BodyTypes
    {
        BodyType_FullBody    = 0,
        BodyType_UpperBody   = 1,
        BodyType_LowerBody   = 2,
        BodyType_Limbs       = 3,
        BodyType_Count
    };

    //Segment Types
    public enum SegmentTypes
    {
        SegmentType_Torso       = 0,
        SegmentType_RightArm    = 1,
        SegmentType_LeftArm     = 2,
        SegmentType_RightLeg    = 3,
        SegmentType_LeftLeg     = 4,
        SegmentType_Count
    };

    //Sub Segment Types 
    public enum SubSegmentTypes
    {
        SubsegmentType_UpperSpine       = 0,
        SubsegmentType_LowerSpine       = 1,
        SubsegmentType_RightUpperArm    = 2,
        SubsegmentType_RightForeArm     = 3,
        SubsegmentType_LeftUpperArm     = 4,
        SubsegmentType_LeftForeArm      = 5,
        SubsegmentType_RightThigh       = 6,
        SubsegmentType_RightCalf        = 7,
        SubsegmentType_LeftThigh        = 8,
        SubsegmentType_LeftCalf         = 9,
        SubsegmentType_Count
    };

    //Sensors positions
    public enum SensorPositions
    {
        SP_UpperSpine       = 0,
        SP_LowerSpine       = 1,
        SP_RightUpperArm    = 2,
        SP_RightForeArm     = 3,
        SP_LeftUpperArm     = 4,
        SP_LeftForeArm      = 5,
        SP_RightThigh       = 6,
        SP_RightCalf        = 7,
        SP_LeftThigh        = 8,
        SP_LeftCalf         = 9,
        SP_RightElbow       = 10,
        SP_LeftElbow        = 11,
        SP_RightKnee        = 12,
        SP_LeftKnee         = 13,
        SP_SensorPositionCount
    }

    //Sensors types
    public enum SensorTypes
    {
        ST_Default      = 0, //No data in SensorData
        ST_Biomech      = 1, //Data: Yaw, Pitch, Roll
        ST_Flexcore     = 2, //Data: Flex Value
        ST_HeartRate    = 3, //Data (TBD)
        ST_SensorTypeCount
    }

    //Body structure maps
    public Dictionary<BodyTypes, List<SegmentTypes>> BodyToSegmentMap = new Dictionary<BodyTypes, List<SegmentTypes>>();
    public Dictionary<SegmentTypes, List<SubSegmentTypes>> SegmentToSubSegmentMap = new Dictionary<SegmentTypes, List<SubSegmentTypes>>();
    public Dictionary<SegmentTypes, List<SensorPositions>> SegmentToSensorPosMap = new Dictionary<SegmentTypes, List<SensorPositions>>();
    public Dictionary<SensorPositions, SensorTypes> SensorPosToSensorTypeMap = new Dictionary<SensorPositions, SensorTypes>();
    public Dictionary<SensorPositions, int> SensorPosToSensorIDMap = new Dictionary<SensorPositions, int>();

    //Build body structure maps
    public void ReadBodyStructureFile()
    {
        //TODO:

    }

    public void CreateBodyToSegmentMap()
    {
        //TODO:
        var vBodyTypes = EnumUtil.GetValues<BodyTypes>();
        foreach (BodyTypes vBodyType in vBodyTypes)
        {
            switch(vBodyType)
            {
                case BodyTypes.BodyType_FullBody:

                    //BodyToSegmentMap.
                    break;
                case BodyTypes.BodyType_UpperBody:
                    break;
                case BodyTypes.BodyType_LowerBody:
                    break;
                case BodyTypes.BodyType_Limbs:
                    break;
                default:
                    break;
            }
            BodySegment vSegment = new BodySegment();
            vSegment.SegmentType = vSegmentType;
            vSegment.IsTracked = true;
            vSegmentType.InitSegment();
        }


   

        SegmentTypes.SegmentType_Torso = 0,
        SegmentTypes.SegmentType_RightArm = 1,
        SegmentTypes.SegmentType_LeftArm = 2,
        SegmentTypes.SegmentType_RightLeg = 3,
        SegmentTypes.SegmentType_LeftLeg = 4,
    }

    public void CreateSegmentToSubSegmentMap()
    {
        //TODO:

    }

    public void CreateSegmentToSensorPosMap()
    {
        //TODO:

    }

    public void CreateSensorPosToSensorTypeMap()
    {
        //TODO:

    }

    public void CreateSensorPosToSensorIDMap()
    {
        //TODO:

    }
}
