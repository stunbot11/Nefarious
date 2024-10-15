using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightSpell : MonoBehaviour
{
    private CircleCollider2D ourCollider;

    public int pierceCounter = 3;

    



    // Start is called before the first frame update
    void Start()
    {
        ourCollider = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Pierce")
        {
            if (pierceCounter <= 0)
                Destroy(gameObject);
            else
                pierceCounter -= 1;
        }

        if (collider.gameObject.tag == "wall")
            Destroy(gameObject);
    }

  

}
