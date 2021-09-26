using System.Collections;
using Misc;
using UnityEngine;

namespace Items
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObscuringItemFader : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FadeOut()
        {
            // Start a coroutine to fade out the sprite
            StartCoroutine(FadeOutRoutine());
        }

        private IEnumerator FadeOutRoutine()
        {
            float currentAlpha = spriteRenderer.color.a;
            float alphaDistance = currentAlpha - Settings.targetAlpha;

            while (currentAlpha-Settings.targetAlpha>0.01f)
            {
                currentAlpha -= (alphaDistance / Settings.fadeOutSeconds)*Time.deltaTime;
                spriteRenderer.color=new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b,
                    currentAlpha);
                yield return null;
            }

            spriteRenderer.color=new Color(spriteRenderer.color.r,spriteRenderer.color.g,
                spriteRenderer.color.b,Settings.targetAlpha);
        }

        public void FadeIn()
        {
            StartCoroutine(FadeInRoutine());
        }

        private IEnumerator FadeInRoutine()
        {
            float currentAlpha = spriteRenderer.color.a;
            float alphaDistance = 1f - currentAlpha;

            while (1f-currentAlpha>0.01f)
            {
                currentAlpha += (alphaDistance / Settings.fadeOutSeconds)*Time.deltaTime;
                spriteRenderer.color=new Color(spriteRenderer.color.r,spriteRenderer.color.g,spriteRenderer.color.b,
                    currentAlpha);
                yield return null;
            }

            spriteRenderer.color=new Color(spriteRenderer.color.r,spriteRenderer.color.g,
                spriteRenderer.color.b,1f);
        }
    }
}
