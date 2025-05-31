using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RHP.BackButtonManager
{
    /// <summary>
    /// Manages back button actions for UI elements.
    /// </summary>
    public class BackButtonActionHandler : MonoBehaviour
    {
        public BackButtonActionTypes backButtonActionType;
        public RemoveHandlerTypes removeHandlerType;

        public BackButtonActionHandler triggerTargetHandler;
        public BackButtonActionHandler targetActionHandler;
        public BackButtonActionHandler targetRemoveHandler;

        public bool autoRegisterOnStart;
        public bool clearAllHandlersOnStart;

        private Button button;

        public bool IsRegistered { get; set; }

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            AddButtonListener();

            switch (backButtonActionType)
            {
                case BackButtonActionTypes.Remove:
                    switch (removeHandlerType)
                    {
                        case RemoveHandlerTypes.ClearAllHandlersOnStart:
                            BackButtonController.Instance.UnregisterHandler(true);
                            break;
                        case RemoveHandlerTypes.RemoveHandlerOnStart:
                            BackButtonController.Instance.UnregisterHandler(false);
                            break;
                    }

                    break;
                case BackButtonActionTypes.Quit when clearAllHandlersOnStart:
                    BackButtonController.Instance.UnregisterHandler(true);
                    break;
            }

            StartCoroutine(DelayedCall());
        }

        IEnumerator DelayedCall()
        {
            yield return null;

            switch (backButtonActionType)
            {
                case BackButtonActionTypes.ButtonClick:
                    if (autoRegisterOnStart)
                    {
                        RegisterHandler();
                    }

                    break;
                case BackButtonActionTypes.Quit:
                    BackButtonController.Instance.SetQuit(true);
                    break;
                case BackButtonActionTypes.Wait:
                    BackButtonController.Instance.SetWait(true);
                    break;
            }
        }

        /// <summary>
        /// Registers this handler with the BackButtonController.
        /// </summary>
        public void RegisterHandler()
        {
            BackButtonController.Instance.RegisterHandler(this);
            IsRegistered = true;
        }

        /// <summary>
        /// Unregisters this handler from the BackButtonController.
        /// </summary>
        public void UnregisterHandler()
        {
            BackButtonController.Instance.UnregisterHandler(this);
            IsRegistered = false;
        }

        private void OnDisable()
        {
            switch (backButtonActionType)
            {
                case BackButtonActionTypes.Quit:
                    BackButtonController.Instance.SetQuit(false);
                    break;
                case BackButtonActionTypes.Wait:
                    BackButtonController.Instance.SetWait(false);
                    break;
            }

            RemoveButtonListener();
        }

        /// <summary>
        /// Invokes the button's onClick event if the action type is ButtonClick.
        /// </summary>
        public void BackButtonPress()
        {
            if (backButtonActionType != BackButtonActionTypes.ButtonClick)
                return;

            button.onClick?.Invoke();
        }

        private void HandleButtonClick()
        {
            switch (backButtonActionType)
            {
                case BackButtonActionTypes.ButtonClick:
                    BackButtonController.Instance.UnregisterHandler(false);
                    IsRegistered = false;

                    if (targetActionHandler != null)
                    {
                        targetActionHandler.RegisterHandler();
                    }

                    break;
                case BackButtonActionTypes.Trigger:
                    triggerTargetHandler.RegisterHandler();
                    break;
                case BackButtonActionTypes.Remove:
                    switch (removeHandlerType)
                    {
                        case RemoveHandlerTypes.ClearAllHandlersOnButtonClick:
                            BackButtonController.Instance.UnregisterHandler(true);
                            IsRegistered = false;
                            break;
                        case RemoveHandlerTypes.RemoveHandlerOnButtonClick:
                            BackButtonController.Instance.UnregisterHandler(false);
                            IsRegistered = false;
                            break;
                        case RemoveHandlerTypes.RemoveTargetHandler:
                            BackButtonController.Instance.UnregisterHandler(targetRemoveHandler);
                            break;
                    }

                    break;
            }
        }

        private void AddButtonListener()
        {
            if (button == null)
                return;

            button.onClick.AddListener(HandleButtonClick);
        }

        private void RemoveButtonListener()
        {
            if (button == null)
                return;

            button.onClick.RemoveListener(HandleButtonClick);
        }
    }

    /// <summary>
    /// Defines the types of actions triggered by the back button.
    /// </summary>
    public enum BackButtonActionTypes
    {
        ButtonClick,
        Trigger,
        Remove,
        Quit,
        Wait
    }

    /// <summary>
    /// Specifies the removal strategies for back button handlers.
    /// </summary>
    public enum RemoveHandlerTypes
    {
        RemoveHandlerOnButtonClick,
        ClearAllHandlersOnButtonClick,
        RemoveHandlerOnStart,
        ClearAllHandlersOnStart,
        RemoveTargetHandler
    }
}