using System.Collections.Generic;
using Items;
using Misc;
using UnityEngine;

namespace Inventory
{
    public class InventoryManager:SingletonMonoBehaviour<InventoryManager>
    {
        private Dictionary<int, ItemDetails> ItemDetailsDictionary;
        [SerializeField] private SO_ItemList itemList = null;

        private void Start()
        {
            CreateItemDetailsDictionary();
        }

        /// <summary>
        /// Populates the itemDetailsDictionary from the scriptable items list
        /// </summary>
        private void CreateItemDetailsDictionary()
        {
            ItemDetailsDictionary=new Dictionary<int, ItemDetails>();

            foreach (ItemDetails itemDetails in itemList.ItemDetails)
            {
                ItemDetailsDictionary.Add(itemDetails.itemCode,itemDetails);
            }
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
    }
}
