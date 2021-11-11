using System.Collections.Generic;
using UnityEngine;

namespace Crops
{
    [CreateAssetMenu(fileName = "CropDetailsList",menuName="Scriptable Objects/Crop/Crop Details List")]
    public class SO_CropDetailsList : ScriptableObject
    {
        [SerializeField] private List<CropDetails> cropDetailsList;

        public CropDetails GetCropDetails(int seedItemCode)
        {
            return cropDetailsList.Find(x => x.seedItemCode == seedItemCode);
        }
    }
}
