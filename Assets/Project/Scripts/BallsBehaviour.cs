//   ==============================================================================
//   | Lynx Mixed Reality                                                         |
//   |======================================                                      |
//   | Lynx Marble                                                                |
//   | Check every second if ball Y position is less than -10 to destry it.       |
//   ==============================================================================
using System.Collections;
using UnityEngine;

namespace Lynx.Marble
{
    public class BallsBehaviour : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(CheckBallHeight());
        }

        /// <summary>
        /// Destroy ball if height is inferior to -10
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckBallHeight()
        {
            while (true)
            {
                if (this.transform.position.y < -10)
                    Destroy(this.gameObject);
                yield return new WaitForSeconds(1);
            }
        }
    }
}