using Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using _Player;

namespace SceneManagement
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTeleport : MonoBehaviour
    {
        [SerializeField] private SceneName sceneName = SceneName.Scene1_Farm;
        [SerializeField] private Vector3 scenePositionGOto=new Vector3();

        private void OnTriggerStay2D(Collider2D other)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                //Calculate Player new position
                float xPosition = Mathf.Approximately(scenePositionGOto.x, 0f)
                    ? player.transform.position.x: scenePositionGOto.x;

                float yPosition= Mathf.Approximately(scenePositionGOto.y, 0f)
                    ? player.transform.position.y: scenePositionGOto.y;

                //float zPosition = 0;

                // Teleport to new scene
                SceneControllerManager.Instance.FadeAndLoadScene(sceneName.ToString(),scenePositionGOto);
            }
        }
    }
}
