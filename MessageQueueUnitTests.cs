using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnotherMessageQueue
{
    [TestClass]
    public class MessageQueueUnitTests
    {
        [TestMethod]
        public void TestSmoke()
        {
            MessageQueue q = new MessageQueue();
        }

        [TestMethod]
        public void TestAddMessage()
        {
            MessageQueue q = new MessageQueue(new InMemoryDal());
            Message m = new Message();
            q.Add(m);
            Assert.IsFalse(q.IsEmpty());
        }

        [TestMethod]
        public void TestGetNextMessage()
        {
            MessageQueue q = new MessageQueue(new InMemoryDal());
            Message m = new Message();
            q.Add(m);
            Message anotherMessage = q.GetNext();
            Assert.AreEqual(m, anotherMessage);
        }

        [TestMethod]
        public void TestRemovingTheLastMessageMakesTheQueueEmpty()
        {
            MessageQueue q = new MessageQueue(new InMemoryDal());
            Message m = new Message();
            q.Add(m);
            q.GetNext();
            Assert.IsTrue(q.IsEmpty());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestBadPersistence()
        {
            Message m = new Message();
            MessageQueue badQueue = new MessageQueue();
            badQueue.Add(m);
        }

        [TestMethod]
        public void TestPersistenceWasCalled()
        {
            InMemoryDal d = new InMemoryDal();
            MessageQueue q = new MessageQueue(d);
            int addCount = 0;
            d.Added += (s, m) => addCount++;
            q.Add(new Message());
            Assert.IsTrue(addCount==1);
        }

        [TestMethod]
        public void TestAddingTwoMessagesCallsDalTwoTimes()
        {
            InMemoryDal d = new InMemoryDal();
            MessageQueue q = new MessageQueue(d);
            Message m = new Message();
            int addCount = 0;
            d.Added += (s, maso) => addCount++;
            q.Add(new Message());
            q.Add(new Message());
            Assert.AreEqual(2, addCount);
        }

        [TestMethod]
        public void TestSomethingComplicated()
        {
            int N = 42000;
            for (int i = 0; i < N; ++i)
            {
                List<Message> inputSequence = new List<Message>();
                for (int mm = 0; mm < i; ++mm)
                {
                    inputSequence.Add(new Message());
                }
                InMemoryDal d = new InMemoryDal();
                MessageQueue q = new MessageQueue(d);
                Message m = new Message();
                var list = new List<Message>();
                d.Added += (s, maso) => list.Add(m);

                foreach (Message message in inputSequence)
                {
                    q.Add(message);
                }

                //Assert.IsTrue(m1.Equals(list[0]));
                //Assert.IsTrue(m2.Equals(list[1]));
                Assert.IsTrue(inputSequence.SequenceEqual(list));
                
            }
        }

        [TestMethod]
        public void TestPersistence()
        {
            MessageQueue q1 = new MessageQueue(new MessageQueueDal());
            Message m1 = new Message();
            q1.Add(m1);

            MessageQueue q2 = new MessageQueue(new MessageQueueDal());
            Message m2 = q2.GetNext();

            Assert.IsTrue(m1.Equals(m2));
        }
    }

    [TestClass]
    public class MessageQueueDalTests
    {
        [TestMethod]
        public void TestSmoke() 
        {
            MessageQueueDal dal = new MessageQueueDal();
            Assert.IsInstanceOfType(dal, typeof(DbContext));
        }
    }


    public interface IMessageQueueDal
    {
        void Add(Message m);
        void RemoveMessage(Message message);
        bool IsEmpty();
        Message GetNext();
    }

    public class InMemoryDal: IMessageQueueDal
    {
        private List<Message> q = new List<Message>();

        public event EventHandler<Message> Added;

        protected virtual void OnAdd(Message e)
        {
            EventHandler<Message> handler = Added;
            if (handler != null) handler(this, e);
        }

        public void Add(Message m)
        {
            q.Add(m);
            OnAdd(m);
        }

        public void RemoveMessage(Message message)
        {
            q.Remove(message);
        }

        public bool IsEmpty()
        {
            return q.Count() == 0;
        }

        public Message GetNext()
        {
            return q.First();
        }
    }
    public class MessageQueueDal: DbContext, IMessageQueueDal
    {
        public DbSet<Message> Messages { get; set; }

        public void Add(Message m)
        {
            Messages.Add(m);
            SaveChanges();
        }

        public void Clear()
        {
            foreach (Message m in Messages)
            {
                Messages.Remove(m);
            }
            SaveChanges();
        }

        public void RemoveMessage(Message message)
        {
            Messages.Remove(message);
            SaveChanges();
        }

        public bool IsEmpty()
        {
            return Messages.Count() == 0;
        }

        public Message GetNext()
        {
            return Messages.First();
        }
    }

    public class Message: IEquatable<Message>
    {
        public bool Equals(Message other)
        {
            return id == other.id;
        }

        public int id { get; set; }
    }

    public class MessageQueue
    {
        private IMessageQueueDal dal;

        public MessageQueue(IMessageQueueDal messageQueueDal)
        {
            dal = messageQueueDal;
        }

        public MessageQueue()
        {
        }

        public void Add(Message message)
        {
            if (dal == null)
                throw new InvalidOperationException();
            dal.Add(message);
        }

        public bool IsEmpty()
        {
            return dal.IsEmpty();
        }

        public Message GetNext()
        {
            Message result = dal.GetNext();
            dal.RemoveMessage(result);
            return result;
        }
    }
}
