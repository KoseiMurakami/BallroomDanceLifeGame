using UnityEngine;
using UnityEngine.UI;

public class OpenningSceneManager : MonoBehaviour
{
    [SerializeField]
    private InputField inputField = default;

    [SerializeField]
    private Button nextButton = default;

    void Update()
    {
        nextButton.interactable = true;

        if (inputField.text == "")
        {
            nextButton.interactable = false;
        }
    }

    /// <summary>
    /// NextButtonを押したときの処理。
    /// </summary>
    public void PushNextButton()
    {
        if (inputField.text != "")
        {
            GameManager.Instance.NickName = inputField.text;
            GameManager.Instance.LoadGameScene("SettingScene");
        }
    }
}
