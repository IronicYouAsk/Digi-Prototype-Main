using UnityEngine;

public class FarmableObject : MonoBehaviour
{
    [Tooltip("What farmable object this is. E.G: tree, Stone...etc")]
    [SerializeField] string thisFarmableObjectType;
    [SerializeField] int hitsToBreak = 1;
    [SerializeField] int amountToGive = 1;


    public void OnHit()
    {
        hitsToBreak -= 1;

        if(hitsToBreak <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    void OnDestroy()
    {
        Debug.Log(thisFarmableObjectType + " Collected x" + amountToGive);
        
        if (GameObject.FindFirstObjectByType<CollectionHandler>() == null) return;
        GameObject.FindFirstObjectByType<CollectionHandler>().UpdateCollectionMsg(thisFarmableObjectType, amountToGive);
    }
}
