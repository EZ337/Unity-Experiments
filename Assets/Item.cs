using UnityEngine;

public class Item : MonoBehaviour
{
    public string RegularMessage = "Acquired Item";
    public string ConditionedMessage = "Oooh Secret Message";
    private string message;
    private bool allowBtnPress = true;
    public bool collectible = false;
    public string ItemName;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.NeverSleep;
        allowBtnPress = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (allowBtnPress && collision.CompareTag("Player") && Input.GetKey(KeyCode.E))
        {
            if (collectible)
            {
                Debug.Log("Amount: " + Inventory.Instance.AddItem(this));
                gameObject.SetActive(false);
                return;
            }

            allowBtnPress = false;
            string message = RegularMessage;

            // Shows the message
            SimpleDialogue.Instance.UpdateText(message);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.attachedRigidbody.sleepMode = RigidbodySleepMode2D.StartAwake;
        SimpleDialogue.Instance.EndDialogue();
        allowBtnPress = true;
    }


    #region
    // override object.Equals
    public override bool Equals(object obj)
    {
        //       
        // See the full list of guidelines at
        //   http://go.microsoft.com/fwlink/?LinkID=85237  
        // and also the guidance for operator== at
        //   http://go.microsoft.com/fwlink/?LinkId=85238
        //

        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        else if (obj is Item itm)
        {
            return ItemName.Equals(itm.ItemName);
        }

        throw new System.NotImplementedException();
    }

    // override object.GetHashCode
    public override int GetHashCode()
    {
        // TODO: write your implementation of GetHashCode() here
        return ItemName.GetHashCode();
    }
    #endregion
}
