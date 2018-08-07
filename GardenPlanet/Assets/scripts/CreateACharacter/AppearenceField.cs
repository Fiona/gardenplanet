using System;
using UnityEngine;

namespace GardenPlanet
{
    public class AppearenceField: MonoBehaviour
    {
        [Header("Settings")]
        public string typeShortName;
        public string overrideConfigName;

        protected CreateACharacterController controller;

        public virtual void Randomise()
        {
            throw new NotImplementedException();
        }

        protected void Awake()
        {
            controller = FindObjectOfType<CreateACharacterController>();
        }

        protected void Start()
        {
            UpdateDisplayAndCharacter();
        }

        protected virtual void UpdateDisplayAndCharacter()
        {
            throw new NotImplementedException();
        }
    }
}