using System.Collections;
using UnityEngine;
using TMPro;
using static GameController;
using UnityEngine.Localization;
using System.Collections.Generic;

namespace Singleplayer
{
    public class AIMatchController : MonoBehaviour, IMatchController
    {
        #region Variables
        private GameController _gc;
        public GameState gameState = GameState.Initial;
        public GameMode gameMode;
        private Coroutine countdownCoroutine;

        public PlatformController playerPlatform; // player in game with bots
        public PlatformController botPlatform;
        public IPlatformController Pitcher { get; set; }
        public GameObject LastTouched { get; set; }
        public LocalizedString winnerText = new LocalizedString("PongLocalization", "WinnerText");
        public GameState GameState
        {
            get { return gameState; }
            set { gameState = value; }
        }
        #endregion

        private void Awake()
        {
            _gc = GetComponent<GameController>();
        }

        #region Connection handlers

        public void EnterMatch()
        {
            gameMode = (GameMode)PlayerPrefs.GetInt("GameMode");

            _gc.menuPanel.SetActive(false);
            _gc.leaveButton.SetActive(true);
            _gc.startButton.SetActive(true);
            _gc.ToggleControls(true);

            var player = Instantiate(_gc.playerPrefab, _gc.playerPrefab.transform.position, _gc.playerPrefab.transform.rotation);
            player.tag = "Player1";
            playerPlatform = player.AddComponent<PlatformController>();
            playerPlatform.SetUp(0);
            playerPlatform.SetColor(0);
            player.AddComponent<InputController>();

            LastTouched = player;
            Pitcher = playerPlatform;

            var ball = Instantiate(_gc.ballPrefab, Pitcher.GetBallStartPosition(), Quaternion.identity);
            _gc.ballController = ball.GetComponent<BallController>();
            if (gameMode == GameMode.Accuracy)
            {
                _gc.ballController.ChangeMaterial(LastTouched.tag);
            }

            var bot = Instantiate(_gc.aiPlayerPrefab, _gc.aiPlayerPrefab.transform.position, _gc.aiPlayerPrefab.transform.rotation);
            bot.tag = "Player2";
            botPlatform = bot.AddComponent<PlatformController>();
            botPlatform.Name = "Benjamin Bot";
            botPlatform.SetUp(1);
            botPlatform.SetColor(1);

            _gc.scoreHandler.InitScore();
            GetComponent<PowerUpsManager>().SetUpPowerUpsTrigger();

            gameState = GameState.Prepare;
        }

        public void ExitMatch()
        {
            GetComponent<PowerUpsManager>().ClearPowerUps();
            _gc.scoreHandler.ClearScores();
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            if (_gc.ballController != null)
                Destroy(_gc.ballController.gameObject);
            Destroy(playerPlatform.gameObject);
            Destroy(botPlatform.gameObject);
        }
        #endregion

        #region Round handling
        public bool MovementAllowed()
        {
            return gameState == GameState.Play;
        }

        /// <summary>
        /// Called on the server
        /// </summary>
        public void StartRound()
        {
            if (_gc.debugMode)
                return;
            playerPlatform.ResetPlatform();
            botPlatform.ResetPlatform();
            _gc.startButton.SetActive(false);

            _gc.ballController.ResetBall();
            StartCoroutine(StartAfterCountdown());
        }

        private IEnumerator StartAfterCountdown()
        {
            _gc.PrepareForRound();
            countdownCoroutine = StartCoroutine(_gc.countdownHandler.CountDown());
            yield return countdownCoroutine;

            _gc.ToggleControlsInteraction(true);
            gameState = GameState.Play;
            if (!_gc.debugMode)
            {
                _gc.ballController.LaunchBall();
            }
        }

        public void FinishRound()
        {
            _gc.ToggleControlsInteraction(false);
            _gc.ballController.StopBall();
            gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
            gameState = GameState.Prepare;
            _gc.startButton.SetActive(true);
        }

        public void FinishMatch(string winnerName)
        {
            GetComponent<PowerUpsManager>().ClearPowerUps();
            _gc.scoreHandler.ClearScores();
            WinnerNotification(winnerName);
        }

        public void WinnerNotification(string winner)
        {
            winnerText.Arguments = new object[] { new Dictionary<string, string>() { { "winner", winner } } };
            _gc.winnerPanel.GetComponentInChildren<TextMeshProUGUI>().text = winnerText.GetLocalizedString();
            _gc.winnerPanel.SetActive(true);
        }
        #endregion

        #region Ingame handlers
        /// <summary>
        /// Handler for accuracy mode only
        /// </summary>
        public void PlatformTouchHandler(GameObject platform)
        {
            if (gameState != GameState.Play) return;

            LastTouched = platform;
            if (_gc.gameMode == GameMode.Accuracy)
                _gc.ballController.ChangeMaterial(LastTouched.tag);
            GetComponent<PowerUpsManager>().TriggerPowerUp();
        }

        public void BackWallTouchHandler(string playerTag)
        {
            if (gameMode == GameMode.Classic)
            {
                FinishRound();
                bool hasWinner = _gc.scoreHandler.SUpdateScore(playerTag);
                PlatformController winner = playerPlatform.CompareTag(playerTag) ? playerPlatform : botPlatform;
                Pitcher = winner;

                if (hasWinner)
                {
                    FinishMatch(winner.GetComponent<PlatformController>().Name);
                }
            }
            else if (gameMode == GameMode.Accuracy)
            {
                LastTouched = playerPlatform.CompareTag(playerTag) ? botPlatform.gameObject : playerPlatform.gameObject;
                _gc.ballController.ChangeMaterial(playerTag);
            }
        }

        public void PowerUpTouchHandler()
        {
            if (gameMode != GameMode.Accuracy) return;

            bool hasWinner = _gc.scoreHandler.SUpdateScore(LastTouched.tag);
            if (hasWinner)
            {
                FinishRound();
                FinishMatch(LastTouched.GetComponent<PlatformController>().Name);
            }
        }
        #endregion
    }
}
