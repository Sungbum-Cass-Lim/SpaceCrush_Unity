using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerCharacter;
    private MeshRenderer playerCharacterRender;

    private Vector2 movementPos;
    private Vector2 startMousePos;
    private Vector2 startPlayerPos;
    private Vector2 moveMouesePos;

    private float originMoveSpeed = 7f;
    private float currentMoveSpeed;

    private float mouseMaxX = 4f;
    private float mouseMinX = -4f;
    public float moveMaxX = 3f;
    public float moveMinX = 3f;

    public TextMeshPro lifeText;
    private int playerLife = 0;
    private float lifeInterverPosY = 0.5f;
    private List<GameObject> currentTailList = new List<GameObject>();

    private float feverColorType = 0;

    private Vector2 playerOriginSize;
    private float crushPushTime = 0.0f;

    private void Start()
    {
        playerOriginSize = playerCharacter.transform.localScale;
        playerCharacterRender = playerCharacter.GetComponent<MeshRenderer>();

        playerLife++;
        currentTailList.Add(gameObject);

        currentMoveSpeed = originMoveSpeed;

        AddLife(WaveMgr.Instance.BlockData.playerLife - 1);
    }

    private void Update()
    {
        PlayerLerpPos();
    }

    void FixedUpdate()
    {
        PlayerMove();
        PlayerCrush();
        PlayerFever();
        TailMove();
    }

    private void PlayerLerpPos()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPlayerPos = transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            moveMouesePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            movementPos = startPlayerPos + (moveMouesePos - startMousePos);

            if (movementPos.x > mouseMaxX)
                movementPos.x = mouseMaxX;

            else if (movementPos.x < mouseMinX)
                movementPos.x = mouseMinX;
        }
    }

    private void PlayerMove()
    {
        transform.position = Vector2.Lerp(transform.position, movementPos, Time.deltaTime * currentMoveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, moveMinX, moveMaxX), -1f);
    }

    private void PlayerCrush()
    {
        if (crushPushTime > 0)
        {
            crushPushTime -= Time.deltaTime;
            playerCharacter.transform.localScale = Vector2.Lerp(playerCharacter.transform.localScale, GameConfig.PLAYER_CRUSH_SIZE, 0.2f);
        }
        else
            playerCharacter.transform.localScale = Vector2.Lerp(playerCharacter.transform.localScale, playerOriginSize, 0.2f);

    }

    private void PlayerFever()
    {
        if (GameMgr.Instance.GameLogic.feverTime > 0)
        {
            currentMoveSpeed = originMoveSpeed + GameConfig.FEVER_UP;

            feverColorType += Time.deltaTime;
            int idx = (int)Mathf.Floor(feverColorType);

            if (idx >= GameConfig.FEVER_COLORS.Length - 2)
                feverColorType = 0;

            float fract = feverColorType - idx;
            playerCharacterRender.material.color = Color.Lerp(GameConfig.FEVER_COLORS[idx], GameConfig.FEVER_COLORS[idx + 1], fract);
        }

        else
        {
            currentMoveSpeed = originMoveSpeed;
            playerCharacterRender.material.color = Color.Lerp(playerCharacterRender.material.color, Color.white, 0.2f);
        }
    }

    private void TailMove()
    {
        //Tail Move
        for (int i = currentTailList.Count - 1; i > 0; i--)
        {
            float moveToPosX = currentTailList[i - 1].transform.position.x;
            float moveToPosY = currentTailList[i].transform.position.y;
            Vector2 moveToPos = new Vector2(moveToPosX, moveToPosY);

            currentTailList[i].transform.position = Vector2.Lerp(currentTailList[i].transform.position, moveToPos, Time.deltaTime * currentMoveSpeed * 1.5f);
        }
    }

    public int GetLife()
    {
        return playerLife;
    }

    public void AddLife(int life)
    {
        for (int i = 0; i < life; i++)
        {
            playerLife++;

            GameObject lifeObj = ObjectPoolMgr.Instance.Load<Transform>(PoolObjectType.Player, "LifeTail").gameObject;
            lifeObj.transform.position = currentTailList[currentTailList.Count - 1].transform.position + (Vector3.down * lifeInterverPosY);

            currentTailList.Add(lifeObj);
        }

        lifeText.text = playerLife.ToString();
    }

    public void RemoveLife()
    {
        playerLife--;

        //TODO: Player 종료 지점 처리 구현 필요
        if (playerLife <= 0)
        {
            WaveMgr.Instance.TestFunc();
            ObjectPoolMgr.Instance.ReleasePool(gameObject);

            return;
        }

        ObjectPoolMgr.Instance.ReleasePool(currentTailList[currentTailList.Count - 1]);
        
        currentTailList.RemoveAt(currentTailList.Count - 1);

        lifeText.text = playerLife.ToString();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            Content.TouchEvent.Invoke();

            if (Content.contentType == ContentType.Block && GameMgr.Instance.GameLogic.feverTime <= 0)
            {
                crushPushTime = 0.15f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            if (Content.contentType == ContentType.Wall)
            {
                startPlayerPos = transform.position;
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                moveMaxX = 3f;
                moveMinX = -3f;
            }
        }
    }
}
