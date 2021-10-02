using Enums;
using System;
using System.Collections.Generic;

namespace Events
{
    public delegate void MovementDelegate(float inputX,float inputY,bool isWalking,bool isRunning,bool isIdle, bool isCarrying,
                                          bool axeRight,bool axeLeft,bool axeUp,bool axeDown,
                                          bool fishingRight,bool fishingLeft,bool fishingUp,bool fishingDown,
                                          bool miscRight,bool miscLeft,bool miscUp,bool miscDown,
                                          bool pickRight,bool pickLeft,bool pickUp,bool pickDown,
                                          bool sickleRight,bool sickleLeft,bool sickleUp,bool sickleDown,
                                          bool hammerRight,bool hammerLeft,bool hammerUp,bool hammerDown,
                                          bool shovelRight,bool shovelLeft,bool shovelUp,bool shovelDown,
                                          bool hoeRight,bool hoeLeft,bool hoeUp,bool hoeDown,
                                          bool idleUp,bool idleDown,bool idleLeft,bool idleRight);


    public static class EventHandler
    {
        // Inventory Update Event
        public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdateEvent;

        // Trigger the Event
        public static void CallInventoryUpdateEvent(InventoryLocation inventoryLocation,
            List<InventoryItem> inventoryItemList)
        {
            InventoryUpdateEvent?.Invoke(inventoryLocation,inventoryItemList);
        }

        // Movement Event
        public static event MovementDelegate MovementEvent;

        // Movement Event Call For Publisher
        public static void CallMovementEvent
        (

            float inputX,float inputY,bool isWalking,bool isRunning,bool isIdle, bool isCarrying,
            bool axeRight,bool axeLeft,bool axeUp,bool axeDown,
            bool fishingRight,bool fishingLeft,bool fishingUp,bool fishingDown,
            bool miscRight,bool miscLeft,bool miscUp,bool miscDown,
            bool pickRight,bool pickLeft,bool pickUp,bool pickDown,
            bool sickleRight,bool sickleLeft,bool sickleUp,bool sickleDown,
            bool hammerRight,bool hammerLeft,bool hammerUp,bool hammerDown,
            bool shovelRight,bool shovelLeft,bool shovelUp,bool shovelDown,
            bool hoeRight,bool hoeLeft,bool hoeUp,bool hoeDown,
            bool idleUp,bool idleDown,bool idleLeft,bool idleRight

        )
        {
            MovementEvent?.Invoke(
                inputX,inputY,isWalking,isRunning,isIdle,isCarrying,axeRight,axeLeft,axeUp,axeDown,
                fishingRight,fishingLeft,fishingUp,fishingDown,
                miscRight,miscLeft,miscUp,miscDown,pickRight,pickLeft,pickUp,pickDown,
                sickleRight,sickleLeft,sickleUp,sickleDown,hammerRight,hammerLeft,hammerUp,
                hammerDown,shovelRight,shovelLeft,shovelUp,shovelDown,hoeRight,hoeLeft,hoeUp,
                hoeDown,idleUp,idleDown,idleLeft,idleRight

                );
        }

        // Time events

        // Advanced game minute
        public static event Action<int, Season, int, string, int, int, int> AdvancedGameMinuteEvent;

        public static void CallAdvancedGameMinuteEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvancedGameMinuteEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Hour
        public static event Action<int, Season, int, string, int, int, int> AdvancedGameHourEvent;

        public static void CallAdvancedGameHourEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvancedGameHourEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Day
        public static event Action<int, Season, int, string, int, int, int> AdvancedGameDayEvent;

        public static void CallAdvancedGameDayEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvancedGameDayEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Season
        public static event Action<int, Season, int, string, int, int, int> AdvancedGameSeasonEvent;

        public static void CallAdvancedGameSeasonEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvancedGameSeasonEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Year
        public static event Action<int, Season, int, string, int, int, int> AdvancedGameYearEvent;

        public static void CallAdvancedGameYearEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvancedGameYearEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }


    }

}