using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ludo.Core;
using Ludo.Data;
using Ludo.Events;
using Ludo.ScriptableObjects;
using UnityEngine;

namespace Ludo.Managers
{
    public class GotiManager : MonoBehaviour
    {
        [SerializeField] private Config config;
        [SerializeField] private GameObject gotiPrefab;
        [SerializeField] private List<PlayerData> players;

        public List<GameData> GameDataList { get; private set; }

        private LudoManager _ludoManager;
        private List<Goti> _goties;

        private void Awake()
        {
            _ludoManager = GetComponent<LudoManager>();
        }

        private void OnEnable() => LudoEvents.OnRoll += CheckMove;
        private void OnDisable() => LudoEvents.OnRoll -= CheckMove;

        public void CreateGoties()
        {
            _goties = new List<Goti>();
            GameDataList = new List<GameData>();

            foreach (var p in _ludoManager.PlayersPlaying)
            {
                var player = players.Find(x => x.Player == p);
                var data = new GameData(player.Player, player.ScoreText);
                GameDataList.Add(data);

                for (var count = _ludoManager.NumberOfGoti; count > 0; --count)
                {
                    var goti = Instantiate(gotiPrefab).GetComponent<Goti>();
                    goti.SetData(player, data, _ludoManager, this);
                    if (player.Player != _ludoManager.MyPlayer)
                    {
                        goti.DisableCollider();
                    }
                    _goties.Add(goti);
                }
            }
        }

        public List<Goti> FindGotiesByPlayer(Player player)
        {
            return _goties.Where(g => g.Player == player).ToList();
        }

        public bool CanGotiMove(int diceNumber, Tile sourceTile, Player player)
        {
            // If the source tile is not part of the player zone, movement is possible.
            if (!sourceTile.TileToMove.IsplayerZone) return true;

            // Get the next tile for the current player.
            var nextTile = sourceTile.TileToMove.GetNextTile(player);

            // If there is no next tile, ensure the dice number doesn't exceed the remaining path.
            if (nextTile == null)
            {
                if (diceNumber > 0)
                {
                    Debug.Log($"Can't Move: Dice number ({diceNumber}) exceeds remaining tiles.");
                    return false;
                }
                return true;
            }

            // Recursively check if the remaining dice number allows movement.
            --diceNumber;
            if (diceNumber == 0) return true;
            return CanGotiMove(diceNumber, nextTile, player);
        }


        private void CheckMove(int diceNumber, Player player)
        {
            var goties = FindGotiesByPlayer(player);
            var canMove = goties.Any(goti => CanGotiMove(diceNumber, goti.CurrentTile, player));

            if (!canMove)
            {
                Debug.Log("No valid moves available. Skipping turn.");
            }

            StartCoroutine(CheckTurn(canMove, diceNumber, player));
        }


        private IEnumerator CheckTurn(bool canMove, int diceNumber, Player player)
        {
            yield return null;
            if (!canMove || _ludoManager.CancelTurnOn3Sixes)
            {
                yield return new WaitForSeconds(config.DelayToChangeTurn);
                _ludoManager.ChangeTurn();
            }
            else
            {
                yield return new WaitForSeconds(config.DelayToMoveAI);
                LudoEvents.OnMove?.Invoke(diceNumber, player);
            }
        }


    }
}

