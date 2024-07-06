using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    private string message;
    private bool allowBtnPress = true;
    public List<string> Dialogues = new List<string>();
    public GameObject InteractIcon;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        allowBtnPress = true;
        InteractIcon.SetActive(true);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (allowBtnPress && collision.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            allowBtnPress = false;
            InteractIcon.SetActive(false);


            // Shows the message
            SimpleDialogue.Instance.UpdateText(message);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
        SimpleDialogue.Instance.EndDialogue();
        allowBtnPress = true;
        InteractIcon.SetActive(false);
    }
}
