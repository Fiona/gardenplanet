using System;
using UnityEngine;

namespace StrawberryNova
{
    public class WorldObjectInteractable : MonoBehaviour
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
            controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseHover);
            glow.GlowTo(new Color(.6f, .75f, .86f), .1f);
        }

        public void FullHighlight()
        {
            controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseOkay);
            glow.GlowTo(new Color(0f, .5f, 1f), .5f);
            controller.ShowInfoPopup(new WorldPosition(transform.position), worldObject.GetInfoPopup());
        }

        public void InteractWith()
        {
            // Start on controller because this object could be deleted mid-coroutine
            controller.StartCoroutine(controller.PlayerInteractWith(worldObject));
        }

}
}