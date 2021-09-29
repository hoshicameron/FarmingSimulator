using System.Collections.Generic;
using Enums;
using Events;
using Items;
using Misc;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager:SingletonMonoBehaviour<InventoryManager>
    {
        private Dictionary<int, ItemDetails> ItemDetailsDictionary;

        // The array of list of items that inventory location hold
        public List<InventoryItem>[] inventoryArrayList;

        // the index of the array is inventory list( from the lnventoryLocation enum),
        // and the value is the capacity of that inventory list
        [HideInInspector] public int[] inventoryListCapacityIntArray;

        [SerializeField] private SO_ItemList itemList = null;

        protected override void Awake()
        {
            base.Awake();

            // Create Inventory Lists
            CreateInventoryLists();

            // Create item Details Dictionary
            CreateItemDetailsDictionary();
        }

        private void CreateInventoryLists()
        {
            inventoryArrayList=new List<InventoryItem>[(int)InventoryLocation.Count];
            for (int i = 0; i < (int)InventoryLocation.Count; i++)
            {
                inventoryArrayList[i]=new List<InventoryItem>();
            }
        }

        /// <summary>
        /// Populates the itemDetailsDictionary from the scriptable items list
        /// </summary>
        private void CreateItemDetailsDictionary()
        {
            ItemDetailsDictionary=new Dictionary<int, ItemDetails>();

            foreach (ItemDetails itemDetails in itemList.ItemDetailsList)
            {
                ItemDetailsDictionary.Add(itemDetails.itemCode,itemDetails);
            }

            // Initialize inventory list capacity array
            inventoryListCapacityIntArray=new int[(int)InventoryLocation.Count];

            // Initialize player inventory list capacity
            inventoryListCapacityIntArray[(int) InventoryLocation.Player] = Settings.playerInitialInventoryCapacity;

        }

        /// <summary>
        ///  Add an item to the inventory list for the inventory location and destroy the gameObjectToDelete
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <param name="item"></param>
        /// <param name="gameObjectToDelete"></param>
        public void AddItem(InventoryLocation inventoryLocation, Item item,GameObject gameObjectToDelete)
        {
            AddItem(inventoryLocation,item);
            Destroy(gameObjectToDelete);
        }

        /// <summary>
        /// Add an item to the inventory list for the inventory location
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <param name="item"></param>
        public void AddItem(InventoryLocation inventoryLocation, Item item)
        {
            int itemCode = item.ItemCode;
            // inventory list point to the reference of the list in the inventory Array list
            List<InventoryItem> inventoryList = inventoryArrayList[(int) inventoryLocation];

            //Check if the inventory already contains the item
            int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

            if (itemPosition != -1)
            {
                AddItemAtPosition(inventoryList, itemCode, itemPosition);
            } else
            {
                AddItemAtPosition(inventoryList, itemCode);
            }

            // Send event that inventory has updated
            EventHandler.CallInventoryUpdateEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);
        }

        /// <summary>
        ///  add item inventory at given position
        /// </summary>
        /// <param name="inventoryList"></param>
        /// <param name="itemCode"></param>
        private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
        {
            InventoryItem inventoryItem=new InventoryItem();

            int quantity = inventoryList[itemPosition].itemQuantity + 1;
            inventoryItem.itemCode = itemCode;
            inventoryItem.itemQuantity = quantity;
            inventoryList[itemPosition] = inventoryItem;

            //DebugPrintInventoryList(inventoryList);
        }



        /// <summary>
        ///  add item to the end of inventory
        /// </summary>
        /// <param name="inventoryList"></param>
        /// <param name="itemCode"></param>
        private void AddItemAtPosition(List<InventoryItem> inventoryList, int itemCode)
        {
            InventoryItem inventoryItem=new InventoryItem();

            inventoryItem.itemCode = itemCode;
            inventoryItem.itemQuantity = 1;
            inventoryList.Add(inventoryItem);

            //DebugPrintInventoryList(inventoryList);
        }

        /// <summary>
        /// Find if an itemCode is already in the inventory. Returns the item position
        /// in the inventory list, or -1 if the item isn't in the inventory
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        private int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
        {
            List<InventoryItem> inventoryList = inventoryArrayList[(int) inventoryLocation];
            for (int i = 0; i < inventoryList.Count; i++)
            {
                if (inventoryList[i].itemCode == itemCode)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Return the itemDetails (from the SO_ItemList dor the itemCode, or null if the item code dosen't exist
        /// </summary>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int itemCode)
        {
            ItemDetails itemDetails;
            if (ItemDetailsDictionary.TryGetValue(itemCode, out itemDetails))
            {
                return itemDetails;
            } else
            {
                return null;
            }
        }

        private void DebugPrintInventoryList(List<InventoryItem> inventoryList)
        {
            foreach (InventoryItem inventoryItem in inventoryList)
            {
                Debug.Log($"Item Description " +
                          $"{InventoryManager.Instance.GetItemDetails(inventoryItem.itemCode).itemDescription}" +
                          $"     Item Quantity: {inventoryItem.itemQuantity}");
            }

            Debug.Log("********************************************************");
        }

        public void RemoveItem(InventoryLocation inventoryLocation, int itemCode)
        {
            List<InventoryItem> inventoryList = inventoryArrayList[(int) inventoryLocation];

            //Check if inventory already contains the item
            int itemPosition = FindItemInInventory(inventoryLocation, itemCode);

            if (itemPosition != -1)
            {
                RemoveItemAtPosition(inventoryList,itemCode,itemPosition);
            }

            // Send event that inventory has updated
            EventHandler.CallInventoryUpdateEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);
        }

        private void RemoveItemAtPosition(List<InventoryItem> inventoryList, int itemCode, int itemPosition)
        {
            InventoryItem inventoryItem=new InventoryItem();

            int quantity = inventoryList[itemPosition].itemQuantity - 1;
            if (quantity > 0)
            {
                inventoryItem.itemQuantity = quantity;
                inventoryItem.itemCode = itemCode;
                inventoryList[itemPosition] = inventoryItem;
            } else
            {
                inventoryList.RemoveAt(itemPosition);
            }
        }

        /// <summary>
        ///  Swap item at foreItem index with item at toItem index in the inventory list
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <param name="fromItem"></param>
        /// <param name="toItem"></param>
        public void SwapInventoryItems(InventoryLocation inventoryLocation, int fromItem, int toItem)
        {
            // If fromItem index and toItem index are within the bound of the list, not the same, and greater and equal to zero
            if (fromItem < inventoryArrayList[(int) inventoryLocation].Count && toItem <
                                                                            inventoryArrayList[(int) inventoryLocation]
                                                                                .Count
                                                                            && fromItem != toItem && fromItem >= 0 &&
                                                                            toItem >= 0)
            {
                InventoryItem fromInventoryItem = inventoryArrayList[(int) inventoryLocation][fromItem];
                InventoryItem toInInventoryItem = inventoryArrayList[(int) inventoryLocation][toItem];

                inventoryArrayList[(int) inventoryLocation][toItem]=fromInventoryItem;
                inventoryArrayList[(int) inventoryLocation][fromItem] = toInInventoryItem;

                // Send event that inventory has updated
                EventHandler.CallInventoryUpdateEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);

            }
        }
    }
}
