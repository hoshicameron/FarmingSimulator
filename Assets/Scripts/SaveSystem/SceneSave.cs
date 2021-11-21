
using System.Collections.Generic;
using Maps;

namespace SaveSystem
{
    [System.Serializable]
    public class SceneSave
    {
        // String key is an identifier name we choose for this list
        public Dictionary<string, bool> boolDictionary;

        // Because we have only one list of item per scene, It's better to use a list than dictionary
        public List<SceneItem> SceneItemList;
        // String key is coordinate of the gridProperty
        public Dictionary<string, GridPropertyDetails> GridPropertyDetailsesDictionary;
    }
}
