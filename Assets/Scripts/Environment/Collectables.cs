using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectables : MonoBehaviour
{
    protected SpriteRenderer collectableSprite;
    [SerializeField] protected Transform collectionPoint;
    [SerializeField] protected float collectionPointRange;
    [SerializeField] [Tooltip("Player Layer")] protected LayerMask playerLayer = 0;

    protected bool Collected()
    {
        Collider2D[] hitPlayer;
        // create a circle in enemyAttackPoint position which has a radius size is equal to enemyAttackRange and last parameter represents what kind of layer is touched
        hitPlayer = Physics2D.OverlapCircleAll(collectionPoint.position, collectionPointRange, playerLayer);
        foreach (Collider2D player in hitPlayer)
        {
            return true;
        }
        return false;
    } 

    protected void Suicide()
    {
        Destroy(this.gameObject);
    }
}
