using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HammerMan: MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private LayerMask Ground;
    private bool isGround = false;
    [SerializeField] private float speed;
    private Vector3 move = Vector3.zero;
    private string currentAnim;
    [SerializeField] private float jumpForce;
    private bool isJumping = false;
    private bool isAttack = false;
    public float saveSpeed; // Lưu speed hiện tại
    private bool isDefend = false;
    void Start()
    {
        move = Vector3.zero;
        saveSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        isGround = checkGround();

        // Jumping logic
        if (Input.GetKeyDown(KeyCode.J) && isGround)
        {
            Jump();
        }

        // Falling logic
        if (!isGround && rb.velocity.y < 0)
        {
           if(!isAttack)
           {
               ChangeAnim("JumpOut");
               isJumping = false;
           }
           else if(isAttack)
           {
                speed = 0; // Set speed = 0 để không bị di chuyển khi nhảy
                ChangeAnim("SkillOut");
                StartCoroutine(WaitSkill());
           }
        }

        // Defend logic
        Defend();
        
        // Attack Normal logic 
        if(Input.GetKeyDown(KeyCode.K) && isGround && !isAttack && !isDefend)
        {
            Attack();
        }

        // Attack Skill logic
        if(Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend)
        {
            isAttack = true;
            AttackSkill();
        }
        
        // Horizontal movement
        move.x = Input.GetAxisRaw("Horizontal");
        transform.position += move * Time.deltaTime * speed;

        
        // Running animation
        if (move.x != 0 && isGround && !isAttack && !isJumping && !isDefend)
        {
            Run();
        }

        // Idle animation
        if (move.x == 0 && isGround && !isJumping && !isAttack && !isDefend)
        {
            ChangeAnim("Idle");
        }
    
    }

    private bool checkGround() //Kiểm tra xem đã chạm mặt đất chưachưa
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, Ground);
        Debug.DrawRay(transform.position, Vector2.down * 1.1f, Color.red); // Debugging ray to see in editor
        return hit.collider != null;
    }

    private void ChangeAnim(string animName) //Thay đổi animation
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
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

    public void Defend() // Defend animation
    {
        if(Input.GetKeyDown(KeyCode.U) && isGround && !isAttack && !isDefend)
        {
            speed = 1.0f;
            ChangeAnim("Defend");
            isDefend = true;
        }
        else if(Input.GetKeyUp(KeyCode.U) && isGround && !isAttack && isDefend)
        {
            speed = saveSpeed;
            isDefend = false;
        }
    }
    
    public void Run()   // Running animation
    {
        ChangeAnim("Run");
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    public void Attack()    // Attack animation
    {
        isAttack = true;
        ChangeAnim("HitNormal");
        StartCoroutine(WaitAttack());
    }

    public IEnumerator WaitAttack() //Delay attack animation
    {
        yield return new WaitForSeconds(0.45f);
        isAttack = false;
    }
    public void Jump() // Jumping animation
    {
        isJumping = true;
        ChangeAnim("Jumping");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }


}
