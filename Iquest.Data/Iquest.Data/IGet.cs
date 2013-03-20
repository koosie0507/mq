using System.Collections.Generic;

namespace Iquest.Messaging.Data
{
	public interface IGet<T>
	{
		#region Public Methods

		IEnumerable<T> All();

		IEnumerable<T> Filter(IFilter<T> filter);

		bool IsEmpty();

		#endregion
	}
}