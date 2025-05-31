using UnityEngine;

namespace RHP.BackButtonManager
{
    /// <summary>
    /// Handles automatic pausing of the game when the application is minimized or loses focus on mobile devices.
    /// This script should be attached to a GameObject that has a 'BackButtonActionHandler' component.
    /// </summary>
    /// <remarks>
    /// - The 'BackButtonActionHandler' component must be present on the same GameObject.
    /// - Editor-only pause simulation provided for testing purposes.
    ///   Configure or disable the key via Inspector to avoid conflicts.
    /// </remarks>
    public class ApplicationPauseControl : MonoBehaviour
    {
        BackButtonActionHandler pauseButtonHandler;

        void Awake()
        {
            pauseButtonHandler = GetComponent<BackButtonActionHandler>();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseButtonHandler == null)
                return;

            if (pauseStatus && pauseButtonHandler.IsRegistered)
            {
                pauseButtonHandler.BackButtonPress();
            }
        }

#if UNITY_EDITOR
        private bool isPaused;

        [Header("Editor Test Settings")]
        [Tooltip("Keyboard shortcut to simulate pause/resume in Unity Editor.")]
        public KeyCode pauseSimulationKey = KeyCode.P;

        void Update()
        {
            if (Input.GetKeyDown(pauseSimulationKey))
            {
                isPaused = !isPaused;
                OnApplicationPause(isPaused);
            }
        }
#endif
    }
}