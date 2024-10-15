using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager gm;

    //dodging stuff
    private bool canDodge = true;
    private bool isDodging = false;
    private bool isPoisoned = false;
    private bool playManaSound = false;
    private bool canPlayMana = false;
    private bool hasCP = false;
    private bool canATKH = true;
    private bool canATKL = true;
    private bool canMelee = false;
    private bool meleeActive = false;
    private bool isHitBoxActive = false;
    private bool canManaCharge = true;
    public Transform meleePos;
    private float meleeRange;


    public GameObject meleeHitBox;
    private float meleeHitBoxLifeSpan = 0.1f;
    private float meleeHitBoxTimer = 0;
    public float meleeCoolDown = 1f;
    private float meleeTimer = 0f;
    public float meleeActiveTimer = 15;
    public float meleeActiveTime = 0;

    public float dodgingPower = 15;
    public float dodgingTime = 0.2f;
    public float dodgingCoolDown = 1;

    public float speed = 10;
    public float poisonTime = 5;
    public float poisonTimer = 0;

    public float lightSpellSpeed = 400f;
    public GameObject lightSpell;
    public float lightSpellLifeSpan = 5;
    public float lsTimer = 0;
    public float lsCoolDown = .25f;

    public float heavySpellSpeed = 200f;
    public GameObject heavySpell;
    public float heavySpellLifeSpan = 2;
    private float hsTimer = 0;
    public float hsCoolDown = 1;

    private CapsuleCollider2D ourCollider;
    private Rigidbody2D myRB;
    private Vector2 tempVelocity;
    private AdvEnemyController AEC;
    public GameObject melee;

    public int health = 100;
    public int maxHealth = 100;

    public bool isHit = false;
    private float hitTimer = 0;
    public float hitTime = .3f;

    public float manaChargeAmount = 0.5f;
    public float manaChargeCoolDown = 2;
    private float manaTimer = 0;
    public float mana = 100;
    public float maxMana = 100;
    private float couldPlayMana = 0f;

    public GameObject dmgIndicator;
    public GameObject poisonIndicator;
    private Transform respawn;
    public GameObject shrine;

    public AudioClip hurt;
    public AudioClip dodge;
    public AudioClip manaCharge;
    public AudioClip lowHP;
    public AudioClip wallATK;
    public AudioClip HpPickup;
    public AudioClip ballATK;
    public AudioClip SpPickup;
    public AudioClip deathXF;
    public AudioClip retryXF;
    public AudioClip meleeXF;
    public AudioClip meleeSummon;
    private float volume = .5f;

    private bool onScroll = false;
    private bool onScroll2 = false;
    private bool onScroll3 = false;
    private bool onSign = false;

    private AudioSource mySpeaker;


    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody2D>();
        ourCollider = GetComponent<CapsuleCollider2D>();
        AEC = GetComponent<AdvEnemyController>();
        mySpeaker = GetComponent<AudioSource>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    // Update is called once per frame
    void Update()
    {
        
        

        //checks if doding to see if player can move
        if (isDodging)
            return;

        tempVelocity.x = Input.GetAxisRaw("Horizontal") * speed;
        tempVelocity.y = Input.GetAxisRaw("Vertical") * speed / 2;

        myRB.velocity = tempVelocity;

        //dodging
        if (Input.GetKeyDown(KeyCode.Space) && canDodge && !gm.paused)
        {
            StartCoroutine(dodging());
            mySpeaker.PlayOneShot(dodge, volume);
            
        }

        //wave atk
        if (Input.GetKeyDown(KeyCode.Mouse1) && canATKH && mana >= 20 && !gm.paused)
        {
            heavyATK();
            mana -= 20;
        }

        if (!canATKH)
        {
            if (hsCoolDown < hsTimer)
            {
                canATKH = true;
                hsTimer = 0;
            }
            else
                hsTimer += Time.deltaTime;
        }
        //bullet atk
        if (Input.GetKeyDown(KeyCode.Mouse0) && canATKL && !meleeActive && mana >= 10 && !gm.paused)
        {
            lightATK();
            mana -= 10;
        }

        if (!canATKL)
        {
            if (lsCoolDown < lsTimer)
            {
                canATKL = true;
                lsTimer = 0;
            }
            else
                lsTimer += Time.deltaTime;
        }

        //switch to melee
        if (Input.GetKeyDown(KeyCode.LeftShift) && !meleeActive && mana >= 50 && !gm.paused)
        {
            meleeActive = true;
            melee.SetActive(true);
            mana -= 50;
            manaInteruption();
            mySpeaker.PlayOneShot(meleeSummon, volume);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            meleeActive = false;
            melee.SetActive(false);
            meleeActiveTimer = 15;
        }

        if (meleeActive)
        {
            if (meleeActiveTime > meleeActiveTimer)
            {
                meleeActive = false;
                melee.SetActive(false);
                meleeActiveTimer = 15;
            }
            else
                meleeActiveTimer -= Time.deltaTime;
        }

        if (isPoisoned)
        {
            poisonIndicator.SetActive(true);
            if(poisonTime < poisonTimer)
            {
                isPoisoned = false;
                poisonTimer = 0;
                poisonIndicator.SetActive(false);
            }
            else
            {
                poisonTimer += Time.deltaTime;
                if (!isHit)
                {
                    health -= 1;
                    isHit = true;
                    dmgIndicator.SetActive(true);
                    mySpeaker.PlayOneShot(hurt, volume);
                    if (health <= 0)
                    {
                        gm.death();
                        mySpeaker.PlayOneShot(deathXF, volume);
                    }
                    if (health <= 30)
                        mySpeaker.PlayOneShot(lowHP, volume);
                }
            }
        }


        //melee atk
        if (Input.GetKeyDown(KeyCode.Mouse0) && meleeActive && canMelee && !gm.paused)
        {
            mySpeaker.PlayOneShot(meleeXF, volume);
            meleeHitBox.SetActive(true);
            isHitBoxActive = true;
            Vector2 mirror = transform.localScale;
            mirror.y *= -1;
            transform.localScale = mirror;
            canMelee = false;
            manaInteruption();
        }

        if (!canMelee)
        {
            if (meleeCoolDown < meleeTimer) 
            {
                canMelee = true;
                meleeTimer = 0;
            }
            else 
                meleeTimer += Time.deltaTime;
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

        if (isHit)
        {
            if (hitTimer < hitTime)
                hitTimer += Time.deltaTime;
            else
            {
                hitTimer = 0;
                isHit = false;
            }
        }

        if (hitTimer > .1)
            dmgIndicator.SetActive(false);

        if (!canManaCharge)
        {
            if (manaTimer < manaChargeCoolDown)
                manaTimer += Time.deltaTime;
            else
            {
                manaTimer = 0;
                canManaCharge = true;
            }
        }

        if (canManaCharge && mana < maxMana && !gm.paused)
        {
            mana += manaChargeAmount;
            //canPlayMana = true;

            //if (playManaSound)
            //{
                //playManaSound = false;
                //mySpeaker.PlayOneShot(manaCharge, volume);
            //}
        }
        //else
        //{
            //canPlayMana = false;
            //couldPlayMana = 0;
        //}

        if (canPlayMana)
        {
            if (couldPlayMana <= manaChargeCoolDown)
                couldPlayMana += Time.deltaTime;
            else
            {
                playManaSound = true;
            }
        }


        if (onScroll)
        {
            if (Input.GetKeyDown(KeyCode.F) && !gm.paused)
            {
                gm.scroll();
            }
            
        }

        if (onScroll2)
        {
            if (Input.GetKeyDown(KeyCode.F) && !gm.paused)
            {
                gm.scroll_2();
            }

        }

        if (onScroll3)
        {
            if (Input.GetKeyDown(KeyCode.F) && !gm.paused)
            {
                gm.scroll_3();
            }

        }

        if (onSign)
        {
            if (Input.GetKeyDown(KeyCode.F) && !gm.paused)
            {
                gm.sign(); 
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "shrine")
        {
            respawn = shrine.GetComponent<Transform>();
            hasCP = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !isHit && !isDodging)
        {
            hit();

        }

        if (collision.gameObject.tag == "PoisonSlime" && !isHit && !isDodging)
        {
            hit();
            isPoisoned = true;
            poisonTimer = 0;
        }

        if (collision.gameObject.tag == "sword" && !isHit && !isDodging)
        {
            hit();
        }

        if (collision.gameObject.tag == "mana")
        {
            if (mana <= (maxMana - 20))
            {
                mana += 20;
                Destroy(collision.gameObject);
                mySpeaker.PlayOneShot(SpPickup, volume);
            }
            else if (mana < maxMana)
            {
                mana = maxMana;
                Destroy(collision.gameObject);
                mySpeaker.PlayOneShot(SpPickup, volume);
            }
        }

        if (collision.gameObject.tag == "health")
        {
            if (health <= (maxHealth - 15))
            {
                health += 15;
                Destroy(collision.gameObject);
                mySpeaker.PlayOneShot(HpPickup, volume);
            }
            else if (health < maxHealth)
            {
                health = maxHealth;
                Destroy(collision.gameObject);
                mySpeaker.PlayOneShot(HpPickup, volume);
                
            }
        }

        if (collision.gameObject.tag == "scroll")
            onScroll = true;

        if (collision.gameObject.tag == "sign")
            onSign = true;

        if (collision.gameObject.tag == "scroll2")
            onScroll2 = true;

        if (collision.gameObject.tag == "scroll3")
            onScroll3 = true;

        if (collision.gameObject.tag == "End_Lvl")
            gm.loadLevel(0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "scroll")
            onScroll = false;

        if (collision.gameObject.tag == "scroll2")
            onScroll2 = false;

        if (collision.gameObject.tag == "scroll3")
            onScroll3 = false;

        if (collision.gameObject.tag == "sign")
            onSign = false;
    }

    public void hit()
    {
        health -= 10;
        isHit = true;
        mySpeaker.PlayOneShot(hurt, volume);
        if (health <= 0)
        {
            gm.death();
            mySpeaker.PlayOneShot(deathXF, volume);
        }
        if (health <= 30)
            mySpeaker.PlayOneShot(lowHP, volume);
        dmgIndicator.SetActive(true);
    }



    private IEnumerator dodging()
    {
        canDodge = false;
        isDodging = true;

        //dodge movment
        myRB.velocity = new Vector2(myRB.velocity.x * dodgingPower, (myRB.velocity.y * dodgingPower));
        yield return new WaitForSeconds(dodgingTime);
        isDodging = false;
        yield return new WaitForSeconds(dodgingCoolDown);
        canDodge = true;
    }

    private void heavyATK()
    {
        GameObject hs = Instantiate(heavySpell, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(hs.GetComponent<BoxCollider2D>(), ourCollider);

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        hs.GetComponent<Rigidbody2D>().rotation = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
        hs.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * heavySpellSpeed);
        Destroy(hs, heavySpellLifeSpan);
        canATKH = false;
        mySpeaker.PlayOneShot(wallATK, volume);
        manaInteruption();
    }

    private void lightATK()
    {
        GameObject ls = Instantiate(lightSpell, transform.position, Quaternion.identity);
        Physics2D.IgnoreCollision(ls.GetComponent<CircleCollider2D>(), ourCollider);

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

        ls.GetComponent<Rigidbody2D>().rotation = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg - 90;
        ls.GetComponent<Rigidbody2D>().AddRelativeForce(transform.up * lightSpellSpeed);
        Destroy(ls, lightSpellLifeSpan);
        canATKL = false;
        mySpeaker.PlayOneShot(ballATK, volume);
        manaInteruption();
    }

    private void manaInteruption()
    {
        canManaCharge = false;
        manaTimer = 0;
        
    }


    public void death()
    {
        if (hasCP)
        {
            transform.SetPositionAndRotation(respawn.position, Quaternion.identity);
            health = maxHealth;
        }
        else
            gm.retryLvl();
    }
}
