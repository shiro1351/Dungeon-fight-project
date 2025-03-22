using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Lightman: MonoBehaviour
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
    [SerializeField] private Bullet bullet;
    private ThanhMau thanhmau;
    [SerializeField] private float luongMauToiDa = 10;
    private float luongMauHienTai;
    void Start()
    {
        move = Vector3.zero;
        saveSpeed = speed;
        luongMauHienTai = luongMauToiDa;
        thanhmau = FindObjectOfType<ThanhMau>(); // Tìm và gán đối tượng ThanhMau
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player is grounded
        isGround = checkGround();

        // Jumping logic
        if (Input.GetKeyDown(KeyCode.J) && isGround && luongMauHienTai >0)
        {
            Jump();
        }

        // Falling logic
        if (!isGround && rb.velocity.y < 0 && luongMauHienTai >0)
        {
               ChangeAnim("Fall");
               isJumping = false;
        }

        // Defend logic
        Defend();
        
        // Attack Normal logic 
        if(Input.GetKeyDown(KeyCode.K) && isGround && !isAttack && !isDefend && luongMauHienTai >0)
        {
            Attack();
        }

        // Attack Skill logic
        if(Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend && luongMauHienTai >0)
        {
            isAttack = true;
            AttackSkill();
        }
        
        // Horizontal movement
        move.x = Input.GetAxisRaw("Horizontal");
        transform.position += move * Time.deltaTime * speed;

        
        // Running animation
        if (move.x != 0 && isGround && !isAttack && !isJumping && !isDefend && luongMauHienTai >0)
        {
            Run();
        }

        // Idle animation
        if (move.x == 0 && isGround && !isJumping && !isAttack && !isDefend && luongMauHienTai >0)
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
        StartCoroutine(WaitSkill());
        StartCoroutine(Fire());
    }

    public IEnumerator WaitSkill() //Delay skill animation
    {
        yield return new WaitForSeconds(0.7f);
        isAttack = false;
        speed = saveSpeed;
    }
    public IEnumerator Fire()
    {
        yield return new WaitForSeconds(0.28f);
        float direction = Mathf.Sign(transform.localScale.x); 
        Bullet newbullet = Instantiate(bullet);
        newbullet.MoveBullet(transform.position + Vector3.right *direction,direction);
    }
    public void Defend() // Defend animation
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
    public void Run()   // Running animation
    {
        ChangeAnim("Run");
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    public void Attack()    // Attack animation
    {
        isAttack = true;
        ChangeAnim("Attack");
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
        ChangeAnim("Jump");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Bot"))
        {
            luongMauHienTai -= 2;
            thanhmau.updateBlood(luongMauHienTai, luongMauToiDa); // Cập nhật thanh máu ngay lập tức
            Debug.Log("Đã va chạm với Bot, máu còn lại: " + luongMauHienTai);
            ChangeAnim("Hit");
        
            if (luongMauHienTai <= 0)
            {
                ChangeAnim("Dead");
                speed = 0;
                Debug.Log("Nhân vật đã chết!");
            }
        }
    }


}
