using TMPro;
using UnityEngine;

public class InputFieldColorChanger : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnInputChanged);
    }

    void OnInputChanged(string text)
    {
        inputField.textComponent.color = Color.white; // Change to your desired color
    }
}
