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
    public enum BodyTypes
    {
        BodyType_FullBody = 0,
        BodyType_UpperBody = 1,
        BodyType_LowerBody = 2,
        BodyType_Limbs = 3,
        BodyType_Count
    };

    //Body Unique GUID for ease of cloud access
    public string BodyGuid;
    //Currently connected suit GUID 
    public string SuitGuid;
    //Currently playing recording GUID
    public string RecordingGuid;
    
    //Body ID
    public Int32 BodyId;
    //Body Type (Default is full body)
    public BodyTypes BodyType = BodyTypes.BodyType_FullBody;

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
    public void InitBody(string vBodyUUID, BodyTypes vBodyType = BodyTypes.BodyType_FullBody)
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
        CreateBodyStructure(vBodyType);

        //TODO: Current body Frame 
        //TODO: Initial body Frame
        //TODO: body thread
        //TODO: Body Pipeline
        //TODO: Body state machine
    }

    public void CreateBodyStructure(BodyTypes vBodyType = BodyTypes.BodyType_FullBody)
    {
        BodyType = vBodyType;

        switch(BodyType)
        {
            case BodyTypes.BodyType_FullBody:
                {

                }
                break;
            case BodyTypes.BodyType_UpperBody:
                {

                }
                break;
            case BodyTypes.BodyType_LowerBody:
                {

                }
                break;
            case BodyTypes.BodyType_Limbs:
                {

                }
                break;
            default:
                //TODO: Invalid Body Type
                break;
        }
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
