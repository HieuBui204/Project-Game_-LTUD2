using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float damage = 10;
    [SerializeField] float checkRadius = 1.5f; // Radius for checking collision
    private LayerMask targetLayerMask; // Lớp các đối tượng chịu ảnh hưởng 

    private void Start()
    {
        targetLayerMask = LayerMask.GetMask("Player");
        // StartCoroutine(RemoveObject());
    }

    void Update()
    {
        doDamage();
    }

    private void doDamage()
    {
        Collider2D target = Physics2D.OverlapCircle(transform.position, checkRadius, targetLayerMask);

        if (target != null)
        {
            target.GetComponent<Player>().DamagePlayer(damage);
            Destroy(this.gameObject);
        }


    }

    IEnumerator RemoveObject()
    {
        yield return new WaitForSeconds(10);
        Destroy(this.gameObject);
    }
}
