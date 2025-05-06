using UnityEngine;

namespace GlobalSnowEffect {
    public class SnowTintController : MonoBehaviour
    {
        public GlobalSnow globalSnow; // Assign in the Inspector

        private Color originalTint;

        void Start()
        {
            if (globalSnow != null)
            {
                originalTint = globalSnow.snowTint;
            }
        }

        public void DarkenSnow()
        {
            if (globalSnow != null)
            {
                // Set a darker blue-gray tint to make it moodier
                globalSnow.snowTint = new Color(0.2f, 0.2f, 0.25f);
            }
        }

        public void ResetSnowTint()
        {
            if (globalSnow != null)
            {
                globalSnow.snowTint = originalTint;
            }
        }
    }
}