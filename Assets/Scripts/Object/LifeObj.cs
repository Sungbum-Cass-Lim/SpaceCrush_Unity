using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeObj : WaveContent
{
    public TextMeshPro lifeText;
    public int life;

    public void Initialize(int lifeValue)
    {
        this.life = lifeValue;
        lifeText.text = lifeValue.ToString();
    }

    protected override void OnTouch()
    {
        Debug.Log("Star Touch");
        GameMgr.Instance.Player.AddLife(life);

        DeleteEvent.Invoke();
    }

    protected override void OnDelete()
    {
        Destroy(gameObject);
    }
}
