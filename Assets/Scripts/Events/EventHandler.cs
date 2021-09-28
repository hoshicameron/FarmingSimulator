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
    }

}