using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
	public class DynamicArray<T> : IEnumerable<T>, IEnumerable, IList<T>, ICollection<T>
	{
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			public DynamicArray<T> m_array;

			public int m_index;

			public T Current => m_array.Array[m_index];

			object IEnumerator.Current => m_array.Array[m_index];

			public Enumerator(DynamicArray<T> array)
			{
				m_array = array;
				m_index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				m_index++;
				return m_index < m_array.Count;
			}

			public void Reset()
			{
				m_index = -1;
			}
		}

		public const int MinCapacity = 4;

		public T[] m_array = m_emptyArray;

		public int m_count;

		public static T[] m_emptyArray = new T[0];

		public int Capacity
		{
			get
			{
				return m_array.Length;
			}
			set
			{
				if (value < m_count)
				{
					throw new InvalidOperationException("Capacity cannot be made smaller than number of elements.");
				}
				if (value == Capacity)
				{
					return;
				}
				if (value > 0)
				{
					T[] array = new T[value];
					if (m_array != null)
					{
						System.Array.Copy(m_array, 0, array, 0, m_count);
					}
					m_array = array;
				}
				else
				{
					m_array = m_emptyArray;
				}
			}
		}

		public int Count
		{
			get
			{
				return m_count;
			}
			set
			{
				while (Capacity < value)
				{
					Capacity = MathUtils.Max(Capacity * 2, 4);
				}
				m_count = value;
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= m_count)
				{
					throw new IndexOutOfRangeException();
				}
				return m_array[index];
			}
			set
			{
				if (index >= m_count)
				{
					throw new IndexOutOfRangeException();
				}
				m_array[index] = value;
			}
		}

		public T[] Array => m_array;

		public bool IsReadOnly => false;

		public DynamicArray()
		{
		}

		public DynamicArray(int capacity)
		{
			Capacity = capacity;
		}

		public DynamicArray(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			Capacity = items.Count();
			foreach (T item in items)
			{
				Add(item);
			}
		}

		public int IndexOf(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < m_count; i++)
			{
				if (@default.Equals(item, m_array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public void Add(T item)
		{
			if (m_count >= Capacity)
			{
				Capacity = MathUtils.Max(Capacity * 2, 4);
			}
			m_array[m_count] = item;
			m_count++;
		}

		public void AddRange(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			ICollection collection = items as ICollection;
			if (collection != null)
			{
				Capacity = MathUtils.Max(Capacity, Count + collection.Count);
				foreach (T item in items)
				{
					m_array[m_count] = item;
					m_count++;
				}
			}
			else
			{
				foreach (T item2 in items)
				{
					Add(item2);
				}
			}
		}

		public void AddRange(DynamicArray<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			Capacity = MathUtils.Max(Capacity, Count + items.Count);
			for (int i = 0; i < items.Count; i++)
			{
				m_array[m_count] = items.Array[i];
				m_count++;
			}
		}

		public bool Remove(T item)
		{
			int num = IndexOf(item);
			if (num >= 0)
			{
				RemoveAt(num);
				return true;
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			if (index < m_count)
			{
				m_count--;
				if (index < m_count)
				{
					System.Array.Copy(m_array, index + 1, m_array, index, m_count - index);
				}
				return;
			}
			throw new IndexOutOfRangeException();
		}

		public void RemoveAtEnd()
		{
			if (m_count > 0)
			{
				m_count--;
				return;
			}
			throw new IndexOutOfRangeException();
		}

		public int RemoveAll(Predicate<T> match)
		{
			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			int i;
			for (i = 0; i < m_count && !match(m_array[i]); i++)
			{
			}
			if (i >= m_count)
			{
				return 0;
			}
			int j = i + 1;
			while (j < m_count)
			{
				for (; j < m_count && match(m_array[j]); j++)
				{
				}
				if (j < m_count)
				{
					m_array[i++] = m_array[j++];
				}
			}
			int result = m_count - i;
			m_count = i;
			return result;
		}

		public void Insert(int index, T item)
		{
			if (index <= m_count)
			{
				if (m_count >= Capacity)
				{
					Capacity = MathUtils.Max(Capacity * 2, 4);
				}
				if (index < m_count)
				{
					System.Array.Copy(m_array, index, m_array, index + 1, m_count - index);
				}
				m_array[index] = item;
				m_count++;
				return;
			}
			throw new IndexOutOfRangeException();
		}

		public void Clear()
		{
			m_count = 0;
		}

		public void Reverse()
		{
			int num = 0;
			int num2 = m_count - 1;
			while (num < num2)
			{
				T val = m_array[num];
				m_array[num] = m_array[num2];
				m_array[num2] = val;
				num++;
				num2--;
			}
		}

		public List<T> ToList()
		{
			List<T> list = new List<T>(Count);
			for (int i = 0; i < Count; i++)
			{
				list.Add(Array[i]);
			}
			return list;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public bool Contains(T item)
		{
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			for (int i = 0; i < m_count; i++)
			{
				if (@default.Equals(item, m_array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			System.Array.Copy(m_array, 0, array, arrayIndex, m_count);
		}
	}
}
