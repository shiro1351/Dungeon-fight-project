using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lightman: Character
{
    [SerializeField] private Bullet bullet;
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
       
       base.Update();
    }

    protected override void Skill() // Skill animation
    {
        if (Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend) // Kiểm tra nếu nhân vật chưa tấn công
        {
        isAttack = true; // Đánh dấu đang tấn công
        ChangeAnim("Skill");
        StartCoroutine(WaitSkill());
        StartCoroutine(Fire());
        }
    }

public IEnumerator Fire()
{
    yield return new WaitForSeconds(0.28f);
    float direction = Mathf.Sign(transform.localScale.x);
    
    Bullet newbullet = Instantiate(bullet); // Tạo một viên đạn mới
    newbullet.MoveBullet(transform.position + Vector3.right * direction, direction);
}


    public IEnumerator WaitSkill() //Delay skill animation
    {
        yield return new WaitForSeconds(0.7f);
        isAttack = false;
        speed = saveSpeed;
    }
    
    protected override void Defend() // Defend animation
    {
        if(Input.GetKeyDown(KeyCode.U) && isGround && !isAttack && !isDefend)
        {
            ChangeAnim("Defend");
            isDefend = true;
            speed = 0;
            Invoke("WaitPrepareToTeleport",0.7f);
        }
    }

    public void WaitPrepareToTeleport()
    {
        float direction = Mathf.Sign(transform.localScale.x); 
        transform.position = transform.position + new Vector3(3,0,0) * direction;
        StartCoroutine(Teleportationation());
    }
    public IEnumerator Teleportationation()
    {
        yield return new WaitForSeconds(2.04f - 0.4f);
        isDefend = false;
        speed = saveSpeed;
    }

}
