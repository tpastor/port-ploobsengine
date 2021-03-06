﻿#region License
/*
    PloobsEngine Game Engine Version 0.3 Beta
    Copyright (C) 2011  Ploobs

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Collections;

namespace PloobsEngine.DataStructure
{
    /// <summary>
    /// Circular List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CircularList<T> : IEnumerable<T>, IEnumerator<T>
    {
        protected T[] items;
        protected int idx;
        protected bool loaded;
        protected int enumIdx;

        /// <summary>
        /// Constructor that initializes the list with the 
        /// required number of items.
        /// </summary>
        public CircularList(int numItems)
        {
            if (numItems <= 0)
            {
                throw new ArgumentOutOfRangeException("numItems can't be negative or 0.");
            }

            items = new T[numItems];
            idx = 0;
            loaded = false;
            enumIdx = -1;
        }

        /// <summary>
        /// Gets/sets the item value at the current index.
        /// </summary>
        public T Value
        {
            get { return items[idx]; }
            set { items[idx] = value; }
        }

        /// <summary>
        /// Returns the count of the number of loaded items, up to
        /// and including the total number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return loaded ? items.Length : idx; }
        }

        /// <summary>
        /// Returns the length of the items array.
        /// </summary>
        public int Length
        {
            get { return items.Length; }
        }

        /// <summary>
        /// Gets/sets the value at the specified index.
        /// </summary>
        public T this[int index]
        {
            get
            {
                RangeCheck(index);
                return items[index];
            }
            set
            {
                RangeCheck(index);
                items[index] = value;
            }
        }

        /// <summary>
        /// Advances to the next item or wraps to the first item.
        /// </summary>
        public void Next()
        {
            if (++idx == items.Length)
            {
                idx = 0;
                loaded = true;
            }
        }

        /// <summary>
        /// Clears the list, resetting the current index to the 
        /// beginning of the list and flagging the collection as unloaded.
        /// </summary>
        public void Clear()
        {
            idx = 0;
            items.Initialize();
            loaded = false;
        }

        /// <summary>
        /// Sets all items in the list to the specified value, resets
        /// the current index to the beginning of the list and flags the
        /// collection as loaded.
        /// </summary>
        public void SetAll(T val)
        {
            idx = 0;
            loaded = true;

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = val;
            }
        }

        /// <summary>
        /// Internal indexer range check helper.  Throws
        /// ArgumentOutOfRange exception if the index is not valid.
        /// </summary>
        protected void RangeCheck(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Indexer cannot be less than 0.");
            }

            if (index >= items.Length)
            {
                throw new ArgumentOutOfRangeException("Indexer cannot be greater than or equal to the number if items in the collection.");
            }
        }

        // Interface implementations:
        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        ///   </returns>
        public T Current
        {
            get { return this[enumIdx]; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get { return this[enumIdx]; }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        ///   </exception>
        public bool MoveNext()
        {
            ++enumIdx;
            bool ret = enumIdx < Count;

            if (!ret)
            {
                Reset();
            }

            return ret;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        ///   </exception>
        public void Reset()
        {
            enumIdx = -1;
        }
    }
}