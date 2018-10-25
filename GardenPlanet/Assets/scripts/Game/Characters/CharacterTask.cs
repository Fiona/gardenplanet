using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

namespace GardenPlanet
{
    public enum TaskPriority
    {
        MEH = 1,
        NOT_IMPORTANT = 2,
        NORMAL = 3,
        IMPORTANT = 4,
        VERY_IMPORTANT = 5,
        CRITICAL = 5
    }

    public class CharacterTask
    {
        public TaskPriority Priority => priority;
        public bool IsRunning => coroutine.IsRunning && !paused;
        public bool IsFinished => done;
        public bool IsStarted => started;
        public string Name => name;
        public bool CanRun => (dependency == null || dependency.done) && !done;

        protected string name = "Unamed";
        protected TaskPriority priority;
        protected Character character;
        protected bool done;
        protected bool started;
        protected bool paused;
        protected CharacterTask dependency;
        protected List<CharacterTask> children;
        protected CoroutineHandle coroutine;

        public CharacterTask(TaskPriority priority, CharacterTask dependency = null)
        {
            this.priority = priority;
            this.dependency = dependency;
            children = new List<CharacterTask>();
        }

        public CharacterTask Then(CharacterTask nextTask)
        {
            children.Add(nextTask);
            nextTask.SetCharacter(character);
            nextTask.SetDependency(this);
            return this;
        }

        public void SetCharacter(Character character)
        {
            this.character = character;
            foreach(var c in children)
                c.SetCharacter(character);
        }

        public void SetDependency(CharacterTask dependency)
        {
            this.dependency = dependency;
        }

        public void SetCharacterAIManager(CharacterAI manager)
        {
            foreach(var c in children)
                manager.AddTask(c);
        }

        public void Start()
        {
            if(!character)
                throw new AITaskException($"{this} - Character not set");
            if(started)
                throw new AITaskException($"{this} - Can't start, already started");
            if(dependency != null && !dependency.done)
                throw new AITaskException($"{this} - Can't start, dependency not done: {dependency}");
            Debug.Log($"{this} - Starting");
            coroutine = Timing.RunCoroutine(TaskRoutine().Append(() => done = true));
            started = true;
        }

        public void Pause()
        {
            if(!character)
                throw new AITaskException($"{this} - Character not set");
            if(!started)
                throw new AITaskException($"{this} - Can't pause, not started");
            Debug.Log($"{this} - Pausing");
            coroutine.IsPaused = true;
            paused = true;
        }

        public void Resume()
        {
            if(!character)
                throw new AITaskException($"{this} - Character not set");
            if(!started || !paused)
                throw new AITaskException($"{this} - Can't resume, not paused or started");
            Debug.Log($"{this} - Resuming");
            coroutine.IsPaused = false;
            paused = false;
        }

        public override string ToString()
        {
            var charString = character == null ? "None" : $"{character.id} - {character.name}";
            return $"AI Task {name}({priority}): {charString}";
        }

        protected virtual IEnumerator<float> TaskRoutine()
        {
            yield break;
        }
    }

    /*
     * Task Type: SpitOutToConsole
     * Just a test task
     */
    public class CharacterTaskSpitOutToConsole : CharacterTask
    {
        private string output;

        public CharacterTaskSpitOutToConsole(TaskPriority priority, string stringToSpit, CharacterTask dependency = null):
            base(priority, dependency)
        {
            name = "SpitOutToConsole";
            output = stringToSpit;
        }

        protected override IEnumerator<float> TaskRoutine()
        {
            yield return Timing.WaitForSeconds(.5f);
            Debug.Log(output);
        }
    }

    /*
     * Task Type: PathToTile
     * Walks the character to a particular tile in a particular map
     */
    public class CharacterTaskPathToTile: CharacterTask
    {
        private MapTilePosition pathToPos;

        public CharacterTaskPathToTile(TaskPriority priority, MapTilePosition pathTo, CharacterTask dependency = null):
            base(priority, dependency)
        {
            name = "PathToTile";
            pathToPos = pathTo;
        }

        protected override IEnumerator<float> TaskRoutine()
        {
            for(var i = 0; i <= 10; i++)
            {
                Debug.Log($"Walking walking {pathToPos}");
                yield return Timing.WaitForOneFrame;
            }
            yield break;
        }
    }

}