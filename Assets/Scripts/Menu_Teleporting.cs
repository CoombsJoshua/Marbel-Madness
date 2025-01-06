using System.Collections;
using OriginLabs;
using UnityEngine;

public class Menu_Teleporting : Menu_
{
    public override MenuType MenuType => MenuType.Teleporting;

    void OnEnable(){
        StartCoroutine(CloseTeleport());
    }

    public IEnumerator CloseTeleport(){
        yield return new WaitForSeconds(2);
        Userinterface.Instance.m_CanvasManager.SwitchCanvas(MenuType.None);
    }
}
