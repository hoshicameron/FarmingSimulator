
using System;
using UnityEngine;

namespace SaveSystem
{
    [ExecuteAlways]
    public class GenerateGUID:MonoBehaviour
    {
        [SerializeField] private string _gUID=string.Empty;

        public string GUid
        {
            get => _gUID;
            set => _gUID = value;
        }

        private void Awake()
        {
            // Only populate in the editor
            if (!Application.IsPlaying(gameObject))
            {
                if (_gUID == string.Empty)
                {
                    // Assign GUID- it's generate random unique 16 string
                    _gUID = System.Guid.NewGuid().ToString();
                }
            }
        }
    }
}
