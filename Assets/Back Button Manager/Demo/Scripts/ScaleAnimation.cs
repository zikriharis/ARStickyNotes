using UnityEngine;
using System.Collections;

namespace RHP.BackButtonManager.Demo
{
    public class ScaleAnimation : MonoBehaviour
    {
        public float animationDuration = 0.15f;
        private Vector3 targetScale = Vector3.one;

        void Awake()
        {
            targetScale = transform.localScale;
        }

        void OnEnable()
        {
            transform.localScale = Vector3.zero;
            StartCoroutine(ScaleMenu(Vector3.zero, targetScale, animationDuration));
        }

        IEnumerator ScaleMenu(Vector3 start, Vector3 end, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                transform.localScale = Vector3.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localScale = end;
        }
    }
}