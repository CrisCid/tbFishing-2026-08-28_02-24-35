using NUnit.Framework;
using TaskBarFisher.Shared.EventBus;

namespace TaskBarFisher.Tests
{
    /// <summary>
    /// Test de ejemplo -- prueba que el bus es POCO puro y corre sin necesidad de Play Mode ni de
    /// ningún GameObject en escena. Sirve de plantilla para los tests que cada módulo agregue en
    /// Fase 1 (Economy, Gacha, Save, etc. son igual de testeables por el mismo motivo).
    /// </summary>
    public class GameEventBusTests
    {
        struct DummyEvent { public int Value; }

        [Test]
        public void Publish_InvokesSubscribedHandler()
        {
            var bus = new GameEventBus();
            var received = -1;
            bus.Subscribe<DummyEvent>(e => received = e.Value);

            bus.Publish(new DummyEvent { Value = 42 });

            Assert.AreEqual(42, received);
        }

        [Test]
        public void Unsubscribe_StopsReceivingEvents()
        {
            var bus = new GameEventBus();
            var callCount = 0;
            void Handler(DummyEvent e) => callCount++;

            bus.Subscribe<DummyEvent>(Handler);
            bus.Unsubscribe<DummyEvent>(Handler);
            bus.Publish(new DummyEvent { Value = 1 });

            Assert.AreEqual(0, callCount);
        }

        [Test]
        public void Publish_WithNoSubscribers_DoesNotThrow()
        {
            var bus = new GameEventBus();
            Assert.DoesNotThrow(() => bus.Publish(new DummyEvent { Value = 1 }));
        }
    }
}
