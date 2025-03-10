using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int health;
    public int stageIndex;


    public void NextStage()
    {

        stageIndex++;

        totalPoint += stagePoint;
        stagePoint = 0;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            HeroKnightUsing.singleton.Die();


            collision.attachedRigidbody.linearVelocity = Vector2.zero;
            collision.transform.position = new Vector3(0, 0, -1);

        }




    }



}
