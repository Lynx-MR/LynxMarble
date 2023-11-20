//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Handle item on table and spawn of new one.                                 |
//   ==============================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Lynx.Marble
{
    public class Spawner : MonoBehaviour
    {
        #region EDITOR PROPERTY
        public GameObject item;
        public float rotateSpeed = 20;
        public float itemRestScale = 0.25f;
        [SerializeField] private float itemHeight = 0.1f;
        [SerializeField] private float spawnSpeed = 0.5f;
        [SerializeField] private LynxMath.easingType easing = LynxMath.easingType.SinInOut;

        #endregion


        #region SCRIPT VARIABLES

        private GameObject curItem;
        private XRGrabInteractable curXRG;

        #endregion

        //Init
        private void Start()
        {
            MakeNewItem();
            StartCoroutine(SpawnAnimation());
            StartCoroutine(RotateCurItem());
        }

        /// <summary>
        /// Create a new item parented to spawner and start spawn coroutine
        /// </summary>
        private void MakeNewItem()
        {
            curItem = Instantiate(item, this.transform);
            curItem.transform.localScale = Vector3.zero;
            curItem.transform.localPosition = new Vector3(0, 0, 0);

            curXRG = curItem.GetComponent<XRGrabInteractable>();
            curXRG.selectEntered.AddListener(ItemSelected);
        }

        /// <summary>
        /// Function called when item is selected that create a new one
        /// </summary>
        /// <param name="arg"></param>
        public void ItemSelected(SelectEnterEventArgs arg)
        {
            StartCoroutine(ScaleSelectedItem(curItem));
            curXRG.selectEntered.RemoveListener(ItemSelected);
            MakeNewItem();
            StartCoroutine(SpawnAnimation());
        }

        /// <summary>
        /// Spawn animation from pos and scale 0 to target pos and scale
        /// </summary>
        /// <returns></returns>
        private IEnumerator SpawnAnimation()
        {
            for (float t = 0; t < 1; t += Time.deltaTime / spawnSpeed)
            {
                float f = LynxMath.Ease(t, easing);
                curItem.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * itemRestScale, f);
                curItem.transform.localPosition = Vector3.Lerp(Vector3.zero, new Vector3(0, itemHeight, 0), f);

                yield return new WaitForEndOfFrame();
            }
            curItem.transform.localScale = Vector3.one * itemRestScale;
            curItem.transform.localPosition = new Vector3(0, itemHeight, 0);
        }

        /// <summary>
        /// Rotate item in spawner
        /// </summary>
        /// <returns></returns>
        private IEnumerator RotateCurItem()
        {
            while (true)
            {
                curItem.transform.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Scale <paramref name="obj"/> when grabbed to a scale of 1
        /// </summary>
        /// <param name="obj">Object to scale</param>
        /// <returns></returns>
        private IEnumerator ScaleSelectedItem(GameObject obj)
        {
            Vector3 baseScale = obj.transform.lossyScale;
            for (float t = 0; t < 1; t += Time.deltaTime / spawnSpeed)
            {
                float f = LynxMath.Ease(t, easing);
                obj.transform.localScale = Vector3.Lerp(baseScale, Vector3.one, f);

                yield return new WaitForEndOfFrame();
            }
            obj.transform.localScale = Vector3.one;
        }
    }
}