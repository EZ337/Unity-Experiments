using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDialogue : MonoBehaviour
{
    public static SimpleDialogue Instance { get; private set; }

    public TextMeshProUGUI dialogueText;
    public Canvas dialogueCanvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        dialogueCanvas.enabled = false;
    }

    public void EndDialogue()
    {
        dialogueCanvas.enabled = false;
    }

    public void UpdateText(string txt)
    {
        dialogueText.text = txt;
        dialogueCanvas.enabled = true;
    }

}
