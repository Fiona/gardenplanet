using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuBarMessage : MonoBehaviour
{
    public class Message
    {
        public string text;
        public EditorMessageType type;
    };

    public Text goodMessage;
    public Text mehMessage;
    public Text badMessage;

    private List<MenuBarMessage.Message> messages;

    public void Awake()
    {
        messages = new List<MenuBarMessage.Message>();
        StartCoroutine(ShowMessages());
    }

    public IEnumerator ShowMessages()
    {
        while(true)
        {
            if(messages.Count > 0)
            {
                var foundMessage = messages[0];
                messages.RemoveAt(0);
                if(foundMessage.type == EditorMessageType.Good)
                {
                    goodMessage.text = foundMessage.text;
                    goodMessage.gameObject.SetActive(true);
                }
                else if(foundMessage.type == EditorMessageType.Meh)
                {
                    mehMessage.text = foundMessage.text;
                    mehMessage.gameObject.SetActive(true);
                }
                else if(foundMessage.type == EditorMessageType.Bad)
                {
                    badMessage.text = foundMessage.text;
                    badMessage.gameObject.SetActive(true);
                }
                yield return new WaitForSeconds(4.0f);
                goodMessage.gameObject.SetActive(false);
                mehMessage.gameObject.SetActive(false);
                badMessage.gameObject.SetActive(false);
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void AddMessage(string messageText, EditorMessageType messageType = EditorMessageType.Good)
    {
        var newMessage = new Message{
            text=messageText,
            type=messageType,
        };
        messages.Add(newMessage);
    }

}
