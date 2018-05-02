using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class InfoPopupDisplayHolder: MonoBehaviour
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

        public void SetDisplay(InfoPopup.InfoPopupDisplay displayInfo, Sprite extraIcon)
        {
            if(string.IsNullOrEmpty(displayInfo.ExtraText))
            {
                info.SetActive(true);
                extraInfo.SetActive(false);
            }
            else
            {
                info.SetActive(false);
                extraInfo.SetActive(true);
                extraInfoText.text = displayInfo.ExtraText;
                extraInfoIcon.sprite = extraIcon;
            }
            text.text = displayInfo.Text;
        }

    }
}