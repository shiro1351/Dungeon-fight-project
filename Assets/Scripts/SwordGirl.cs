using System.Collections;
using UnityEngine;

public class SwordGirl : Character
{
    [SerializeField] private IceForGirl iceEffect; // Hiệu ứng băng

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Skill()
    {
        if (Input.GetKeyDown(KeyCode.I) && isGround && !isAttack && !isDefend)
        {
            isAttack = true;
            ChangeAnim("Skill");
            speed = 0f;
            StartCoroutine(WaitSkill());
        }
    }

    private IEnumerator WaitSkill()
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
}
