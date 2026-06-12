using MobileFramework.Core.Events;
using NUnit.Framework;

namespace MobileFramework.Tests.EditMode.Events
{
    public class EventBusTests
    {
        private readonly struct TestEvent
        {
            public readonly int Value;

            public TestEvent(int value) => Value = value;
        }

        private readonly struct OtherEvent
        {
        }

        private EventBus _bus;

        [SetUp]
        public void SetUp() => _bus = new EventBus();

        [Test]
        public void Emit_DeliversPayloadToSubscriber()
        {
            var received = 0;
            _bus.Subscribe<TestEvent>(e => received = e.Value);

            _bus.Emit(new TestEvent(42));

            Assert.AreEqual(42, received);
        }

        [Test]
        public void Emit_ReachesAllSubscribers()
        {
            var count = 0;
            _bus.Subscribe<TestEvent>(_ => count++);
            _bus.Subscribe<TestEvent>(_ => count++);

            _bus.Emit(new TestEvent(1));

            Assert.AreEqual(2, count);
        }

        [Test]
        public void Emit_DoesNotReachSubscribersOfOtherEvents()
        {
            var called = false;
            _bus.Subscribe<OtherEvent>(_ => called = true);

            _bus.Emit(new TestEvent(1));

            Assert.IsFalse(called);
        }

        [Test]
        public void Unsubscribe_StopsDelivery()
        {
            var count = 0;
            void Handler(TestEvent e) => count++;

            _bus.Subscribe<TestEvent>(Handler);
            _bus.Emit(new TestEvent(1));
            _bus.Unsubscribe<TestEvent>(Handler);
            _bus.Emit(new TestEvent(1));

            Assert.AreEqual(1, count);
        }

        [Test]
        public void Clear_RemovesAllSubscribers()
        {
            var count = 0;
            _bus.Subscribe<TestEvent>(_ => count++);
            _bus.Subscribe<OtherEvent>(_ => count++);

            _bus.Clear();
            _bus.Emit(new TestEvent(1));
            _bus.Emit(new OtherEvent());

            Assert.AreEqual(0, count);
            Assert.AreEqual(0, _bus.CountSubscribers<TestEvent>());
        }

        [Test]
        public void SubscriberAddedDuringEmit_NotInvokedInSameRound_ButInvokedNext()
        {
            var lateCalls = 0;
            _bus.Subscribe<TestEvent>(_ => _bus.Subscribe<TestEvent>(__ => lateCalls++));

            _bus.Emit(new TestEvent(1));
            Assert.AreEqual(0, lateCalls, "il listener aggiunto durante l'emissione non deve girare nello stesso round");

            _bus.Emit(new TestEvent(1));
            Assert.AreEqual(1, lateCalls);
        }

        [Test]
        public void ClearDuringEmit_DoesNotThrow_AndNextEmitIsSilent()
        {
            var count = 0;
            _bus.Subscribe<TestEvent>(_ =>
            {
                count++;
                _bus.Clear();
            });

            Assert.DoesNotThrow(() => _bus.Emit(new TestEvent(1)));
            _bus.Emit(new TestEvent(1));

            Assert.AreEqual(1, count);
        }

        [Test]
        public void CountSubscribers_TracksSubscriptions()
        {
            void Handler(TestEvent e)
            {
            }

            Assert.AreEqual(0, _bus.CountSubscribers<TestEvent>());
            _bus.Subscribe<TestEvent>(Handler);
            Assert.AreEqual(1, _bus.CountSubscribers<TestEvent>());
            _bus.Unsubscribe<TestEvent>(Handler);
            Assert.AreEqual(0, _bus.CountSubscribers<TestEvent>());
        }
    }
}
