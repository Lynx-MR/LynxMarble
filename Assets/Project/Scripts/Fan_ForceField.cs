//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Add force to object entering inside trigger box.                           |
//   | Force will be proportional to distance to trigger origine.                 |
//   ==============================================================================
using UnityEngine;

namespace Lynx.Marble
{
    public class Fan_ForceField : MonoBehaviour
    {
        [SerializeField] private float m_Thrust = 1f;

        [SerializeField] private float falloffDistance = 0.5f;

        void OnTriggerStay(Collider other)
        {
            if (other.attachedRigidbody)
            {
                float strenghMult = Mathf.Lerp(1, 0, Mathf.Clamp01(Vector3.Distance(other.transform.position, transform.position)));
                if (falloffDistance <= 0)
                    strenghMult = 1;
                other.attachedRigidbody.AddForce(transform.forward * m_Thrust * strenghMult);
            }
        }
    }
}