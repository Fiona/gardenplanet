using StompyBlondie;
using UnityEngine;
using System.Collections;

namespace StrawberryNova
{
    public class InWorldItem : MonoBehaviour
    {
        public ItemType itemType;
        public Hashtable attributes;
        public bool droppedByPlayer;

        GameController controller;
        Glowable highlightGlow;
        Glowable fullHighlightGlow;
        bool doHighlight;
        bool doFullHighlight;
        bool beingPickedUp;

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            StartCoroutine(DropAnim());
            if(itemType.CanPickup)
            {
                highlightGlow = gameObject.AddComponent<Glowable>();
                fullHighlightGlow = gameObject.AddComponent<Glowable>();
            }
        }

        public void LateUpdate()
        {
            // If dropped we need to not be picked up unless the player has moved away
            if(droppedByPlayer)
            {
                var hits = Physics.OverlapSphere(transform.position, Consts.PLAYER_AUTO_PICKUP_RADIUS,
                    1<<Consts.COLLISION_LAYER_PLAYER);
                if(hits.Length == 0)
                    droppedByPlayer = false;
            }

            if(beingPickedUp || !itemType.CanPickup)
                return;
            if(doFullHighlight)
            {
                controller.ShowInfoPopup(new WorldPosition(transform.position), itemType.DisplayName);
                fullHighlightGlow.GlowTo(new Color(0f, .5f, 1f), 1f);
                controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseOkay);
            }
            else if(doHighlight)
            {
                highlightGlow.GlowTo(new Color(.6f, .75f, .86f), .5f);
                controller.GameInputManager.SetMouseTexture(controller.GameInputManager.mouseHover);
            }

            doHighlight = false;
            doFullHighlight = false;
        }

        public void Highlight()
        {
            doHighlight = true;
        }

        public void FullHighlight()
        {
            doFullHighlight = true;
        }

        public void Pickup(bool autoPickup = false)
        {
            if(autoPickup && droppedByPlayer)
                return;
            if(beingPickedUp || !itemType.CanPickup)
                return;
            beingPickedUp = true;
            Object.Destroy(fullHighlightGlow);
            Object.Destroy(highlightGlow);
            StartCoroutine(PickupAnim());
        }

        private IEnumerator DropAnim()
        {
            var endScale = transform.localScale;
            foreach(var val in LerpHelper.LerpOverTime(.25f))
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, endScale, val);
                yield return new WaitForFixedUpdate();
            }
            transform.localScale = endScale;
        }

        private IEnumerator PickupAnim()
        {
            var startScale = transform.localScale;
            var startPos = transform.position;
            foreach(var val in LerpHelper.LerpOverTime(.25f))
            {
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, val);
                var playerPos = controller.player.transform.position - new Vector3(0f, -.25f, 0f);
                transform.position = Vector3.Lerp(startPos, playerPos, val);
                yield return new WaitForFixedUpdate();
            }
            transform.localScale = Vector3.zero;
            controller.GivePlayerItem(itemType, attributes, 1);
            Object.Destroy(this.gameObject);
        }

    }
}