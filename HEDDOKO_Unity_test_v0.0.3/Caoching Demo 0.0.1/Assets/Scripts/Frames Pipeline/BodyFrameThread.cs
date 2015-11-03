/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Utils;
/**
* BodyFrameThread class 
* @brief child class for communication threads
*/
public class BodyFrameThread : ThreadedJob
{
    #region class fields
    private BodyFrameBuffer mBuffer;  //buffer  
    private SourceDataType mDataSourceType;
    private List<BodyRawFrame> mRawFrames;
    private bool mContinueWorking;

    #endregion
    #region properties
    internal BodyFrameBuffer BodyFrameBuffer
    {
        get
        {
            if (mBuffer == null)
            {
                mBuffer = new BodyFrameBuffer();
            }
            return mBuffer;
        }
    }
    #endregion


    //TODO: Handling different sources: Recording or Suit Comm
    //TODO: Handling Frame by Frame transmition  

    //From recording 
    //from suit comm

    #region Constructors
    /** 
    * @brief Parameterized constructor that takes in a list of rawframes, transforming thus rawdata into bodyframe data when the thread is started 
    * @param recording 
    */
    public BodyFrameThread(List<BodyRawFrame> mRawFrames, BodyFrameBuffer vBuffer)
    {
        this.mRawFrames = mRawFrames;
        this.mBuffer = vBuffer;
        mDataSourceType = SourceDataType.Recording;
    }
    /**
     * @brief Default constructor
     */
    public BodyFrameThread()
    {

    }

    #endregion
    #region polymorphic functions

    public override void Start()
    {
        base.Start();
        mContinueWorking = true;
    }

    #endregion
    /**
    * ThreadFunction()
    * @brief The thread loop, overwrite this in the base class
    */
    protected override void ThreadFunction()
    {
        switch (mDataSourceType)
        {
            case SourceDataType.BrainFrame:
                BodyFrameBuffer.AllowOverflow = true;
                BrainFrameTask();
                //todo
                break;
            case SourceDataType.DataStream:
                BodyFrameBuffer.AllowOverflow = true;
                DataStreamTask();

                //todo
                break;

            case SourceDataType.Recording:
                BodyFrameBuffer.AllowOverflow = false;
                RecordingTask();
                break;
            case SourceDataType.Suit:
                //todo
                break;
        }

    }

    #region helper functions for ThreadFunction()

    /**
   * TaskForRecording()
   * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the tasks waits until cancelled. this task is for the case that the data 
     * comes from a recording
   */
    private void RecordingTask()
    {
        int vBodyFrameIndex = 0;
        while (mContinueWorking)
        {
            while (!BodyFrameBuffer.IsFull())
            {
                if (!mContinueWorking)
                {
                    break;
                }
                try
                {
                    BodyFrame vBodyFrame = BodyFrame.ConvertRawFrame(mRawFrames[vBodyFrameIndex]);//convert to body frame  : Todo: this can be optimized, we can reduce these calls, but the proposal would induce an additional memory cost
                    BodyFrameBuffer.Enqueue(vBodyFrame);
                    vBodyFrameIndex++;
                    //todo: can set a flag for restarting this task over again
                    if (vBodyFrameIndex >= mRawFrames.Count) //reset back to 0
                    {
                        vBodyFrameIndex = 0;
                    }
                }
                catch (Exception e)
                {
                    mContinueWorking = false;
                    UnityEngine.Debug.Log(e.StackTrace);
                    break;
                }

            }
        }
    }
    /**
    * BrainFrameTask()
    * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the oldest frame gets overwritten. this task is for the case that the data 
    * comes from a the brainframe
    */
    private void BrainFrameTask()
    {
        //todo
    }
    /**
    * DataStreamTask()
    * @brief Helping function that ensures that pushes data onto a circular buffer. If the buffer is filled,then the oldest frame gets overwritten. this task is for the case that the data 
    * comes from a the brainframe
    */

    private void DataStreamTask()
    {

    }
    #endregion

    #region threading functions


    /**
    * CleanUp
    * @brief Helping function cleans ups  
    */

    public void StopThread()
    {
        mContinueWorking = false;
    }
    /**
    * OnFinished()
    * @brief Callback when the thread is done executing
    */
    protected override void OnFinished()
    {
        //This is executed by the Unity main thread when the job is finished 
        //TODO: 
    }
    #endregion


    /// <summary>
    /// Where does the data originate from?
    /// </summary>
    public enum SourceDataType
    {
        Suit,
        Recording,
        BrainFrame,
        DataStream,
        Other
    }


}
