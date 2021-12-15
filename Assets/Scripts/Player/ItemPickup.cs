
using Enums;
using Inventory;
using Items;
using Sounds;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent(out Item item))
        {
            // Get item details
            ItemDetails itemDetails = InventoryManager.Instance.GetItemDetails(item.ItemCode);

            // if the item can be picked up
            if (itemDetails.canPickedUp == true)
            {
                // Add item to the Inventory
                InventoryManager.Instance.AddItem(InventoryLocation.Player,item,other.gameObject);

                // Play pickup sound
                AudioManager.Instance.PlaySound(SoundName.EffectPickupSound);
            }
        }

    }
}
