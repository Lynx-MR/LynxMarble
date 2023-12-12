//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Handle table for spawning item and associated spawners.                    |
//   ==============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit;

namespace Lynx.Marble
{
    public class TableManager : MonoBehaviour
    {
        #region INSPECTOR PROPERTY
        [System.Serializable]
        public struct cItem
        {
            public GameObject item;
            public float baseScale;
        }


        [Tooltip("GameObject that will move the table")]
        [SerializeField] private Transform tableMover;
        [Tooltip("GameObject that will rotate the table on Y axis")]
        [SerializeField] private Transform tableRotator;
        [Tooltip("Rotation multiplicator from angulat movement")]
        [SerializeField] private float RotationFactor = 3;
        [Tooltip("Drag factor of angular velocity")]
        [SerializeField] private float RotationDrag = 0.33f;

        [Space]
        [Tooltip("Base spawner that will manage spawnable items")]
        [SerializeField] private GameObject itemSpawners;
        [Tooltip("Item that will be displayed on the table")]
        [SerializeField] private cItem[] items;
        [Tooltip("Distance from the center for spawner objects")]
        [SerializeField] private float tableRadius = 0.15f;

        #endregion

        #region SCRIPT VARIABLES
        private Transform interactor;
        private bool isRotating = false;

        #endregion

        // Bind the tableRotation function to XRSI
        private void Awake()
        {
            XRSimpleInteractable RotatorXrsi = tableRotator.GetComponent<XRSimpleInteractable>();
            RotatorXrsi.firstSelectEntered.AddListener(StartTableRotation);
            RotatorXrsi.lastSelectExited.AddListener(StopTableRotation);
            LynxHandtrackingAPI.LeftHandDynamicUpdate += LeftHandUpdate;
            LynxHandtrackingAPI.LeftHandDynamicUpdate += RightHandUpdate;

            for(int i = 0; i< items.Length; ++i)
            {
                GameObject spawner = Instantiate(itemSpawners, tableRotator);
                spawner.GetComponent<Spawner>().item = items[i].item;
                spawner.GetComponent<Spawner>().itemRestScale = items[i].baseScale;
                spawner.transform.localPosition = Quaternion.AngleAxis((360 / items.Length) * i, Vector3.up) * new Vector3(tableRadius, 0, 0);
            }
            RecenterTable();
        }

        #region PRIVATE METHODES
        private void LeftHandUpdate()
        {
            XRHand hand = LynxHandtrackingAPI.LeftHand;
            if(hand.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPose))
            {
                LineRenderer rend = GetComponent<LineRenderer>();
            }
        }

        private void RightHandUpdate()
        {

        }
        #endregion

        #region PUBLIC METHODES

        /// <summary>
        /// Save interactor and start coroutine calculating rotation
        /// </summary>
        /// <param name="arg"></param>
        public void StartTableRotation(SelectEnterEventArgs arg)
        {
            interactor = arg.interactorObject.transform;
            isRotating = true;
            StartCoroutine(CalculateRotation());
        }

        /// <summary>
        /// stop coroutine calculating rotation
        /// </summary>
        /// <param name="arg"></param>
        public void StopTableRotation(SelectExitEventArgs arg)
        {
            isRotating = false;
        }

        /// <summary>
        /// Place table in front of user
        /// </summary>
        public void RecenterTable()
        {
            tableMover.position = Camera.main.transform.position + Camera.main.transform.forward * 0.5f;
        }

        #endregion

        #region COROUTINES

        /// <summary>
        /// Calculate rotation offset with the angle between starting pinch pos and current pinch pos
        /// </summary>
        /// <returns></returns>
        private IEnumerator CalculateRotation()
        {
            Quaternion baseRotation = tableRotator.rotation;
            Vector3 baseInteractorPos = interactor.position - tableRotator.position;
            float oldAngle = 0;
            float angleVelocity = 0;
            
            while (isRotating)
            {
                Vector3 interactorPos = interactor.position - tableRotator.position;
                float angle = Vector3.SignedAngle(baseInteractorPos, interactorPos, Vector3.up) * RotationFactor;
                angleVelocity = (angle - oldAngle)/Time.deltaTime;
                oldAngle = angle;
                Quaternion offset = Quaternion.AngleAxis(angle, Vector3.up);

                tableRotator.rotation = baseRotation * offset;
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine(RotationVelocity(angleVelocity));
        }

        /// <summary>
        /// Once Rotation by user ended, simulate rotation velocity 
        /// </summary>
        /// <param name="baseVelocity">Rotation Velocity at the end of the user rotating</param>
        /// <returns></returns>
        private IEnumerator RotationVelocity(float baseVelocity)
        {
            Quaternion baseRotation = tableRotator.rotation;
            float angleVelocity = baseVelocity;
            float angleOffset = 0;
            float timeToStop = Mathf.Abs(angleVelocity) / RotationDrag;
            float t = 0;

            while (!isRotating && t<=1)
            {
                angleOffset += angleVelocity * Time.deltaTime;
                angleVelocity = Mathf.Lerp(angleVelocity, 0, t);

                t += Time.deltaTime / timeToStop;

                Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);
                tableRotator.rotation = baseRotation * offset;

                yield return new WaitForEndOfFrame();
            }
        }
        #endregion
    }
}