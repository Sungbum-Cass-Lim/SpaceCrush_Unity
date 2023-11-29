using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementPos;
    Vector2 startMousePos;
    Vector2 startPlayerPos;
    Vector2 moveMouesePos;

    public float moveSpeed = 7f;
    public float moveMaxX = 4f;
    public float moveMinX = -4f;

    public bool isWall = false;
    public Vector2 wallPos;

    private int life = 0;
    private float lifeInterverPosY = 0.5f;
    public GameObject lifeStarPrefab;
    public TextMeshPro lifeText;

    public List <GameObject> moveLifeList = new List<GameObject>();
    Stack <GameObject> currentLifeStack = new Stack<GameObject>();

    private void Start()
    {
        life++;
        currentLifeStack.Push(gameObject);
        moveLifeList.Add(gameObject);

        AddLife(10);// WaveMgr.Instance.BlockData.playerLife - 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMove();
        TailMove();
    }

    private void PlayerMove()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            startPlayerPos = transform.position;
        }
        if (Input.GetMouseButton(0))
        {
            moveMouesePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            movementPos = startPlayerPos + (moveMouesePos - startMousePos);

            if (movementPos.x > moveMaxX)
                movementPos.x = moveMaxX;
            else if (movementPos.x < moveMinX)
                movementPos.x = moveMinX;
        }

        //TODO: Wall에 대한 Player 움직임 만들어야함
        if(isWall == true && wallPos.x > moveMouesePos.x)
        {
            moveMouesePos.x += 1.0f;
        }
        else if(isWall == true && wallPos.x < moveMouesePos.x)
        {
            moveMouesePos.x -= 1.0f;
        }

        transform.position = Vector2.Lerp(transform.position, movementPos, Time.deltaTime * moveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -3f, 3f), -1f);
    }

    private void TailMove()
    {
        for(int i = moveLifeList.Count - 1; i > 0; i--)
        {
            if (moveLifeList[i])
            {
                float moveToPosX = moveLifeList[i - 1].transform.position.x;
                float moveToPosY = moveLifeList[i].transform.position.y;
                Vector2 moveToPos = new Vector2 (moveToPosX, moveToPosY);

                moveLifeList[i].transform.position = Vector2.Lerp(moveLifeList[i].transform.position, moveToPos, Time.deltaTime * moveSpeed * 1.5f);
            }
            else
            {
                moveLifeList.RemoveAt(i);
            }
        }
    }

    public void AddLife(int lifeValue)
    {
        for (int i = 0; i < lifeValue; i++)
        {
            life++;

            GameObject lifeStar = Instantiate(lifeStarPrefab);
            lifeStar.transform.position = currentLifeStack.Peek().transform.position + (Vector3.down * lifeInterverPosY);

            moveLifeList.Add(lifeStar);
            currentLifeStack.Push(lifeStar);
        }

        lifeText.text = life.ToString();
    }

    public void RemoveLife()
    {
        if (currentLifeStack.TryPop(out var lifeStar))
        {
            life--;

            lifeText.text = life.ToString();
            Destroy(lifeStar);
        }
        else
            Debug.Log("Game Over");
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WaveContent>(out var Content))
        {
            Content.TouchEvent.Invoke();
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<WallObj>(out var Content))
        {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isWall = false;
            wallPos = Vector2.zero;
        }
    }
}
