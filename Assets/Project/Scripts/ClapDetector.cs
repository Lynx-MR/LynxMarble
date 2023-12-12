//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Call an Event on clap                                                      |
//   ==============================================================================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;

namespace Lynx
{
    [Serializable]
    public class Event : UnityEvent { }

    public class ClapDetector : MonoBehaviour
    {
        #region SCRIPT PROPERTIES

        public Event OnSingleClap;
        public Event OnDoubleClap;
        public Event OnTripleClap;
        [SerializeField] private float clapThreshold = 0.1f;

        #endregion

        #region SCRIPT VARIABLES

        private Vector3 leftPalm = Vector3.negativeInfinity;
        private Vector3 rightPalm = Vector3.positiveInfinity;

        private bool handsAreClose = false;
        private bool detectorIsRunning = false;

        #endregion

        private void Start()
        {
            LynxHandtrackingAPI.HandSubsystem.updatedHands += OnHandUpdate;
        }


        //Save Hands position and call checkClapState
        void OnHandUpdate(XRHandSubsystem subsystem,
                  XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
                  XRHandSubsystem.UpdateType updateType)
        {
            if(updateType == XRHandSubsystem.UpdateType.Dynamic)
            {
                XRHand handL = LynxHandtrackingAPI.LeftHand;
                if (handL.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPoseL))
                {
                    leftPalm = palmPoseL.position;
                }
                XRHand handR = LynxHandtrackingAPI.RightHand;
                if (handR.GetJoint(XRHandJointID.Palm).TryGetPose(out Pose palmPoseR))
                {
                    rightPalm = palmPoseR.position;
                }

                CheckClapState();
            }
        }

        /// <summary>
        /// Update state of hands (closed od spread) 
        /// If they are close, start clapDetection Coroutine
        /// </summary>
        private void CheckClapState()
        {
            if (Vector3.Distance(leftPalm, rightPalm) < clapThreshold)
                handsAreClose = true;
            else
                handsAreClose = false;


            if (!detectorIsRunning && handsAreClose)
            {
                detectorIsRunning = true;
                StartCoroutine(ClapDetection());
            }
        }

        /// <summary>
        /// Save the number of clap and call coresponding event
        /// </summary>
        /// <returns></returns>
        IEnumerator ClapDetection()
        {
            float t = 0;
            int nbrOfTap = 0;
            bool handsPastState = handsAreClose;

            while (t < 0.5)
            {
                if(handsAreClose != handsPastState)
                {
                    t = 0;
                    if (!handsAreClose && nbrOfTap == 0)
                        ++nbrOfTap;
                    else if (handsAreClose)
                        ++nbrOfTap;

                }
                handsPastState = handsAreClose;
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            if (nbrOfTap == 1)
                OnSingleClap.Invoke();
            else if (nbrOfTap == 2)
                OnDoubleClap.Invoke();
            else if (nbrOfTap == 3)
                OnTripleClap.Invoke();
            detectorIsRunning = false;
        }
    }
}