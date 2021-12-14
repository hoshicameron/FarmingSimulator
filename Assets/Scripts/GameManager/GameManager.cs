using System.Collections;
using System.Collections.Generic;
using Enums;
using Misc;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Weather currentWeather;
    protected override void Awake()
    {
        base.Awake();

        // TODO: Need a resolution setting option screen
        Screen.SetResolution(1920,1080,FullScreenMode.FullScreenWindow,0);
        Application.targetFrameRate = 30;

        // Set starting weather
        currentWeather = Weather.Dry;
    }
}
