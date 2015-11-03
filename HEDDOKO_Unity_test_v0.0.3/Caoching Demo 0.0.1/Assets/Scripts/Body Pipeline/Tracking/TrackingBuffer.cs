/** 
* @file TrackingBuffer.cs
* @brief Contains the TrackingThread class
* @author Moyhammed Haider(mazen@heddoko.com)
* @date November 2015
* Copyright Heddoko(TM) 2015, all rights reserved
*/ 
using System.Collections.Generic; 
using Assets.Scripts.Utils;

namespace Assets.Scripts.Body_Pipeline.Tracking
{
    /**
    * TrackingBuffer class 
    * @brief Contains the TrackingBuffer class that extends from CircularQueue. Accepts a dictionary of 
    * SensorPositions and a 3X3 float array
    */
    public class TrackingBuffer : CircularQueue<Dictionary<BodyStructureMap.SensorPositions, float[,]>>
    {

        /**
        * TrackingBuffer(int capacity)
        * @param int capacity: the capacity of the buffer
        * @brief Constructor for a TrackingBuffer with a set capacity
        * @return a new TrackingBuffer with a set capacity
        */
        public TrackingBuffer(int mCapacity)
        : base(mCapacity, true)
        {

        }
        /**
        * TrackingBuffer()
        * @brief Constructor for a TrackingBuffer 
        * @return a new TrackingBuffer with a fixed capacity of 255 items
        */
        public TrackingBuffer() : base()
        {

        }
    }
}
