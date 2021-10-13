using System.Collections.Generic;
using UnityEngine;

namespace HelperClasses
{
    public static class HelperMethods
    {
        public static bool GetComponentsAtBoxLocation<T>(out List<T> listComponentsAtPosition, Vector2 point,
            Vector2 size, float angle)
        {
            bool found = false;
            List<T> componentList=new List<T>();

            Collider2D[] collider2DdArray = Physics2D.OverlapBoxAll(point, size, angle);

            // Loop through all colliders to get an object of type T
            for (int i = 0; i < collider2DdArray.Length; i++)
            {
                T tComponent = collider2DdArray[i].gameObject.GetComponent<T>();
                if (tComponent != null)
                {
                    found = true;
                    componentList.Add(tComponent);
                } else
                {
                    tComponent = collider2DdArray[i].gameObject.GetComponentInChildren<T>();
                    if (tComponent != null)
                    {
                        found = true;
                        componentList.Add(tComponent);
                    }
                }
            }

            listComponentsAtPosition = componentList;
            return found;
        }
    }
}
