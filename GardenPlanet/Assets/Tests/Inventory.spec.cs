using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;

namespace GardenPlanet
{
    [TestFixture]
    public class InventoryTests
    {
        private Dictionary<string, ItemType> itemTypes;

        [SetUp]
        public void SetUpItemTypes()
        {
            itemTypes = new Dictionary<string, ItemType>
            {
                {
                    "cabbage", new ItemType
                    {
                        data = new ItemType.ItemTypeData
                        {
                            ID = "cabbage",
                            DisplayName = "Cabbage",
                            Description = "Beep boop cabbage",
                            Category = "Produce",
                            StackSize = 32,
                            CanPickup = true,
                            Attributes = new Hashtable(),
                            Script = "Consumable",
                            Appearance = null
                        }
                    }
                },
                {
                    "shovel", new ItemType
                    {
                        data = new ItemType.ItemTypeData
                        {
                            ID = "shovel",
                            DisplayName = "Shuvel",
                            Description = "Bury bury dig dig",
                            Category = "Tool",
                            StackSize = 1,
                            CanPickup = true,
                            Attributes = new Hashtable(),
                            Script = "Shovel",
                            Appearance = null
                        }
                    }
                }
            };
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

        [Test]
        public void TestThatAStackWithASingleItemIsCreated()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(1));
        }

        [Test]
        public void TestThatAStackWithASingleItemOfMoreThanOneQuantityIsCreated()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 10), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(10));
        }

        [Test]
        public void TestThatTwoStacksAreCreatedWithTwoItemTypes()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["shovel"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(1));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["shovel"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(1));
        }

        [Test]
        public void TestThatStacksCombineWhenAddingExistingItemTypes()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 5), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(6));
        }

        [Test]
        public void TestThatANewStackIsCreatedWhenExistingStackIsFull()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 6), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(6));
        }

        [Test]
        public void TestThatItemsWillOverflowIntoNewStacks()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 20), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(4));
        }

        [Test]
        public void TestThatItemsWillOverflowIntoMultipleStacks()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 20), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 100), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(4));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(24));
        }

        [Test]
        public void TestThatSameItemTypesWithDifferentAttibutesBecomeDifferentStacks()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], new Attributes{{"quality",1}}, 2), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], new Attributes{{"quality",5}}, 5), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(2));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(5));
        }

        [Test]
        public void TestThatWontAddWhenInventoryIsFull()
        {

        }

        [Test]
        public void TestThatWillStillAddItemToFullInventoryWhenItWillFit()
        {

        }

        [Test]
        public void TestRemoveItem()
        {
            
        }
    }
}