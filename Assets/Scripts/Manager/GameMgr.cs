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
    private static bool muteBackup;

    [Header("Current Game Info")]
    public GameState gameState = GameState.Title;
    public PlayerController Player = null;
    public int GameScore = 0;

    [Header("Importent Game Component")]
    public GameLogic GameLogic = null;
    public TitleLogic TitleLogic = null;

    protected override void InitializeSingleton()
    {
        SoundMgr.Instance.SetMute(muteBackup);
        gameState = GameState.Title;

        GameScore = 0;
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

        muteBackup = SoundMgr.isMute; 

        SoundMgr.Instance.SetMute(true);
    }
}
