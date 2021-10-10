using UnityEditor;
using Enums;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Maps
{
    [ExecuteAlways]
    public class TilemapGridProperties : MonoBehaviour
    {
        private Tilemap tilemap;
        private Grid grid;
        [SerializeField] private SO_GridProperties so_GridProperties = null;
        [SerializeField] private GridBoolProperty gridBoolProperty = GridBoolProperty.Diggable;

        private void OnEnable()
        {
            // Only populate in the editor
            if (!Application.IsPlaying(gameObject))
            {
                tilemap = GetComponent<Tilemap>();
                if (so_GridProperties != null)
                {
                    so_GridProperties.GridPropertiesList.Clear();
                }
            }
        }

        private void OnDisable()
        {
            // Only populate in the editor
            if (!Application.IsPlaying(gameObject))
            {
                UpdateGridProperties();
                if (so_GridProperties != null)
                {
                    // This is required to ensure that the updated grid properties gameObject get saved when
                    // the game is saved - otherwise they are not saved.
                    EditorUtility.SetDirty(so_GridProperties);
                }
            }
        }

        private void UpdateGridProperties()
        {
            // Compress tilemap bounds-
            tilemap.CompressBounds();

            // Only populate in the editor
            if (!Application.IsPlaying(gameObject))
            {
                if (so_GridProperties != null)
                {
                    Vector3Int startCell = tilemap.cellBounds.min;
                    Vector3Int endCell = tilemap.cellBounds.max;
                    for (int x =startCell.x; x < endCell.x; x++)
                    {
                        for (int y = startCell.y; y < endCell.y; y++)
                        {
                            TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                            if (tile != null)
                            {
                                so_GridProperties.GridPropertiesList.Add(new GridProperty(new GridCoordinate(x,y),
                                    gridBoolProperty,true));

                            }
                        }
                    }
                }
            }

        }

        private void Update()
        {
            // Only populate in the editor
            if (!Application.IsPlaying(gameObject))
            {
                Debug.Log("Disable Property Tilemaps");
            }
        }
    }
}
