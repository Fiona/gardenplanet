using UnityEngine;
using UnityEngine.Events;

namespace GardenPlanet
{
    [System.Serializable]
    public class AnimatorParamEvent : UnityEvent<Animator>
    {
    }

    public class StateMachineBehaviourCallbacks: StateMachineBehaviour
    {
        public AnimatorParamEvent onStateEnterEvents;
        public AnimatorParamEvent onStateUpdateEvents;
        public AnimatorParamEvent onStateExitEvents;
        public AnimatorParamEvent onStateMoveEvents;
        public AnimatorParamEvent onStateIKEvents;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            onStateEnterEvents.Invoke(animator);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            onStateUpdateEvents.Invoke(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            onStateExitEvents.Invoke(animator);
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            onStateMoveEvents.Invoke(animator);
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            onStateIKEvents.Invoke(animator);
        }
    }
}