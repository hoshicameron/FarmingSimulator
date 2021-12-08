using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Enums;
using Items;
using Misc;
using SaveSystem;
using UI;
using UnityEngine;
using _Player;
using EventHandler = Events.EventHandler;

namespace Inventory
{
    public class InventoryManager:SingletonMonoBehaviour<InventoryManager>,ISaveable
    {
        private UIInventoryBar inventoryBar;

        private Dictionary<int, ItemDetails> ItemDetailsDictionary;

        private int[] selectedInventoryItem; // The index of the array is the inventory list, and the value is the item code

        public string ISaveableUniqueID { get; set; }
        public GameObjectSave GameObjectSave { get; set; }

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

            //Initialize selected inventory item array
            selectedInventoryItem = new int[(int) InventoryLocation.Count];

            for (int i = 0; i < selectedInventoryItem.Length; i++)
            {
                selectedInventoryItem[i] = -1; // We haven't select any item
            }

            ISaveableUniqueID = GetComponent<GenerateGUID>().GUid;

            GameObjectSave=new GameObjectSave();
        }

        private void Start()
        {
            inventoryBar = FindObjectOfType<UIInventoryBar>();
        }

        private void OnEnable()
        {
            ISaveableRegister();
        }

        private void OnDisable()
        {
            ISaveableDeregister();
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
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);
        }

        public void AddItem(InventoryLocation inventoryLocation, int itemCode)
        {
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
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);
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
        public int FindItemInInventory(InventoryLocation inventoryLocation, int itemCode)
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

        /// <summary>
        /// Returns the item details (from the SO_ItemList) for the currently selected item in the inventory,
        /// or null if isn't selected
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <returns></returns>
        public ItemDetails GetSelectedInventoryItemDetails(InventoryLocation inventoryLocation)
        {
            int itemCode = GetSelectedInventoryItem(inventoryLocation);
            if (itemCode == -1)
            {
                return null;
            } else
            {
                return GetItemDetails(itemCode);
            }
        }


        /// <summary>
        /// Get selected item for inventoryLocation - returns itemCode or -1 if nothing is selected
        /// </summary>
        /// <param name="inventoryLocation"></param>
        /// <returns></returns>
        private int GetSelectedInventoryItem(InventoryLocation inventoryLocation)
        {
            return selectedInventoryItem[(int) inventoryLocation];
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
            EventHandler.CallInventoryUpdatedEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);
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
                EventHandler.CallInventoryUpdatedEvent(inventoryLocation,inventoryArrayList[(int)inventoryLocation]);

            }
        }

        /// <summary>
        /// Clear the selected inventory item for inventoryLocation
        /// </summary>
        /// <param name="inventoryLocation"></param>
        public void ClearSelectedInventoryItem(InventoryLocation inventoryLocation)
        {
            selectedInventoryItem[(int)inventoryLocation] = -1;
        }


        /// <summary>
        /// Get the item type description for an item type - returns the item type description as a string for a given ItemType
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public string GetItemTypeDescription(ItemType itemType)
        {
            string itemTypeDescription;
            switch (itemType)
            {
                case ItemType.Watering_Tool:
                    itemTypeDescription = Settings.WateringTool;
                    break;
                case ItemType.HoeingTool:
                    itemTypeDescription = Settings.HoeTool;
                    break;
                case ItemType.Chopping_Tool:
                    itemTypeDescription = Settings.ChoppingTool;
                    break;
                case ItemType.BreakingTool:
                    itemTypeDescription = Settings.BreakingTool;
                    break;
                case ItemType.Reaping_Tool:
                    itemTypeDescription = Settings.ReapingTool;
                    break;
                case ItemType.Collecting_Tool:
                    itemTypeDescription = Settings.CollectingTool;
                    break;

                default:
                    itemTypeDescription = itemType.ToString();
                    break;
            }

            return itemTypeDescription;
        }

        public void SetSelectedItemInventory(InventoryLocation inventoryLocation, int itemcode)
        {
            selectedInventoryItem[(int) inventoryLocation] = itemcode;
        }


        public void ISaveableRegister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Add(this);
        }

        public void ISaveableDeregister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
        }

        public GameObjectSave ISaveableSave()
        {
            // Create new scene save
            SceneSave sceneSave = new SceneSave();

            // Remove any existing scene save for persistent scene for this gameobject
            GameObjectSave.sceneData.Remove(Settings.PersistentScene);

            // Add inventory lists array to persistent scene save
            sceneSave.listInvItemArray = inventoryArrayList;

            // Add  inventory list capacity array to persistent scene save
            sceneSave.intArrayDictionary = new Dictionary<string, int[]>();
            sceneSave.intArrayDictionary.Add("inventoryListCapacityArray", inventoryListCapacityIntArray);

            // Add scene save for gameobject
            GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

            return GameObjectSave;
        }

        public void ISaveableLoad(GameSave gameSave)
        {
            if (gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
            {
                GameObjectSave = gameObjectSave;

                // Need to find inventory lists - start by trying to locate saveScene for game object
                if (gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
                {
                    // list inv items array exists for persistent scene
                    if (sceneSave.listInvItemArray != null)
                    {
                        inventoryArrayList = sceneSave.listInvItemArray;

                        //  Send events that inventory has been updated
                        for (int i = 0; i < (int)InventoryLocation.Count; i++)
                        {
                            EventHandler.CallInventoryUpdatedEvent((InventoryLocation)i, inventoryArrayList[i]);
                        }

                        // Clear any items player was carrying
                        Player.Instance.ClearCarriedItem();

                        // Clear any highlights on inventory bar
                        inventoryBar.ClearHighlightOnInventorySlots();
                    }

                    // int array dictionary exists for scene
                    if (sceneSave.intArrayDictionary != null && sceneSave.intArrayDictionary.TryGetValue("inventoryListCapacityArray", out int[] inventoryCapacityArray))
                    {
                        inventoryListCapacityIntArray = inventoryCapacityArray;
                    }
                }

            }
        }

        public void ISaveableStoreScene(string sceneName)
        {
            // Nothing required her since the inventory manager is on a persistent scene;
        }

        public void ISaveableRestoreScene(string sceneName)
        {
            // Nothing required here since the inventory manager is on a persistent scene;
        }
    }
}
