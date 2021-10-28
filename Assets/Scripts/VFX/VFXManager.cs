using System.Collections;
using Enums;
using Misc;
using UnityEngine;
using EventHandler = Events.EventHandler;

namespace VFX
{
    public class VFXManager : SingletonMonoBehaviour<VFXManager>
    {
        private WaitForSeconds twoSeconds;
        [SerializeField] private GameObject reapingPrefab = null;

        protected override void Awake()
        {
            base.Awake();
            twoSeconds=new WaitForSeconds(2.0f);
        }

        private void OnDisable()
        {
            EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
        }

        private void OnEnable()
        {
            EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
        }

        private IEnumerator DisableHarvestActionEffect(GameObject effectGameObject, WaitForSeconds secondsToWait)
        {
            yield return secondsToWait;
            effectGameObject.SetActive(false);
        }

        private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
        {
            switch (harvestActionEffect)
            {
                case HarvestActionEffect.DeciduousLeavesFalling:
                    break;
                case HarvestActionEffect.PineConesFalling:
                    break;
                case HarvestActionEffect.ChoppingTreeTrunk:
                    break;
                case HarvestActionEffect.BreakingStone:
                    break;
                case HarvestActionEffect.Reaping:
                    GameObject reaping =
                        PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                    reaping.SetActive(true);
                    StartCoroutine(DisableHarvestActionEffect(reaping, twoSeconds));
                    break;
                case HarvestActionEffect.None:
                    break;
                default:
                    break;
            }
        }
    }
}
