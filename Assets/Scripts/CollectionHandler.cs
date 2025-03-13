using UnityEngine;
using TMPro;
using System.Collections;

public class CollectionHandler : MonoBehaviour
{
    [SerializeField] float fadeAwayTimer = 5f;
    private GameObject collectionTextObject;
    private TMP_Text collectionText;

    void Start()
    {
        collectionTextObject = GameObject.FindWithTag("CollectionText");
        Debug.Log(collectionTextObject.name);
        collectionTextObject.SetActive(false);
    }

    public void UpdateCollectionMsg(string itemName, int amountToGive)
    {
        if (collectionTextObject == null) {Debug.LogWarning("NullCTObject"); return;}

        collectionTextObject.SetActive(true);
        collectionText = collectionTextObject.GetComponent<TMP_Text>();
        collectionText.text = itemName + " Collected x" + amountToGive;

        StartCoroutine(FadeAway());
    }

    IEnumerator FadeAway()
    {
        yield return new WaitForSeconds(fadeAwayTimer);
        collectionTextObject.SetActive(false);
    }
}
