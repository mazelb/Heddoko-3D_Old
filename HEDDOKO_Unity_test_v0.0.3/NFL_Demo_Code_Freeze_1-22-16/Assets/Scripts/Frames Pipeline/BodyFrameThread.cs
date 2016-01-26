/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Assets.Scripts.Frames_Pipeline;
using Assets.Scripts.Utils;
//using Assets.Scripts.Utils.Debugging;
using Assets.Scripts.Utils.UnityUtilities;
using HeddokoLib.adt;
using HeddokoLib.networking;
using HeddokoLib.utils;
using UnityEngine;

/**
* BodyFrameThread class 
* @brief child class for communication threads
todo: can create an interface for handling these, subsequently every routine that needs to use the bodyframe buffer can just use an interface call. 
*/
public class BodyFrameThread : ThreadedJob
{
    private BodyFrameBuffer mBuffer;   
    private SourceDataType mDataSourceType;
    private PlaybackState mCurrentPlaybackState = PlaybackState.Pause;
    private PlaybackSettings mPlaybackSettings;
    private List<BodyRawFrame> mRawFrames;
    private BodyFramesRecording mBodyFramesRecording;
    private bool mContinueWorking;
    private CircularQueue<HeddokoPacket> mInboundSuitBuffer = new CircularQueue<HeddokoPacket>(1024, true);
    private bool mPauseWorker;
    private object mWorkerThreadLockHandle = new object();
    private Vector3[] vPreviouslyValidValues = new Vector3[9];
    
    //For debug purposes
    public bool IsDebugging { get; set; }

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
    * @brief Parameterized constructor that takes in a list of rawframes, transforming thus rawdata into bodyframe data when the thread is started 
    * @param recording 
    */
    public BodyFrameThread(BodyFramesRecording vRecording, BodyFrameBuffer vBuffer)
    {
        mBodyFramesRecording = vRecording;
        this.mBuffer = vBuffer;
        mDataSourceType = SourceDataType.Recording;
    }

    /**
    * @brief Default constructor
    */
    public BodyFrameThread()
    {

    }

    /// <summary>
    /// Preps the buffer to accept raw data from brainpacks
    /// </summary>
    /// <param name="vBuffer"></param>
    public BodyFrameThread(BodyFrameBuffer vBuffer, SourceDataType vDataType)
    {
        this.mBuffer = vBuffer;
        if (vDataType == SourceDataType.BrainFrame)
        {
            mBuffer.AllowOverflow = true;
        }
        if (vDataType == SourceDataType.Recording)
        {
            mBuffer.AllowOverflow = false;
        }

        mDataSourceType = SourceDataType.BrainFrame;
    }

    public override void Start()
    {
        ContinueWorking = true;
        base.Start();

    }

    public void PauseWorker()
    {
        mPauseWorker = !mPauseWorker;
    }

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
                //   RecordingPlaybackTask();
                break;
            case SourceDataType.Suit:
                //todo
                break;
        }

    }

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
                    //convert to body frame  : Todo: this can be optimized, we can reduce these calls, but the proposal would induce an additional memory cost
                    BodyFrame vBodyFrame = BodyFrame.ConvertRawFrame(mRawFrames[vBodyFrameIndex]);
                    BodyFrameBuffer.Enqueue(vBodyFrame);
                    vBodyFrameIndex++;

                    //reset back to 0
                    if (vBodyFrameIndex >= mRawFrames.Count)
                    {
                        break;
                        // vBodyFrameIndex = 0;
                    }
                }
                catch (Exception e)
                {
                    //ContinueWorking = false;
                    string vMessage = e.GetBaseException().Message;
                    vMessage += "\n" + e.Message;
                    vMessage += "\n" + e.StackTrace; 
                    break;
                }

            }
        }
    }

    private void RecordingPlaybackTask()
    {

        // long vStartTime = DateTime.Now.Ticks;
        float vStartTime = TimeUtility.Time;
        //frame position
        int vPosition = 0; 

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
                    //vStartTime = DateTime.Now.Ticks;  //reset the start time
                    continue;
                }
                try
                {

                    float vDeltaTime = TimeUtility.Time - vStartTime;
                    float vElapsedTime = vDeltaTime + mBodyFramesRecording.Statistics.AverageSecondsBetweenFrames;

                    vElapsedTime /= mBodyFramesRecording.Statistics.TotalTime;
                    //  float mPlaybackSpeed = mPlaybackSettings.PlaybackSpeed;
                    vPosition = (int)(1 * mBodyFramesRecording.Statistics.TotalFrames * vElapsedTime);

                    if (vPosition >= mBodyFramesRecording.Statistics.TotalFrames)
                    {
                        mPauseWorker = true;
                        vPosition = mBodyFramesRecording.Statistics.TotalFrames - 1;
                        vStartTime = DateTime.Now.Ticks;
                        break;
                    }

                    //convert to body frame 
                    BodyFrame vBodyFrame = BodyFrame.ConvertRawFrame(mBodyFramesRecording.RecordingRawFrames[vPosition]);
                    BodyFrameBuffer.Enqueue(vBodyFrame);
                }
                catch (Exception e)
                {
                    //ContinueWorking = false;
                    string vMessage = e.GetBaseException().Message;
                    vMessage += "\n" + e.Message;
                    vMessage += "\n" + e.StackTrace;
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
        int vStartTime = 0;
        int vCounter = 0;
        while (true)
        {
            string vLogMessage = "";
            Stopwatch vStopwatch = new Stopwatch();
            vStopwatch.Start();
            if (!ContinueWorking)
            {
                //finished working
                break;
            }

            if (InboundSuitBuffer.Count == 0)
            {
                continue;
            }

            HeddokoPacket vPacket = InboundSuitBuffer.Dequeue();
            if (vPacket == null)
            {
                continue;
            }

            string vUnwrappedString = "";

            try
            {
                bool vAllClear = false;
                //first unwrap the string and break it down 
                vUnwrappedString = HeddokoPacket.Unwrap(vPacket.Payload);

                //Debug here

                //todo place a check here for valid data
                string[] vExploded = vUnwrappedString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //WriteToDiskSubTask(vUnwrappedString);
                //this data is invalid
                if (vExploded.Length != 12)
                {
                    continue;
                }

                //the first value is a timestamp in int
                int vTimeStamp = Convert.ToInt32(vExploded[0]);
                if (++vCounter == 1)
                {
                    vStartTime = vTimeStamp;
                }
                //get the bitmask from index 1
                Int16 vBitmask = Convert.ToInt16(vExploded[1], 16);

                int vStartIndex = 2;
                int vEndIndex = 11;
                int vBitmaskCheck = 0;

                //is used to set vPreviouslyValid values indicies 
                int vSetterIndex = 0;

                for (int i = vStartIndex; i < vEndIndex; i++, vBitmaskCheck++, vSetterIndex++)
                {
                    //get the bitmask and check if the sensors values are valid(not disconnected)
                    //data is valid 
                    if ((vBitmask & (1 << vBitmaskCheck)) == (1 << vBitmaskCheck))
                    {
                        //conversion happens here, todo: place a check here for invalid data(less than 4 bytes in length
                        string[] v3data = vExploded[i].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        float vRoll = ConversionTools.ConvertHexStringToFloat((v3data[0]));
                        float vPitch = ConversionTools.ConvertHexStringToFloat((v3data[1]));
                        float vYaw = ConversionTools.ConvertHexStringToFloat((v3data[2]));

                        vPreviouslyValidValues[vSetterIndex] = new Vector3(vPitch, vRoll, vYaw);
                    }
                }
                BodyFrame vBodyFrame = BodyFrame.CreateBodyFrame(vPreviouslyValidValues);
                //Todo: convert the timestamp to a float
                vBodyFrame.Timestamp = (float)(vTimeStamp - vStartTime) / 1000f;//vTimeStamp;
                BodyFrameBuffer.Enqueue(vBodyFrame);
                vLogMessage += vUnwrappedString;

            }
            catch (IndexOutOfRangeException)
            {
                vLogMessage = "IndexOutOfRangeException in BodyFrameThread. Contents of vUnwrappedString are " +
                                 vUnwrappedString;
            }
            catch (Exception e)
            {
                vLogMessage = e.Message + "\n" + e.GetBaseException() + "\n" + e.StackTrace;

            }

            vStopwatch.Stop();
            if (IsDebugging)
            {
                double vTotalMs = vStopwatch.Elapsed.TotalMilliseconds;
                //RawframeConversion.WriteLog(vTotalMs, vLogMessage);
            }

        }
    }
    
    /// <summary>
    /// Close the file
    /// </summary>
    private void CloseFile()
    {
        try
        {
            //mStreamWriter.Flush();
          //  mStreamWriter.Close();
        }
        catch(Exception)
        {
            
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

    /**
    * CleanUp
    * @brief Helping function cleans ups  
    */
    public void StopThread()
    {
      
        ContinueWorking = false; 
        CloseFile();
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

    /// <summary>
    /// Represent the recording playback state
    /// </summary>
    public enum PlaybackState
    {
        Pause,
        Stop,
        Rewind,
        Forward //=> with this we have multiple playback speeds. 
    }

}
