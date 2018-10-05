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

        // Animation event: BedStartDone
        public void AnimatorBedStartDone()
        {
            character.AnimatorBedStartDone();
        }

        // Animation event: BedEndDone
        public void AnimatorBedEndDone()
        {
            character.AnimatorBedEndDone();
        }
    }
}