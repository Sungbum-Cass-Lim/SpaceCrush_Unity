using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Title = 0,
    Game,
    GameEnd,
}

public class GameMgr : SingletonComponentBase<GameMgr>
{
    public GameState gameState = GameState.Title;
    public PlayerController Player = null;

    protected override void InitializeSingleton()
    {
        gameState = GameState.Title;
    }

    public void GameStart(PlayerController CurPlayer)
    {
        gameState = GameState.Game;
        Player = CurPlayer;
    }

    public void GameOver()
    {
        gameState = GameState.GameEnd;
    }
}
