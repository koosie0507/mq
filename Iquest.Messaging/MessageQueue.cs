using System;
using System.Collections;
using System.Collections.Generic;

using Iquest.Messaging.Data;
using Iquest.Messaging.Data.Model;

namespace Iquest.Messaging.Queue
{
	public class MessageQueue<T> : IEnumerable<T>
		where T : Message
	{
		#region Constructors

		public MessageQueue()
		{
			this.items = new LinkedList<T>();
		}

		public MessageQueue(IPersistence<T> persistence)
			: this()
		{
			if (ReferenceEquals(null, persistence))
			{
				throw new ArgumentNullException();
			}
			this.persistence = persistence;
		}

		#endregion

		#region Public Properties

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		public IPersistence<T> Persistence
		{
			get
			{
				return this.persistence;
			}
		}

		#endregion

		#region Public Methods

		private bool SafeGet<TResult>(Func<IPersistence<T>, TResult> callback, out TResult item)
		{
			item = default(TResult);
			if(ReferenceEquals(null, this.persistence))
			{
				return false;
			}

			item = callback(this.persistence);
			return !ReferenceEquals(null, item);
		}

		public void Enqueue(T message)
		{
			this.items.AddLast(message);

			IAdd<T> insertion;
			if(SafeGet(x => x.NewAdd(), out insertion))
			{
				insertion.Add(message);
			}
		}

		public bool IsEmpty()
		{
			return this.items.Count == 0;
		}

		public T Dequeue()
		{
			if (IsEmpty())
			{
				throw new QueueUnderflowException();
			}

			try
			{
				return this.items.First.Value;
			}
			finally
			{
				this.items.RemoveFirst();
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>1</filterpriority>
		public IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Constants and Fields

		private readonly LinkedList<T> items;

		private readonly IPersistence<T> persistence;

		#endregion
	}
}