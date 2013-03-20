namespace Iquest.Messaging.Data
{
	public interface IFilter<in T>
	{
		#region Public Methods

		bool IsMatch(T item);

		#endregion
	}
}