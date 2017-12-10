using StompyBlondie;
using UnityEngine;
using System.Collections;

namespace StrawberryNova
{
    public class InWorldItem : MonoBehaviour
    {
        public ItemType itemType;
        public Hashtable attributes;

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
            if(beingPickedUp || !itemType.CanPickup)
                return;
            if(doFullHighlight)
            {
                fullHighlightGlow.GlowTo(new Color(0f, .5f, 1f), 1f);
                controller.ShowPopup(itemType.DisplayName);    
            }
            else if (doHighlight)
                highlightGlow.GlowTo(new Color(.6f, .75f, .86f), .5f);
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

        public void Pickup()
        {
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