using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public Shader bgEffctShader;
    public List<BGData> bgDataList = new List<BGData>();
    public List<RectTransform> bgGearList = new List<RectTransform>();

    public float feverLerpTime = 0;

    private void Awake()
    {
        foreach(var bgData in bgDataList)
        {
            Material material = new Material(bgEffctShader);

            bgData.Renderer.material = material;
            material.SetFloat("_MoveSpeed", bgData.moveSpeed);
        }
    }

    private void Update()
    {
        foreach (var gears in bgGearList)
        {
            if (gears.anchoredPosition.y <= -960)
                gears.anchoredPosition = new Vector2(gears.anchoredPosition.x, 960);

            gears.anchoredPosition -= new Vector2(0, Time.deltaTime * 90);
            gears.Rotate(Vector3.forward * Time.deltaTime * 30);
        }

        foreach (var bgData in bgDataList)
        {
            Material material = bgData.Renderer.material;
        }
    }

    public void BgFeverStart()
    {
        foreach (var bgData in bgDataList)
        {
            Material material = bgData.Renderer.material;
            material.SetFloat("_MoveSpeed", bgData.moveSpeed * GameConfig.FEVER_UP);
        }
    }

    public void BgFeverEnd()
    {
        foreach (var bgData in bgDataList)
        {
            Material material = bgData.Renderer.material;
            material.SetFloat("_MoveSpeed", bgData.moveSpeed);
        }
    }
}
