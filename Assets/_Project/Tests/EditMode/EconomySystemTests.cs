using NUnit.Framework;
using TaskBarFisher.Core.Economy;
using TaskBarFisher.Shared.Contracts;
using TaskBarFisher.Shared.EventBus;

namespace TaskBarFisher.Tests
{
    public class EconomySystemTests
    {
        [Test]
        public void Add_IncreasesBalance_AndPublishesEvent()
        {
            var bus = new GameEventBus();
            CurrencyChangedEvent? lastEvent = null;
            bus.Subscribe<CurrencyChangedEvent>(e => lastEvent = e);
            var economy = new EconomySystem(bus);

            economy.Add("soft", 100);

            Assert.AreEqual(100, economy.GetBalance("soft"));
            Assert.IsTrue(lastEvent.HasValue);
            Assert.AreEqual(100, lastEvent.Value.NewBalance);
            Assert.AreEqual(100, lastEvent.Value.Delta);
        }

        [Test]
        public void Spend_WithInsufficientBalance_ReturnsFalse_AndDoesNotChangeBalance()
        {
            var economy = new EconomySystem(new GameEventBus());
            economy.Add("soft", 50);

            var success = economy.Spend("soft", 100);

            Assert.IsFalse(success);
            Assert.AreEqual(50, economy.GetBalance("soft"));
        }

        [Test]
        public void Spend_WithSufficientBalance_DecreasesBalance()
        {
            var economy = new EconomySystem(new GameEventBus());
            economy.Add("soft", 100);

            var success = economy.Spend("soft", 30);

            Assert.IsTrue(success);
            Assert.AreEqual(70, economy.GetBalance("soft"));
        }
    }
}
