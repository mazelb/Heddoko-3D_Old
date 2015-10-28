/** 
* @file Body.cs
* @brief Contains the Body class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/
using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Body_Data.view;
using Assets.Scripts.Utils;

/**
* Body class 
* @brief Body class (represents one body suit)
*/
public class Body
{
    //Body Unique GUID for ease of cloud access
    public string BodyGuid;
    //Currently connected suit GUID 
    public string SuitGuid;

    //Body Composition
    public List<BodySegment> BodySegments = new List<BodySegment>();

    //Current body Frame 
    public BodyFrame CurrentBodyFrame;

    //Initial body Frame
    public BodyFrame InitialBodyFrame;
    private BodyFrameThread mBodyFrameThread = new BodyFrameThread();

    //view associated with this model
    private BodyView mView;

    #region properties

    /**
    * mBodyView
    * @param 
    * @brief View associated with this body
    * @note: a new gameobject is created and this Body is added into it as a compnent
    * @return returns the view associated with this body
    */
    public BodyView BodyView
    {
        get
        {
            if (mView == null)

            {
                GameObject viewGO = new GameObject("body view " + BodyGuid);
                mView = viewGO.AddComponent<BodyView>();
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
        List<BodyStructureMap.SegmentTypes> segmentList = BodyStructureMap.Instance.BodyToSegmentMap[vBodyType]; //Get the list of segments from the bodystructuremap
        List<Sensor> sensors = new List<Sensor>(); //create a list of sensors that will be shared by all the segments
        foreach (BodyStructureMap.SegmentTypes type in segmentList)
        {
            BodySegment vSegment = new BodySegment();
            vSegment.mSegmentType = type;
            vSegment.SensorList = sensors;
            vSegment.InitializeBodySegment(type);
            BodySegments.Add(vSegment);
            #region using unity functions
            vSegment.AssociatedView.transform.parent = BodyView.transform;
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
        Tracking();
        for (int i = 0; i < BodySegments.Count; i++)
        {
            BodySegments[i].UpdateSensorsData(CurrentBodyFrame);
        }
    }


    /**
    * SetInitialFrame(BodyFrame vInitialFrame)
    * @param vInitialFrame, sets the initial frame, subsequently the initial orientations point of the body's subsegment
    * @brief  Set the current body frame from the passed in parameter
    */
    public void SetInitialFrame(BodyFrame vInitialFrame)
    {
        InitialBodyFrame = vInitialFrame;
        for (int i = 0; i < BodySegments.Count; i++)
        {
            BodySegments[i].UpdateInitialSensorsData(vInitialFrame);
        }
    }

    /**
     * Tracking( )
     * @param Tracking function within the pipeline
     * @brief  Tracking transforms the raw data from the sensors into 9 transformation matrices that will be ultimately applied to 9 body joints.  
     * It also applies the adjustments necessary to the IMU sensors local coordinate system
     */
    private void Tracking()
    {
        //get the list of keys from the current body frame
        List<BodyStructureMap.SensorPositions> keys = new List<BodyStructureMap.SensorPositions>(CurrentBodyFrame.MapSensorPosToValue.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            Vector3 vCurrentValue = CurrentBodyFrame.MapSensorPosToValue[keys[i]];
            Vector3 vInitialValue = InitialBodyFrame.MapSensorPosToValue[keys[i]];  
            

            float[,] vInitialRotation = MatrixTools.RotationGlobal(vInitialValue.z, vInitialValue.x, vInitialValue.y);
            float[,] vCurrentRotMat = MatrixTools.RotationLocal(vCurrentValue.z, vCurrentValue.x, vCurrentValue.y);
            float[,] vOrientationMatrix = MatrixTools.multi(vInitialRotation, vCurrentRotMat);
            NodQuaternionOrientation vNodQuaternion = MatrixTools.MatToQuat(vOrientationMatrix);
            Quaternion vQuatOrientation = new Quaternion(vNodQuaternion.x, vNodQuaternion.y, vNodQuaternion.z, vNodQuaternion.w);
            Vector3 vnewVal = vQuatOrientation.eulerAngles * Mathf.Deg2Rad; //make sure to transform it back to a radian to keep the data consistent with calculations 

            CurrentBodyFrame.MapSensorPosToValue[keys[i]] = vnewVal;
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
        BodyFramesRecording vBodyFramesRec = BodyRecordingsMgr.Instance.GetRecordingByUUID(vRecUUID);

        if (vBodyFramesRec != null && vBodyFramesRec.RecordingRawFrames.Count > 0)
        {
            mBodyFrameThread = new BodyFrameThread(vBodyFramesRec.RecordingRawFrames);
            //get the first frame and set it as the initial frame
            BodyFrame vFrame = BodyFrame.ConvertRawFrame(vBodyFramesRec.RecordingRawFrames[0]);
            SetInitialFrame(vFrame);
            BodyView.Init(this, mBodyFrameThread.BodyFrameBuffer);
            mBodyFrameThread.Start();
            BodyView.mStartUpdating = true;
        }
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
    * ApplyTracking()
    * @param Applies tracking on the passed in
    * @brief  Play a recording from the given recording UUID. 
    */
    void ApplyTracking(BodyFrame frame)
    {

    }
    /**
    * StopThread()
    * @param Stops the current thread
    * @brief  Play a recording from the given recording UUID. 
    */
    internal void StopThread()
    {

        mBodyFrameThread.StopThread();
    }





}
