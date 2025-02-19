
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour, IInteractable
{
    public bool IsOpened { get; private set; }
    public string ChestID { get; private set; }
    public GameObject itemPrefab;
    public Sprite openedSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject); 
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        OpenChest();
    }

    public bool CanInteract()
    {
        return !IsOpened;
    }

    private void OpenChest()
    {
        SetOpened(true);

        if (itemPrefab) 
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>().StartBounce();
        }
    }

    public void SetOpened(bool opened)
    {
        if(IsOpened == opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }
}
