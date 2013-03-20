using Iquest.Messaging.Data.Model;

using NUnit.Framework;

namespace Iquest.Messaging.Data.Tests
{
	[TestFixture]
	public sealed class MessageTests
	{
		#region Public Methods

		[TestCase(0, 0, true)]
		[TestCase(1, 0, false)]
		[TestCase(0, 1, false)]
		public void Equals_HappyFlow_ReturnsExpectedValue(int id1, int id2, bool expected)
		{
			Assert.That(new Message { Id = id1 }.Equals(new Message { Id = id2 }), Is.EqualTo(expected));
		}

		[Test]
		public void Equals_NullOther_ReturnsFalse()
		{
			Assert.That(new Message().Equals(null), Is.False);
		}

		#endregion
	}
}