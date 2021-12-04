using System.Collections.Generic;

namespace SaveSystem
{
    [System.Serializable]
    public class GameSave
    {
        //string key = GUID gameObject ID
        public Dictionary<string, GameObjectSave> gameObjectData;

        public GameSave()
        {
            gameObjectData=new Dictionary<string, GameObjectSave>();
        }
    }
}
