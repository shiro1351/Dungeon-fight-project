using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HammerMan: Character
{
  
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        base.Update();
        // Falling logic
        if (!isGround && rb.velocity.y < 0)
        {
           if(isAttack)
           {
                speed = 0; // Set speed = 0 để không bị di chuyển khi nhảy
                ChangeAnim("FallSkill");
                StartCoroutine(WaitSkill());
           }
        }


        // Attack Skill logic
        if(Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend)
        {
            isAttack = true;
            AttackSkill();
        }
        
    
    }

    public void AttackSkill() // Skill animation
    {
        ChangeAnim("Skill");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse); 
    }

    public IEnumerator WaitSkill() //Delay skill animation
    {
        yield return new WaitForSeconds(0.7f);
        isAttack = false;
        speed = saveSpeed;
    }

}
