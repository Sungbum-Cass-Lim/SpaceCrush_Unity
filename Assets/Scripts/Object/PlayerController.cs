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

    public Vector2 startMousePos;
    public Vector2 startPlayerPos;
    public Vector2 moveMouesePos;
    public Vector2 movementPos;

    private float originMoveSpeed = 8f;
    private float currentMoveSpeed;

    public WallObj CurrentWall;
    public bool isWall = false;
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
        PlayerCrush();
        PlayerFever();
    }

    void FixedUpdate()
    {
        PlayerMove();
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

    private void PlayerCrush()
    {
        if (crushPushTime > 0)
        {
            crushPushTime -= Time.deltaTime;
            playerCharacter.transform.localScale = Vector2.Lerp(playerCharacter.transform.localScale, GameConfig.PLAYER_CRUSH_SIZE, 13 * Time.deltaTime);
        }
        else
        {
            crushPushTime = 0;
            playerCharacter.transform.localScale = Vector2.Lerp(playerCharacter.transform.localScale, playerOriginSize, 13 * Time.deltaTime);
        }
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

    private void PlayerMove()
    {

        transform.position = Vector2.Lerp(transform.position, movementPos, Time.deltaTime * currentMoveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, moveMinX, moveMaxX), -1f);

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

            if(playerLife <= 15)
            {
                GameObject lifeObj = ObjectPoolMgr.Instance.Load<Transform>(PoolObjectType.Player, "LifeTail").gameObject;
                lifeObj.transform.position = currentTailList[currentTailList.Count - 1].transform.position + (Vector3.down * lifeInterverPosY);

                currentTailList.Add(lifeObj);
            }
        }

        lifeText.text = playerLife.ToString();
    }

    public void RemoveLife()
    {
        playerLife--;

        //TODO: Player 종료 지점 처리 구현 필요
        if (playerLife <= 0)
        {
            ObjectPoolMgr.Instance.ReleasePool(gameObject);
            WaveMgr.Instance.SendLog();

            return;
        }

        if (playerLife < 15)
        {
            StartCoroutine(RemoveLifeEffect(currentTailList[currentTailList.Count - 1].transform));

            ObjectPoolMgr.Instance.ReleasePool(currentTailList[currentTailList.Count - 1]);
            currentTailList.RemoveAt(currentTailList.Count - 1);
        }

        lifeText.text = playerLife.ToString();
    }

    IEnumerator RemoveLifeEffect(Transform removeLifePos)
    {
        Transform removeLifeEffect = ObjectPoolMgr.Instance.Load<Transform>(PoolObjectType.Effect, "LifeTailRemove");
        removeLifeEffect.position = removeLifePos.position;

        yield return YieldCacheMgr.WaitForSeconds(0.5f);

        ObjectPoolMgr.Instance.ReleasePool(removeLifeEffect.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            Content.TouchEvent.Invoke();

            //TODO: 줄일 필요 있음
            if ((Content.contentType == ContentType.Block || Content.contentType == ContentType.Obstacle) && GameMgr.Instance.GameLogic.feverTime <= 0)
            {
                crushPushTime = 0.15f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            if (Content.contentType == ContentType.Wall && Content == CurrentWall)
            {
                startPlayerPos = transform.position;
                startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                //TODO: EndPoint Reset(스타트 포인트만 리셋해서 엔드포인트가 끝에 잡혀있으면 혼자 움직이는게 보인다.)
                moveMouesePos = startMousePos;
                movementPos = startPlayerPos;

                moveMaxX = 3f;
                moveMinX = -3f;
            }
        }
    }
}
