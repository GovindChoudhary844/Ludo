using Ludo.Data;
using Ludo.Managers;
using Ludo.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Core
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] private Config config;
        [SerializeField] private bool isSafeTile;
        [SerializeField] private bool isLastTile;
        [SerializeField] private MoveTile nextTile;

        public bool LastTile => isLastTile;
        public MoveTile TileToMove => nextTile;

        private List<Goti> _goties;

        private LudoManager _ludoManager;

        private void Awake()
        {
            _ludoManager = FindObjectOfType<LudoManager>();
        }

        public void AddGoti(Goti goti)
        {
            if (_goties == null) _goties = new List<Goti>();

            _goties.Add(goti);
            TransformGoties();
        }

        public void RemoveGoti(Goti goti)
        {
            if (_goties.Contains(goti))
            {
                _goties.Remove(goti);
            }
            TransformGoties();
        }

        private void TransformGoties()
        {
            if (_goties.Count == 1)
            {
                PositionSingleGoti(_goties[0]);
            }
            else
            {
                PositionMultipleGoties();
            }
        }

        private void PositionSingleGoti(Goti goti)
        {
            goti.transform.position = transform.position + config.gotiOffset;
            goti.ResetScale();
        }

        private void PositionMultipleGoties()
        {
            for (var index = 0; index < _goties.Count; ++index)
            {
                Goti goti = _goties[index];
                goti.transform.localScale = config.GotiScaleOnTile;

                Vector3 gotiPos = transform.position + config.gotiOffset;
                gotiPos.x += ((index % 2 == 0) ? -1 : 1) * config.GotiOffsetOnTile;

                if (_goties.Count > 2)
                {
                    gotiPos.y += ((index < 2) ? -1 : 1) * config.GotiOffsetOnTile;
                }

                goti.transform.position = gotiPos;
            }
        }

        public (bool isKilled, Goti gotiKilled) CheckKill(Player strikingPlayer)
        {
            if (_goties.Count < 2 || isSafeTile)
                return (false, null);

            if (_goties.Count == 2)
            {
                foreach (var goti in _goties)
                {
                    if (goti.Player != strikingPlayer)
                    {
                        return (true, goti);
                    }
                }
            }

            return (false, null);
        }

        public bool CheckGameOver()
        {
            if(_goties.Count == _ludoManager.NumberOfGoti && isLastTile) return true;
            return false;
        }

    }
}
