using TMPro;
using UnityEngine;

namespace GardenPlanet
{
    public class BirthdayField: MonoBehaviour
    {
        [Header("Object references")]
        public TextMeshProUGUI dayText;
        public TextMeshProUGUI seasonText;

        public GlobalButton dayPrevButton;
        public GlobalButton dayNextButton;
        public GlobalButton seasonPrevButton;
        public GlobalButton seasonNextButton;

        private int dayValue;
        private int seasonValue;
        private CACCharacter character;

        private void Start()
        {
            character = FindObjectOfType<CACCharacter>();
            dayValue = character.GetInformation().dayBirthday;
            seasonValue = character.GetInformation().seasonBirthday;

            dayPrevButton.SetCallback(DayPrevButtonPressed);
            dayNextButton.SetCallback(DayNextButtonPressed);
            seasonPrevButton.SetCallback(SeasonPrevButtonPressed);
            seasonNextButton.SetCallback(SeasonNextButtonPressed);

            UpdateTextAndCharacter();
        }

        private void UpdateTextAndCharacter()
        {
            dayText.text = dayValue.ToString();
            seasonText.text = Consts.SEASONS[seasonValue-1].displayName;

            character.SetBirthday(dayValue, seasonValue);
        }

        private void DayPrevButtonPressed()
        {
            dayValue--;
            if(dayValue < 1)
                dayValue = Consts.NUM_DAYS_IN_SEASON;
            UpdateTextAndCharacter();
        }

        private void DayNextButtonPressed()
        {
            dayValue++;
            if(dayValue > Consts.NUM_DAYS_IN_SEASON)
                dayValue = 1;
            UpdateTextAndCharacter();
        }

        private void SeasonPrevButtonPressed()
        {
            seasonValue--;
            if(seasonValue < 1)
                seasonValue = Consts.SEASONS.Length;
            UpdateTextAndCharacter();
        }

        private void SeasonNextButtonPressed()
        {
            seasonValue++;
            if(seasonValue > Consts.SEASONS.Length)
                seasonValue = 1;
            UpdateTextAndCharacter();
        }
    }
}