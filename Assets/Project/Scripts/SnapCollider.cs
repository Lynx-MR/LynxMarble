//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Used to make public event of objects trigger collider.                     |
//   ==============================================================================
using UnityEngine;
using UnityEngine.Events;


namespace Lynx.Marble
{
    [RequireComponent(typeof(Collider))]
    public class SnapCollider : MonoBehaviour
    {
        [HideInInspector]
        public UnityEvent<Collider> onTriggerEnter;
        [HideInInspector]
        public UnityEvent<Collider> onTriggerExit;

        void OnTriggerEnter(Collider col)
        {
            if (onTriggerEnter != null) onTriggerEnter.Invoke(col);
        }
        void OnTriggerExit(Collider col)
        {
            if (onTriggerExit != null) onTriggerExit.Invoke(col);
        }
    }
}