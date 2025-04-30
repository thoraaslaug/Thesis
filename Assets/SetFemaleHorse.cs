using MalbersAnimations.Controller;
using MalbersAnimations.HAP;
using UnityEngine;

namespace MalbersAnimations.HAP
{
    public class SetFemaleHorse : MonoBehaviour
    {

        public MRider maleRider;
        public MRider femaleRider;
        public Mount horse;
        public Transform frontMountPoint;
        public Transform backMountPoint;

        void Start()
        {
            if (maleRider && frontMountPoint && horse)
            {
                maleRider.Montura = horse;
                maleRider.Montura.MountPoint = frontMountPoint;
                maleRider.MountAnimal();
            }

            if (femaleRider && backMountPoint && horse)
            {
                femaleRider.Montura = horse;
                femaleRider.Montura.MountPoint = backMountPoint;
                femaleRider.MountAnimal();
            }
        }
    }
}
