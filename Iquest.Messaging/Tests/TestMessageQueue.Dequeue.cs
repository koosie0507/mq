using System.Linq;

using Iquest.Messaging.Data.Model;

using NUnit.Framework;

namespace Iquest.Messaging.Queue.Tests
{
	[TestFixture(TypeArgs = new[] { typeof(Message) })]
	sealed partial class TestMessageQueue<T>
		where T : Message, new()
	{
		#region Public Methods

		[Test]
		public void Dequeue_HappyFlow_QueueWithOneItemBecomesEmpty()
		{
			var queue = new MessageQueue<T>();
			queue.Enqueue(new T());

			queue.Dequeue();

			Assert.That(queue.IsEmpty(), Is.True);
		}

		[TestCaseSource("GetQueuedItems")]
		public void Dequeue_HappyFlow_ReturnsFirstPush(T[] input)
		{
			T expected = input[0];
			MessageQueue<T> queue = SetupMessageQueue(input);

			T actual = queue.Dequeue();

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test]
		public void Dequeue_EmptyQueue_ThrowsQueueUnderflowException()
		{
			var queue = new MessageQueue<T>();

			Assert.Throws<QueueUnderflowException>(() => queue.Dequeue());
		}

		[TestCaseSource("GetQueuedItems")]
		public void Dequeue_HappyFlow_RemovesFirstItem(T[] input)
		{
			MessageQueue<T> queue = SetupMessageQueue(input);

			queue.Dequeue();

			Assert.That(queue, Is.EquivalentTo(input.Skip(1)));
		}

		#endregion
	}
}