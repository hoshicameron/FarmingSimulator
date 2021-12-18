using System.Collections;
using System.Collections.Generic;
using Enums;
using Events;
using Misc;
using TimeSystem;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

namespace Lighting
{
    public class LightingController : MonoBehaviour
    {
        [SerializeField] private LightingSchedule lightingSchedule;
        [SerializeField] private bool isLightingFlicker = false;
        [SerializeField] [Range(0f, 1.0f)] private float lightFlickerIntensity;
        [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMin;
        [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMax;

        private Light2D light2D;
        private Dictionary<string, float> lightingBrightnessDictionary;
        private float currentLightIntensity;
        private float lightFlickerTimer = 0f;
        private Coroutine fadeInLightRoutine;

        private void Awake()
        {
            // Get 2d light
            light2D = GetComponent<Light2D>();

            // Disable if no light
            if (light2D == null)
                enabled = false;

            lightingBrightnessDictionary=new Dictionary<string, float>();

            foreach ( LightingBrightness lightingBrightness in lightingSchedule.lightingBrightnesses)
            {
                string key = lightingBrightness.season.ToString()+lightingBrightness.hour.ToString();
                lightingBrightnessDictionary.Add(key,lightingBrightness.lightIntensity);
            }
        }

        private void OnEnable()
        {
            EventHandler.AdvanceGameHourEvent+= EventHandler_AdvanceGameHourEvent;
            EventHandler.AfterSceneLoadEvent += EventHandler_AfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            EventHandler.AdvanceGameHourEvent-= EventHandler_AdvanceGameHourEvent;
            EventHandler.AfterSceneLoadEvent -= EventHandler_AfterSceneLoadEvent;
        }

        private void EventHandler_AfterSceneLoadEvent()
        {
            SetLightingAfterSceneLoaded();
        }



        private void EventHandler_AdvanceGameHourEvent(int gameYear, Season gameSeason,int gameDay,string gameDayOfWeek,
                            int gameHour,int gameMinute,int gameSecond)
        {
            SetLightingIntensity(gameSeason, gameHour, true);
        }

        private void Update()
        {
            if (isLightingFlicker)
                lightFlickerTimer -= Time.deltaTime;
        }

        private void LateUpdate()
        {
            if (lightFlickerTimer <= 0 && isLightingFlicker)
            {
                LightFlicker();
            } else
            {
                light2D.intensity = currentLightIntensity;
            }
        }

        private void SetLightingAfterSceneLoaded()
        {
            Season season = TimeManager.Instance.GetGameSeason();
            int hour = TimeManager.Instance.GetGameTime().Hours;

            // Set light Intensity Immediately after scene loaded
            SetLightingIntensity(season,hour,false);
        }

        private void SetLightingIntensity(Season gameSeason, int gameHour, bool fadeIn)
        {
            int i = 0;

            // Get light intensity for nearest game hour that is less than or equal to the current game hour for the same seaon
            while (i<=23)
            {
                string key = gameSeason.ToString() + gameHour.ToString();
                if (lightingBrightnessDictionary.TryGetValue(key,out float targetLightingIntensity))
                {

                    if (fadeIn)
                    {
                        // Stop fade in coroutine if already running
                        if(fadeInLightRoutine!=null)    StopCoroutine(fadeInLightRoutine);

                        // Fade in to new light intensity level
                        fadeInLightRoutine = StartCoroutine(FadeLightRoutine(targetLightingIntensity));
                    } else
                    {
                        currentLightIntensity = targetLightingIntensity;
                    }
                    break;
                }

                i++;

                gameHour--;
                if (gameHour < 0) gameHour = 23;
            }

        }

        private IEnumerator FadeLightRoutine(float targetLightingIntensity)
        {
            // Calculate how fast the light should be fade based on current and target intensity and duration
            float fadeSpeed = Mathf.Abs(currentLightIntensity - targetLightingIntensity) / Settings.lightFadeDuration;

            // Loop while fading
            while (Mathf.Approximately(currentLightIntensity, targetLightingIntensity))
            {
                // Move current intensity toward target intensity
                currentLightIntensity = Mathf.MoveTowards(currentLightIntensity, targetLightingIntensity,
                    fadeSpeed * Time.deltaTime);

                yield return null;
            }

            currentLightIntensity = targetLightingIntensity;
        }


        private void LightFlicker()
        {
            // Calculate random flicker
            light2D.intensity = Random.Range(currentLightIntensity,
                currentLightIntensity + (currentLightIntensity * lightFlickerIntensity));

            // If the light is to flicker calculate random flicker interval
            lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
        }
    }
}
