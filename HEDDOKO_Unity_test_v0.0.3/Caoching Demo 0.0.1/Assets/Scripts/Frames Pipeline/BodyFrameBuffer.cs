/** 
* @file BodyFrameBuffer.cs
* @brief Contains the BodyFrameBuffer class and its constructors
* @author  Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
using Assets.Scripts.Utils;
/**
* BodyFrameBuffer  class 
* @brief BodySubsegmentView class, that acts as a circular buffer for BodyFrames by inheriting from CircularQueue.cs
*/
public class BodyFrameBuffer : CircularQueue<BodyFrame>
{

    /**
    * BodyFrameBuffer(int capacity)
    * @param int capacity: the capacity of the buffer
    * @brief Constructor for a BodyFrameBuffer with a set capacity
    * @return a new BodyFrameBuffer with a set capacity
    */
    public BodyFrameBuffer(int mCapacity)
        : base(mCapacity, true)
    {
        
    }
    /**
    * BodyFrameBuffer()
    * @brief Constructor for a BodyFrameBuffer 
    * @return a new BodyFrameBuffer with a fixed capacity of 255 items
    */
    public BodyFrameBuffer() : base()
    {
        
    }
     
}
