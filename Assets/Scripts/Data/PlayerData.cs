using Ludo.Core;
using System;
using TMPro;
using UnityEngine;

namespace Ludo.Data
{
    [Serializable]
    public enum Player
    {
        None,
        BLUE,
        RED,
        GREEN,
        YELLOW,
    }

    [Serializable]
    public class PlayerData
    {
        [SerializeField] private Player player;
        [SerializeField] private Color32 color;
        [SerializeField] private Tile sourceTile;
        [SerializeField] private TextMeshPro scoreText;

        public Player Player => player;
        public Color32 Color => color;
        public Tile SourceTile => sourceTile;
        public TextMeshPro ScoreText => scoreText;
    }

}
