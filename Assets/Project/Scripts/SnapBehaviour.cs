//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Calculate object postion if snaping points are close enough.               |
//   ==============================================================================
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lynx.Marble
{
    [RequireComponent(typeof(Collider))]
    public class SnapBehaviour : MonoBehaviour
    {
        #region INSPECTOR PROPERTYS

        [SerializeField] private Transform[] snapPoints;
        [SerializeField] private Transform platform;
        [SerializeField] private SnapCollider snapTrigger;
        [SerializeField] private float snapDistance = 0.05f;

        #endregion

        #region SCRIPT VARIABLES

        private Transform[] OtherSnapPoints;
        private bool isGrabed = false;
        private bool isInCollision = false;
        private Vector3 basePlatformPos;

        #endregion

        private void Start()
        {
            //bind function to differents events
            XRGrabInteractable xrgi = this.GetComponent<XRGrabInteractable>();
            xrgi.firstSelectEntered.AddListener(StartGrab);
            xrgi.lastSelectExited.AddListener(EndGrab);
            snapTrigger.onTriggerEnter.AddListener(TriggerEnter);
            snapTrigger.onTriggerExit.AddListener(TriggerExit);

            //save platform localPositon for later use
            basePlatformPos = platform.localPosition;
        }


#if UNITY_EDITOR
        //usefull for debuging
        private void Update()
        {
            if (!isGrabed && Selection.Contains(gameObject) != isGrabed)
                StartGrab(new SelectEnterEventArgs());
            isGrabed = Selection.Contains(gameObject);
        }
#endif

        #region BINDED FUNCTION

        /// <summary>
        /// Save collided object snap points and start Snap coroutine if object is currently dragged
        /// </summary>
        /// <param name="collision"></param>
        private void TriggerEnter(Collider collision)
        {
            if (collision.gameObject.GetComponent<SnapBehaviour>())
            {
                OtherSnapPoints = collision.gameObject.GetComponent<SnapBehaviour>().snapPoints;
                isInCollision = true;
                if (isGrabed)
                    StartCoroutine(Snap());
            }
        }

        /// <summary>
        /// Tell Snap coroutine that collision ended
        /// </summary>
        /// <param name="collision"></param>
        private void TriggerExit(Collider collision)
        {
            if (collision.gameObject.GetComponent<SnapBehaviour>())
            {
                isInCollision = false;
            }
        }

        /// <summary>
        /// Start coroutine if curently in collision && set grabed state
        /// </summary>
        /// <param name="arg"></param>
        private void StartGrab(SelectEnterEventArgs arg)
        {
            isGrabed = true;
            if (isInCollision)
                StartCoroutine(Snap());

        }

        /// <summary>
        /// Set grabed state
        /// </summary>
        /// <param name="arg"></param>
        private void EndGrab(SelectExitEventArgs arg)
        {
            isGrabed = false;
        }

        #endregion

        #region COROUTINE

        /// <summary>
        /// Check distance betweens snappoints and snap points distance is close enough
        /// </summary>
        /// <returns></returns>
        private IEnumerator Snap()
        {
            Vector3 offset = Vector3.zero;
            while (isGrabed && isInCollision)
            {
                //reset platform pos for snaping calculation
                platform.localPosition = basePlatformPos;
                //get points index for closest points from one anothers
                float closestDistance = float.PositiveInfinity;
                int iIDX = -1;
                int jIDX = -1;
                for (int i = 0; i < snapPoints.Length; ++i)
                {
                    for (int j = 0; j < OtherSnapPoints.Length; ++j)
                    {
                        float curDist = Vector3.Distance(snapPoints[i].position, OtherSnapPoints[j].position);
                        if (curDist < closestDistance)
                        {
                            iIDX = i;
                            jIDX = j;
                            closestDistance = curDist;
                        }
                    }
                }

                //if close enough snap objects together
                if (closestDistance < snapDistance)
                {
                    offset = OtherSnapPoints[jIDX].position - snapPoints[iIDX].position;
                    platform.position += offset;
                }
                yield return new WaitForFixedUpdate();
            }

            //if coroutine end by droping the object, offset the parent to recenter colliders
            if (!isGrabed)
                this.transform.position += offset;
            platform.localPosition = basePlatformPos;
        }

        #endregion

        #region PUBLIC METHODES

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return a list of all snapPoints transform</returns>
        public Transform[] getSnapPoints()
        {
            return snapPoints;
        }

        #endregion
    }
}