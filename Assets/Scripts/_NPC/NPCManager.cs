using System.Collections.Generic;
using Enums;
using Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using _AStar;
using Events;
using SceneManagement;

namespace _NPC
{
    [RequireComponent(typeof(AStar))]
    public class NPCManager : SingletonMonoBehaviour<NPCManager>
    {
        [SerializeField] private SO_SceneRouteList so_SceneRouteList = null;
        private Dictionary<string, SceneRoute> sceneRouteDictionary;

        [HideInInspector]
        public NPC[] npcArray;

        private AStar aStar;

        protected override void Awake()
        {
            base.Awake();


            // Create sceneRoute dictionary
            sceneRouteDictionary = new Dictionary<string, SceneRoute>();

            if (so_SceneRouteList.sceneRouteList.Count > 0)
            {
                foreach (SceneRoute sceneRoute in so_SceneRouteList.sceneRouteList)
                {
                    // Check for duplicate routes in dictionary
                    if (sceneRouteDictionary.ContainsKey(sceneRoute.fromSceneName.ToString() + sceneRoute.toSceneName.ToString()))
                    {
                        Debug.Log("** Duplicate Scene Route Key Found ** Check for duplicate routes in the scriptable object scene route list");
                        continue;
                    }

                    // Add route to dictionary
                    sceneRouteDictionary.Add(sceneRoute.fromSceneName.ToString() + sceneRoute.toSceneName.ToString(), sceneRoute);
                }
            }


            aStar = GetComponent<AStar>();

            // Get NPC gameobjects in scene
            npcArray = FindObjectsOfType<NPC>();
        }

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        }

        private void AfterSceneLoad()
        {

            SetNPCsActiveStatus();
        }

        private void SetNPCsActiveStatus()
        {
            foreach (NPC npc in npcArray)
            {
                NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

                if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                {
                    npcMovement.SetNPCActiveInScene();
                }
                else
                {
                    npcMovement.SetNPCInactiveInScene();
                }
            }
        }

        public SceneRoute GetSceneRoute(string fromSceneName, string toSceneName)
        {
            // Get scene route from dictionary
            return sceneRouteDictionary.TryGetValue(fromSceneName + toSceneName, out var sceneRoute) ? sceneRoute : null;
        }


        public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStepStack)
        {
            return aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStepStack);
        }
    }
}
