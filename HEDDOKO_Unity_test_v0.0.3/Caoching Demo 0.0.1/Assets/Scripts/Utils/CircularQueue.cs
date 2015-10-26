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
    * BodyFrameThread class 
    * @brief The CircularQueue class accepts a generic type and operates asynchrounously, that is data is consumed only as quickly as the consumer
    *        can consume it, and produced when a producer produces it.  When inserting a new item, the oldest value will be overwritten, however 
    *       this can be alleviated by setting the AllowOverflow property to false. Enqueuing a filled queue will result in a
    *      CircularQueueOverflowException, so the programmer needs to handle this exception(perhaps wait until the queue is cleared?)
    */
    public class CircularQueue<T> : IEnumerable<T>, ICollection<T>
    {
        private int capacity; //how large is the queue?
        private int count;
        private int headIndex;
        private int tailIndex;
        private T[] queue;
        private const int defaultCapacity = 255; //default size to use in the parameterles constructor

        private object syncronizedRoot;
        #region properties
        public bool AllowOverflow { get; set; }

        public int Capacity
        {
            get
            {
                return capacity;
            }
            set
            {
                if (value == capacity)
                    return;
                if (value < count)
                {
                    throw new ArgumentOutOfRangeException("Trying to resize the queue to a size smaller than the current count of " + count);
                }
                var temp = new T[value];
                if (count > 0)
                {
                    CopyTo(temp);
                }
                queue = temp;
                capacity = value;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public T[] CirclQueue
        {
            get { return queue; }
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
            : this(defaultCapacity, false)
        {

        }
        /**
        * CircularQueue(bool allowOverflow)
        * @brief Constructor, sets the capacity to 255, and allow overflow is set according to the parameter passed in 
        * @param  allowOverflow
        * @return a new CircularQueueObject
        */
        public CircularQueue(bool allowOverflow)
            : this(defaultCapacity, allowOverflow)
        {

        }
        /**
        * CircularQueue(bool capacity)
        * @brief Constructor, sets the capacity according to the paramater passed in and doesn't allow overflow 
        * @param  capacity
        * @return a new CircularQueueObject
        */
        public CircularQueue(int capacity)
            : this(capacity, false)
        {

        }

       /**
       * CircularQueue(int capacity, bool allowOverflow)
       * @brief Constructor, sets the capacity according to the paramater passed in and sets overflow according to the allowOverflow parameter passed in
       * @param  capacity, allowOverflow
       * @return a new CircularQueueObject
       */
        public CircularQueue(int capacity, bool allowOverflow)
        {
            if (capacity < 0)
            {
                throw new EmptyCircularQueueException();
            }
            this.capacity = capacity;
            count = 0;
            headIndex = 0;
            tailIndex = 0;
            queue = new T[capacity];
            AllowOverflow = allowOverflow;
        }
        #endregion
       /**
       * Contains(T obj)
       * @brief Does the circular queue contain the object?
       * @param  obj
       * @return false if  object isn't found
       */
        public bool Contains(T obj)
        {
            int queueIndex = headIndex;
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < count; i++, queueIndex++)
            {
                if (queueIndex == capacity)
                {
                    queueIndex = 0;
                }
                if (obj == null && queue[queueIndex] == null)
                {
                    return true;
                }
                else if ((queue[queueIndex] != null) && comparer.Equals(queue[queueIndex], obj))
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
            count = 0;
            headIndex = 0;
            tailIndex = 0;
        }
       /**
       * Enqueue(T[] tA)
       * @brief  Enqueues an array of the same type as contained in the circular queue
       * @param  T[] tA
       * @return  number of items that were placed in the circular queue 
       */
        public int Enqueue(T[] tA)
        {
            return Enqueue(tA, 0, tA.Length);
        }
        /**
        * Enqueue(T[] tA, int offset, int length)
        * @brief  Enqueues an array of the same type as contained in the circular queue, starting from the offset, for a length of the array
        * @note If the AllowOverflow flag is set to false and the number of element that want to be placed in the queue is greater than the 
        * number of available slots then this method will throw a CircularQueueOverflowException
        * @param    tA,  offset, length
        * @return  number of items that were placed in the circular queue 
        */
        public int Enqueue(T[] tA, int offset, int length)
        {
            if (!AllowOverflow && length > Capacity - Count)
            {
                throw new CircularQueueOverflowException();
            }

            int startIndex = offset;
            for (int i = 0; i < length; i++, tailIndex++, startIndex++)
            {
                if (tailIndex == capacity)
                {
                    tailIndex = 0;
                }
                queue[tailIndex] = tA[startIndex];
            }

            count = Math.Min(Count + length, capacity);
            return count;
        }
        /**
        * Enqueue(T item)
        * @brief  Enqueues an item in the circular queue
        * @note If the AllowOverflow flag is set to false and the circular queue is full, then a CircularQueueOverflowException is thrown
        * @param item
        * @return 
        */
        public void Enqueue(T item)
        {
            if (!AllowOverflow && IsFull())
            {
                throw new CircularQueueOverflowException();
            }
            queue[tailIndex] = item;
            if (++tailIndex == capacity)
            {
                tailIndex = 0;
            }
            count++;
        }
        /**
        * IsFull( )
        * @brief Checks if the circular queue is full  
        * @param 
        * @return true if capacity is reached
        */
        public bool IsFull()
        {
            return count == capacity;
        }

       /**
       * Skip(int nAmount)
       * @brief Moves the head of circular buffer by nAmount
       * @note if the amount is larger than the number of actual elements in the buffer, then wrap the skip by count -1
       * @param nAmount
       * @return  
       */
        public void Skip(int nAmount)
        {
            headIndex += nAmount;
            if (headIndex >= Count)
            {
                headIndex -= Count -1; //wrap
            }
        }


        /**
        * Dequeue(T[] tA)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array 
        * @param  A
        * @return  the number of elements that were dequeued
        */
        public int Dequeue(ref T[] tA)
        {
            return Dequeue(ref tA, 0, tA.Length);
        }
        /**
        *  Dequeue(ref T[] tA, int offset, int length)
        * @brief Reference an array of type T and Dequeue the contents of the circular queue into this array, starting at the offset for a set length
        * @param tA,offset,length
        * @return the number of elements that were dequeued
        */
        public int Dequeue(ref T[] tA, int offset, int length)
        {
            int actualCount = Math.Min(length, count);
            int startIndex = offset;
            for (int i = 0; i < actualCount; i++, headIndex++, startIndex++)
            {
                if (headIndex == capacity)
                {
                    headIndex = 0;
                }
                tA[startIndex] = queue[headIndex];
            }
            count -= actualCount;
            return actualCount;
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
            var item = queue[headIndex];
            if (++headIndex == capacity)
            {
                headIndex = 0;
            }
            count--;
            return item;
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
            var item = queue[headIndex];
            return item;
        }

        #region copy buffered queue to array 
        /**
        * CopyTo(  T[] tA)
        * @brief  Copy the circular queue into the requested array tA without modifying the circular queue
        * @param  T[] tA
        * @return  
        */
        public void CopyTo(  T[] tA)
        {
            CopyTo(tA, 0);
        }

        public void CopyTo(  T[] tA, int index)
        {
            CopyTo(0,   tA, index, count);
        }

        public void CopyTo(int index,   T[] tA, int arrayIndex, int length)
        {
            if (length > Count)
            {
                throw new ArgumentOutOfRangeException("Length requested is too large");
            }

            int queueindex = headIndex;
            for (int i = 0; i < count; i++, queueindex++, arrayIndex++)
            {
                if (queueindex == capacity)
                {
                    queueindex = 0;
                }
                tA[arrayIndex] = queue[queueindex];
            }
        }

        #endregion
        
        public IEnumerator<T> GetEnumerator()
        {
            int queueIndex = headIndex;
            for (int i = 0; i < Count; i++, queueIndex++)
            {
                if (queueIndex == capacity)
                {
                    queueIndex = 0;
                }
                yield return queue[queueIndex];
            }
        }
       
        public T[] ToArray()
        {
            var tA = new T[Count];
            CopyTo(tA);
            return tA;
        }


        int ICollection<T>.Count
        {
            get
            {
                return count;
            }
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            if (Count == 0)
            {
                return false;
            }
            Dequeue();
            return true;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }



        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class EmptyCircularQueueException : Exception
    {

    }

    public class CircularQueueOverflowException : Exception
    {

    }
}
