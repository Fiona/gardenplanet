using UnityEngine;

namespace GardenPlanet
{
    public class CharacterAnimatorEventListener: MonoBehaviour
    {
        public Character character;
        
        // Animation event: Nom some
        public void AnimatorNom()
        {
            character.AnimatorNom();
        }

        // Animation event: EatItem
        public void AnimatorEatItemDone()
        {
            character.AnimatorEatItemDone();
        }

        // Animation event: Close eyes
        public void AnimatorCloseEyes()
        {
            character.AnimatorCloseEyes();
        }

        // Animation event: YawnDone
        public void AnimatorYawnDone()
        {
            character.AnimatorYawnDone();
        }

        // Animation event: PassOutMid
        public void AnimatorPassOutMid()
        {
            character.AnimatorPassOutMid();
        }

        // Animation event: PassOutDone
        public void AnimatorPassOutDone()
        {
            character.AnimatorPassOutDone();
        }

    }
}