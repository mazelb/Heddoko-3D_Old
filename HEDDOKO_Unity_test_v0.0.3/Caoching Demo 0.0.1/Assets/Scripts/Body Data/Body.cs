/** 
* @file Body.cs
* @brief Contains the Body class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Utils;
using System.Linq;
using Assets.Scripts.Body_Pipeline.Tracking;

/**
* Body class 
* @brief Body class (represents one body suit)
*/
[Serializable]
public class Body
{
    [SerializeField]
    //Body Unique GUID for ease of cloud access
    public string BodyGuid;
    [SerializeField]
    //Currently connected suit GUID 
    public string SuitGuid;
    //Currently playing recording``` GUIDCurrentBodyFrame
    //Body Composition
    [SerializeField]
    public List<BodySegment> BodySegments = new List<BodySegment>();

    [SerializeField]
    //Current body Frame 
    public BodyFrame CurrentBodyFrame;
    [SerializeField]
    //Initial body Frame
    public BodyFrame InitialBodyFrame;
    private BodyFrameThread mBodyFrameThread = new BodyFrameThread();
    private TrackingThread mTrackingThread;

    //view associated with this model
    #region properties
    private BodyView mView;
    /**
    * View
    * @param 
    * @brief View associated with this body
    * @note: a new gameobject is created and this Body is added into it as a component
    * @return returns the view associated with this body
    */
    public BodyView View
    {
        get
        {
            if (mView == null)
            {
                GameObject viewGO = new GameObject("body view " + BodyGuid);
                mView = viewGO.AddComponent<BodyView>();
                mView.AssociatedBody = this;
            }
            return mView;
        }

    }

    /**
    * MBodyFrameThread
    * @param 
    * @brief BodyFrameThread needed to start updated body 
    * @return returns the view associated with this body
    */
    public BodyFrameThread MBodyFrameThread
    {
        get
        {
            if (mBodyFrameThread == null)
            {
                mBodyFrameThread = new BodyFrameThread();
            }
            return mBodyFrameThread;
        }
    }
    #endregion


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
    public void InitBody(string vBodyUUID)
    {
        InitBody(vBodyUUID, BodyStructureMap.BodyTypes.BodyType_FullBody);
    }
    /**
 * InitBody(string vBodyUUID , BodyStructureMap.BodyTypes vBodyType)
 * @param vBodyUUID the new body UUID (could be empty), BodyType is the desired BodyType
 * @brief Initializes a new body with a certain body type
 */
    public void InitBody(string vBodyUUID, BodyStructureMap.BodyTypes vBodyType)
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
        //TODO: body thread
        //TODO: Body Pipeline
        //TODO: Body state machine
    }

    /**
    * CreateBodyStructure(BodyStructureMap.BodyTypes vBodyType )
    * @param  vBodyType: the desired BodyType
    * @brief Initializes a new body structure's internal properties with the desired body type
    */
    public void CreateBodyStructure(BodyStructureMap.BodyTypes vBodyType)
    {
        List<BodyStructureMap.SegmentTypes> vSegmentList = BodyStructureMap.Instance.BodyToSegmentMap[vBodyType]; //Get the list of segments from the bodystructuremap 
        foreach (BodyStructureMap.SegmentTypes type in vSegmentList)
        {
            BodySegment vSegment = new BodySegment();
            vSegment.SegmentType = type;
            vSegment.InitializeBodySegment(type);
            vSegment.ParentBody = this;
            BodySegments.Add(vSegment);
            #region using unity functions
            vSegment.AssociatedView.transform.parent = View.transform;
            #endregion
        }
    }
    /**
    * UpdateBody(BodyFrame vFrame )
    * @param vFrame, the body frame, setCurrentBodyFrame: sets the passed in body frame as the current bodyframe
    * @brief  Set the current body frame from the passed in parameter 
    */
    public void UpdateBody(BodyFrame vFrame)
    {
        CurrentBodyFrame = vFrame;
        //Tracking();
        for (int i = 0; i < BodySegments.Count; i++)
        {
            BodySegments[i].UpdateSensorsData(vFrame);
        }
    }
    /**
    * SetInitialFrame(BodyFrame vInitialFrame)
    * @param BodyFrame vInitialFrame, sets the initial frame, subsequently the initial orientations point of the body's subsegment
    * @brief  Set the current body frame from the passed in parameter
    */
    public void SetInitialFrame(BodyFrame vInitialFrame)
    {
        InitialBodyFrame = vInitialFrame;
        for (int i = 0; i < BodySegments.Count; i++)
        {
            BodySegments[i].UpdateInitialSensorsData(InitialBodyFrame);
        } 
    }
 
    /**
    * PlayRecording(string vRecUUID)
    * @param vRecUUID, the recording UUID
    * @brief  Play a recording from the given recording UUID. 
    */
    public void PlayRecording(string vRecUUID)
    {
        //get the raw frames from recording 
        //first try to get the recording from the recording manager. 
        BodyFramesRecording bodyFramesRec = BodyRecordingsMgr.Instance.GetRecordingByUUID(vRecUUID);

        if (bodyFramesRec != null && bodyFramesRec.RecordingRawFrames.Count > 0)
        {
            BodyFrameBuffer vBuffer1 = new BodyFrameBuffer();
            TrackingBuffer vBuffer2 = new TrackingBuffer();

            mBodyFrameThread = new BodyFrameThread(bodyFramesRec.RecordingRawFrames, vBuffer1);
            mTrackingThread = new TrackingThread(this, vBuffer1, vBuffer2);
            //get the first frame and set it as the initial frame
            BodyFrame frame = BodyFrame.ConvertRawFrame(bodyFramesRec.RecordingRawFrames[0]);
            SetInitialFrame(frame);
            View.Init(this, vBuffer2);
            mBodyFrameThread.Start();
            mTrackingThread.Start();
            View.StartUpdating = true;
        }
    }
    /**
    * ApplyTracking(Body vBody)
    * @param Body vBody: The body to apply tracking to. 
    * @brief  Applies tracking on the requested body. 
    */
    public static void ApplyTracking(Body vBody)
    {
        //get a collection of transformation matrices
        Dictionary<BodyStructureMap.SensorPositions, float[,]> vDic = GetTracking(vBody);
        //get the list of segments of the speicied vBody
        List<BodySegment> vListBodySegments = vBody.BodySegments;
        foreach (BodySegment vBodySegment in vListBodySegments)
        {
            //of the current body segment, get the appropriate subsegments
            List<BodyStructureMap.SensorPositions> vSensPosList =
                BodyStructureMap.Instance.SegmentToSensorPosMap[vBodySegment.SegmentType];
            //create a Dictionary of BodyStructureMap.SensorPositions, float[,] , which will be passed
            //to the segment
            Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary = new Dictionary<BodyStructureMap.SensorPositions, float[,]>(2);

            foreach (BodyStructureMap.SensorPositions vSenPos in vSensPosList)
            {
                if (vDic.ContainsKey(vSenPos))
                {
                    float[,] vTrackedMatrix = vDic[vSenPos];
                    vFilteredDictionary.Add(vSenPos, vTrackedMatrix);
                }
            }
            vBodySegment.UpdateSegment(vFilteredDictionary);
        }
    }
    /**
    * ApplyTracking(Body vBody)
    * @param Body vBody: The body to apply tracking to. 
    * @brief  Applies tracking on the requested body. 
    */
    public static void ApplyTracking(Body vBody, Dictionary<BodyStructureMap.SensorPositions, float[,]> vDic)
    {
        
        //get the list of segments of the speicied vBody
        List<BodySegment> vListBodySegments = vBody.BodySegments;
        foreach (BodySegment vBodySegment in vListBodySegments)
        {
            //of the current body segment, get the appropriate subsegments
            List<BodyStructureMap.SensorPositions> vSensPosList =
                BodyStructureMap.Instance.SegmentToSensorPosMap[vBodySegment.SegmentType];
            //create a Dictionary of BodyStructureMap.SensorPositions, float[,] , which will be passed
            //to the segment
            Dictionary<BodyStructureMap.SensorPositions, float[,]> vFilteredDictionary = new Dictionary<BodyStructureMap.SensorPositions, float[,]>(2);

            foreach (BodyStructureMap.SensorPositions vSenPos in vSensPosList)
            {
                if (vDic.ContainsKey(vSenPos))
                {
                    float[,] vTrackedMatrix = vDic[vSenPos];
                    vFilteredDictionary.Add(vSenPos, vTrackedMatrix);
                }
            }
            vBodySegment.UpdateSegment(vFilteredDictionary);
        }
    }
    /**
    * GetTracking()
    * @brief  Play a recording from the given recording UUID. 
    * @return Returns a dictionary and their respective   transformation matrix
    */
    public static Dictionary<BodyStructureMap.SensorPositions, float[,]> GetTracking(Body vBody)
    {
        Dictionary<BodyStructureMap.SensorPositions, float[,]> vDic = new Dictionary<BodyStructureMap.SensorPositions, float[,]>(9);
        //from the current frame, get the list of keys and apply the tracking algorithm on individual subframes
        List<BodyStructureMap.SensorPositions> vKeyList = new List<BodyStructureMap.SensorPositions>(vBody.CurrentBodyFrame.FrameData.Keys);
        for (int i = 0; i < vKeyList.Count; i++)
        {
            BodyStructureMap.SensorPositions vKey = vKeyList[i];
            Vector3 vInitialRawEuler = vBody.InitialBodyFrame.FrameData[vKey];
            Vector3 vCurrentRawEuler = vBody.CurrentBodyFrame.FrameData[vKey];
            //get the current value

            //if (vKey == BodyStructureMap.SensorPositions.SP_LowerSpine)
            //{
            //    vInitialRawEuler = vBody.InitialBodyFrame.FrameData[BodyStructureMap.SensorPositions.SP_UpperSpine];
            //    vCurrentRawEuler = vBody.CurrentBodyFrame.FrameData[BodyStructureMap.SensorPositions.SP_UpperSpine]; 
            //}

            Vector3 vInitRawEuler = new Vector3(vInitialRawEuler.x, vInitialRawEuler.y, vInitialRawEuler.z);
            Vector3 vCurrRawEuler = new Vector3(vCurrentRawEuler.x, vCurrentRawEuler.y, vCurrentRawEuler.z);

            float[,] vInitGlobalMatrix = MatrixTools.RotationGlobal(vInitRawEuler.z, vInitRawEuler.x, vInitRawEuler.y);
            float[,] vCurrentLocalMatrix = MatrixTools.RotationLocal(vCurrRawEuler.z, vCurrRawEuler.x, vCurrRawEuler.y);
            float[,] vOrientationMatrix = MatrixTools.multi(vInitGlobalMatrix, vCurrentLocalMatrix);
            vDic.Add(vKey, vOrientationMatrix);
        }
        return vDic;
    }

    /**
    *  GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes vSegmentType)
    * @param  BodyStructureMap.SegmentTypes vSegmentType the interested segment the caller wants
    * @brief  Based off the passed parameter, return a segment of that type
    * @param  Returns a segment from body that is of type vSegmentType
    */
    public BodySegment GetSegmentFromSegmentType(BodyStructureMap.SegmentTypes vSegmentType)
    {
        BodySegment vSegment = BodySegments.First(x => x.SegmentType == vSegmentType);
        return vSegment;
    }
    public void PauseRecording(string vRecUUID)
    {
        //TODO: 
    }

    public void StopRecording(string vRecUUID)
    {
        //TODO: 
    }

    /**
    * StopThread()
    * @param Stops the current thread
    * @brief  Play a recording from the given recording UUID. 
    */
    internal void StopThread()
    {
        if (mBodyFrameThread != null)
        {
            mBodyFrameThread.StopThread();
        }
        if (mTrackingThread != null)
        {
            mTrackingThread.StopThread();
        }
    }
    #region Unity functions


    #endregion






}
