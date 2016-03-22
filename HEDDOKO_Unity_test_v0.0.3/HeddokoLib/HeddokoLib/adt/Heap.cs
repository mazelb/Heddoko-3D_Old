/*@file Heap.cs
* @brief Contains the Heap class
* @author Mohammed Haider(mohammed@heddoko.com)
* @date March 2016
* Copyright Heddoko(TM) 2016, all rights reserved
*/
using System;
using System.Collections.Generic;

namespace HeddokoLib.adt
{
    /// <summary>
    /// A generic heap, with a list based implementation
    /// </summary>
    /// <typeparam name="T">A HeapItem</typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {
        private List<T> mItems = new List<T>();
        private int mCurrentItemCount;

        public int Count
        {
            get { return mCurrentItemCount; }
        }





        /// <summary>
        /// Add an vItem to the heap
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            item.HeapIndex = mCurrentItemCount;
            mItems[mCurrentItemCount] = item;
            SortUp(item);
            mCurrentItemCount++;
        }

        /// <summary>
        /// Removes the first vItem from the heap.
        /// </summary>
        /// <returns>The first vItem</returns>
        public T RemoveFirstItem()
        {
            T vFirstItems = mItems[0];
            mCurrentItemCount--;
            mItems[0] = mItems[mCurrentItemCount];
            mItems[0].HeapIndex = 0;
            SortDown((mItems[0]));
            return vFirstItems;
        }

        /// <summary>
        /// Bubble down
        /// </summary>
        /// <param name="vItem"></param>
        private void SortDown(T vItem)
        {
            while (true)
            {
                int vChildIndexLeft = (2 * vItem.HeapIndex) + 1;

                int vChildIndexRight = (2 * vItem.HeapIndex) + 2;
                int vSwapItem = 0;

                if (vChildIndexLeft < mCurrentItemCount)
                {
                    vSwapItem = vChildIndexLeft;

                    if (vChildIndexRight < mCurrentItemCount)
                    {
                        if (mItems[vChildIndexLeft].CompareTo(mItems[vChildIndexRight]) < 0)
                        {
                            vSwapItem = vChildIndexRight;
                        }

                    }
                    if (vItem.CompareTo(mItems[vSwapItem]) < 0)
                    {
                        Swap(vItem, mItems[vSwapItem]);
                    }
                    else
                        return;

                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Update the vItem T
        /// </summary>
        /// <param name="vItem"></param>
        public void UpdateItem(T vItem)
        {
            SortUp(vItem);
        }

        public bool Contains(T item)
        {
            return Equals(mItems[item.HeapIndex], item);
        }

        void SortUp(T vItem)
        {
            int parentIndex = (vItem.HeapIndex - 1) / 2;

            while (true)
            {
                T vParentItem = mItems[parentIndex];
                if (vItem.CompareTo(vParentItem) > 0)
                //if vItem has a higher priority than parent vItem then CompareTo returns 1, if its lower then return -1, else if its the same then return 0
                {
                    Swap(vItem, vParentItem);
                }
                else
                {
                    break;
                }
            }

        }

        /// <summary>
        /// Swaps between Item A and B
        /// </summary>
        /// <param name="vItemA"></param>
        /// <param name="vItemB"></param>
        private void Swap(T vItemA, T vItemB)
        {
            mItems[vItemA.HeapIndex] = vItemB;
            mItems[vItemB.HeapIndex] = vItemA;
            int tempItemAIndex = vItemA.HeapIndex;
            vItemA.HeapIndex = vItemB.HeapIndex;
            vItemB.HeapIndex = tempItemAIndex;

        }

    }

    public interface IHeapItem<T> : IComparable
    {
        int HeapIndex { get; set; }
    }
}
