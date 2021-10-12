using System;
using Enums;
using Inventory;
using Items;
using Maps;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using _Player;
using EventHandler = Events.EventHandler;

namespace UI
{
    public class GridCursor : MonoBehaviour
    {
        private Canvas canvas;
        private Grid grid;
        private Camera mainCamera;

        [SerializeField] private Image cursorImage = null;
        [SerializeField] private RectTransform cursorRectTransform=null;
        [SerializeField] private Sprite greenCursorSprite=null;
        [SerializeField] private Sprite redCursorSprite=null;

        public bool CursorPositionIsValid { get; set; }
        public int ItemUserGridRadius { get; set; } = 0;
        public ItemType SelectedItemType { get; set; }
        private bool CursorIsEnabled { get; set; } = false;

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += SceneLoaded;
        }

        private void SceneLoaded()
        {
            grid = GameObject.FindObjectOfType<Grid>();
        }

        private void Start()
        {
            mainCamera=Camera.main;
            canvas = GetComponentInParent<Canvas>();
        }

        private void Update()
        {
            if (CursorIsEnabled)
            {
                DisplayCursor();
            }
        }

        private Vector3Int DisplayCursor()
        {
            if (grid!=null)
            {
                // Get grid position for cursor
                Vector3Int gridPosition = GetGridPositionForCursor();

                // Get grid position for player
                Vector3Int playerGridPosition = GetGridPositionForPlayer();

                // Set cursor sprite
                SetCursorValidity(gridPosition, playerGridPosition);

                // Get rect transform position for cursor
                cursorRectTransform.position = GetRectTransformPositionForCursor(gridPosition);

                return gridPosition;
            } else
            {
                return Vector3Int.zero;
            }
        }

        public void DisableCursor()
        {
            cursorImage.color = Color.clear;
            CursorIsEnabled = false;
        }

        public void EnableCursor()
        {
            cursorImage.color = new Color(1f,1f,1f,1f);
            CursorIsEnabled = true;
        }


        private void SetCursorValidity(Vector3Int cursorGridPosition, Vector3Int playerGridPosition)
        {
            SetCursorToValid();

            // Check item use radius is valid
            if(Mathf.Abs(cursorGridPosition.x- playerGridPosition.x)>ItemUserGridRadius ||
                Mathf.Abs(cursorGridPosition.y- playerGridPosition.y)>ItemUserGridRadius)
            {
                SetCursorToInvalid();
                return;
            }

            // Get selected item details
            ItemDetails itemDetails =
                InventoryManager.Instance.GetSelectedInventoryItemDetails(InventoryLocation.Player);
            if (itemDetails == null)
            {
                SetCursorToInvalid();
                return;
            }

            // Get grid property details at cursor position
            GridPropertyDetails gridPropertyDetails =
                GridPropertiesManager.Instance.GetGridPropertyDetails(cursorGridPosition.x, cursorGridPosition.y);
            if (gridPropertyDetails != null)
            {
                // Determine cursor validity based on inventory item selected and grid property details
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        if (!IsCursorValidForSeed(gridPropertyDetails))
                        {
                            SetCursorToInvalid();
                            return;
                        }
                        break;
                    case ItemType.Commodity:
                        if (!IsCursorValidForCommodity(gridPropertyDetails))
                        {
                            SetCursorToInvalid();
                            return;
                        }
                        break;
                    case ItemType.Watering_Tool:
                        break;
                    case ItemType.HoeingTool:
                        break;
                    case ItemType.Chopping_Tool:
                        break;
                    case ItemType.BreakingTool:
                        break;
                    case ItemType.Reaping_Tool:
                        break;
                    case ItemType.Collecting_Tool:
                        break;
                    case ItemType.Reapable_scanary:
                        break;
                    case ItemType.Furniture:
                        break;
                    case ItemType.None:
                        break;
                    case ItemType.Count:
                        break;
                    default:
                        break;
                }
            } else
            {
                SetCursorToInvalid();
                return;
            }
        }

        private bool IsCursorValidForCommodity(GridPropertyDetails gridPropertyDetails)
        {
            return gridPropertyDetails.canDropItem;
        }

        private bool IsCursorValidForSeed(GridPropertyDetails gridPropertyDetails)
        {
            return gridPropertyDetails.canDropItem;
        }

        private void SetCursorToInvalid()
        {
            cursorImage.sprite=redCursorSprite;
            CursorPositionIsValid = false;
        }

        private void SetCursorToValid()
        {
            cursorImage.sprite=greenCursorSprite;
            CursorPositionIsValid = true;
        }

        private Vector3Int GetGridPositionForPlayer()
        {
            return grid.WorldToCell(Player.Instance.transform.position);
        }

        private Vector3Int GetGridPositionForCursor()
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -mainCamera.transform.position.z));
            return grid.WorldToCell(worldPosition);
        }
        private Vector3 GetRectTransformPositionForCursor(Vector3Int gridPosition)
        {
            Vector3 gridWorldPosition = grid.WorldToCell(gridPosition);
            Vector2 gridScreenPosition = mainCamera.WorldToScreenPoint(gridWorldPosition);
            return RectTransformUtility.PixelAdjustPoint(gridScreenPosition, cursorRectTransform, canvas);
        }


    }
}
