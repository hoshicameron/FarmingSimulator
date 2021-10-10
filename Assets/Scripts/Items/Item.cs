using Enums;
using Inventory;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Items
{
    public class Item : MonoBehaviour
    {
        [ItemCodeDescription][SerializeField] private int itemCode;

        public int ItemCode
        {
            get { return itemCode; }
            set { itemCode = value; }
        }

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            if (ItemCode != 0)
            {
                Init(ItemCode);
            }
        }

        public void Init(int itemCodeParam)
        {
            if (itemCodeParam != 0)
            {
                ItemCode = itemCodeParam;

                ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(ItemCode);

                spriteRenderer.sprite = itemDetails.itemSprite;
                //If item type is reapable scenary then add nudgeable component
                if (itemDetails.itemType == ItemType.Reapable_scanary)
                {
                    gameObject.AddComponent<ItemNudge>();
                }
            }
        }
    }
}
