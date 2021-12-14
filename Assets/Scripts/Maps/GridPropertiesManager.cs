using System.Collections.Generic;
using System.Linq;
using Crops;
using Enums;
using Misc;
using SaveSystem;
using SceneManagement;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using EventHandler = Events.EventHandler;


namespace Maps
{
    [RequireComponent(typeof(GenerateGUID))]
    public class GridPropertiesManager : SingletonMonoBehaviour<GridPropertiesManager>,ISaveable
    {
        private Transform cropParentTransform;
        private Tilemap groundDecoration1;    // Dug ground
        private Tilemap groundDecoration2;    // Watered ground

        private bool isFirstTimeSceneLoaded = true;

        private Grid grid;

        private Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
        [SerializeField] private SO_CropDetailsList so_CropDetailsList = null;
        [SerializeField] private SO_GridProperties[] so_gridPropertiesArray = null;
        [SerializeField] private Tile[] dugGroundTiles = null;
        [SerializeField] private Tile[] wateredGroundTiles = null;
        [SerializeField] private RuleTile dugRuleTile = null;
        [SerializeField] private RuleTile wateredRuleTile = null;

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
            EventHandler.AdvanceGameDayEvent += AdvanceDay;
        }



        private void OnDisable()
        {
            ISaveableDeregister();
            EventHandler.AfterSceneLoadEvent -= AfterSceneLoadEvent;
            EventHandler.AdvanceGameDayEvent -= AdvanceDay;
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

            ClearDisplayAllPlantedCrops();
        }

        private void ClearDisplayAllPlantedCrops()
        {
            //Destroy all crops in scene
            Crop[] cropArray;
            cropArray = FindObjectsOfType<Crop>();

            foreach (Crop crop in cropArray)
            {
                Destroy(crop.gameObject);
            }
        }

        public void DisplayWateredgGround(GridPropertyDetails gridPropertyDetails)
        {
            // Watered
            if (gridPropertyDetails.daySinceWatered>-1)
            {
                ConnectWateredGround(gridPropertyDetails);
            }
        }

        private void ConnectWateredGround(GridPropertyDetails gridPropertyDetails)
        {
            if (wateredRuleTile != null)
            {
                groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY,0),wateredRuleTile );
            } else
            {
                // Select tile based on surrounding dug tiles
            Tile wateredTile0 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY);
            groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY,0),wateredTile0 );

            // Set 4 tiles if dug surrounding current tile - up,down, left, right now that this central tile has been dug
            GridPropertyDetails adjacentGridPropertyDetails;

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY + 1);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile wateredTile1 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY+1);
                groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY+1,0),wateredTile1 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX, gridPropertyDetails.gridY - 1);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile wateredTile2 = SetWateredTile(gridPropertyDetails.gridX, gridPropertyDetails.gridY-1);
                groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY-11,0),wateredTile2 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX -1 , gridPropertyDetails.gridY);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile wateredTile3 = SetWateredTile(gridPropertyDetails.gridX-1, gridPropertyDetails.gridY);
                groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX -1 ,gridPropertyDetails.gridY,0),wateredTile3 );
            }

            adjacentGridPropertyDetails =
                GetGridPropertyDetails(gridPropertyDetails.gridX +1 , gridPropertyDetails.gridY);

            if (adjacentGridPropertyDetails != null && adjacentGridPropertyDetails.daySinceDug > -1)
            {
                Tile wateredTile4 = SetWateredTile(gridPropertyDetails.gridX +1, gridPropertyDetails.gridY);
                groundDecoration2.SetTile(new Vector3Int(gridPropertyDetails.gridX +1 ,gridPropertyDetails.gridY,0),wateredTile4 );
            }
            }


        }

        private Tile SetWateredTile(int gridX, int gridY)
        {
            // Get whether surrounding tiles ( up, down, left and right) are dug or not
            bool upDug = IsGridSquareWatered(gridX, gridY + 1);
            bool downDug = IsGridSquareWatered(gridX, gridY - 1);
            bool rightDug = IsGridSquareWatered(gridX+1, gridY);
            bool leftDug = IsGridSquareWatered(gridX-1, gridY);

            #region Set appropriate tile based on whether surrounding tiles are dug or not
            if (!upDug && !downDug && !rightDug && !leftDug)
            {
                return wateredGroundTiles[0];
            }

            else if(!upDug && downDug && rightDug && !leftDug)
            {
                return wateredGroundTiles[1];
            }

            else if(!upDug && downDug && rightDug && leftDug)
            {
                return wateredGroundTiles[2];
            }

            else if(!upDug && downDug && !rightDug && leftDug)
            {
                return wateredGroundTiles[3];
            }

            else if(!upDug && downDug && !rightDug && !leftDug)
            {
                return wateredGroundTiles[4];
            }

            else if(upDug && downDug && rightDug && !leftDug)
            {
                return wateredGroundTiles[5];
            }

            else if(upDug && downDug && rightDug && leftDug)
            {
                return wateredGroundTiles[6];
            }

            else if(upDug && downDug && !rightDug && leftDug)
            {
                return wateredGroundTiles[7];
            }
            else if(upDug && downDug && !rightDug && !leftDug)
            {
                return wateredGroundTiles[8];
            }

            else if(upDug && !downDug && rightDug && !leftDug)
            {
                return wateredGroundTiles[9];
            }

            else if(upDug && !downDug && rightDug && leftDug)
            {
                return wateredGroundTiles[10];
            }

            else if(upDug && !downDug && !rightDug && leftDug)
            {
                return wateredGroundTiles[11];
            }

            else if(upDug && !downDug && !rightDug && !leftDug)
            {
                return wateredGroundTiles[12];
            }

            else if(!upDug && !downDug && rightDug && !leftDug)
            {
                return wateredGroundTiles[13];
            }
            else if(!upDug && !downDug && rightDug && leftDug)
            {
                return wateredGroundTiles[14];
            }

            else if(!upDug && !downDug && !rightDug && leftDug)
            {
                return wateredGroundTiles[15];
            }

            return null;

            #endregion
        }

        private bool IsGridSquareWatered(int xGrid, int yGrid)
        {
            GridPropertyDetails gridPropertyDetails = GetGridPropertyDetails(xGrid, yGrid);

            if (gridPropertyDetails == null)
            {
                return false;
            } else if (gridPropertyDetails.daySinceWatered>-1)
            {
                return true;
            } else
            {
                return false;
            }
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

                DisplayWateredgGround(gridPropertyDetails);

                DisplayPlantedCrops(gridPropertyDetails);

            }
        }

        public void DisplayPlantedCrops(GridPropertyDetails gridPropertyDetails)
        {
            if (gridPropertyDetails.seedItemCode > -1)
            {
                // Get crop details
                CropDetails cropDetails = so_CropDetailsList.GetCropDetails(gridPropertyDetails.seedItemCode);

                if (cropDetails != null)
                {
                    // Prefer to use
                    GameObject cropPrefab;

                    //Instantiate crop prefab at grid location
                    int growthStages = cropDetails.growthDays.Length;

                    int currentGrowthStage = 0;

                    // Workout what growthStage we currently in based on growthDays

                    for (int i=growthStages-1 ; i>=0; i--)
                    {
                        if (gridPropertyDetails.growthDays >= cropDetails.growthDays[i])
                        {
                            currentGrowthStage = i;
                            break;
                        }
                    }

                    cropPrefab = cropDetails.growthPrefab[currentGrowthStage];

                    Sprite growthSprite = cropDetails.growthSprite[currentGrowthStage];

                    Vector3 worldPosition =
                        groundDecoration2.CellToWorld(new Vector3Int(gridPropertyDetails.gridX,
                            gridPropertyDetails.gridY,0));

                    worldPosition= new Vector3(worldPosition.x+Settings.gridCellSize*0.5f,worldPosition.y,worldPosition.z);

                    GameObject cropInstance = Instantiate(cropPrefab, worldPosition, Quaternion.identity);

                    cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = growthSprite;
                    cropInstance.transform.SetParent(cropParentTransform);
                    cropInstance.GetComponent<Crop>().cropGridPosition=
                        new Vector2Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY);
                }
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
                sceneSave.gridPropertyDetailsDictionary = gridPropertyDictionary;

                // If starting scene set the gridPropertyDetailsDictionary member variable to the current iteration
                if (so_GridProperties.sceneName.ToString() == SceneControllerManager.Instance.startingSceneName.ToString())
                {
                    this.gridPropertyDetailsDictionary = gridPropertyDictionary;
                }

                // Add bool dictionary and set first time scene loaded to true
                sceneSave.boolDictionary =new Dictionary<string, bool>();
                sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded",true);


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
        public GridPropertyDetails GetGridPropertyDetails(int gridX, int gridY, Dictionary<string,GridPropertyDetails> gridPropertyDetailsDictionary)
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
        /// Return crop object at th gridX,gridY position or null if no crop found
        /// </summary>
        /// <param name="gridPropertyDetails"></param>
        /// <returns></returns>
        public Crop GetCropObjectAtGridLocation(GridPropertyDetails gridPropertyDetails)
        {
            Vector3 worldPosition =
                grid.GetCellCenterWorld(new Vector3Int(gridPropertyDetails.gridX, gridPropertyDetails.gridY, 0));
            Collider2D[] collider2DArray = Physics2D.OverlapPointAll(worldPosition);

            // Loop through colliders to get crop game object
            Crop crop = null;
            for (int i = 0; i < collider2DArray.Length; i++)
            {
                crop = collider2DArray[i].gameObject.GetComponentInParent<Crop>();
                if(crop!=null && crop.cropGridPosition== new Vector2Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY))
                    break;
                crop = collider2DArray[i].gameObject.GetComponentInChildren<Crop>();
                if(crop!=null && crop.cropGridPosition== new Vector2Int(gridPropertyDetails.gridX,gridPropertyDetails.gridY))
                    break;
            }

            return crop;
        }

        public CropDetails GetCropDetails(int sendItemCode)
        {
            return so_CropDetailsList.GetCropDetails(sendItemCode);
        }

        /// <summary>
        /// Returns for each scene a vector2Int with the grid dimensions, or vector2Int.zero if scene not found
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="gridDimensions"></param>
        /// <param name="gridOrigin"></param>
        /// <returns></returns>
        public bool GetGridDimensions(SceneName sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin)
        {
            gridDimensions=Vector2Int.zero;
            gridOrigin=Vector2Int.zero;

            // Loop through all scenes
            foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
            {
                if (so_GridProperties.sceneName == sceneName)
                {
                    gridDimensions.x = so_GridProperties.gridWidth;
                    gridDimensions.y = so_GridProperties.gridHeight;

                    gridOrigin.x = so_GridProperties.originX;
                    gridOrigin.y = so_GridProperties.originY;

                    return true;
                }
            }

            return false;
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

            if (GameObject.FindGameObjectWithTag(Tags.CropsParentTransform) != null)
            {
                cropParentTransform = GameObject.FindGameObjectWithTag(Tags.CropsParentTransform).transform;
            } else
            {
                cropParentTransform = null;
            }
        }

        private void AdvanceDay(int gameYear, Season season, int gameDay, string gameDayOfWeek, int gameHour,
            int gameMinute, int gameSecond)
        {
            // Clear Display All Grid Property Details
            ClearDisplayGridPropertyDetails();

            // Loop through all scenes - by looping through all gridProperties in the array
            foreach (SO_GridProperties so_GridProperties in so_gridPropertiesArray)
            {
                // Get grid property details dictionary for scene
                if (GameObjectSave.sceneData.TryGetValue(so_GridProperties.sceneName.ToString(),
                    out SceneSave sceneSave))
                {
                    if (sceneSave.gridPropertyDetailsDictionary != null)
                    {
                        for (int i = sceneSave.gridPropertyDetailsDictionary.Count-1; i >= 0; i--)
                        {
                            KeyValuePair<string, GridPropertyDetails> item =
                                sceneSave.gridPropertyDetailsDictionary.ElementAt(i);
                            GridPropertyDetails gridPropertyDetails = item.Value;

                            #region Update all grid properties to reflect the advanced in the day

                            // If ground is watered, then clear water
                            if (gridPropertyDetails.daySinceWatered > -1)
                            {
                                gridPropertyDetails.daySinceWatered = -1;
                            }

                            // if a crop is Planted
                            if (gridPropertyDetails.growthDays > -1)
                            {
                                gridPropertyDetails.growthDays += 1;
                            }

                            // Set gridPropertyDetails
                            SetGridPropertyDetails(gridPropertyDetails.gridX,gridPropertyDetails.gridY,gridPropertyDetails,
                                sceneSave.gridPropertyDetailsDictionary);

                            #endregion
                        }
                    }
                }
            }

            // Display grid property details to reflect changed values
            DisplayGridPropertyDetails();
        }

        public void ISaveableRegister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Add(this);
        }

        public void ISaveableDeregister()
        {
            SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
        }

        public void ISaveableLoad(GameSave gameSave)
        {
            if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID,out GameObjectSave gameObjectSave))
            {
                GameObjectSave = gameObjectSave;

                // Restore data for current scene
                ISaveableRestoreScene(SceneManager.GetActiveScene().name);
            }
        }

        public void ISaveableStoreScene(string sceneName)
        {
            // Remove sceneSave for scene
            GameObjectSave.sceneData.Remove(sceneName);

            // Create sceneSave for scene
            SceneSave sceneSave=new SceneSave();

            // Create and add dictionary grid property details
            sceneSave.gridPropertyDetailsDictionary = gridPropertyDetailsDictionary;

            // Create and add bool dictionary for first time since loaded
            sceneSave.boolDictionary=new Dictionary<string, bool>();
            sceneSave.boolDictionary.Add("isFirstTimeSceneLoaded", isFirstTimeSceneLoaded);

            // Add sceneSave to gameObject scene data
            GameObjectSave.sceneData.Add(sceneName,sceneSave);
        }

        public GameObjectSave ISaveableSave()
        {
            // Store current scene data
            ISaveableStoreScene(SceneManager.GetActiveScene().name);

            return GameObjectSave;
        }

        public void ISaveableRestoreScene(string sceneName)
        {
            // Get sceneSave for scene - it exists since we created it in initialize
            if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
            {
                // Get grid property details dictionary - it exists since we created it in initialize
                if (sceneSave.gridPropertyDetailsDictionary != null)
                {
                    gridPropertyDetailsDictionary = sceneSave.gridPropertyDetailsDictionary;
                }

                // Get dictionary of bools - it exists since we created it in initialize
                if (sceneSave.boolDictionary != null && sceneSave.boolDictionary.TryGetValue("isFirstTimeSceneLoaded",
                        out bool storedIsFirstTimeSceneLoaded))
                {
                    isFirstTimeSceneLoaded = storedIsFirstTimeSceneLoaded;
                }

                // Instantiate any crop prefabs initially present in the scene
                if (isFirstTimeSceneLoaded)
                {
                    EventHandler.CallInstantiateCropPrefabsEvent();
                }


                // if grid property exist
                if (gridPropertyDetailsDictionary.Count > 0)
                {
                    // grid property details found for the current scene destroy existing ground decoration
                    ClearDisplayGridPropertyDetails();

                    //Instantiate grid property details for current scene
                    DisplayGridPropertyDetails();
                }

                if (isFirstTimeSceneLoaded == true)
                {
                    isFirstTimeSceneLoaded = false;
                }
            }
        }
    }
}
