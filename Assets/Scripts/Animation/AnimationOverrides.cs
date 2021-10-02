using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{

    [SerializeField] private GameObject character = null;//Player game objects and other NPCs
    [SerializeField] private SO_AnimationType[] soAnimationTypeArray;//All animator  Scriptable objects

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;

    private void Start()
    {
        // Initialize animation type dictionary keyed by animation clip
        animationTypeDictionaryByAnimation=new Dictionary<AnimationClip, SO_AnimationType>();

        // Populate first dictionary with scriptable objects
        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip,item);
        }

        // Initialize animation type dictionary keyed by string
        animationTypeDictionaryByCompositeAttributeKey=new Dictionary<string, SO_AnimationType>();

        foreach (SO_AnimationType item in soAnimationTypeArray)
        {
            string key = item.characterPart.ToString() + item.partVariantColour.ToString() +
                         item.partVariantType.ToString() +item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key,item);
        }
    }
    /// <summary>
    /// Get character attribute list and swap animation based on it
    /// </summary>
    /// <param name="characterAttributeList"></param>
    public void ApplyCharacterCustomizationParameters(List<CharacterAttribute> characterAttributeList)
    {
        // StopWatch s1 =StopWatch.StartNew()

        // Loop through all character attributes and set the animation override controller for each
        foreach (CharacterAttribute characterAttribute in characterAttributeList)
        {
            Animator currentAnimator = null;
            // populate the list with current animation clip an the animation that we want to swap
            List<KeyValuePair<AnimationClip,AnimationClip>> animationKeyValuePairList=new List<KeyValuePair<AnimationClip, AnimationClip>>();

            // set the animator name that we want to locate by character Attribute's character part name
            string animatorSOAssetName = characterAttribute.characterPart.ToString();

            // Find animator in scene that match the scriptable object animator type
            Animator[] animatorsArray = character.GetComponentsInChildren<Animator>();

            foreach (Animator animator in animatorsArray)
            {
                if (animator.name == animatorSOAssetName)
                {
                    currentAnimator = animator; // Current animator that we want to apply overrides to
                    break;
                }
            }

            // Get base current animation for animator
            if (currentAnimator != null)
            {
                AnimatorOverrideController aoc =new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
                // List of all animation clip within the animator override controller
                List<AnimationClip> animationsList =new List<AnimationClip>(aoc.animationClips);

                foreach (AnimationClip animationClip in animationsList)
                {
                    // Find animation in directory in the first dictionary
                    bool foundAnimation =
                        animationTypeDictionaryByAnimation.TryGetValue(animationClip, out var so_AnimationType);

                    if (foundAnimation)
                    {
                        string key = characterAttribute.characterPart.ToString() +
                                     characterAttribute.partVariantColour.ToString()+characterAttribute.partVariantType.ToString()+
                                     so_AnimationType.animationName.ToString();

                        bool foundSwapAnimation =
                            animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key,out var swapSoAnimationType);

                        if (foundSwapAnimation)
                        {
                            AnimationClip swapAnimationClip = swapSoAnimationType.animationClip;
                            animationKeyValuePairList.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip,swapAnimationClip));
                        }
                    }
                }

                // Apply animation update to animations override controller and update animator with the new controller
                aoc.ApplyOverrides(animationKeyValuePairList);
                currentAnimator.runtimeAnimatorController = aoc;
            }
        }

        // s1.stop();
        // Debug.Log("Time to apply character customization : "+ s1.Elapsed+ "  elapsed seconds);

    }
}
