/*@file CircularQueue.cs
* @brief Contains the CircularQueue<T> class class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date October 2015
*/
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{

    /**
    * CircularQueue class 
    * @brief The CircularQueue class accepts a generic type and operates asynchrounously, that is data is consumed only as quickly as the consumer
    * can consume it, and produced when a producer produces it.  When inserting a new item, the oldest value will be overwritten, however 
    * this can be alleviated by setting the AllowOverflow property to false. Enqueuing a filled queue will result in a
    * CircularQueueOverflowException, so the programmer needs to handle this exception(perhaps wait until the queue is cleared?)
    */
    public class CircularQueue<T> : IEnumerable<T>, ICollection<T>
    {
        private int mCapacity; //how large is the queue?
        private int mCount;
        private int mHeadIndex;
        private int mTailIndex;
        private T[] maQueue;
        private const int mDefaultCapacity = 255; //default size to use in the parameterles constructor

        #region properties
        /**
        * AllowOverFlow
        * @brief Property that allows the circular queue to overflow
        * @note Please note that if this bool is set to false, then an CircularQueueOverflowException will be thrown. 
        * @return Is the circular queue allowed to overflow?
        */ 
        public bool AllowOverFlow
        {
            get;
            set;
        }
        /**
        * Capacity 
        * @brief Property that gets and sets the maximum capacity of the circular queue. 
        * @note Please note that if capacity is set to a value that is less than the current number of elements in the list, then a ArgumentOutOfRangeException is thrown
        * @return The current capacity
        */
        public int Capacity
        {
            get
            {
                return mCapacity;
            }
            set
            {
                if (value == mCapacity)
                    return;
                if (value < mCount)
                {
                    throw new ArgumentOutOfRangeException("Trying to resize the queue to a size smaller than the current count of " + mCount);
                }
                var temp = new T[value];
                if (mCount > 0)
                {
                    CopyTo(temp);
                }
                maQueue = temp;
                mCapacity = value;
            }
        }
        /**
        * Count 
        * @brief Property that gets the current number of elements in the ciruclar queue  
        */
        public int Count
        {
            get
            {
                return mCount;
            }
        }
        /**
        * CirQueue 
        * @brief Property that returns the current queue. 
        * @note Please note that if capacity is set to a value that is less than the current number of elements in the list, then a ArgumentOutOfRangeException is thrown
        * @return The current capacity
        */
        public T[] CirQueue
        {
            get
            {
                return maQueue;
            }
        }

        #endregion
        #region constructors
        /**
        * CircularQueue()
        * @brief Default constructor, sets the capacity to 255 and doesn't allow overflow
        * @param  
        * @return a new CircularQueueObject
        */
        public CircularQueue()
            : this(mDefaultCapacity, false)
        {

        }
        /**
        * CircularQueue(bool vAllowOverFlow)
        * @brief Constructor, sets the capacity to 255, and allow overflow is set according to the parameter passed in 
        * @param  vAllowOverFlow
        * @return a new CircularQueueObject
        */
        public CircularQueue(bool vAllowOverFlow)
            : this(mDefaultCapacity, vAllowOverFlow)
        {

        }
        /**
        * CircularQueue(bool vCapacity)
        * @brief Constructor, sets the capacity according to the paramater passed in and doesn't allow overflow 
        * @param  vCapacity
        * @return a new CircularQueueObject
        */
        public CircularQueue(int vCapacity)
            : this(vCapacity, false)
        {

        }

        /**
        * CircularQueue(int capacity, bool vAllowOverflow)
        * @brief Constructor, sets the capacity according to the paramater passed in and sets overflow according to the allowOverflow parameter passed in
        * @param  capacity, vAllowOverflow
        * @return a new CircularQueueObject
        */
        public CircularQueue(int capacity, bool vAllowOverflow)
        {
            if (capacity < 0)
            {
                throw new EmptyCircularQueueException();
            }
            this.mCapacity = capacity;
            mCount = 0;
            mHeadIndex = 0;
            mTailIndex = 0;
            maQueue = new T[capacity];
            AllowOverFlow = vAllowOverflow;
        }
        #endregion
        /**
        * Contains(T vObj)
        * @brief Does the circular queue contain the object?
        * @param  vObj
        * @return false if  object isn't found
        */
        public bool Contains(T vObj)
        {
            int vQueueIndex = mHeadIndex;
            var vComparer = EqualityComparer<T>.Default;
            for (int i = 0; i < mCount; i++, vQueueIndex++)
            {
                if (vQueueIndex == mCapacity)
                {
                    vQueueIndex = 0;
                }
                if (vObj == null && maQueue[vQueueIndex] == null)
                {
                    return true;
                }
                else if ((maQueue[vQueueIndex] != null) && vComparer.Equals(maQueue[vQueueIndex], vObj))
                {
                    return true;
                }
            }
            return false;
        }
       /**
       * Clear()
       * @brief  clears the circular queue 
       * @param  
       * @return  
       */
        public void Clear()
        {
            mCount = 0;
            mHeadIndex = 0;
            mTailIndex = 0;
        }
        /**
        * Enqueue(T[] vA)
        * @brief  Enqueues an array of the same type as contained in the circular queue
        * @param  T[] vA
        * @return  number of items that were placed in the circular queue 
        */
        public int Enqueue(T[] vA)
        {
            return Enqueue(vA, 0, vA.Length);
        }
        /**
        *  Enqueue(T[] vA, int vOffset, int vLength)
        * @brief  Enqueues an array of the same type as contained in the circular queue, starting from the offset, for a length of the array
        * @note If the AllowOverflow flag is set to false and the number of element that want to be placed in the queue is greater than the 
        * number of available slots then this method will throw a CircularQueueOverflowException
        * @param    vA: array to be enqueued,  vOffset: offset start, vLength: number of elements to copy from the array
        * @return  number of items that were placed in the circular queue 
        */
        public int Enqueue(T[] vA, int vOffset, int vLength)
        {
            if (!AllowOverFlow && vLength > Capacity - Count)
            {
                throw new CircularQueueOverflowException();
            }

            int startIndex = vOffset;
            for (int i = 0; i < vLength; i++, mTailIndex++, startIndex++)
            {
                if (mTailIndex == mCapacity)
                {
                    mTailIndex = 0;
                }
                maQueue[mTailIndex] = vA[startIndex];
            }

            mCount = Math.Min(Count + vLength, mCapacity);
            return mCount;
        }
        /**
        * Enqueue(T vItem)
        * @brief  Enqueues an item in the circular queue
        * @note If the AllowOverflow flag is set to false and the circular queue is full, then a CircularQueueOverflowException is thrown
        * @param vItem: the item to enqueue
        * @return 
        */
        public void Enqueue(T vItem)
        {
            if (!AllowOverFlow && IsFull())
            {
                throw new CircularQueueOverflowException();
            }
            maQueue[mTailIndex] = vItem;
            if (++mTailIndex == mCapacity)
            {
                mTailIndex = 0;
            }
            mCount++;
        }
        /**
        * IsFull( )
        * @brief Checks if the circular queue is full  
        * @param 
        * @return true if capacity is reached
        */
        public bool IsFull()
        {
            return mCount == mCapacity;
        }

        /**
        * Skip(int vNAmount)
        * @brief Moves the head of circular buffer by vNAmount
        * @note if the amount is larger than the number of actual elements in the buffer, then wrap the skip by count -1
        * @param vNAmount: the amount to move the head by
        * @return  
        */
        public void Skip(int vNAmount)
        {
            mHeadIndex += vNAmount;
            if (mHeadIndex >= Count)
            {
                mHeadIndex -= Count -1; //wrap
            }
        }


        /**
        * Dequeue(T[] vA)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array 
        * @param  vA: the reference array to dequeue elements into
        * @return  the number of elements that were dequeued
        */
        public int Dequeue(ref T[] vA)
        {
            return Dequeue(ref vA, 0, vA.Length);
        }
        /**
        *  Dequeue(ref T[] vA, int vOffset, int vLength)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array, starting at the offset for a set length
        * @param vA: The array  to reference vOffset: the offset start vLength: number of elements to dequeue
        * @return the number of elements that were dequeued
        */
        public int Dequeue(ref T[] vA, int vOffset, int vLength)
        {
            int vActualCount = Math.Min(vLength, mCount);
            int vStartIndex = vOffset;
            for (int i = 0; i < vActualCount; i++, mHeadIndex++, vStartIndex++)
            {
                if (mHeadIndex == mCapacity)
                {
                    mHeadIndex = 0;
                }
                vA[vStartIndex] = maQueue[mHeadIndex];
            }
            mCount -= vActualCount;
            return vActualCount;
        }

        /**
        * Dequeue()
        * @brief Dequeues an element from the circular queue 
        * @note Throws an EmptyCircularQueueException when empty.
        * @param  
        * @return the element that was dequeued
        */
        public T Dequeue()
        {
            if (Count == 0)
            {
                throw new EmptyCircularQueueException();
            }
            var vItem = maQueue[mHeadIndex];
            if (++mHeadIndex == mCapacity)
            {
                mHeadIndex = 0;
            }
            mCount--;
            return vItem;
        } 
        /**
        * Peek()
        * @brief Peek at the head of the circular queue, without moving the head
        * @note Throws an EmptyCircularQueueException when empty.
        * @param  
        * @return the element that was peeked
        */
        public T Peek()
        {
            if (Count == 0)
            {
                throw new EmptyCircularQueueException();
            }
            var item = maQueue[mHeadIndex];
            return item;
        }

        #region copy buffered queue to array 
        /**
        * CopyTo(  T[] vA)
        * @brief  Copy the circular queue into the requested array vA without modifying the circular queue
        * @param  T[] vA: The array to copy into
        * @return  
        */
        public void CopyTo(T[] vA)
        {
            CopyTo(vA, 0);
        }
        /**
        * CopyTo(T[] vA, int vIndex)
        * @brief  Copy the circular queue into the requested array vA without modifying the circular queue copying elements into the passed array starting  at vIndex
        * @param  T[] vA: The array to copy into, vIndex: start index
        * @return  
        */
        public void CopyTo(T[] vA, int vIndex)
        {
            CopyTo(0,   vA, vIndex, mCount);
        }
        /**
        *  CopyTo(int vIndex, T[] vA, int vArrayIndex, int vLength)
        * @brief  Copy the circular queue into the requested array vA without modifying the circular queue copying elements into the passed array starting  at vIndex and vLength amount
        * @param  T[] vA: The array to copy into, vIndex: start index
        * @note   Note that exception ArgumentOutOfRangeException will be thrown if the requested vLength is larger than the actual count in the circular queue
        * @return  
        */
        public void CopyTo(int vIndex, T[] vA, int vArrayIndex, int vLength)
        {
            if (vLength > Count)
            {
                throw new ArgumentOutOfRangeException("Length requested is too large");
            }

            int vQueueIndex = mHeadIndex;
            for (int i = 0; i < mCount; i++, vQueueIndex++, vArrayIndex++)
            {
                if (vQueueIndex == mCapacity)
                {
                    vQueueIndex = 0;
                }
                vA[vArrayIndex] = maQueue[vQueueIndex];
            }
        }

        #endregion
        /**
        * GetEnumerator()
        * @brief  Return an enumerated version of the circular queue. 
        * @return  Yields an iterated enumerated item from the circular queue
        */
        public IEnumerator<T> GetEnumerator()
        {
            int vQueueIndex = mHeadIndex;
            for (int i = 0; i < Count; i++, vQueueIndex++)
            {
                if (vQueueIndex == mCapacity)
                {
                    vQueueIndex = 0;
                }
                yield return maQueue[vQueueIndex];
            }
        }
        /**
        * ToArray()
        * @brief  Converts the circular queue to an array
        * @return The circular queue represented as an array/
        */
        public T[] ToArray()
        {
            var vA = new T[Count];
            CopyTo(vA);
            return vA;
        }

        /**
        * ICollection<T>.Count
        * @brief  Returns the count 
        * @return  returns the total number of elements in the circular queue
        */
        int ICollection<T>.Count
        {
            get
            {
                return mCount;
            }
        }
        /**
        * ICollection<T>.IsReadOnly
        * @brief  Is the circular queue read only
        * @return  false
        */
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }
        /**
        * ICollection<T>.Add(T vItem)
        * @brief  Adds an element to the circular queue 
        * @param T vItem: the item to add in the circular queue
        * @note: note that if the circular queue is at full capacity and allow over flow is set to false, then a CircularQueueOverflowException will be thrown
        */
        void ICollection<T>.Add(T vItem)
        {
            Enqueue(vItem);
        }
        /**
        * ICollection<T>.Remove(T vItem)
        * @brief  Removes an element from the circular queue. The vItem is arbitrary, as its required to be written in order to adhere to the ICollection\<T> interface 
        * @param T vItem: Arbitrary item, please see brief
        * @note: note: if the circular queue is empty a EmptyCircularQueueException will be thrown
        */
        bool ICollection<T>.Remove(T vItem)
        {
            if (Count == 0)
            {
                return false;
            }
            Dequeue();
            return true;
        }
        /**
        * GetEnumerator
        * @brief  Returns an enumerated item from the circular queue 
        * @return  IEnumerator<T> Return an enumerated item 
        */
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }


        /**
        * GetEnumerator
        * @brief  Returns an enumerable item from the circular queue 
        * @return  IEnumerator Return an enumerated item 
        */
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    /**
    * EmptyCircularQueueException class 
    * @brief The EmptyCircularQueueException exception is to be thrown in cases where the circular queue is empty and not acceptable to try to retrieve items
    * from an empty queue
    */
    public class EmptyCircularQueueException : Exception
    {

    }
    /**
    * CircularQueueOverflowException class 
    * @brief The CircularQueueOverflowException exception is to be thrown in cases where the circular queue is full and not acceptable to try to add new items 
    */
    public class CircularQueueOverflowException : Exception
    {

    }
}
