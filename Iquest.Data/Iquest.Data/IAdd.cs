using System.Collections.Generic;

namespace Iquest.Messaging.Data
{
	public interface IAdd<T>
	{
		#region Public Methods

		void Add(T item);

		void AddRange(IEnumerable<T> items);

		#endregion
	}
}