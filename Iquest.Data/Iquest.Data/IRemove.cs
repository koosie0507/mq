namespace Iquest.Messaging.Data
{
	public interface IRemove<in T>
	{
		#region Public Methods

		void Remove(T item);

		#endregion
	}
}