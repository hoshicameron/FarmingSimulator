using Enums;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Events
{
    public delegate void MovementDelegate(float inputX,float inputY,bool isWalking,bool isRunning,bool isIdle, bool isCarrying,
                                          bool axeRight,bool axeLeft,bool axeUp,bool axeDown,
                                          bool fishingRight,bool fishingLeft,bool fishingUp,bool fishingDown,
                                          bool miscRight,bool miscLeft,bool miscUp,bool miscDown,
                                          bool harvestRight,bool harvestLeft,bool harvestUp,bool harvestDown,
                                          bool pickRight,bool pickLeft,bool pickUp,bool pickDown,
                                          bool sickleRight,bool sickleLeft,bool sickleUp,bool sickleDown,
                                          bool hammerRight,bool hammerLeft,bool hammerUp,bool hammerDown,
                                          bool shovelRight,bool shovelLeft,bool shovelUp,bool shovelDown,
                                          bool hoeRight,bool hoeLeft,bool hoeUp,bool hoeDown,
                                          bool idleUp,bool idleDown,bool idleLeft,bool idleRight);


    public static class EventHandler
    {
        // Drop Selected item event
        public static event Action DropSelectedItemEvent;

        public static void CallDropSelectedItemEvent()
        {
            DropSelectedItemEvent?.Invoke();
        }

        // Remove selected item from inventory
        public static event Action RemoveSelectedItemFromInventoryEvent;

        public static void CallRemoveSelectedItemFromInventoryEvent()
        {
            RemoveSelectedItemFromInventoryEvent?.Invoke();
        }

        // Harvest Action Effect Event
        public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

        public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
        {
            HarvestActionEffectEvent?.Invoke(effectPosition,harvestActionEffect);
        }

        // Inventory Update Event
        public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

        // Trigger the Event
        public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation,
            List<InventoryItem> inventoryItemList)
        {
            InventoryUpdatedEvent?.Invoke(inventoryLocation,inventoryItemList);
        }

        //Instantiate crop prefabs
        public static event Action InstantiateCropPrefabsEvent;

        public static void CallInstantiateCropPrefabsEvent()
        {
            InstantiateCropPrefabsEvent?.Invoke();
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
            bool harvestRight,bool harvestLeft,bool harvestUp,bool harvestDown,
            bool pickRight,bool pickLeft,bool pickUp,bool pickDown,
            bool sickleRight,bool sickleLeft,bool sickleUp,bool sickleDown,
            bool hammerRight,bool hammerLeft,bool hammerUp,bool hammerDown,
            bool shovelRight,bool shovelLeft,bool shovelUp,bool shovelDown,
            bool hoeRight,bool hoeLeft,bool hoeUp,bool hoeDown,
            bool idleUp,bool idleDown,bool idleLeft,bool idleRight

        )
        {
            MovementEvent?.Invoke(
                inputX,inputY,
                isWalking,isRunning,isIdle,isCarrying
                ,axeRight,axeLeft,axeUp,axeDown,
                fishingRight,fishingLeft,fishingUp,fishingDown,
                miscRight,miscLeft,miscUp,miscDown,
                harvestRight,harvestLeft,harvestUp,harvestDown,
                pickRight,pickLeft,pickUp,pickDown,
                sickleRight,sickleLeft,sickleUp,sickleDown,
                hammerRight,hammerLeft,hammerUp,hammerDown,
                shovelRight,shovelLeft,shovelUp,shovelDown,
                hoeRight,hoeLeft,hoeUp,hoeDown,
                idleUp,idleDown,idleLeft,idleRight

                );
        }

        // Time events

        // Advanced game minute
        public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

        public static void CallAdvanceGameMinuteEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvanceGameMinuteEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Hour
        public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

        public static void CallAdvanceGameHourEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvanceGameHourEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Day
        public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

        public static void CallAdvanceGameDayEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvanceGameDayEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Season
        public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

        public static void CallAdvanceGameSeasonEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvanceGameSeasonEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Advanced game Year
        public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

        public static void CallAdvanceGameYearEvent(int gameYear, Season season, int gameDay, string gameDayOfWeek,
            int gameHour, int gameMinute, int gameSecond)
        {
            AdvanceGameYearEvent?.Invoke(gameYear,season,gameDay,gameDayOfWeek,gameHour,gameMinute,gameSecond);
        }

        // Scene Load Events - in the order they happen

        // Before scene unload Fade Out Event- (purely notification)
        public static event Action BeforeSceneUnloadFadeOutEvent;
        public static void CallBeforeSceneUnloadFadeOutEvent()
        {
            BeforeSceneUnloadFadeOutEvent?.Invoke();
        }

        // Before Scene unload event
        public static event Action BeforeSceneUnloadEvent;

        public static void CallBeforeSceneUnloadEvent()
        {
            BeforeSceneUnloadEvent?.Invoke();
        }

        // After scene loaded event
        public static event Action AfterSceneLoadEvent;

        public static void CallAfterSceneLoadEvent()
        {
            AfterSceneLoadEvent?.Invoke();
        }


        // After scene load fade in event
        public static event Action AfterSceneLoadFadeInEvent;

        public static void CallAfterSceneLoadFadeInEvent()
        {
            AfterSceneLoadFadeInEvent?.Invoke();
        }

    }

}