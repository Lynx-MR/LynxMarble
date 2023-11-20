//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Used to delete objects when near the trashcan.                             |
//   ==============================================================================
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Lynx.Marble
{
    public class DeleteInTrash : MonoBehaviour
    {
        [SerializeField] private MeshRenderer rend;

        private bool isGrabed = false;
        private bool isInCollision = false;

        private void Start()
        {
            //bind function to differents events
            XRGrabInteractable xrgi = this.GetComponent<XRGrabInteractable>();
            xrgi.firstSelectEntered.AddListener(StartGrab);
            xrgi.lastSelectExited.AddListener(EndGrab);
        }


        #region BINDED FUNCTION
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.tag == "TrashBox")
                isInCollision = true;
        }
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "TrashBox")
                isInCollision = false;
        }

        /// <summary>
        /// Start coroutine if curently in collision && set grabed state
        /// </summary>
        /// <param name="arg"></param>
        private void StartGrab(SelectEnterEventArgs arg)
        {
            isGrabed = true;
            StartCoroutine(CheckTrashable());

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


        /// <summary>
        /// Check delete distance and color object in red if clsoe enough to be deleted
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckTrashable()
        {
            // save original colors
            Vector4[] baseColors = new Vector4[rend.materials.Length];
            for (int i = 0; i < rend.materials.Length; i++)
            {
                baseColors[i] = rend.materials[i].GetColor("_FresnelColor");
            }

            while (isGrabed)
            {
                //set to red if clsoe enough, else reset base colors
                if (isInCollision)
                {
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        rend.materials[i].SetColor("_FresnelColor", Color.red);
                    }
                }
                else
                {
                    for (int i = 0; i < rend.materials.Length; i++)
                    {
                        rend.materials[i].SetColor("_FresnelColor", baseColors[i]);
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            if (isInCollision)
                Destroy(this.gameObject);

            for (int i = 0; i < rend.materials.Length; i++)
            {
                rend.materials[i].SetColor("_FresnelColor", baseColors[i]);
            }
        }
    }
}