﻿/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using HeddokoLib.adt;
using HeddokoLib.networking;

/**
* BodyFrameThread class 
* @brief child class for communication threads
todo: can create an interface for handling these, subsequently every routine that needs to use the bodyframe buffer can just use an interface call. 
*/
public class BodyFrameThread : ThreadedJob
{
    #region class fields
    private BodyFrameBuffer mBuffer;  //buffer  
    private SourceDataType mDataSourceType;
    private List<BodyRawFrame> mRawFrames;
    private bool mContinueWorking;
    private CircularQueue<HeddokoPacket> mInboundSuitBuffer = new CircularQueue<HeddokoPacket>();
    private bool mPauseWorker;
    private object mWorkerThreadLockHandle = new object();
    #endregion
    #region properties

    public bool ContinueWorking
    {
        get
        {
            bool tmp;
            lock (mWorkerThreadLockHandle)
            {
                tmp = mContinueWorking;
            }
            return tmp;
        }
        set
        {
            lock (mWorkerThreadLockHandle)
            {
                mContinueWorking = value;
            }
        }
    }


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

    internal CircularQueue<HeddokoPacket> InboundSuitBuffer
    {
        get { return mInboundSuitBuffer; }
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
        ContinueWorking = true;
        base.Start();
       
    }
    public void PauseWorker()
    {
        mPauseWorker = !mPauseWorker;
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
        while (ContinueWorking)
        {
            while (true)
            {
                if (!ContinueWorking)
                {
                    break;
                }
                if (BodyFrameBuffer.IsFull() || mPauseWorker)
                {
                    continue;
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
                    //ContinueWorking = false;
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
        while (true)
        {

            if (!ContinueWorking)
            {
                //finished working
                break;
            }
            try
            {
                HeddokoPacket vPacket = InboundSuitBuffer.Dequeue();
                string vUnwrappedString = HeddokoPacket.Unwrap(vPacket.Payload);
                BodyRawFrame vRawFrame = new BodyRawFrame
                {
                    RawFrameData = vUnwrappedString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                };
                if (vRawFrame.RawFrameData.Length > 2)
                {
                    //todo: //the first column is the timestamp(int) and the second column is extra information. we just care about the remaining columns
                    Array.Copy(vRawFrame.RawFrameData, 2, vRawFrame.RawFrameData, 0, vRawFrame.RawFrameData.Length - 2);

                    //convert to bodyframe
                    BodyFrame vBodyFrame = BodyFrame.ConvertRawFrame(vRawFrame);
                }

            }
            catch (EmptyCircularQueueException vECQx)
            {
                //this error is fine, continue
            }

        }
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
        ContinueWorking = false;
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
