/** 
* @file RecordingPlaybackTask .cs
* @brief Contains the PlaybackControl abstract class
* @author Mohammed Haider (mohammed@heddoko.com)
* @date February 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Scripts.Frames_Pipeline.BodyFrameConversion;
using Assets.Scripts.UI.AbstractViews.AbstractPanels.PlaybackAndRecording;
using Assets.Scripts.UI.PlaybackAndRecording;
using Assets.Scripts.Utils;

namespace Assets.Scripts.Frames_Pipeline
{
    public class RecordingPlaybackTask
    {

        public PlaybackControl PlaybackControl;
        public float PlaybackSpeed
        {
            get { return mPlaybackSpeed; }
            set
            {
                mIteratorAdder = Math.Sign(value) * 1;
                //check if playback speed is 0, then set pause to true
                if (mPlaybackSpeed == 0)
                {
                    IsPaused = true;
                    return;
                }

                mPlaybackSpeed = value;
                if (Math.Abs(mPlaybackSpeed) < 1)
                {
                    InSlowMo = true;
                }
                else
                {
                    InSlowMo = false;
                }
            }
        }

        public bool IsPlaying;
        public bool IsPaused { get; set; }
        public bool LoopPlaybackEnabled;
        private bool mIsRewinding;
        public static bool InSlowMo;
        private BodyFrameBuffer mFrameBuffer;
        private int mCurrentIdx;
        private int mFirstPos = 0;
        public int IteratorAdder;
        private int mFinalFramePos = 0;


        private BodyFrame[] mConvertedFrames;

        private float mPlaybackSpeed = 1f;

        private List<BodyRawFrame> mRawFrames;
        

        /// <summary>
        /// Depending if the current state is rewinding or going forward, then the 
        /// the index iterator changes the local position of the converted frame data
        /// </summary>
        private int mIteratorAdder = 1;

        /// <summary>
        /// Sets the current playback state to rewinding. 
        /// </summary>
        public bool IsRewinding
        {
            get { return mIsRewinding; }
            set
            {
                mIsRewinding = value;

                //swap the first pos and final pos
                int vTemp = mFirstPos;
                mFirstPos = mFinalFramePos;
                mFinalFramePos = vTemp;
                //check if rewinding, set the iterator to -1, else to 1
                if (value)
                {
                    mIteratorAdder = -1;
                }
                else
                {
                    mIteratorAdder = 1;
                }

            }
        }

        /// <summary>
        /// Returns the frame count from the recording
        /// </summary>
        public int RecordingCount
        {
            get
            {
                if (mConvertedFrames == null)
                {
                    return -1;
                }
                return mConvertedFrames.Length;
            }
        }
        /// <summary>
        /// Returns the total recording time
        /// </summary>
        public float TotalRecordingTime
        {
            get
            {
                if (mConvertedFrames == null)
                {
                    return -1f;
                }
                if (mConvertedFrames.Length == 0)
                {
                    return 0f;
                }
                BodyFrame vFirst = mConvertedFrames[0];
                BodyFrame vLast = mConvertedFrames[RecordingCount - 1];
                float vElapsedTime = vLast.Timestamp - vFirst.Timestamp;
                return vElapsedTime;
            }
        }
        /// <summary>
        /// Return current body frame
        /// </summary>
        public BodyFrame CurrentBodyFrame
        {
            get
            {
                if (mConvertedFrames == null)
                {
                    return null;
                }
                if (mConvertedFrames.Length == 0)
                {
                    return null;
                }
                return mConvertedFrames[mCurrentIdx];
            }
        }


        public RecordingPlaybackTask(List<BodyRawFrame> vRawFrames, BodyFrameBuffer vFrameBuffer)
        {
            mRawFrames = vRawFrames;
            mFrameBuffer = vFrameBuffer;
        }
        /// <summary>
        /// The recording play back task allows for recording playback by converting 
        /// Rawbody frames in to body frames. 
        /// </summary>
        public void Play()
        {
            int vTotalCount = mRawFrames.Count;
            // long vStartTime = DateTime.Now.Ticks;
            if (vTotalCount == 0)
            {
                return;
            }

            //first convert all the frames
            mConvertedFrames = new BodyFrame[vTotalCount];
            for (int i = 0; i < vTotalCount - 1; i++)
            {
                mConvertedFrames[i] = RawFrameConverterManager.ConvertRawFrame(mRawFrames[i]);
            }
            //calculate a delta time between frames, capture the first time stamp
            BodyFrame vFirstFrame = mConvertedFrames[0];
            float vPrevTimeStamp = vFirstFrame.Timestamp;

            float vRecDeltatime = 0;

            //the position of the first and last frame
            mCurrentIdx = 0;
            mFirstPos = 0;
            mFinalFramePos = vTotalCount - 1;

            //start looping
            while (IsPlaying)
            {
                if (!IsPlaying)
                {
                    break;
                }

                if (mFrameBuffer.IsFull() || IsPaused)
                {
                    continue;
                }
                try
                {
                    float vDeltaTime = OutterThreadToUnityThreadIntermediary.FrameDeltaTime;
                    //check if we've reached the last position

                    if (mCurrentIdx == mFinalFramePos)
                    {
                        //check if looping is enabled, set vCurrPos to first postion
                        if (LoopPlaybackEnabled)
                        {
                            mCurrentIdx = mFirstPos;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    IteratorAdder = mIteratorAdder;
                    BodyFrame vCurrBodyFrame = null;
                    BodyFrame vEnquedBodyFrame = null;
                    //if the current position is last -1
                    if (mCurrentIdx == mFinalFramePos)
                    {
                        vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
                        mFrameBuffer.Enqueue(vEnquedBodyFrame);
                    }

                    else if (!InSlowMo)
                    {
                        //check the delta time from the previous time frame and the current converted raw body frame
                        vCurrBodyFrame = mConvertedFrames[mCurrentIdx];
                        vRecDeltatime = Math.Abs(vCurrBodyFrame.Timestamp - vPrevTimeStamp);

                        //divide the recording delta time by a playback speed. Iterate through the converted frames
                        //and until the recording delta time surpasses the playback(of course check if the position
                        //gets out of range
                        while (true)
                        {

                            if (Math.Abs((vRecDeltatime / PlaybackSpeed)) > vDeltaTime)
                            {
                                vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
                                break;
                            }
                            mCurrentIdx += mIteratorAdder;
                            if (mCurrentIdx == mFinalFramePos)
                            {
                                vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
                                break;
                            }
                            vCurrBodyFrame = mConvertedFrames[mCurrentIdx];
                            vRecDeltatime = Math.Abs(vCurrBodyFrame.Timestamp - vPrevTimeStamp);
                        }
                        vPrevTimeStamp = mConvertedFrames[mCurrentIdx].Timestamp;
                        mFrameBuffer.Enqueue(vEnquedBodyFrame);
                    }
                    else
                    {
                        vCurrBodyFrame = mConvertedFrames[mCurrentIdx];
                        vRecDeltatime = Math.Abs(vCurrBodyFrame.Timestamp - vPrevTimeStamp);
                        int vSleepTime = (int)((vRecDeltatime / Math.Abs(PlaybackSpeed)) * 1000);
                        Thread.Sleep(vSleepTime);
                        vEnquedBodyFrame = mConvertedFrames[mCurrentIdx];
                        mFrameBuffer.Enqueue(vEnquedBodyFrame);
                        vPrevTimeStamp = vCurrBodyFrame.Timestamp;
                        mCurrentIdx += mIteratorAdder;

                    }
                }
                catch (Exception vException)
                {
                    //todo set up a debug logger
                    string vMessage = vException.GetBaseException().Message;
                    vMessage += "\n" + vException.Message;
                    vMessage += "\n" + vException.StackTrace;
                    break;
                }


            }
        }
        /// <summary>
        /// Sets the playback to be played from the passed in index
        /// </summary>
        /// <param name="vConvertedRecIdx"></param>
        public void PlayFromIndex(int vConvertedRecIdx)
        {
            if (vConvertedRecIdx < 0 || vConvertedRecIdx >= mConvertedFrames.Length)
            {
                return;
            }

            mCurrentIdx = vConvertedRecIdx;
            mFrameBuffer.Enqueue(mConvertedFrames[mCurrentIdx]);
        }

        /// <summary>
        /// returns a body frame at the current index
        /// </summary>
        /// <param name="vIndex"></param>
        /// <returns></returns>
        public BodyFrame GetBodyFrameAtIndex(int vIndex)
        {
            if (vIndex < 0 || vIndex >= mConvertedFrames.Length)
            {
                return null;
            }
            else
            {
                return mConvertedFrames[vIndex];
            }
        }


    }
}