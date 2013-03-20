using System;

using Iquest.Messaging.Data;

using Moq;

using NUnit.Framework;

namespace Iquest.Messaging.Queue.Tests
{
	sealed partial class TestMessageQueue<T>
	{
		#region Public Methods

		[Test]
		public void Ctor_HappyFlow_SetsPersistence()
		{
			var persistence = Mock.Of<IPersistence<T>>();

			var sut = new MessageQueue<T>(persistence);

			Assert.That(sut.Persistence, Is.InstanceOf<IPersistence<T>>());
			Assert.That(sut.Persistence, Is.SameAs(persistence));
		}

		[Test]
		public void Ctor_NullPersistence_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MessageQueue<T>(null));
		}

		[Test]
		public void Enqueue_PersistenceIsSet_CallsPersistenceNewAdd()
		{
			var persistenceMock = new Mock<IPersistence<T>>();
			var sut = new MessageQueue<T>(persistenceMock.Object);

			sut.Enqueue(new T());

			persistenceMock.Verify(x => x.NewAdd(), Times.Once());
		}

		[Test]
		public void Enqueue_PersistenceIsSet_CallsPersistenceNewAddAdd()
		{
			var add = new Mock<IAdd<T>>();
			var persistenceMock = new Mock<IPersistence<T>>();
			persistenceMock.Setup(x => x.NewAdd()).Returns(add.Object);
			var sut = new MessageQueue<T>(persistenceMock.Object);
			var expectedMessage = new T();

			sut.Enqueue(expectedMessage);

			add.Verify(x=>x.Add(expectedMessage), Times.Once());
		}

		#endregion
	}
}