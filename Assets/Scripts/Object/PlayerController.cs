using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector2 movementPos;
    Vector2 startMousePos;
    Vector2 startPlayerPos;
    Vector2 moveMouesePos;

    public float moveSpeed = 6.0f;
    float moveMaxX = 4f;
    float moveMinX = -4f;

    public int life = 0;
    private float lifeInterverPosY = 0.5f;
    public GameObject lifeStarPrefab;
    Stack<GameObject> lifeStarStack = new Stack<GameObject>();

    // Update is called once per frame
    void Update()
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

            if(movementPos.x > moveMaxX)
                movementPos.x = moveMaxX;
            else if(movementPos.x < moveMinX)
                movementPos.x = moveMinX;
        }

        transform.position = Vector2.Lerp(transform.position, movementPos, Time.deltaTime * moveSpeed);
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, -3f, 3f), -1f);
    }

    private void TailMove()
    {

    }

    public void AddLife(int lifeValue)
    {
        for(int i = 0; i < lifeValue; i++)
        {
            life++;

            GameObject lifeStar = Instantiate(lifeStarPrefab);
            lifeStar.transform.SetParent(transform);
            lifeStar.transform.position = transform.position + (Vector3.down * life * lifeInterverPosY);

            lifeStarStack.Push(lifeStar);
        }

        //TODO: life 텍스트 추가
    }

    public void RemoveLife()
    {
        --life;
        Destroy(lifeStarStack.Pop());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<WaveContent>(out var Content))
        {
            Content.TouchEvent.Invoke();
        }
    }
}
