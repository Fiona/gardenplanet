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

        // Animation event: CloseEyes
        public void AnimatorCloseEyes()
        {
            character.AnimatorCloseEyes();
        }

        // Animation event: OpenEyes
        public void AnimatorOpenEyes()
        {
            character.AnimatorOpenEyes();
        }

        // Animation event: PassOutMid
        public void AnimatorPassOutMid()
        {
            character.AnimatorPassOutMid();
        }

        // Animation event: LeftFootStep
        public void AnimatorLeftFootStep()
        {
            character.AnimatorLeftFootStep();
        }

        // Animation event: RightFootStep
        public void AnimatorRightFootStep()
        {
            character.AnimatorRightFootStep();
        }
    }
}