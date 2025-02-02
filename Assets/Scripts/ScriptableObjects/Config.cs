using System.Collections.Generic;
using UnityEngine;

namespace Ludo.ScriptableObjects
{
    [CreateAssetMenu(menuName = "ludo/Config")]
    public class Config : ScriptableObject
    {
        [Header("Tile")]
        public Vector3 GotiScaleOnTile;
        public float GotiOffsetOnTile;

        [Header("Goti")]
        public Vector3 gotiOffset;
        public float DelayToMove;
        public float DelayToKill;

        [Header("Game")]
        public float DelayToChangeTurn;
        public float DelayToMoveAI;
        public float DelayBeforeKill;
        public float DelayToShowDice;
        public float DelayToRoll;
        public float DelayBeforeGameOver;
        public float TimeScale;
        public int NumberOfGoti;
        public int MinDiceNumber;
        public int MaxDiceNumber;

        [Header("Score")]
        public int MovePoints;
        public int KillPoints;
        public int GoalPoints;

        [Header("Sprites")]
        public List<Sprite> DiceNumbers;
        public Sprite DiceDefault;
    }
}
