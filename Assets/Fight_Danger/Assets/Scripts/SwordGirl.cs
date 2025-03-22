using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SwordGirl : MonoBehaviour
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
    [SerializeField] private IceForGirl iceEffect; //Hieu ung bang
    void Start()
    {
        move = Vector3.zero;
        saveSpeed = speed;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGround = checkGround();

        if (Input.GetKeyDown(KeyCode.J) && isGround)
        {
            Jump();
        }

        if (!isGround && rb.velocity.y < 0)
        {
            ChangeAnim("Fall");
            isJumping = false;
        }

        Defend();

        if (Input.GetKeyDown(KeyCode.K) && isGround && !isAttack && !isDefend)
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend)
        {
            isAttack = true;
            AttackSkill();
        }

        move.x = Input.GetAxisRaw("Horizontal");
        transform.position += move * Time.deltaTime * speed;

        if (move.x != 0 && isGround && !isAttack && !isJumping && !isDefend)
        {
            Run();
        }

        if (isGround && !isJumping && !isAttack && !isDefend && move.x == 0)
        {
            ChangeAnim("Idle");
        }
    }

    private bool checkGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, Ground);
        Debug.DrawRay(transform.position, Vector2.down * 2.0f, Color.red);
        return hit.collider != null;
    }

    private void ChangeAnim(string animName)
    {
        if (currentAnim != animName)
        {
            anim.ResetTrigger(currentAnim);
            currentAnim = animName;
            anim.SetTrigger(currentAnim);
        }
    }

    public void AttackSkill()
    {
        ChangeAnim("Skill");
        speed = 0f;
        isAttack =true;
        move = Vector3.zero;
        StartCoroutine(WaitSkill());
    }

    public IEnumerator WaitSkill()
    {
        yield return new WaitForSeconds(0.7f);
        isAttack = false;
        speed = saveSpeed;
        float direction = Mathf.Sign(transform.localScale.x); 
        Quaternion rotation = direction == 1 ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

        IceForGirl newEffect = Instantiate(iceEffect);

        Vector3 icePosition = transform.position + Vector3.right * direction + new Vector3(0, -0.5f, 0);
        newEffect.PlayEffect(icePosition);
    }



    public void Defend()
    {
        if (Input.GetKeyDown(KeyCode.U) && isGround && !isAttack && !isDefend)
        {
            speed = 1.0f;
            ChangeAnim("Defend");
            isDefend = true;
        }
        else if (Input.GetKeyUp(KeyCode.U) && isGround && !isAttack && isDefend)
        {
            speed = saveSpeed;
            isDefend = false;
        }
    }

    public void Run()
    {
        ChangeAnim("Run");
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }

    public void Attack()
    {
        isAttack = true;
        ChangeAnim("Attack");
        StartCoroutine(WaitAttack());
    }

    public IEnumerator WaitAttack()
    {
        yield return new WaitForSeconds(0.45f);
        isAttack = false;
    }

    public void Jump()
    {
        isJumping = true;
        ChangeAnim("Jump");
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        transform.localScale = new Vector3(Mathf.Sign(move.x) * 2.0f, 2.0f, 1);
    }
}
