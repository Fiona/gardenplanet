using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace GardenPlanet
{
    [TestFixture]
    public class InventoryTests
    {
        private Dictionary<string, ItemType> itemTypes;
        private Inventory.InventoryItemEntry fakeItemEntry;

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
                            Attributes = new Attributes(),
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
                            Attributes = new Attributes(),
                            Script = "Shovel",
                            Appearance = null
                        }
                    }
                }
            };

            fakeItemEntry = new Inventory.InventoryItemEntry{
                itemType = itemTypes["shovel"],
                quantity = 1
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
        public void TestThatMultipleStacksAreCreatedAtOnce()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 64), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(32));
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
            Assert.That(inv.Items[2].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[2].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[3].itemType, Is.EqualTo(itemTypes["cabbage"]));
            Assert.That(inv.Items[3].quantity, Is.EqualTo(24));
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
            var inv = new Inventory(2);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.AddItem(itemTypes["shovel"], null), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
        }

        [Test]
        public void TestThatWillStillAddItemToFullInventoryWhenItWillFit()
        {
            var inv = new Inventory(2);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 16), Is.EqualTo(true));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(32));
        }

        [Test]
        public void TestWontAddItemsWhenWontFitEvenWhenOverflowing()
        {
            var inv = new Inventory(3);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 64), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(16));
        }

        [Test]
        public void TestRemoveItemRemovesStack()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestRemoveItemRemovesMultipleStacks()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 64), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null, 64), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestRemoveOnlyItemRequested()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["shovel"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].itemType, Is.EqualTo(itemTypes["shovel"]));
        }

        [Test]
        public void TestWontRemoveWhenNotEnoughItems()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null, 2), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(1));
        }

        [Test]
        public void TestWontRemoveWhenRequestedItemNotInInventory()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.RemoveItem(itemTypes["shovel"], null), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(1));
        }

        [Test]
        public void TestWontRemoveWhenNothingInInventory()
        {
            var inv = new Inventory(10);
            Assert.That(inv.Items.Count, Is.EqualTo(0));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestWillOnlyRemoveItemsWithMatchingAttributes()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], new Attributes{{"quality", 5}}, 16), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["cabbage"], new Attributes{{"quality", 2}}, 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], new Attributes{{"quality", 2}}, 8), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(2));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(16));
            Assert.That(inv.Items[1].quantity, Is.EqualTo(8));
        }

        [Test]
        public void TestWontRemoveIfAttributesDontMatch()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], new Attributes{{"quality", 5}}), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.RemoveItem(itemTypes["cabbage"], null), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(1));
        }

        [Test]
        public void TestWillRemoveItemFromInventoryItemEntry()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null), Is.EqualTo(true));
            Assert.That(inv.RemoveItem(inv.Items[0]), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestWillReduceItemsFromInventoryItemEntry()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.RemoveItem(inv.Items[0], 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(16));
        }

        [Test]
        public void TestWillNotRemoveItemIfInventoryItemEntryNotInInventory()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.RemoveItem(fakeItemEntry), Is.EqualTo(false));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(32));
        }

        [Test]
        public void TestWillNotRemoveItemsFromInventoryItemEntryWhenNotEnoughItems()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.RemoveItem(inv.Items[0], 16), Is.EqualTo(true));
            Assert.That(inv.Items.Count, Is.EqualTo(1));
            Assert.That(inv.Items[0].quantity, Is.EqualTo(16));
        }

        [Test]
        public void TestItemEntryExistsReturnsTrueWhenExists()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.ItemEntryExists(inv.Items[0]), Is.EqualTo(true));
        }

        [Test]
        public void TestItemEntryExistsReturnsFalseWhenDoesNotExist()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.ItemEntryExists(fakeItemEntry), Is.EqualTo(false));
        }

        [Test]
        public void TestInventoryItemEntryReturnsCategoryInAttributeDisplay()
        {
            var inv = new Inventory(10);
            Assert.That(inv.AddItem(itemTypes["cabbage"], null, 32), Is.EqualTo(true));
            Assert.That(inv.AddItem(itemTypes["shovel"], null), Is.EqualTo(true));
            var attributeDisplayCabbage = inv.Items[0].GetAttributeDisplay();
            var attributeDisplayShovel = inv.Items[1].GetAttributeDisplay();
            Assert.That(
                attributeDisplayCabbage[0],
                Is.EqualTo(new KeyValuePair<string, string>("Category", "Produce"))
            );
            Assert.That(
                attributeDisplayShovel[0],
                Is.EqualTo(new KeyValuePair<string, string>("Category", "Tool"))
            );
        }

    }
}