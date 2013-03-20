using System;

namespace Iquest.Messaging.Data.Model
{
	public class Message : IEquatable<Message>
	{
		#region Public Properties

		public int Id { get; set; }

		#endregion

		#region Public Methods

		public bool Equals(Message other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}

			return this.Id == other.Id;
		}

		#endregion
	}
}