using Enums;
using Maps;

namespace SceneManagement
{
    [System.Serializable]
    public class ScenePath
    {
        public SceneName sceneName;
        public GridCoordinate fromGridCell;
        public GridCoordinate toGridCell;
    }
}
