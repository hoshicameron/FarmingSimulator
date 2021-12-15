using System.Collections.Generic;
using UnityEngine;

namespace Sounds
{
    [CreateAssetMenu(fileName = "so_SoundList" , menuName = "Scriptable Objects/Sounds/Sound List")]
    public class SO_SoundList : ScriptableObject
    {
        [SerializeField] public List<SoundItem> soundDetails;
    }
}
