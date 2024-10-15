using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public GameManager gm;

    public bool isFighting = false;
    private bool canAttack = true;
    private bool isHitBoxActive = false;
    private bool isDodging = false;

    public int health = 50;
    public int damage = 1;
    private float speed = 6;
    private float distance;

    public float attackCoolDown = 2;
    public float atkCoolDownTimer = 0;
    private float meleeHitBoxLifeSpan = 0.1f;
    private float meleeHitBoxTimer = 0;

    public float dodgingPower = 12;
    public float dodgingTime = 1f;
    public float dodgingCoolDown = 2;

    private Rigidbody2D myRB;
    private Transform player;

    public GameObject stabHitBox;
    public GameObject slashHitBox;

    public GameObject knight;

    private Vector2 upRight;
    private Vector2 downRight;
    private Vector2 upLeft;
    private Vector2 downLeft;
    private Vector2 up;
    private Vector2 down;
    private Vector2 right;
    private Vector2 left;

    private float rotation;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").transform;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

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
        if (isDodging)
            return;

        distance = Vector2.Distance(transform.position, player.transform.position);

        //movement
        if (isFighting)
        {
            Vector2 targetPos = player.position - transform.position;

            rotation = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
            if (rotation >= -112.5 && rotation <= -67.5)
                myRB.velocity = down;
            else if (rotation >= -167.5 && rotation <= -112.5)
                myRB.velocity = downLeft;
            else if (rotation <= -157.5 && rotation >= -180 || rotation <= 180 && rotation >= 157.5)
                myRB.velocity = left;
            else if (rotation >= 112.5 && rotation <= 157.5)
                myRB.velocity = upLeft;
            else if (rotation >= 67.5 && rotation <= 112.5)
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
        {
            //int attackChoice = Random.Range(1, 3);

            //if (attackChoice == 1)
            //stab();
            //else if (attackChoice == 2)
            slash();
            canAttack = false;
            atkCoolDownTimer = 0;
        }

        else if (distance >= 2  && canAttack)
        {
            int attackChoice = Random.Range(1, 11);
            if (attackChoice == 1)
                stab();
            else if (attackChoice == 10)
                summon();
            canAttack = false;
            atkCoolDownTimer = 0;
        }

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
                stabHitBox.SetActive(false);
                slashHitBox.SetActive(false);
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
                death();
                gm.loadLevel(1);
            }
        }
    }


    //attacks
    private void stab()
    {

        stabHitBox.SetActive(true);
        canAttack = false;
        isHitBoxActive = true;
        StartCoroutine(dodging());
    }

    private IEnumerator dodging()
    {
        Vector2 targetPos = player.position - transform.position;
        targetPos.Normalize();
        isDodging = true;

        //dodge movment
        myRB.velocity = new Vector2(targetPos.x * dodgingPower, (targetPos.y * dodgingPower));
        yield return new WaitForSeconds(dodgingTime);
        isDodging = false;
        yield return new WaitForSeconds(dodgingCoolDown);
        
    }

    private void slash()
    {
        slashHitBox.SetActive(true);
        canAttack = false;
        isHitBoxActive = true;
    }

    private void bash()
    {
        
    }

    private void summon()
    {
        Instantiate(knight, transform.position, Quaternion.identity);
    }

    private void death()
    {
        Destroy(gameObject);
    }
}
