using System;
using UnityEngine;

namespace StrawberryNova
{
    public class WorldObjectInteractable: MonoBehaviour
    {
        public WorldObject worldObject;

        Glowable glow;
        GameController controller;
        GameObject appearenceObject;

        public void Start()
        {
            if(appearenceObject == null)
                appearenceObject = transform.GetChild(0).GetChild(0).gameObject;
            glow = appearenceObject.AddComponent<Glowable>();
        }

        public void Awake()
        {
            controller = FindObjectOfType<GameController>();
        }

        public void Update()
        {
            if(controller.objectCurrentlyInteractingWith == worldObject)
                FullHighlight();
        }

        public void SetAppearenceObject(GameObject appearence)
        {
            appearenceObject = appearence;
        }

        public void Highlight()
        {
            glow.GlowTo(new Color(.6f, .75f, .86f), .5f);
        }

        public void FullHighlight()
        {
            glow.GlowTo(new Color(0f, .5f, 1f), .5f);
            if(worldObject != null && controller != null && controller.objectCurrentlyInteractingWith == null)
                controller.ShowPopup(worldObject.GetDisplayName());
        }

        public void InteractWith()
        {
            StartCoroutine(controller.PlayerInteractWith(worldObject));
        }

    }
}