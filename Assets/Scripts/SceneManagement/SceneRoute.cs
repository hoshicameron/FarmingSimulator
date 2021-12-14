
using System.Collections.Generic;
using Enums;

namespace SceneManagement
{
    [System.Serializable]
    public class SceneRoute
    {
        public SceneName fromSceneName;
        public SceneName toSceneName;
        public List<ScenePath> scenePathList;
    }
}
