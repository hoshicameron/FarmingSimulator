using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    public float xInput;
    public float yInput;
    public bool isWalking;
    public bool isRunning;
    public bool isIdle;
    public bool isCarrying;
    public bool axeRight;
    public bool axeLeft;
    public bool axeUp;
    public bool axeDown;
    public bool fishingRight;
    public bool fishingLeft;
    public bool fishingUp;
    public bool fishingDown;
    public bool miscRight;
    public bool miscLeft;
    public bool miscUp;
    public bool miscDown;
    public bool pickRight;
    public bool pickLeft;
    public bool pickUp;
    public bool pickDown;
    public bool sickleRight;
    public bool sickleLeft;
    public bool sickleUp;
    public bool sickleDown;
    public bool hammerRight;
    public bool hammerLeft;
    public bool hammerUp;
    public bool hammerDown;
    public bool shovelRight;
    public bool shovelLeft;
    public bool shovelUp;
    public bool shovelDown;
    public bool hoeRight;
    public bool hoeLeft;
    public bool hoeUp;
    public bool hoeDown;
    public bool idleUp;
    public bool idleDown;
    public bool idleLeft;
    public bool idleRight;

    private void Update()
    {
        EventHandler.CallMovementEvent(
            xInput,yInput,isWalking,isRunning,isIdle,isCarrying,axeRight,axeLeft,axeUp,
            axeDown,fishingRight,fishingLeft,fishingUp,fishingDown,miscRight,
            miscLeft,miscUp,miscDown,pickRight,pickLeft,pickUp,pickDown,sickleRight,
            sickleLeft,sickleUp,sickleDown,hammerRight,hammerLeft,hammerUp,hammerDown,
            shovelRight,shovelLeft,shovelUp,shovelDown,hoeRight,hoeLeft,hoeUp,hoeDown,
            idleUp,idleDown,idleLeft,idleRight
            );
    }


}


