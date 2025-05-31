using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RHP.BackButtonManager
{
    /// <summary>
    /// Controls back button input and manages registered back button action handlers.
    /// </summary>
    public class BackButtonController : MonoBehaviour
    {
        private static BackButtonController _instance;
        public static BackButtonController Instance => _instance;

        private Stack<BackButtonActionHandler> handlerStack = new();
        private bool isQuit;
        private bool isWait;

        #region BackButtonViewer Fields

        public bool IsQuit => isQuit;
        public bool IsWait => isWait;
        public BackButtonActionHandler[] GetRegisteredHandlers() => handlerStack.ToArray();
        public event Action OnStatusChanged;
        public event Action OnEscapePressed;

        #endregion

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

#if UNITY_EDITOR || UNITY_ANDROID
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ProcessBackButtonInput();
            }
        }
#endif

        private void ProcessBackButtonInput()
        {
#if UNITY_EDITOR
            OnEscapePressed?.Invoke();
#endif

            if (isWait)
                return;

            if (handlerStack.Count == 0)
            {
                if (isQuit)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }

                return;
            }

            var backButtonActionHandler = handlerStack.Peek();
            backButtonActionHandler.BackButtonPress();
        }

        /// <summary>
        /// Registers a back button action handler.
        /// </summary>
        /// <param name="handler">The handler to register.</param>
        public void RegisterHandler(BackButtonActionHandler handler)
        {
            Push(handler);
            OnStatusChanged?.Invoke();
        }

        private void Push(BackButtonActionHandler handler)
        {
            handlerStack.Push(handler);
        }

        /// <summary>
        /// Unregisters back button action handlers.
        /// </summary>
        /// <param name="isClearAllHandlers">If true, all handlers are unregistered; otherwise, only the most recent is removed.</param>
        public void UnregisterHandler(bool isClearAllHandlers)
        {
            if (handlerStack.Count > 0)
            {
                if (isClearAllHandlers)
                {
                    handlerStack.Clear();
                    SetWait(false);
                }
                else
                {
                    handlerStack.Pop();
                }

                OnStatusChanged?.Invoke();
            }
        }

        /// <summary>
        /// Unregisters a specific back button action handler.
        /// </summary>
        /// <param name="handler">The handler to unregister.</param>
        public void UnregisterHandler(BackButtonActionHandler handler)
        {
            var tempList = handlerStack.ToList();
            tempList.Remove(handler);
            handlerStack = new Stack<BackButtonActionHandler>(tempList);
            OnStatusChanged?.Invoke();
        }

        /// <summary>
        /// Sets the quit flag.
        /// </summary>
        /// <param name="isQuit">If true, quitting is enabled.</param>
        public void SetQuit(bool isQuit)
        {
            this.isQuit = isQuit;
            OnStatusChanged?.Invoke();
        }

        /// <summary>
        /// Sets the wait flag, disabling back button input if true.
        /// </summary>
        /// <param name="isWait">If true, back button input is disabled.</param>
        public void SetWait(bool isWait)
        {
            this.isWait = isWait;
            OnStatusChanged?.Invoke();
        }
    }
}