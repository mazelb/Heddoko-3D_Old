/** 
* @file Body.cs
* @brief Contains the Body class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/**
* Body class 
* @brief Body class (represents one body suit)
*/
public class Body : MonoBehaviour
{
    //Body Unique GUID for ease of cloud access
    public string BodyGuid;
    //Currently connected suit GUID 
    public string SuitGuid;
    //Currently playing recording GUID
    public string RecordingGuid;
    
    //Body ID
    public Int32 BodyId;
    
    //Body Type (Default is full body)
    public BodyStructureMap.BodyTypes BodyType;

    //Body Thread handle 
    private BodyFrameThread mBodyFrameThread = new BodyFrameThread();

    //Body Composition
    public List<BodySegment> BodySegments = new List<BodySegment>();

    //Current body Frame 
    public BodyFrame CurrentBodyFrame;

    //Initial body Frame
    public BodyFrame InitialBodyFrame;

    //TODO: Handling Body Pipeline stages / Body state machine

    /**
    * CreateNewBodyUUID()
    * @brief Creates a new body UUID
    */
    public void CreateNewBodyUUID()
    {
        BodyGuid = Guid.NewGuid().ToString();
    }

    /**
    * InitBody()
    * @param vBodyUUID the new body UUID (could be empty)
    * @brief Initializes a new body 
    */
    public void InitBody(string vBodyUUID/*, BodyTypes vBodyType = BodyTypes.BodyType_FullBody*/)
    {
        //Init the body UUID (given or created)
        if (string.IsNullOrEmpty(vBodyUUID))
        {
            CreateNewBodyUUID();
        }
        else
        {
            BodyGuid = vBodyUUID;
        }

        //Init all structures
        CreateBodyStructure(/*vBodyType*/);

        //TODO: Current body Frame 
        //TODO: Initial body Frame
        //TODO: body thread
        //TODO: Body Pipeline
        //TODO: Body state machine
    }

    public void CreateBodyStructure(/*BodyTypes vBodyType = BodyTypes.BodyType_FullBody*/)
    {
        //TODO: make the body creating more systematic (right now its almost hardcoded)
        //BodyType = vBodyType;

        //switch(BodyType)
        //{
        //    case BodyTypes.BodyType_FullBody:
        //        {
        //            var vSegmentTypes = EnumUtil.GetValues<BodySegment.SegmentTypes>();
        //            foreach(BodySegment.SegmentTypes vSegmentType in vSegmentTypes)
        //            {
        //                BodySegment vSegment = new BodySegment();
        //                vSegment.SegmentType = vSegmentType;
        //                vSegment.IsTracked = true;
        //                vSegmentType.InitSegment();
        //            }
        //        }
        //        break;
        //    case BodyTypes.BodyType_UpperBody:
        //        {

        //        }
        //        break;
        //    case BodyTypes.BodyType_LowerBody:
        //        {

        //        }
        //        break;
        //    case BodyTypes.BodyType_Limbs:
        //        {

        //        }
        //        break;
        //    default:
        //        //TODO: Invalid Body Type
        //        break;
        //}
    }

    public void UpdateBody(BodyFrame vFrame)
    {

    }

    public void PlayRecording(string vRecUUID)
    {
        //TODO: 
    }

    public void PauseRecording(string vRecUUID)
    {
        //TODO: 
    }

    public void StopRecording(string vRecUUID)
    {
        //TODO: 
    }
}
