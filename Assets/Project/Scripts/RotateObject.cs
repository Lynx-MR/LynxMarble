//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Constantly rotate an object in local axis.                                 |
//   ==============================================================================
using UnityEngine;

namespace Lynx.Marble
{
    public class RotateObject : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 90;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;

        private Transform trs;
        void Start()
        {
            trs = this.transform;
        }

        // Update is called once per frame
        void Update()
        {
            trs.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }
}