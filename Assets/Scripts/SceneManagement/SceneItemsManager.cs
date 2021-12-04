using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Events;
using Items;
using Misc;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    [RequireComponent(typeof(GenerateGUID))]
    public class SceneItemsManager : SingletonMonoBehaviour<SceneItemsManager>,ISaveable
    {
        private Transform parentItem;
        [SerializeField] private GameObject itemPrefab = null;

        public string ISaveableUniqueID { get; set; }
        public GameObjectSave GameObjectSave { get; set; }


        private void AfterSceneLoad()
        {
            parentItem = GameObject.FindGameObjectWithTag(Tags.ItemsParentTransform).transform;
        }
        private void OnEnable()
        {
            ISaveableRegister();
            EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        }

        private void OnDisable()
        {
            ISaveableDeregister();
            EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        }

        /// <summary>
        /// Instantiate single item
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="itemPosition"></param>
        public void InstantiateSceneItems(int itemCode, Vector3 itemPosition)
        {
            GameObject itemGameObject = Instantiate(itemPrefab, itemPosition, Quaternion.identity, parentItem);
            Item item = itemGameObject.GetComponent<Item>();
            item.Init(itemCode);
        }

        protected override void Awake()
        {
            base.Awake();

            ISaveableUniqueID = GetComponent<GenerateGUID>().GUid;
            GameObjectSave=new GameObjectSave();
        }

        /// <summary>
        /// Destroy items currently in the scene
        /// </summary>
        private void DestroySceneItems()
        {
            // Get all items in the scene
            Item[] itemsInScene = GameObject.FindObjectsOfType<Item>();

            // Loop through all scene items and destroy them
            for (int i = 0; i < itemsInScene.Length; i++)
            {
                Destroy(itemsInScene[i].gameObject);
            }
        }

        private void InstantiateSceneItems(List<SceneItem> sceneItemList)
        {
            GameObject itemGameObject;
            foreach (SceneItem sceneItem in sceneItemList)
            {
                itemGameObject = Instantiate(itemPrefab, new Vector3(sceneItem.position.x, sceneItem.position.y,
                    sceneItem.position.z), Quaternion.identity, parentItem);

                Item item = itemGameObject.GetComponent<Item>();
                item.ItemCode = sceneItem.itemCode;
                item.name = sceneItem.itemName;
            }
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
            // Remove old scene save for gameObject if exists
            GameObjectSave.sceneData.Remove(sceneName);

            // Get all items in the scene
            List<SceneItem> sceneItemList=new List<SceneItem>();
            Item[] itemsInScene = FindObjectsOfType<Item>();

            // Loop through all scene items
            foreach (Item item in itemsInScene)
            {
                SceneItem sceneItem=new SceneItem();
                sceneItem.itemCode = item.ItemCode;
                sceneItem.position=new Vector3Serializable(item.transform.position.x,
                                            item.transform.position.y,item.transform.position.z);
                sceneItem.itemName = item.name;

                // Add scene item to list
                sceneItemList.Add(sceneItem);
            }

            // Create list scene items in scene save and set to scene item list
            SceneSave sceneSave=new SceneSave();
            sceneSave.SceneItemList = sceneItemList;

            // Add scene item to list
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
            if (GameObjectSave.sceneData.TryGetValue(sceneName, out SceneSave sceneSave))
            {
                if (sceneSave.SceneItemList != null)
                {
                    // Scene List items found -destroy existing item in scene
                    DestroySceneItems();

                    // Now instantiate the List of scene items
                    InstantiateSceneItems(sceneSave.SceneItemList);
                }

            }
        }
    }
}
