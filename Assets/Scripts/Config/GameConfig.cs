using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfig
{
    public const int FILED_WIDHT_SIZE = 5;
    public const int FILED_HEIGHT_SIZE = 7;

    public const float FEVER_TIME = 8;
    public const float FEVER_UP = 1.5f;
    public static readonly Color[] FEVER_COLORS = 
    {
        new Color(0.9f, 0.35f, 0.35f),
        new Color(1f, 0.73f, 0f),
        new Color(0.35f, 0.9f, 0.35f),
        new Color(0.4f, 0.6f, 1f),
        new Color(0.7f, 0.35f, 0.9f)
    };

    public static readonly Vector2 PLAYER_CRUSH_SIZE = new Vector3(0.55f, 0.2f, 1f);
}
