using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MEC;

namespace GardenPlanet
{
    public class CharacterAI : MonoBehaviour
    {
        public CharacterTask CurrentTask => currentTask;

        private List<CharacterTask> tasks;
        private CharacterTask currentTask;
        private Character character;

        public void AddTask(CharacterTask task)
        {
            task.SetCharacter(character);
            task.SetCharacterAIManager(this);
            tasks.Add(task);
            Debug.Log($"AI Task Added {task}");
            RunHighestPriorityTask();
        }

        private void Start()
        {
            tasks = new List<CharacterTask>();
        }

        private void Awake()
        {
            character = GetComponent<Character>();
            Timing.RunCoroutine(DoAI().CancelWith(character.gameObject));
        }

        private void Update()
        {
            // Switch task if finished
            if(currentTask != null && currentTask.IsFinished)
            {
                tasks.Remove(currentTask);
                currentTask = null;
                RunHighestPriorityTask();
            }
        }

        private void RunHighestPriorityTask()
        {
            var filteredTasks = tasks.Where(t => t.CanRun).ToList();
            // No tasks!
            if(filteredTasks.Count == 0)
                return;
            // Get highest
            var highestPriority = filteredTasks.OrderByDescending(t => (int)t.Priority).First();
            // If already doing or an equal priority then discard
            if(currentTask != null && (highestPriority == currentTask || currentTask.Priority == highestPriority.Priority))
                return;
            // Stop current task because we're switching
            if(currentTask != null)
            {
                if(currentTask.IsRunning)
                    currentTask.Pause();
            }
            // Resume or start
            currentTask = highestPriority;
            if(currentTask.IsStarted)
                currentTask.Resume();
            else
                currentTask.Start();
        }

        private IEnumerator<float> DoAI()
        {
            yield return Timing.WaitForSeconds(1f);

            if(character.id != Consts.CHAR_ID_PLAYER)
            {
                AddTask(
                    new CharacterTaskSpitOutToConsole(TaskPriority.NORMAL, "Hello")
                        .Then(new CharacterTaskSpitOutToConsole(TaskPriority.NORMAL, "Second one"))
                        .Then(new CharacterTaskSpitOutToConsole(TaskPriority.NORMAL, "Third one"))
                );

                AddTask(
                    new CharacterTaskSpitOutToConsole(TaskPriority.VERY_IMPORTANT, "High priority message")
                );
            }

            while(true)
            {
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}