using StompyBlondie;
using UnityEngine;
using System.Collections;

namespace GardenPlanet
{
    public class InWorldItem : MonoBehaviour
    {
        public ItemType itemType;
        public Hashtable attributes;
        public bool droppedByPlayer;
        public Character beingHeldBy;

        GameController controller;
        Glowable highlightGlow;
        Glowable fullHighlightGlow;
        bool doHighlight;
        bool doFullHighlight;
        bool beingPickedUp;
        Rigidbody rigidBody;

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            rigidBody = gameObject.GetComponent<Rigidbody>();
            if(itemType.CanPickup)
            {
                highlightGlow = gameObject.AddComponent<Glowable>();
                fullHighlightGlow = gameObject.AddComponent<Glowable>();
            }
        }

        public void LateUpdate()
        {
            // Held items don't do any special things other than tracking position
            if(beingHeldBy != null)
            {
                transform.position = beingHeldBy.holdItemHolder.position;
                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    beingHeldBy.transform.rotation.eulerAngles.y,
                    transform.rotation.eulerAngles.z
                    );
                return;
            }

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
                controller.ShowInfoPopup(
                    new WorldPosition(transform.position),
                    new InfoPopup.InfoPopupDisplay {Text = itemType.DisplayName}
                );
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

        public void HoldInCharactersHands(Character characterHolding)
        {
            beingHeldBy = characterHolding;
            transform.position = beingHeldBy.holdItemHolder.position;
            if(rigidBody == null)
                rigidBody = gameObject.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            gameObject.DisableAllColliders();
            StartCoroutine(StartHoldingAnim());
        }

        public void PutBackIntoInventory()
        {
            if(beingHeldBy == null)
                return;
            StartCoroutine(PutBackAnim());
        }

        public void DropFromHands(bool heldByPlayer = false)
        {
            if(beingHeldBy == null)
                return;

            rigidBody.useGravity = true;
            rigidBody.freezeRotation = false;
            var rotationPush = 25f;
            rigidBody.angularVelocity = new Vector3(
                Random.Range(-rotationPush, rotationPush),
                Random.Range(-rotationPush, rotationPush),
                Random.Range(-rotationPush, rotationPush)
            );
            var vel = rigidBody.velocity;
            vel.x = Random.Range(-1f, 1f);
            vel.z = Random.Range(-1f, 1f);
            rigidBody.velocity = vel;

            gameObject.EnableAllColliders();
            beingHeldBy = null;
            if(heldByPlayer)
                droppedByPlayer = true;
        }

        private IEnumerator StartHoldingAnim()
        {
            var endScale = transform.localScale;
            foreach(var val in LerpHelper.LerpOverTime(.25f))
            {
                transform.localScale = Vector3.Lerp(Vector3.zero, endScale, val);
                yield return new WaitForFixedUpdate();
            }
            transform.localScale = endScale;
        }

        private IEnumerator PutBackAnim()
        {
            var startScale = transform.localScale;
            foreach(var val in LerpHelper.LerpOverTime(.25f))
            {
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, val);
                yield return new WaitForFixedUpdate();
            }
            Destroy(gameObject);
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
            Destroy(gameObject);
        }

    }
}