using Enums;
using Events;
using Misc;
using UnityEngine;

namespace TimeSystem
{
    public class TimeManager : SingletonMonoBehaviour<TimeManager>
    {
        private int gameYear = 1;
        private Season gameSeason = Season.Spring;
        private int gameday = 1;
        private int gameHour = 6;
        private int gameMinute = 30;
        private int gameSecond = 0;
        private string gameDayOfWeek = "Mon";

        private bool gameClockPaused = false;

        // Game tick will updated with Time.DeltaTime every frame to determine whether game second happened or not
        private float gameTick = 0f;

        private void Start()
        {
            EventHandler.CallAdvancedGameMinuteEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                gameMinute,gameSecond);
        }

        private void Update()
        {
            if (!gameClockPaused)
            {
                GameTick();
            }
        }

        private void GameTick()
        {
            gameTick += Time.deltaTime;
            if (gameTick >= Settings.secondsPerGameSeconds)
            {
                gameTick -= Settings.secondsPerGameSeconds;

                UpdateGameSeconds();
            }
        }

        private void UpdateGameSeconds()
        {
            gameSecond++;
            if (gameSecond > 59)
            {
                gameSecond = 0;
                gameMinute++;
                if (gameMinute > 59)
                {
                    gameMinute = 0;
                    gameHour++;
                    if (gameHour > 23)
                    {
                        gameHour = 0;
                        gameday++;
                        if (gameday > 30)
                        {
                            gameday=1;
                            int gs = (int) gameSeason;
                            gs++;
                            gameSeason = (Season) gs;
                            if (gs > 3)
                            {
                                gs = 0;
                                gameYear++;
                                // Restart the game
                                if (gameYear > 9999)
                                    gameYear = 1;
                                EventHandler.CallAdvancedGameYearEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                                    gameMinute,gameSecond);
                            }

                            EventHandler.CallAdvancedGameSeasonEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                                gameMinute,gameSecond);
                        }

                        gameDayOfWeek = GetDayOfWeek();
                        EventHandler.CallAdvancedGameDayEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                            gameMinute,gameSecond);
                    }

                    EventHandler.CallAdvancedGameHourEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                        gameMinute,gameSecond);
                }
                EventHandler.CallAdvancedGameMinuteEvent(gameYear,gameSeason,gameday,gameDayOfWeek,gameHour,
                    gameMinute,gameSecond);

                /*Debug.Log($"Game Year: {gameYear},Game Season: {gameSeason},Game Day: {gameday}," +
                          $"Game Day Of Week: {gameDayOfWeek},Game Hour: {gameHour},Game Minute: {gameMinute}," +
                          $"Game Second: {gameSecond}");*/
            }
            // Call Advanced game Second event would go here if required
        }

        private string GetDayOfWeek()
        {
            int totalDays = (((int) gameSeason) * 30) + gameday;
            int dayOfWeek = totalDays % 7;

            switch (dayOfWeek)
            {
                case   1:
                    return "Mon";
                case   2:
                    return "Tue";
                case   3:
                    return "Wed";
                case   4:
                    return "Thu";
                case   5:
                    return "Fri";
                case   6:
                    return "Sat";
                case   0:
                    return "Sun";
                default:
                    return "";
            }
        }

        // TODo: Remove
        /// <summary>
        ///  Advance 1 game minute
        /// </summary>
        public void TestAdvanceGameMinute()
        {
            for (int i = 0; i < 60; i++)
            {
                UpdateGameSeconds();
            }
        }

        // TODo: Remove
        /// <summary>
        ///  Advance 1 game day
        /// </summary>
        public void TestAdvanceGameDay()
        {
            for (int i = 0; i < 86400; i++)
            {
                UpdateGameSeconds();
            }
        }
    }
}
