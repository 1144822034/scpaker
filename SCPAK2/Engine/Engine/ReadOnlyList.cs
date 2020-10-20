using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine
{
	public struct ReadOnlyList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			public IList<T> m_list;

			public int m_index;

			public T Current => m_list[m_index];

			object IEnumerator.Current => m_list[m_index];

			public Enumerator(IList<T> list)
			{
				m_list = list;
				m_index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return ++m_index < m_list.Count;
			}

			public void Reset()
			{
				m_index = -1;
			}
		}

		public IList<T> m_list;

		public static ReadOnlyList<T> m_empty = new ReadOnlyList<T>(new T[0]);

		public static ReadOnlyList<T> Empty => m_empty;

		public T this[int index]
		{
			get
			{
				return m_list[index];
			}
			set
			{
				throw new NotSupportedException("List is readonly.");
			}
		}

		public int Count => m_list.Count;

		public bool IsReadOnly => true;

		public ReadOnlyList(IList<T> list)
		{
			m_list = list;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(m_list);
		}

		public int IndexOf(T item)
		{
			return m_list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException("List is readonly.");
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException("List is readonly.");
		}

		public void Add(T item)
		{
			throw new NotSupportedException("List is readonly.");
		}

		public void Clear()
		{
			throw new NotSupportedException("List is readonly.");
		}

		public bool Contains(T item)
		{
			return m_list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			m_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException("List is readonly.");
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(m_list);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(m_list);
		}
	}
}
