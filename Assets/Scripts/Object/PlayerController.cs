using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject playerCharacter;
    private MeshRenderer playerCharacterRender;

    public Vector2 movementPos;
    public Vector2 startMousePos;
    public Vector2 startPlayerPos;
    public Vector2 moveMouesePos;

    public float moveSpeed = 7f;
    private float moveMaxX = 4f;
    private float moveMinX = -4f;

    public GameObject lifePrefab;
    public TextMeshPro lifeText;
    private int currentlife = 0;
    private float lifeInterverPosY = 0.5f;

    public float feverTime = 0.0f;
    private float feverColorType = 0;

    private Vector2 playerOriginSize;
    private float crushPushTime = 0.0f;

    private List<GameObject> moveLifeList = new List<GameObject>();
    private Stack<GameObject> currentlifeStack = new Stack<GameObject>();

    private void Start()
    {
        playerOriginSize = playerCharacter.transform.localScale;
        playerCharacterRender = playerCharacter.GetComponent<MeshRenderer>();

        currentlife++;
        currentlifeStack.Push(gameObject);
        moveLifeList.Add(gameObject);

        AddLife(WaveMgr.Instance.BlockData.playerLife - 1);
    }

    private void Update()
    {
        PlayerLerpPosTarget();
    }

    void FixedUpdate()
    {
        PlayerMove();
        PlayerCrush();
        PlayerFever();
        TailMove();
    }

    private void PlayerLerpPosTarget()
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

            if (movementPos.x > moveMaxX)
                movementPos.x = moveMaxX;

            else if (movementPos.x < moveMinX)
                movementPos.x = moveMinX;
        }
    }

    private void PlayerMove()
    {
        transform.position = Vector2.Lerp(transform.position, movementPos, Time.deltaTime * moveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -3f, 3f), -1f);
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
        if (feverTime > 0)
        {
            feverTime -= Time.deltaTime;

            feverColorType += Time.deltaTime;
            int idx = (int)Mathf.Floor(feverColorType);

            if (idx >= GameConfig.FEVER_COLORS.Length - 2)
                feverColorType = 0;

            float fract = feverColorType - idx;
            playerCharacterRender.material.color = Color.Lerp(GameConfig.FEVER_COLORS[idx], GameConfig.FEVER_COLORS[idx + 1], fract);
        }

        else
            playerCharacterRender.material.color = Color.Lerp(playerCharacterRender.material.color, Color.white, 0.2f);
    }

    private void TailMove()
    {
        for (int i = moveLifeList.Count - 1; i > 0; i--)
        {
            if (moveLifeList[i])
            {
                float moveToPosX = moveLifeList[i - 1].transform.position.x;
                float moveToPosY = moveLifeList[i].transform.position.y;
                Vector2 moveToPos = new Vector2(moveToPosX, moveToPosY);

                moveLifeList[i].transform.position = Vector2.Lerp(moveLifeList[i].transform.position, moveToPos, Time.deltaTime * moveSpeed * 1.5f);
            }
            else
            {
                moveLifeList.RemoveAt(i);
            }
        }
    }

    public void AddLife(int life)
    {
        for (int i = 0; i < life; i++)
        {
            currentlife++;

            GameObject lifeObj = Instantiate(lifePrefab);
            lifeObj.transform.position = currentlifeStack.Peek().transform.position + (Vector3.down * lifeInterverPosY);

            moveLifeList.Add(lifeObj);
            currentlifeStack.Push(lifeObj);
        }

        lifeText.text = currentlife.ToString();
    }

    public void RemoveLife()
    {
        if (currentlifeStack.TryPop(out var lifeObj))
        {
            currentlife--;

            lifeText.text = currentlife.ToString();
            Destroy(lifeObj);
        }
        else
            Debug.Log("Game Over");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            Content.TouchEvent.Invoke();

            if (Content.contentType == ContentType.Block && feverTime <= 0)
            {
                crushPushTime = 0.15f;
            }
        }
    }
}