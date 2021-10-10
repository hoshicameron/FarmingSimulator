using System.Collections.Generic;
using Misc;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    public class SaveLoadManager : SingletonMonoBehaviour<SaveLoadManager>
    {
        // in this list we can store variety of object if they implement ISaveable interface
        public List<ISaveable> iSaveableObjectList;

        protected override void Awake()
        {
            base.Awake();

            iSaveableObjectList=new List<ISaveable>();
        }

        public void StoreCurrentSceneData()
        {
            // Loop through all ISaveable objects and trigger store scene data for each
            foreach (ISaveable iSaveableObject in iSaveableObjectList)
            {
                iSaveableObject.ISaveableStoreScene(SceneManager.GetActiveScene().name);
            }
        }

        public void RestoreCurrentSceneData()
        {
            // Loop through all ISaveable objects and trigger restore scene data for each
            foreach (ISaveable iSaveableObject in iSaveableObjectList)
            {
                iSaveableObject.ISaveableRestoreScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
