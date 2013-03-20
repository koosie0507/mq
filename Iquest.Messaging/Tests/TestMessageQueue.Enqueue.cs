using System.Collections.Generic;
using System.Linq;

using Iquest.Messaging.Data.Model;

using NUnit.Framework;

namespace Iquest.Messaging.Queue.Tests
{
	[TestFixture(TypeArgs = new[] { typeof(Message) })]
	public sealed partial class TestMessageQueue<T>
		where T : Message, new()
	{
		#region Public Methods

		[Test]
		public void Enqueue_HappyFlow_EmptyQueueBecomesNonEmpty()
		{
			var queue = new MessageQueue<T>();

			queue.Enqueue(new T());

			Assert.That(queue.IsEmpty(), Is.False);
		}

		[TestCase(0)]
		[TestCase(1)]
		public void Enqueue_HappyFlow_IncreasesQueueCountByOne(int n)
		{
			MessageQueue<T> queue = SetupMessageQueue(n);

			queue.Enqueue(new T());

			Assert.That(queue, Has.Count.EqualTo(n + 1));
		}

		[TestCaseSource("GetQueuedItems")]
		public void Enqueue_HappyFlow_QueueContainsAllItemsInQueuedSequence(T[] input)
		{
			var queue = new MessageQueue<T>();
			foreach (T item in input)
			{
				queue.Enqueue(item);
			}

			Assert.That(queue, Is.EquivalentTo(input));
		}

		#endregion

		#region Private Methods

		private IEnumerable<TestCaseData> GetQueuedItems()
		{
			yield return new TestCaseData((object)new[] { new T() });
			yield return new TestCaseData((object)new[] { new T(), new T() });
		}

		private static MessageQueue<T> SetupMessageQueue(IEnumerable<T> input)
		{
			var queue = new MessageQueue<T>();

			foreach (T item in input)
			{
				queue.Enqueue(item);
			}
			return queue;
		}

		private static MessageQueue<T> SetupMessageQueue(int count)
		{
			return SetupMessageQueue(Enumerable.Repeat(new T(), count));
		}

		#endregion
	}
}