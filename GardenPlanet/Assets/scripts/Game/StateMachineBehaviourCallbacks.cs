/*
 * 2018 Stompy Blondie Games
 * Licensed under MIT. See accompanying LICENSE file for details.
 */
using UnityEngine;
using UnityEngine.Events;

namespace GardenPlanet.Utils
{
    [System.Serializable]
    public class AnimatorParamEvent : UnityEvent<Animator>
    {
    }

    /*
     * Added to any Mechanim state machine behaviour to pass-through to any other method that can be represented
     * by a UnityEvent.
     *
     * Attach this and a behaviour containing the method in question and wire the method up to the desired state
     * event list. Enter and exit events are most useful.
     */
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