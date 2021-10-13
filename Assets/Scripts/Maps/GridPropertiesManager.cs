using System.Collections.Generic;
using Enums;
using Misc;
using SaveSystem;
using SceneManagement;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using EventHandler = Events.EventHandler;


namespace Maps
{
    [RequireComponent(typeof(GenerateGUID))]
    public class GridPropertiesManager : SingletonMonoBehaviour<GridPropertiesManager>,ISaveable
    {
        private Tilemap groundDecoration1;    // Dug ground
        private Tilemap groundDecoration2;    // Watered ground

        private Grid grid;

        private Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
        [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;
        [SerializeField] private Tile[] dugGroundTiles = null;
        [SerializeField] private RuleTile dugRuleTile = null;

        public string ISaveableUniqueID { get; set; }
        public GameObjectSave GameObjectSave { get; set; }

        protected override void Awake()
        {
            base.Awake();
            ISaveableUniqueID = GetComponent<GenerateGUID>().GUid;
            GameObjectSave = new GameObjectSave();
        }

        private void OnEnable()
        {
            ISaveableRegister();
            EventHandler.AfterSceneLoadEvent += AfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            ISaveableDeregister();
            EventHandler.AfterSceneLoadEvent -= AfterSceneLoadEvent;
        }

        private void Start()
        {
            InitializeGridProperties();
        }

        private void ClearDisplayGroundDecorations()
        {
            // Remove ground decorations

            groundDecoration1.ClearAllTiles();
            groundDecoration2.ClearAllTiles();
        }

        private void ClearDisplayGridPropertyDetails()
        {
            ClearDisplayGroundDecorations();
        }

        public void DisplayDugGround(GridPropertyDetails gridPropertyDetails)
        {
            // Dug
            if (gridPropertyDetails.daySinceDug > -1)
            {
                ConnectDugGround(gridPropertyDetails);
            }
        }

        private void ConnectDugGround(GridPropertyDetails gridPropertyDetails)
        {
            if (dugRuleTile != null)
            {
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY,0),dugRuleTile );
            } else
            {
                // Select tile based on surrounding dug tiles
            Tile dugTile0 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY,0),dugTile0 );

            // Set 4 tiles if dug surrounding current tile - up,down, left, right now that this central tile has been dug
            GridPropertyDetails adjacentGridPropertyDetails;

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile dugTile1 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY+1);
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY+1,0),dugTile1 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile dugTile2 = SetDugTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY-1);
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY-11,0),dugTile2 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX -1 , gridPropertyDetails.gridY);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile dugTile3 = SetDugTile(gridPropertyDetails.gridX-1, gridPropertyDetails.gridY);
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX -1 ,gridPropertyDetails.gridY,0),dugTile3 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX +1 , gridPropertyDetails.gridY);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile dugTile4 = SetDugTile(gridPropertyDetails.gridX +1, gridPropertyDetails.gridY);
                groundDecoration1.SetTile(new Vector3Int(gridPropertyDetails.gridX +1 ,gridPropertyDetails.gridY,0),dugTile4 );
            }
            }


        }

        private Tile SetDugTile(int gridX, int gridY)
        {
            // Get whether surrounding tiles ( up, down, left and right) are dug or not
            bool upDug = IsGridSquareDug(gridX, gridY + 1);
            bool downDug = IsGridSquareDug(gridX, gridY - 1);
            bool rightDug = IsGridSquareDug(gridX+1, gridY);
            bool leftDug = IsGridSquareDug(gridX-1, gridY);

            #region Set appropriate tile based on whether surrounding tiles are dug or not
            if (!upDug && !downDug && !rightDug && !leftDug)
            {
                return dugGroundTiles[0];
            }

            else if(!upDug && downDug && rightDug && !leftDug)
            {
                return dugGroundTiles[1];
            }

            else if(!upDug && downDug && rightDug && leftDug)
            {
                return dugGroundTiles[2];
            }

            else if(!upDug && downDug && !rightDug && leftDug)
            {
                return dugGroundTiles[3];
            }

            else if(!upDug && downDug && !rightDug && !leftDug)
            {
                return dugGroundTiles[4];
            }

            else if(upDug && downDug && rightDug && !leftDug)
            {
                return dugGroundTiles[5];
            }

            else if(upDug && downDug && rightDug && leftDug)
            {
                return dugGroundTiles[6];
            }

            else if(upDug && downDug && !rightDug && leftDug)
            {
                return dugGroundTiles[7];
            }
            else if(upDug && downDug && !rightDug && !leftDug)
            {
                return dugGroundTiles[8];
            }

            else if(upDug && !downDug && rightDug && !leftDug)
            {
                return dugGroundTiles[9];
            }

            else if(upDug && !downDug && rightDug && leftDug)
            {
                return dugGroundTiles[10];
            }

            else if(upDug && !downDug && !rightDug && leftDug)
            {
                return dugGroundTiles[11];
            }

            else if(upDug && !downDug && !rightDug && !leftDug)
            {
                return dugGroundTiles[12];
            }

            else if(!upDug && !downDug && rightDug && !leftDug)
            {
                return dugGroundTiles[13];
            }
            else if(!upDug && !downDug && rightDug && leftDug)
            {
                return dugGroundTiles[14];
            }

            else if(!upDug && !downDug && !rightDug && leftDug)
            {
                return dugGroundTiles[15];
            }

            return null;

            #endregion
        }

        private bool IsGridSquareDug(int xGrid, int yGrid)
        {
            GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

            if (gridPropertyDetails == null)
            {
                return false;
            } else if (gridPropertyDetails.daySinceDug>-1)
            {
                return true;
            } else
            {
                return false;
            }
        }

        private void DisplayGridPropertyDetails()
        {
            // Loop through all grid items
            foreach (KeyValuePair<string,GridPropertyDetails> item in gridPropertyDetailsDictionary)
            {
                GridPropertyDetails gridPropertyDetails = item.Value;

                DisplayDugGround(gridPropertyDetails);
            }
        }


        /// <summary>
        /// This initializes the grid property dictionary with the values from the SO_GridProperties assets and stores for each scene
        /// in GameObjectSave sceneData
        /// </summary>
        private void InitializeGridProperties()
        {
            // Loop through all gridProperties in the array
            foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
            {
                // Create dictionary of grid property details
                Dictionary<string,GridPropertyDetails> gridPropertyDictionary=new Dictionary<string, GridPropertyDetails>();

                // Populate grid property dictionary - Iterate through all the grid properties in the so_gridProperties list
                foreach (GridProperty gridProperty in so_GridProperties.GridPropertiesList)
                {
                    GridPropertyDetails gridPropertyDetails;


                    gridPropertyDetails =  GetGridPropertyDetails(gridProperty.gridCoordinate.x,
                        gridProperty.gridCoordinate.y,gridPropertyDictionary);

                    if (gridPropertyDetails == null)
                    {
                        gridPropertyDetails=new GridPropertyDetails();
                    }


                    switch (gridProperty.gridBoolProperty)
                    {
                        case GridBoolProperty.Diggable:
                            gridPropertyDetails.isDiggable = gridProperty.gridBoolValue;
                            break;
                        case GridBoolProperty.canDropItem:
                            gridPropertyDetails.canDropItem = gridProperty.gridBoolValue;
                            break;
                        case GridBoolProperty.canPlaceFurniture:
                            gridPropertyDetails.canPlaceFurniture = gridProperty.gridBoolValue;
                            break;
                        case GridBoolProperty.isPath:
                            gridPropertyDetails.isPath = gridProperty.gridBoolValue;
                            break;
                        case GridBoolProperty.isNPCObstcale:
                            gridPropertyDetails.isNPCObsticle = gridProperty.gridBoolValue;
                            break;
                        default:
                            break;
                    }

                    SetGridPropertyDetails(gridProperty.gridCoordinate.x, gridProperty.gridCoordinate.y,
                        gridPropertyDetails, gridPropertyDictionary);
                }

                // Create scene save for this gameObject
                SceneSave sceneSave = new SceneSave();

                // Add grid property dictionary to scene save data
                sceneSave.GridPropertyDetailsesDictionary = gridPropertyDictionary;

                // If starting scene set the gridPropertyDetailsDictionary member variable to the current iteration
                if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
                {
                    this.gridPropertyDetailsDictionary = gridPropertyDictionary;
                }

                // Add scene save to game object scene data
                GameObjectSave.sceneData.Add(so_GridProperties.sceneName.ToString(),sceneSave);
            }
        }

        /// <summary>
        /// Set the grid property details for the tile set (gridX,gridY) for current scene
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridPropertyDetails"></param>
        public void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails)
        {
            SetGridPropertyDetails(gridX,gridY,gridPropertyDetails,gridPropertyDetailsDictionary);
        }

        /// <summary>
        /// Set the grid property details for the tile set (gridX,gridY) for grid property dictionary
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridPropertyDetails"></param>
        /// <param name="gridPropertyDetailsDictionary"></param>
        private void SetGridPropertyDetails(int gridX, int gridY, GridPropertyDetails gridPropertyDetails,
            Dictionary<string,GridPropertyDetails> gridPropertyDetailsDictionary)
        {
            // Construct key from coordinate
            string key = $"x{gridX}y{gridY}";

            gridPropertyDetails.gridX = gridX;
            gridPropertyDetails.gridY = gridY;

            // set Value
            gridPropertyDetailsDictionary[key] = gridPropertyDetails;
        }

        /// <summary>
        /// Returns the grid property details at the grid location for the supplied dictionary, or null if no
        /// properties exist at that location
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <param name="gridPropertyDetailsDictionary"></param>
        /// <returns></returns>
        private GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string,GridPropertyDetails> gridPropertyDetailsDictionary)
        {
            // Construct key from coordinate
            string key = $"x{gridX}y{gridY}";

            GridPropertyDetails gridPropertyDetails;

            // Check if grid property details exist for coordinate and retrieve
            if (!gridPropertyDetailsDictionary.TryGetValue(key, out gridPropertyDetails))
            {
                // if not found
                return null;
            } else
            {
                return gridPropertyDetails;
            }

        }
        /// <summary>
        /// Get the grid property Details for the tile at (gridX, gridY). If no grid property details exist null is returned
        /// and can assume that all grid property details values are null or false
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridY"></param>
        /// <returns></returns>
        public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY)
        {
            return GetGridPropertyDetails(gridX, gridY, gridPropertyDetailsDictionary);
        }

        private void AfterSceneLoadEvent()
        {
            // Get grid
            grid = GameObject.FindObjectOfType<Grid>();

            // Get tilemaps
            groundDecoration1 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration1).GetComponent<Tilemap>();
            groundDecoration2 = GameObject.FindGameObjectWithTag(Tags.GroundDecoration2).GetComponent<Tilemap>();
        }

        public void ISaveableRegister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Add(this);
        }

        public void ISaveableDeregister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
        }

        public void ISaveableStoreScene(string sceneName)
        {
            // Remove sceneSave for scene
            GameObjectSave.sceneData.Remove(sceneName);

            // Create sceneSave for scene
            SceneSave sceneSave=new SceneSave();

            // Create and add dictionary grid property details
            sceneSave.GridPropertyDetailsesDictionary = gridPropertyDetailsDictionary;

            // Add sceneSave to gameObject scene data
            GameObjectSave.sceneData.Add(sceneName,sceneSave);
        }

        public void ISaveableRestoreScene(string sceneName)
        {
            // Get sceneSave for scene - it exists since we created it in initialize
            if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
            {
                // Get grid property details dictionary - it exists since we created it in initialize
                if (sceneSave.GridPropertyDetailsesDictionary != null)
                {
                    gridPropertyDetailsDictionary = sceneSave.GridPropertyDetailsesDictionary;
                }

                // if grid property exist
                if (gridPropertyDetailsDictionary.Count > 0)
                {
                    // grid property details found for the current scene destroy existing ground decoration
                    ClearDisplayGroundDecorations();

                    //Instantiate grid property details for current scene
                    DisplayGridPropertyDetails();
                }
            }
        }
    }
}
