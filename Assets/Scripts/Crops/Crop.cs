using System.Collections;
using Enums;
using Events;
using Inventory;
using Items;
using Maps;
using SceneManagement;
using Sounds;
using UnityEngine;

namespace Crops
{
    public class Crop : MonoBehaviour
    {
        private int harvestActionCount = 0;

        [Tooltip("This should populated from child game object")] [SerializeField]
        private SpriteRenderer cropHarvestedSpriteRenderer = null;

        [HideInInspector] public Vector2Int cropGridPosition;

        [Tooltip("This should be populated from child transform gameobject showing harvest effect spawn point")]
        [SerializeField] private Transform harvestActionEffectTransform = null;

        public void ProcessToolAction(ItemDetails equippedItemDetails,bool isToolRight,bool isToolLeft,
                                       bool isToolUp,bool isToolDown)
        {
            // Get gridPropertyDetails
            GridPropertyDetails gridPropertyDetails =
                GridPropertiesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);
            if(gridPropertyDetails == null)
                return;

            // Get seed item details
            ItemDetails seedItemDetails = InventoryManager.Instance.GetItemDetails(gridPropertyDetails.seedItemCode);
            if(seedItemDetails == null)
                return;
            CropDetails cropDetails = GridPropertiesManager.Instance.GetCropDetails(seedItemDetails.itemCode);
            if(cropDetails == null)
                return;

            // Get animator for crop if present
            Animator animator = GetComponentInChildren<Animator>();

            // Trigger tool animation
            if (animator != null)
            {
                if (isToolRight || isToolUp)
                {
                    animator.SetTrigger("usetoolright");
                }else if (isToolLeft || isToolDown)
                {
                    animator.SetTrigger("usetoolleft");
                }
            }

            // Trigger tool particle effect on crap
            if (cropDetails.isHarvestActionEffect)
            {
                EventHandler.CallHarvestActionEffectEvent(harvestActionEffectTransform.position,
                    cropDetails.harvestActionEffect);
            }



            // Get required harvest action for tool
            int requireHarvestActions = cropDetails.RequireHarvestActionsForTool(equippedItemDetails.itemCode);
            if(requireHarvestActions == -1)
                return; // this tool can't be used to harvest this crop

            // Increment harvest action count
            harvestActionCount++;

            // Check if required harvest actions made
            if (harvestActionCount >= requireHarvestActions)
                HarvestCrop(isToolRight,isToolUp,cropDetails, gridPropertyDetails,animator);
        }

        private void HarvestCrop(bool isUsingToolRight,bool isUsingToolUp,
                        CropDetails cropDetails, GridPropertyDetails gridPropertyDetails,Animator animator)
        {

            // Is there a harvested animation
            if (cropDetails.isHarvestedAnimation && animator != null)
            {
                if (cropHarvestedSpriteRenderer != null)
                {
                    cropHarvestedSpriteRenderer.sprite = cropDetails.harvestedSprite;
                }
            }

            if (isUsingToolRight || isUsingToolUp)
            {
                animator.SetTrigger("harvestright");
            } else
            {
                animator.SetTrigger("harvestleft");
            }

            // Is there a harvest sound
            if (cropDetails.harvestSound != SoundName.None)
            {
                AudioManager.Instance.PlaySound(cropDetails.harvestSound);
            }

            // Delete crop from grid properties
            gridPropertyDetails.seedItemCode = -1;
            gridPropertyDetails.growthDays = -1;
            gridPropertyDetails.daySinceWatered = -1;
            gridPropertyDetails.daySinceLastHarvest = -1;

            // should the crop be hidden before the harvested animation
            if (cropDetails.hideCropBeforeHarvestedAnimation)
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }

            // Should boc collider be disabled before harvest
            if (cropDetails.disableCropCollidersBeforeHarvestedAnimation)
            {
                //Disable any box colliders
                Collider2D[] collider2Ds = GetComponentsInChildren<Collider2D>();
                foreach (Collider2D collider2D in collider2Ds)
                {
                    collider2D.enabled = false;
                }
            }
            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
                                                                  gridPropertyDetails.gridY,gridPropertyDetails);

            // is there a harvested animation - Destroy this crop gameObject after animation completed
            if (cropDetails.isHarvestedAnimation && animator != null)
            {
                StartCoroutine(ProcessHarvestActionAfterAnimation(cropDetails, gridPropertyDetails, animator));
            } else
            {
                HarvestActions(cropDetails,gridPropertyDetails);
            }
        }

        private IEnumerator ProcessHarvestActionAfterAnimation(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails, Animator animator)
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Harvested"))
            {
                yield return null;
            }

            HarvestActions(cropDetails,gridPropertyDetails);
        }

        private void HarvestActions(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
        {
            SpawnHarvestedItem(cropDetails);

            // Does this crop transform into another crap
            if (cropDetails.harvestedTransformItemCode > 0)
            {
                CreateHarvestedTransformCrop(cropDetails, gridPropertyDetails);
            }


            Destroy(gameObject);
        }

        private void CreateHarvestedTransformCrop(CropDetails cropDetails, GridPropertyDetails gridPropertyDetails)
        {
            // Update crop in grid properties
            gridPropertyDetails.seedItemCode = cropDetails.harvestedTransformItemCode;
            gridPropertyDetails.growthDays = 0;
            gridPropertyDetails.daySinceLastHarvest = -1;
            gridPropertyDetails.daySinceWatered = -1;

            GridPropertiesManager.Instance.SetGridPropertyDetails(gridPropertyDetails.gridX,
                                                                        gridPropertyDetails.gridY,gridPropertyDetails);

            // Display planted crop
            GridPropertiesManager.Instance.DisplayPlantedCrops(gridPropertyDetails);
        }

        private void SpawnHarvestedItem(CropDetails cropDetails)
        {

            // Spawn the item(s) to be produced
            for (int i = 0; i < cropDetails.cropProducedItemCode.Length; i++)
            {
                int cropsToProduce;
                // Calculate how many crops to produce
                if (cropDetails.cropProducedMinQuantity[i] == cropDetails.cropProducedMaxQuantity[i] ||
                    cropDetails.cropProducedMaxQuantity[i] < cropDetails.cropProducedMinQuantity[i])
                {
                    cropsToProduce = cropDetails.cropProducedMinQuantity[i];
                } else
                {
                    cropsToProduce = Random.Range(cropDetails.cropProducedMinQuantity[i],
                        cropDetails.cropProducedMaxQuantity[i] + 1);
                }

                for (int j = 0; j < cropsToProduce; j++)
                {
                    Vector3 spawnPosition;
                    if (cropDetails.spawnCropProducedAtPlayerPosition)
                    {
                        // Add item to player inventory
                        InventoryManager.Instance.AddItem(InventoryLocation.Player,cropDetails.cropProducedItemCode[i]);
                    } else
                    {
                        // Random Position
                        spawnPosition=new Vector3(transform.position.x+Random.Range(-1,1f),
                                                      transform.position.y+Random.Range(-1,1f),0f);
                        SceneItemsManager.Instance.InstantiateSceneItems(cropDetails.cropProducedItemCode[i],spawnPosition);
                    }
                }
            }
        }
    }
}
