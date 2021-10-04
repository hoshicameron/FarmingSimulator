using UnityEngine;
using Cinemachine;
using Events;
using Misc;

namespace SceneManagement
{
    public class SwitchConfineBoundingShape : MonoBehaviour
    {
        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += SceneLoaded;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= SceneLoaded;
        }

        private void SceneLoaded()
        {
            SwitchBoundingShape();
        }

        /// <summary>
        /// Switch the collider that cinemachine uses to define the ege of the screen
        /// </summary>
        private void SwitchBoundingShape()
        {
            PolygonCollider2D polygonCollider2D =
                GameObject.FindGameObjectWithTag(Tags.BoundsConfiner).GetComponent<PolygonCollider2D>();

            CinemachineConfiner cinemachineConfiner = GetComponent<CinemachineConfiner>();
            cinemachineConfiner.m_BoundingShape2D = polygonCollider2D;

            //Since the confiner bounds have changed need to call this to clear the cach
            cinemachineConfiner.InvalidatePathCache();
        }
    }
}
