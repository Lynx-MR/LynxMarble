//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | add force to rigidboday on layer 9 (ball) entering the trigger.            |
//   ==============================================================================
using UnityEngine;

namespace Lynx.Marble
{
    public class Booster_Behaviour : MonoBehaviour
    {
        [SerializeField] private float boosterStrengh = 1;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer == 9)
            {
                other.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * boosterStrengh, ForceMode.Acceleration);
            }
        }
    }
}