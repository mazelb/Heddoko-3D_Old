/** 
* @file TrackingThread.cs
* @brief Contains the TrackingThread class
* @author Moyhammed Haider(mazen@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Body_Pipeline.Tracking
{
    /**
    * TrackingThread class 
    * @brief Contains the TrackingThread class and all functionalities required to execute it
    */
    public class TrackingThread : ThreadedJob
    {
        #region class fields
        private BodyFrameBuffer mInputBuffer;  //buffer 
        private TrackingBuffer mOutputBuffer;
        private Body mBody;
        
        private List<BodyRawFrame> mRawFrames;
        private bool mContinueWorking;
        private bool mPauseWorker;
        #endregion
       
  

        #region Constructors
        /** 
        * @brief Parameterized constructor that takes in a list of rawframes, transforming thus rawdata into bodyframe data when the thread is started 
        * @param recording 
        */
        public TrackingThread(Body vBody, BodyFrameBuffer vInputBuffer, TrackingBuffer vOutputBuffer )
        {
            mBody = vBody;
            mInputBuffer = vInputBuffer;
            mOutputBuffer = vOutputBuffer;
        }

        #endregion
        #region polymorphic functions

        public override void Start()
        {
            base.Start();
            mContinueWorking = true;
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
            while (mContinueWorking)
            {
                while (mInputBuffer.Count > 0)
                {
                    if (!mContinueWorking)
                    {
                        break;
                    }
                    if(  mPauseWorker || mOutputBuffer.IsFull() && !mOutputBuffer.AllowOverflow )
                    {
                        continue;
                    }
                    try
                    { 
                        BodyFrame vProcessedBodyFrame = mInputBuffer.Dequeue();
                        mBody.CurrentBodyFrame = vProcessedBodyFrame;
                        Dictionary<BodyStructureMap.SensorPositions, float[,]> vTrackedRotationMatrix =
                            Body.GetTracking( mBody);
                        mOutputBuffer.Enqueue(vTrackedRotationMatrix);
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e.StackTrace + "\n"+e);
                    }
                }
            }

        }



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

 

    }
}
