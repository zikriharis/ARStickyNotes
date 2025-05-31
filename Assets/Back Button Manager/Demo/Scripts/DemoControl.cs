using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace RHP.BackButtonManager.Demo
{
    public class DemoControl : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject touchBlock;
        public GameObject popupSample1;
        public GameObject popupSample2;
        public GameObject menuSample;
        public GameObject subMenuSample;
        public GameObject visibleMenuSample1;
        public GameObject visibleMenuSample2;
        public GameObject gamePlaySample;
        public GameObject waitSample;
        public GameObject waitSamplePopup;
        public TextMeshProUGUI waitCounterText;
        public GameObject removeHandlerSample;

        [Header("Back Button Action Handlers")]
        public BackButtonActionHandler visibleMenu2ButtonCloseHandler;

        // Popup Sample 1
        public void ShowPopupSample1(bool isShow)
        {
            popupSample1.SetActive(isShow);
            touchBlock.SetActive(isShow);
        }

        // Popup Sample 2
        public void ShowPopupSample2(bool isShow)
        {
            popupSample2.SetActive(isShow);
            touchBlock.SetActive(isShow);
        }

        public void PopupSample2ButtonOk()
        {
            // Do Something..
            popupSample2.SetActive(false);
            touchBlock.SetActive(false);
        }

        // Menu Sample
        public void OpenMenuSample(bool isOpen)
        {
            menuSample.SetActive(isOpen);
        }

        public void OpenSubMenuSample(bool isOpen)
        {
            subMenuSample.SetActive(isOpen);
        }

        // Visible Menu Sample 1
        public void ShowVisibleMenuSample1(bool isShow)
        {
            touchBlock.SetActive(isShow);
            visibleMenuSample1.GetComponent<Animator>().SetBool("IsShow", isShow);
        }

        // Visible Menu Sample
        public void ShowVisibleMenuSample2(bool isShow)
        {
            if (isShow && UnityEngine.Random.Range(0, 2) == 0)
            {
                Debug.Log("Menu Sample 2 Not Show");
                return;
            }

            if (isShow)
            {
                visibleMenu2ButtonCloseHandler.RegisterHandler();
            }

            touchBlock.SetActive(isShow);
            visibleMenuSample2.GetComponent<Animator>().SetBool("IsShow", isShow);
        }

        // Game Play Sample
        public void GamePlaySampleOnOff(bool isOn)
        {
            gamePlaySample.SetActive(isOn);
            if (isOn)
            {
                gamePlaySample.GetComponent<Animator>().Play("Resume", 0, 1);
            }
        }

        public void GamePlayPauseSample(bool isPause)
        {
            gamePlaySample.GetComponent<Animator>().SetBool("IsPause", isPause);
        }

        // Wait Sample
        public void ShowWaitSample(bool isShow)
        {
            waitSample.SetActive(isShow);

            if (isShow)
            {
                waitSamplePopup.SetActive(true);
                StopAllCoroutines();
                StartCoroutine(DisableWaitSample());
            }
        }

        IEnumerator DisableWaitSample()
        {
            for (int i = 0; i < 3; i++)
            {
                waitCounterText.text = (3 - i).ToString();
                yield return new WaitForSeconds(1f);
            }

            waitSamplePopup.SetActive(false);
        }

        // Remove Handler Sample
        public void ShowRemoveHandlerSample(bool isShow)
        {
            removeHandlerSample.SetActive(isShow);
        }
    }
}