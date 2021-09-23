using Events;
using Misc;
using UnityEngine;

namespace Animation
{
    public class MovementAnimationParameterControl : MonoBehaviour
    {
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            EventHandler.MovementEvent+=SetAnimationParameters;
        }

        private void OnDisable()
        {
            EventHandler.MovementEvent -= SetAnimationParameters;
        }

        private void SetAnimationParameters(
            float inputX, float inputY, bool isWalking,bool isRunning, bool isIdle, bool isCarrying,bool axeRight,
            bool axeLeft, bool axeUp, bool axeDown, bool fishingRight,bool fishingLeft, bool fishingUp,
            bool fishingDown, bool miscRight, bool miscLeft,bool miscUp, bool miscDown, bool pickRight,
            bool pickLeft, bool pickUp, bool pickDown, bool sickleRight, bool sickleLeft, bool sickleUp,
            bool sickleDown, bool hammerRight, bool hammerLeft, bool hammerUp, bool hammerDown, bool shovelRight,
            bool shovelLeft, bool shovelUp,bool shovelDown, bool hoeRight, bool hoeLeft, bool hoeUp,
            bool hoeDown, bool idleUp, bool idleDown,bool idleLeft, bool idleRight)
        {
            animator.SetFloat(Settings.xInput,inputX);
            animator.SetFloat(Settings.yInput,inputY);
            animator.SetBool(Settings.isWalking,isWalking);
            animator.SetBool(Settings.isRunning,isRunning);

            if (axeRight)
               animator.SetTrigger(Settings.axeRight);
            if (axeLeft)
                animator.SetTrigger(Settings.axeLeft);
            if (axeUp)
                animator.SetTrigger(Settings.axeUp);
            if (axeDown)
                animator.SetTrigger(Settings.axeDown);

            if (fishingRight)
                animator.SetTrigger(Settings.fishingRight);
            if (fishingLeft)
                animator.SetTrigger(Settings.fishingLeft);
            if (fishingUp)
                animator.SetTrigger(Settings.fishingUp);
            if (fishingDown)
                animator.SetTrigger(Settings.fishingDown);

            if (miscRight)
                animator.SetTrigger(Settings.miscRight);
            if (miscLeft)
                animator.SetTrigger(Settings.miscLeft);
            if (miscUp)
                animator.SetTrigger(Settings.miscUp);
            if (miscDown)
                animator.SetTrigger(Settings.miscDown);

            if (pickRight)
                animator.SetTrigger(Settings.pickRight);
            if (pickLeft)
                animator.SetTrigger(Settings.pickLeft);
            if (pickUp)
                animator.SetTrigger(Settings.pickUp);
            if (pickDown)
                animator.SetTrigger(Settings.pickDown);

            if (sickleRight)
                animator.SetTrigger(Settings.sickleRight);
            if (sickleLeft)
                animator.SetTrigger(Settings.sickleLeft);
            if (sickleUp)
                animator.SetTrigger(Settings.sickleUp);
            if (sickleDown)
                animator.SetTrigger(Settings.sickleDown);

            if (hammerRight)
                animator.SetTrigger(Settings.hammerRight);
            if (hammerLeft)
                animator.SetTrigger(Settings.hammerLeft);
            if (hammerUp)
                animator.SetTrigger(Settings.hammerUp);
            if (hammerDown)
                animator.SetTrigger(Settings.hammerDown);

            if (shovelRight)
                animator.SetTrigger(Settings.shovelRight);
            if (shovelLeft)
                animator.SetTrigger(Settings.shovelLeft);
            if (shovelUp)
                animator.SetTrigger(Settings.shovelUp);
            if (shovelDown)
                animator.SetTrigger(Settings.shovelDown);

            if (hoeRight)
                animator.SetTrigger(Settings.hoeRight);
            if (hoeLeft)
                animator.SetTrigger(Settings.hoeLeft);
            if (hoeUp)
                animator.SetTrigger(Settings.hoeUp);
            if (hoeDown)
                animator.SetTrigger(Settings.hoeDown);

            if (idleRight)
                animator.SetTrigger(Settings.idleRight);
            if (idleLeft)
                animator.SetTrigger(Settings.idleLeft);
            if (idleUp)
                animator.SetTrigger(Settings.idleUp);
            if (idleDown)
                animator.SetTrigger(Settings.idleDown);
        }

        private void AnimationEventPlayFootstepSound()
        {

        }
    }
}
