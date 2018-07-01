using NUnit.Framework;
using NUnit.Framework.Constraints;
using UnityEditor;

namespace GardenPlanet
{
    [TestFixture]
    public class InventoryTests
    {

        [SetUp]
        public void SetUpItemTypes()
        {

        }

        [Test]
        public void TestThatWhenConstructedTheItemListIsEmpty()
        {
            var inv = new Inventory(10);
            Assert.That(inv.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestThatYouPassTheMaximumInventorySizeWithTheConstructor()
        {
            var inv = new Inventory(20);
            Assert.That(inv.maximumItemStacks, Is.EqualTo(20));
        }

    }
}