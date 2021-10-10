using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Maps
{
    [CreateAssetMenu(fileName = "so_GridProperties",menuName ="Scriptable Objects/Grid Properties")]
    public class SO_GridProperties : ScriptableObject
    {
        public SceneName sceneName;
        public int gridWidth;
        public int gridHeight;
        public int originX;
        public int originY;

        [SerializeField] public List<GridProperty> GridPropertiesList;
    }
}
