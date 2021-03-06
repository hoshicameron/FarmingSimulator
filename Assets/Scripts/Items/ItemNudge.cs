using System.Collections;
using Enums;
using Sounds;
using UnityEngine;

namespace Items
{
    public class ItemNudge : MonoBehaviour
    {
        // the pause we have between the rotation
        private WaitForSeconds pause;
        private bool isAnimating = false;

        private void Awake()
        {
            pause=new WaitForSeconds(0.04f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isAnimating)
            {
                if (transform.position.x < other.transform.position.x)
                {
                    StartCoroutine(RotateAntiClock());
                } else
                {
                    StartCoroutine(RotateClock());
                }

                // Play Rustle sound if player
                if (other.CompareTag("Player"))
                {
                    AudioManager.Instance.PlaySound(SoundName.EffectRustle);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!isAnimating)
            {
                if (transform.position.x > other.transform.position.x)
                {
                    StartCoroutine(RotateAntiClock());
                } else
                {
                    StartCoroutine(RotateClock());
                }

                // Play Rustle sound if player
                if (other.CompareTag("Player"))
                {
                    AudioManager.Instance.PlaySound(SoundName.EffectRustle);
                }
            }
        }

        private IEnumerator RotateClock()
        {
            isAnimating = true;
            for (int i = 0; i < 4; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0f,0f,-2f);
                yield return pause;
            }
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0f,0f,2f);
                yield return pause;
            }
            gameObject.transform.GetChild(0).Rotate(0f,0f,-2f);
            yield return pause;

            isAnimating = false;
        }

        private IEnumerator RotateAntiClock()
        {
            isAnimating = true;
            for (int i = 0; i < 4; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0f,0f,2f);
                yield return pause;
            }
            for (int i = 0; i < 5; i++)
            {
                gameObject.transform.GetChild(0).Rotate(0f,0f,-2f);
                yield return pause;
            }
            gameObject.transform.GetChild(0).Rotate(0f,0f,2f);
            yield return pause;

            isAnimating = false;
        }
    }
}
