using UnityEngine;

namespace Items
{
    public class TriggerObscuringItemFader : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Get the gameobject we have collided with, and then get all the obscuring item fader components
            // on it and it children - and then trigger the fade out

            ObscuringItemFader[] obscuringItemFaderArray =
                other.gameObject.GetComponentsInChildren<ObscuringItemFader>();
            if (obscuringItemFaderArray.Length > 0)
            {
                for (int i = 0; i < obscuringItemFaderArray.Length; i++)
                {
                    obscuringItemFaderArray[i].FadeOut();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            // Get the gameobject we have collided with, and then get all the obscuring item fader components
            // on it and it children - and then trigger the fade in

            ObscuringItemFader[] obscuringItemFaderArray =
                other.gameObject.GetComponentsInChildren<ObscuringItemFader>();
            if (obscuringItemFaderArray.Length > 0)
            {
                for (int i = 0; i < obscuringItemFaderArray.Length; i++)
                {
                    obscuringItemFaderArray[i].FadeIn();
                }
            }
        }
    }
}
