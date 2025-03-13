using System.Collections;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    [SerializeField] string farmableObjectTag = "farmableObject";
    [SerializeField] string attackKey = "e";
    [SerializeField] float attackWaitTimer = .5f;

    private bool farmable = false;
    private bool attackOnCooldown = false;
    private GameObject otherCol;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(attackKey) && farmable && !attackOnCooldown)
        {
            StartCoroutine(AttackCoroutine(otherCol));
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag(farmableObjectTag))
        {
            farmable = true;
            otherCol = other.gameObject;
        }
    }
    void OnTriggerExit(Collider other)
    {
        farmable = false;
    }

    IEnumerator AttackCoroutine(GameObject other)
    {
        Debug.Log("Attack" + other.name);
        otherCol.GetComponent<FarmableObject>().OnHit();

        attackOnCooldown = true;
        yield return new WaitForSeconds(attackWaitTimer);
        attackOnCooldown = false;
    }
}
