using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class InfoPopupDisplay: MonoBehaviour
    {
        [HideInInspector]
        public RectTransform rectTransform;
        [HideInInspector]
        public CanvasGroup canvasGroup;

        public GameObject info;
        public GameObject extraInfo;
        public TextMeshProUGUI text;
        public TextMeshProUGUI extraInfoText;
        public Image extraInfoIcon;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
        }

        public void SetText(string _text, string extraText)
        {
            if(extraText == "")
            {
                info.SetActive(true);
                extraInfo.SetActive(false);
            }
            else
            {
                info.SetActive(false);
                extraInfo.SetActive(true);
                extraInfoText.text = extraText;
            }
            text.text = _text;
        }

    }
}