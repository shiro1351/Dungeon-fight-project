using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ThanhMau : MonoBehaviour
{
    public Image thanhMau;
    public void updateBlood(float luongMauHienTai, float luongMauToiDa)
    {
        thanhMau.fillAmount = luongMauHienTai / luongMauToiDa;
    }
}
