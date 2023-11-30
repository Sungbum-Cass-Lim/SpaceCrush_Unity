using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

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
    public GameLogic GameLogic = null;
    public TitleLogic TitleLogic = null;

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
        Player = null;
    }
}
