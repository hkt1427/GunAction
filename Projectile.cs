using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    float speed = 10;

    float lifetime = 3;
    float skinWidth = .1f;

    EnemyController enemyController;
    HealthEnemy healthEnemy;

    Vector3 prepos;

    RaycastHit hit;

    void Start()
    {
        Destroy(gameObject, lifetime);
        enemyController = GetComponent<EnemyController>();
        healthEnemy = FindObjectOfType<HealthEnemy>();


        // Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        // if (initialCollisions.Length > 0)
        // {
        //     OnHitObject(initialCollisions[0], transform.position);
        // }

        prepos = transform.position;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        // CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);

        Ray ray = new Ray(transform.position, transform.forward);

         if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            string tagName = hit.collider.gameObject.tag;
            if(tagName == "Enemy")
            {
                HealthEnemy healthEnemy = hit.collider.gameObject.GetComponent<HealthEnemy>();
                healthEnemy.TakenDamage(1);
            }
            Destroy(gameObject);
        }
    }
}
