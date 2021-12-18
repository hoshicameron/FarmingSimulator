using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Lighting
{
    [CreateAssetMenu(fileName = "LightingSchedule" ,menuName ="Scriptable Objects/Lighting/Lighting Schedule")]
    public class LightingSchedule : ScriptableObject
    {
        public LightingBrightness[] lightingBrightnesses;
    }

    [System.Serializable]
    public struct LightingBrightness
    {
        public Season season;
        public int hour;
        public float lightIntensity;
    }
}