using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceForGirl : MonoBehaviour
{
    private PolygonCollider2D polygon;
    private SpriteRenderer sprite;
    public bool isPlay = false;
    // Start is called before the first frame update
    void Start()
    {
        polygon =  GetComponent<PolygonCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isPlay)
        {
            UpdateCollider(); 
        }
        
    }

    void UpdateCollider()
    {
        if (polygon != null && sprite != null && sprite.sprite != null)
        {
            Destroy(polygon); // Xóa Collider cũ
            polygon = gameObject.AddComponent<PolygonCollider2D>(); // Tạo Collider mới
        }
    }

    public void PlayEffect(Vector3 position)
    {  
        isPlay = true;
        transform.position = position;  // Đặt vị trí của hiệu ứng
        Destroy(gameObject,2);
    }

    public IEnumerator DestroyEffect()
    {
        yield return new WaitForSeconds(0.45f);
        Destroy(gameObject);
    }
}
