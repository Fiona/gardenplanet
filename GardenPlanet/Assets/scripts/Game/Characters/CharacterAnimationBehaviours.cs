using UnityEngine;

namespace GardenPlanet
{
    public class CharacterAnimationBehaviours: StateMachineBehaviour
    {
        private Character Char(Animator animator)
        {
            return animator.gameObject.GetComponent<CharacterAnimatorEventListener>().character;
        }

        public void BedStartDone(Animator animator)
        {
            Char(animator).AnimatorBedStartDone();
        }

        public void BedEndDone(Animator animator)
        {
            Char(animator).AnimatorBedEndDone();
        }

        public void YawnDone(Animator animator)
        {
            Char(animator).AnimatorYawnDone();
        }

        public void PassOutDone(Animator animator)
        {
            Char(animator).AnimatorPassOutDone();
        }

        public void EatItemDone(Animator animator)
        {
            Char(animator).AnimatorEatItemDone();
        }

    }
}