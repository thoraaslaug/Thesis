using UnityEngine;
using MalbersAnimations.Scriptables;  // For Mount Manager

namespace MalbersAnimations.HAP
{
    public class ForceDismountTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            var rider = other.GetComponentInParent<MRider>();

            if (rider != null && rider.Mounted)
            {
                rider.Start_Dismounting(); // This begins the dismount process
            }
        }
    }
}