using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(NavMeshAgent))]

public class EnemyController : LivingEntity
{
    public bool moveEnabled = true;

    [SerializeField]
    float attackInterval = 1;

    [SerializeField]
    string targetTag = "Player";

    [SerializeField]
    float deadTime = 3;

    public bool attacking = false;
    float enemySpeed;

    Animator animator;
    CapsuleCollider capsuleCollider;
    BoxCollider boxCollider;
    Rigidbody rb;
    NavMeshAgent agent;
    Transform target;
    GameManager gameManager;
    LivingEntity targetEntity;
    Projectile projectile;

    Material skinMaterial;
    Color originalColor;

    public AudioSource audioSourceDead;
    public AudioSource audioSourceAttack;

    public AudioClip deadAudio;
    public AudioClip attackAudio;

    void Start()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        audioSourceDead = GetComponent <AudioSource>();
        audioSourceAttack = GetComponent <AudioSource>();

        target = GameObject.FindGameObjectWithTag(targetTag).transform;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        targetEntity = target.GetComponent<LivingEntity>();

        InitCharacter();
    }

    void Update()
    {
        if (moveEnabled)
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    void InitCharacter()
    {
        enemySpeed = agent.speed;
    }

    void Move()
    {
        agent.speed = enemySpeed;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);

        agent.SetDestination(target.position);
        rb.velocity = agent.desiredVelocity;
    }

    void Stop()
    {
        agent.speed = 0;
        animator.SetFloat("Speed", agent.speed, 0.1f, Time.deltaTime);
    }

    public IEnumerator Dead()
    {
        moveEnabled = false;
        Stop();
        animator.SetTrigger("Dead");
        capsuleCollider.enabled = false;
        boxCollider.enabled = false;
        rb.isKinematic =  true;
        audioSourceDead.PlayOneShot (deadAudio);
        yield return new WaitForSeconds(deadTime);
        gameManager.Count(1);
        Destroy(gameObject);
        gameManager.Delay();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(AttackTimer());

            var hitPlayer = collision.gameObject;
            var healthPlayer = hitPlayer.GetComponent<Health>();
            if (healthPlayer != null)
            {
                healthPlayer.TakenDamage(1);
            }
        }

        // if (collision.gameObject.tag == "Bullet")
        // {
        //     var hitEnemy = collision.gameObject;
        //     var healthEnemy = hitEnemy.GetComponent<HealthEnemy>();
        //     if (healthEnemy != null)
        //     {
        //         healthEnemy.TakenDamage(1);
        //         Debug.Log("gaga");
        //     }
        // }

    }


    IEnumerator AttackTimer()
    {
        if (!attacking)
        {
            attacking = true;
            moveEnabled = false;
            capsuleCollider.enabled = false;

            animator.SetTrigger("Attack");
            audioSourceAttack.PlayOneShot (attackAudio);
            yield return new WaitForSeconds(attackInterval);

            attacking = false;
            moveEnabled = true;
            capsuleCollider.enabled = true;
        }

        yield return null;
    }

    // public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    // {
    //     if (damage >= health)
    //     {
    //         if (OnDeathStatic != null)
    //         {
    //             OnDeathStatic();
    //         }
            
    //     }
    //     base.TakeHit(damage, hitPoint, hitDirection);
    // }
}
