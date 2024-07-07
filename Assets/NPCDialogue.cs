using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    private string message;
    public bool npcInDialogue = false;
    public List<ComplexDialogue> Dialogues = new List<ComplexDialogue>();
    public GameObject InteractIcon;
    private int indx = 0;

    [NaughtyAttributes.Button]
    public void NextDialogue()
    {
        if (indx < Dialogues.Count)
        {
            SimpleDialogue.Instance.UpdateText(Dialogues[indx++]);
        }
        else
        {
            EndDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        npcInDialogue = false;
        InteractIcon.SetActive(true);
        indx = 0;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (npcInDialogue)
        {
            InteractIcon.SetActive(false);
        }

        if (!npcInDialogue && collision.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            npcInDialogue = true;
            NextDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
        EndDialogue();
    }

    private void EndDialogue()
    {
        SimpleDialogue.Instance.EndDialogue();
        npcInDialogue = false;
        indx = 0;
    }
}


[Serializable]
public class ComplexDialogue
{
    // Condition
    public string LineToSay;

    public static implicit operator string(ComplexDialogue d) { return d.LineToSay; }
}