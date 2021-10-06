
using System.Collections.Generic;

namespace SaveSystem
{
    [System.Serializable]
    public class GameObjectSave
    {
        // String key = Scene name
        public Dictionary<string,SceneSave> sceneData;

        public GameObjectSave()
        {
            sceneData=new Dictionary<string, SceneSave>();
        }

        public GameObjectSave(Dictionary<string, SceneSave> sceneData)
        {
            this.sceneData = sceneData;
        }
    }
}
