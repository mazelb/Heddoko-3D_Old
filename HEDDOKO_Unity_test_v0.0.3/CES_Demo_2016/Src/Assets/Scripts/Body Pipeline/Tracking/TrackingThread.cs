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
        private BodyFrameBuffer mInputBuffer;
        private TrackingBuffer mOutputBuffer;
        private Body mBody;
        private object mWorkerThreadLockHandle = new object();
        private List<BodyRawFrame> mRawFrames;
        private bool mContinueWorking;
        private bool mPauseWorker;

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
            while (ContinueWorking)
            {
                while (mInputBuffer.Count > 0)
                {
                    if (!ContinueWorking)
                    {
                        break;
                    }
                    if(  mPauseWorker || (mOutputBuffer.IsFull() && !mOutputBuffer.AllowOverflow )  )
                    {
                        continue;
                    }
                    try
                    { 
                        BodyFrame vProcessedBodyFrame = mInputBuffer.Dequeue();
                        mBody.CurrentBodyFrame = vProcessedBodyFrame;
                        //check if the body has an initialframe set, if not set it
                        if (mBody.InitialBodyFrame == null)
                        { 
                            // we need to tell the view that this initial frame has been set. wait until the next iteration before updating. 
                            mBody.SafelySetInitialBodyFrame(vProcessedBodyFrame);
                            continue;
                        }
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
    }
}
