using System.Collections.Generic;
using UnityEngine;

namespace GardenPlanet
{
    public class CharacterAI: MonoBehaviour
    {
        private List<CharacterTask> tasks;
        private CharacterTask currentTask;
        private Character character;

        public void Awake()
        {
            character = GetComponent<Character>();
        }

        public void AddTask(CharacterTask taskType)
        {
            taskType.SetCharacter(character);
        }
    }
}