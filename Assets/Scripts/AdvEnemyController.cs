using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvEnemyController : MonoBehaviour
{
    public int health = 5;
    public int damage = 1;

    private bool canAttack = true;
    private bool isHitBoxActive = false;
    private bool changedDirection;

    private float attackCoolDown = 2;
    private float atkCoolDownTimer = 0;
    private float meleeHitBoxLifeSpan = 0.1f;
    private float meleeHitBoxTimer = 0;

    private float distance;
    public float speed = 4;
    private float rotation;

    const float dropChance = 1 / 4f;

    private Rigidbody2D myRB;
    private Transform player;
    public GameObject healthDrop;
    public GameObject manaDrop;
    public GameObject meleeHitBox;

    private Vector2 upRight;
    private Vector2 downRight;
    private Vector2 upLeft;
    private Vector2 downLeft;
    private Vector2 up;
    private Vector2 down;
    private Vector2 right;
    private Vector2 left;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;

        upRight = new Vector2(speed, speed / 2);
        downLeft = upRight * -1;
        downRight = new Vector2(speed, -(speed / 2));
        upLeft = downRight * -1;
        up = new Vector2(0, speed / 2);
        down = up * -1;
        right = new Vector2(speed, 0);
        left = right * -1;

    }

    // Update is called once per frame
    void Update()
    {

        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();
        
        
        //move twords player dectection stuff math things
        if (distance <= 5)
        {
            Vector2 targetPos = player.position - transform.position;

            rotation = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            if (rotation >= -112.5 && rotation <= -67.5)
                myRB.velocity = down;
            else if(rotation >= -167.5 && rotation <= -112.5)
                myRB.velocity = downLeft;
            else if(rotation <= -157.5 && rotation >= -180 || rotation <= 180 && rotation >= 157.5)
                myRB.velocity = left;
            else if(rotation >= 112.5 && rotation <= 157.5)
                myRB.velocity = upLeft;
            else if(rotation >= 67.5 && rotation <= 112.5)
                myRB.velocity = up;
            else if (rotation >= 22.5 && rotation <= 67.5)
                myRB.velocity = upRight;
            else if (rotation >= -22.5 && rotation <= 22.5)
                myRB.velocity = right;
            else if (rotation >= -67.5 && rotation <= -22.5)
                myRB.velocity = downRight;
        }
        else
            myRB.velocity = Vector2.zero;

        if (distance <= 2 && canAttack)
            attack();

        if (!canAttack)
        {
            if (attackCoolDown < atkCoolDownTimer)
            {
                canAttack = true;
                atkCoolDownTimer = 0;
            }
            else
                atkCoolDownTimer += Time.deltaTime;
        }

        if (isHitBoxActive)
        {
            if (meleeHitBoxLifeSpan < meleeHitBoxTimer)
            {
                isHitBoxActive = false;
                meleeHitBox.SetActive(false);
                meleeHitBoxTimer = 0;
            }
            else
                meleeHitBoxTimer += Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "weapon")
        {
            health -= 1;


            if (health <= 0)
            {
                if (Random.Range(0f, 1f) <= dropChance)
                    consumableDrop();
                Destroy(gameObject);
            }
        }
    }

    private void consumableDrop()
    {
        int dropChoice = Random.Range(0, 1);

        if (dropChoice == 1)
            Instantiate(healthDrop, transform.position, Quaternion.identity);
        if (dropChoice == 0)
            Instantiate(manaDrop, transform.position, Quaternion.identity);
    }

    private void attack()
    {
        canAttack = false;
        isHitBoxActive = true;
        meleeHitBox.SetActive(true);
    }
}



//382.5 - 22.5 u
//22.5 - 67.5 ur
//67.5 - 112.5 r
//122.5 - 157.5 dr
//157.5 - 202.5 d
//202.5 - 247.5 dl
//247.5 - 292.5 l
//292.5 - 337.5 ul