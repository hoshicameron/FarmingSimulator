using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Enums;
using Events;
using Misc;
using SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using _Player;

namespace SceneManagement
{
    public class SceneControllerManager : SingletonMonoBehaviour<SceneControllerManager>
    {
        private bool isFading;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private CanvasGroup faderCanvasGroup = null;
        [SerializeField] private Image faderImage = null;
        public SceneName startingSceneName;

        // This is the main external point pf contact and influence from the rest of the project.
        // This will be called when the player wants to switch scenes.

        /// <summary>
        /// Load scene by given scene name and spawn player in spawn position
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="spawnPosition"></param>
        public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)
        {
            // If fade isn't happening then start fading and switching scenes.
            if (!isFading)
            {
                StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
            }
        }

        //This is the coroutine where the 'building blocks' of the script put together
        private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
        {
            // Call before scene unload fade out event
            EventHandler.CallBeforeSceneUnloadFadeOutEvent();

            // Start fading to black and wait for it so finish before continuing.
            yield return StartCoroutine(Fade(1f));

            // Store scene data
            SaveLoadManager.Instance.StoreCurrentSceneData();

            // Set player position
            Player.Instance.gameObject.transform.position = spawnPosition;

            // Call before scene unload event
            EventHandler.CallBeforeSceneUnloadEvent();

            // Unload the current active scene
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

            // Start loading the given scene and wait for it to finish
            yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

            // Call after scene load event
            EventHandler.CallAfterSceneLoadEvent();

            // Load scene data
            SaveLoadManager.Instance.RestoreCurrentSceneData();

            // Start fading back in and wait for it to finish before exiting the function
            yield return StartCoroutine(Fade(0f));

            // Call after scene load fade in event
            EventHandler.CallAfterSceneLoadFadeInEvent();
        }

        private IEnumerator LoadSceneAndSetActive(string sceneName)
        {
            // Allow the given scene to load over several frames and add it
            // to the already loaded scenes( just the persistent scene at this point).
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            // Find the scene that most recently loaded (The one at the last index of the loaded scenes).
            Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            // Set the newly loaded scene as the active scene (this marks it as the one to be unlocked next).
            SceneManager.SetActiveScene(newlyLoadedScene);
        }

        private IEnumerator Start()
        {
            // Set the initial alpha to start off with a black screen
            faderImage.color=new Color(0f,0f,0f,1f);
            faderCanvasGroup.alpha = 1f;

            // Start the first scene loading and wait for it to finish
            yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName.ToString()));

            // If this event has any subscribers, call it.
            EventHandler.CallAfterSceneLoadEvent();

            SaveLoadManager.Instance.RestoreCurrentSceneData();

            // Once the scene is finished loading, start fading in.
            StartCoroutine(Fade(0f));


        }

        private IEnumerator Fade(float finalAlpha)
        {

            // Set the fading flag to true so the FadeAndSwitchScenes coroutine won't be called again.
            isFading = true;

            // Make the CanvasGroup blocks raycast into the scene so no more input can be accepted.
            faderCanvasGroup.blocksRaycasts = true;

            // Calculate how fast the CanvasGroup should fade based on it's current alpha,
            // it's final alpha and how long it has to change between the two
            float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

            // While the CanvasGroup hasn't reached the final alpha yet...
            while (!Mathf.Approximately(faderCanvasGroup.alpha,finalAlpha))
            {
                // ... move the alpha twards it's target alpha
                faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha,
                                        finalAlpha, fadeSpeed * Time.deltaTime);

                // Wait for a frame then continue
                yield return null;
            }

            // Set the flag to false since the fade has finished.
            isFading = false;

            // Stop the CanvasGroup from blocking raycast to input is no longer ignored.
            faderCanvasGroup.blocksRaycasts = false;
        }
    }
}
