using UnityEngine;

namespace Misc
{
    public static class Settings
    {
        //Player Movement
        public const float runningSpeed = 5.333f;
        public const float walkingSpeed = 2.666f;


        //Player Animation Parameters
        public static int xInput;
        public static int yInput;
        public static int isWalking;
        public static int isRunning;

        public static int axeRight;
        public static int axeLeft;
        public static int axeUp;
        public static int axeDown;

        public static int fishingRight;
        public static int fishingLeft;
        public static int fishingUp;
        public static int fishingDown;

        public static int miscRight;
        public static int miscLeft;
        public static int miscUp;
        public static int miscDown;

        public static int pickRight;
        public static int pickLeft;
        public static int pickUp;
        public static int pickDown;

        public static int sickleRight;
        public static int sickleLeft;
        public static int sickleUp;
        public static int sickleDown;

        public static int hammerRight;
        public static int hammerLeft;
        public static int hammerUp;
        public static int hammerDown;

        public static int shovelRight;
        public static int shovelLeft;
        public static int shovelUp;
        public static int shovelDown;

        public static int hoeRight;
        public static int hoeLeft;
        public static int hoeUp;
        public static int hoeDown;

        //Shared Animation Parameters
        public static int idleRight;
        public static int idleLeft;
        public static int idleUp;
        public static int idleDown;


        //Static Constructor
        static Settings()
        {
         xInput=Animator.StringToHash("XInput");
         yInput=Animator.StringToHash("YInput");
         isWalking=Animator.StringToHash("IsWalking");
         isRunning=Animator.StringToHash("IsRunning");

         axeRight=Animator.StringToHash("AxeRight");
         axeLeft=Animator.StringToHash("AxeLeft");
         axeUp=Animator.StringToHash("AxeUp");
         axeDown=Animator.StringToHash("AxeDown");

         fishingRight=Animator.StringToHash("FishingRight");
         fishingLeft=Animator.StringToHash("FishingLeft");
         fishingUp = Animator.StringToHash("FishingUp");
         fishingDown = Animator.StringToHash("FishingDown");

         miscRight=Animator.StringToHash("MiscRight");
         miscLeft=Animator.StringToHash("MiscLeft");
         miscUp=Animator.StringToHash("MiscUp");
         miscDown=Animator.StringToHash("MiscDown");

         pickRight=Animator.StringToHash("PickRight");
         pickLeft=Animator.StringToHash("PickLeft");
         pickUp=Animator.StringToHash("PickUp");
         pickDown=Animator.StringToHash("PickDown");

         sickleRight=Animator.StringToHash("SickleRight");
         sickleLeft=Animator.StringToHash("SickleLeft");
         sickleUp=Animator.StringToHash("SickleUp");
         sickleDown=Animator.StringToHash("SickleDown");

         hammerRight=Animator.StringToHash("HammerRight");
         hammerLeft=Animator.StringToHash("HammerLeft");
         hammerUp=Animator.StringToHash("HammerUp");
         hammerDown=Animator.StringToHash("HammerDown");

         shovelRight=Animator.StringToHash("ShovelRight");
         shovelLeft=Animator.StringToHash("ShovelLeft");
         shovelUp=Animator.StringToHash("ShovelUp");
         shovelDown=Animator.StringToHash("ShovelDown");

         hoeRight=Animator.StringToHash("HoeRight");
         hoeLeft=Animator.StringToHash("HoeLeft");
         hoeUp=Animator.StringToHash("HoeUp");
         hoeDown=Animator.StringToHash("HoeDown");

        //Shared Animation Parameters
         idleRight=Animator.StringToHash("IdleRight");
         idleLeft=Animator.StringToHash("IdleLeft");
         idleUp=Animator.StringToHash("IdleUp");
         idleDown=Animator.StringToHash("IdleDown");
        }
    }
}