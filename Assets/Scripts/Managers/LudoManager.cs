using System.Collections.Generic;
using Ludo.Data;
using Ludo.Events;
using Ludo.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ludo.Managers
{
    public class LudoManager : MonoBehaviour
    {
        [SerializeField] private Config config;
        [SerializeField] private Player myPlayer = Player.BLUE;
        [SerializeField] private List<Player> playersPlaying;

        public int DiceNumber { get; private set; }
        public int NumberOfGoti { get; private set; }
        public bool CancelTurnOn3Sixes => _multipleSixes == 3;
        public Player MyPlayer => myPlayer;
        public List<Player> PlayersPlaying => playersPlaying;

        private Player _currentPlayer;
        private GotiManager _gotiManager;
        private PersistentData _persistentData;
        private int _currentPlayerIndex = 0;
        private int _multipleSixes = 0;
        private bool _isGameOver;

        private void Awake()
        {
            Time.timeScale = config.TimeScale;

            _persistentData = FindObjectOfType<PersistentData>();
            if (_persistentData != null)
            {
                playersPlaying = _persistentData.GetPlayers();
                NumberOfGoti = _persistentData.NumberOfGoti;
            }
            else
            {
                NumberOfGoti = config.NumberOfGoti;
            }

            _gotiManager = GetComponent<GotiManager>();
            _gotiManager.CreateGoties();
        }

        private void Start()
        {
            _currentPlayer = playersPlaying[_currentPlayerIndex];
            LudoEvents.OnTurnChange?.Invoke(_currentPlayer);
        }

        private void OnEnable() => LudoEvents.OnRoll += SetDice;
        private void OnDisable() => LudoEvents.OnRoll -= SetDice;

        private void SetDice(int diceNumber, Player _)
        {
            DiceNumber = diceNumber;
            if (DiceNumber == 6)
            {
                ++_multipleSixes;
            }
        }

        public bool IsMyTurn(Player player) => player == _currentPlayer;

        public void ChangeTurn(bool multipleTurn = false)
        {
            if (_isGameOver)
            {
                Debug.Log("Can't change turn anymore - Game is already over!");
                return;
            }

            if (multipleTurn)
            {
                LudoEvents.OnTurnChange?.Invoke(_currentPlayer);
                return;
            }

            if (DiceNumber == 6 && _multipleSixes < 3)
            {
                LudoEvents.OnTurnChange?.Invoke(_currentPlayer);
                return;
            }

            DiceNumber = 0;
            _multipleSixes = 0;

            ++_currentPlayerIndex;
            if (_currentPlayerIndex == playersPlaying.Count)
            {
                _currentPlayerIndex = 0;
            }
            _currentPlayer = playersPlaying[_currentPlayerIndex];
            LudoEvents.OnTurnChange?.Invoke(_currentPlayer);
            AudioManager.Instance.Play(AudioName.TURN);
        }

        public void SetGameOver()
        {
            _isGameOver = true;
            Invoke(nameof(OnGameOver), config.DelayBeforeGameOver);
        }

        private void OnGameOver()
        {
            _persistentData.SetGameData(_gotiManager.GameDataList);
            SceneManager.LoadScene("GameOver");
        }

        //test
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G) || Input.GetKeyDown(KeyCode.Escape))
            {
                SetGameOver();
            }
        }

    }
}