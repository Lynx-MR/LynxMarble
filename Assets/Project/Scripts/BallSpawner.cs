//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Spawn ball prefab every "delay" when not grabed.                           |
//   ==============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lynx.Marble
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private float delayBetweenSpawn = 1;
        [SerializeField] private GameObject ballPrefab;

        private bool coroutineIsRunning = false;

        //Bind Event to object grab
        private void Awake()
        {
            XRGrabInteractable XRGI = this.GetComponent<XRGrabInteractable>();
            XRGI.firstSelectEntered.AddListener(GrabStart);
            XRGI.lastSelectExited.AddListener(GrabEnd);
        }

        #region EDITOR DEBUG
#if UNITY_EDITOR
        private bool isGrabed = false;
        //usefull for debuging
        private void Update()
        {
            if (Selection.Contains(gameObject) && !isGrabed)
            {
                isGrabed = true;
                coroutineIsRunning = false;
            }
            if (!Selection.Contains(gameObject) && isGrabed)
            {
                isGrabed = false;
                StopCoroutine(SpawnBalls());
            }
        }
#endif
        #endregion

        /// <summary>
        /// Stop SpawnBalls coroutine when grabed
        /// </summary>
        /// <param name="arg"></param>
        public void GrabStart(SelectEnterEventArgs arg)
        {
            coroutineIsRunning = false;
        }

        /// <summary>
        /// Start SpawnBalls Coroutine when object is dropped
        /// </summary>
        /// <param name="arg"></param>
        public void GrabEnd(SelectExitEventArgs arg)
        {
            StartCoroutine(SpawnBalls());
        }

        /// <summary>
        /// Instantiate a new ball every "delayBetweenSpawn"
        /// </summary>
        /// <returns></returns>
        IEnumerator SpawnBalls()
        {
            if (!coroutineIsRunning)
            {
                coroutineIsRunning = true;
                while (coroutineIsRunning)
                {
                    GameObject ball = Instantiate(ballPrefab);
                    ball.transform.position = this.transform.position;
                    yield return new WaitForSeconds(delayBetweenSpawn);
                }
            }
        }
    }
}