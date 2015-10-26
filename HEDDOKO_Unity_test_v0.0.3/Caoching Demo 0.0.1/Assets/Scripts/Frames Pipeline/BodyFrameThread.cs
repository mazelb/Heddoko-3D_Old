/**
* @file BodyFrameThread.cs
* @brief Contains the bodyframethread class
* @author Mazen Elbawab (mazen@heddoko.com)
* @date June 2015
*/

using System;
using System.Collections.Generic; 

/**
* BodyFrameThread class 
* @brief child class for communication threads
*/
public class BodyFrameThread : ThreadedJob
{
    #region class fields
    private BodyFrameBuffer buffer;  //buffer  
    private SourceDataType dataType;
    private List<BodyRawFrame> rawFrames;
    private bool continueWorking;
   
    #endregion
    #region properties
    internal BodyFrameBuffer BodyFrameBuffer
    {
        get
        {
            if (buffer == null)
            {
                buffer = new BodyFrameBuffer(); 
            }
            return buffer;
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
    public BodyFrameThread( List<BodyRawFrame> rawFrames )
    {
        this.rawFrames = rawFrames;
        dataType = SourceDataType.Recording;
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
        continueWorking = true;
    }

    #endregion
    /**
    * ThreadFunction()
    * @brief The thread loop, overwrite this in the base class
    */
    protected override void ThreadFunction()
    {
        switch (dataType)
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
        int bodyFrameRecordingIndex = 0;
        while (continueWorking)
        {
            while (!BodyFrameBuffer.IsFull())
            {
                BodyFrame bframe = BodyFrame.ConvertRawFrame(rawFrames[bodyFrameRecordingIndex]);//convert to body frame  : Todo: this can be optimized, we can reduce these calls, but the proposal would induce an additional memory cost
                BodyFrameBuffer.Enqueue(bframe);
                bodyFrameRecordingIndex++;
                //todo: can set a flag for restarting this task over again
                if (bodyFrameRecordingIndex >= rawFrames.Count) //reset back to 0
                {
                    bodyFrameRecordingIndex = 0;
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
        continueWorking = false;
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

    public struct CallbackActionStruct
    {
        public object[] objects;
        public Action callbackAction;
    }

}
