using Enums;
using UnityEngine;

namespace _NPC
{
    public class NPCMovementStep : MonoBehaviour
    {
        public SceneName sceneName;
        public int hour;
        public int minute;
        public int second;
        public Vector2Int gridCoordinate;
    }
}
