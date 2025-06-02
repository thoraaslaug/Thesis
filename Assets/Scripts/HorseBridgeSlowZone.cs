using UnityEngine;
using MalbersAnimations.Controller;

public class HorseBridgeSlowZone : MonoBehaviour
{
    [Tooltip("Speed Set to change (usually 1 = Ground)")]
    public int speedSetIndex = 1;

    [Tooltip("Speed index to use (0 = walk, 1 = trot, etc.)")]
    public int slowSpeedIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        var animal = other.GetComponentInParent<MAnimal>();
        if (animal != null && animal.speedSets.Count > speedSetIndex)
        {
            var set = animal.speedSets[speedSetIndex];

            if (set != null && slowSpeedIndex < set.Speeds.Count)
            {
                animal.SetCustomSpeed(set.Speeds[slowSpeedIndex]);
                Debug.Log($"🐎 Set horse to slow speed: {set.Speeds[slowSpeedIndex].name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var animal = other.GetComponentInParent<MAnimal>();
        if (animal != null && animal.speedSets.Count > speedSetIndex)
        {
            var set = animal.speedSets[speedSetIndex];
            int defaultIndex = 1; // e.g. trot or normal speed

            if (set != null && defaultIndex < set.Speeds.Count)
            {
                animal.SetCustomSpeed(set.Speeds[defaultIndex]);
                Debug.Log($"🐎 Horse back to normal speed: {set.Speeds[defaultIndex].name}");
            }
        }
    }
}