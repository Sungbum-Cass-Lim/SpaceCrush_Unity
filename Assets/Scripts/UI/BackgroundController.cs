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

    private float gearSpeed = 90;
    private float feverSpeed = 1;

    private void Awake()
    {
        foreach (var bgData in bgDataList)
        {
            Material material = new Material(bgEffctShader);
            bgData.Renderer.material = material;
        }
    }

    private void Update()
    {
        foreach (var gears in bgGearList)
        {
            if (gears.anchoredPosition.y <= -960)
                gears.anchoredPosition = new Vector2(gears.anchoredPosition.x, 960);

            gears.anchoredPosition -= new Vector2(0, Time.deltaTime * gearSpeed * feverSpeed);
            gears.Rotate(Vector3.forward * Time.deltaTime * 30);
        }

        foreach (var bgData in bgDataList)
        {
            bgData.Renderer.material.mainTextureOffset += Vector2.up * bgData.moveSpeed * Time.deltaTime * feverSpeed;
        }
    }

    public void BgFeverStart()
    {
        feverSpeed = GameConfig.FEVER_UP;   
    }

    public void BgFeverEnd()
    {
        feverSpeed = 1;
    }
}
