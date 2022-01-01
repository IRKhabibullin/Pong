using System.Collections;
using UnityEngine;
using TMPro;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using static GameController;

namespace Multiplayer
{
    public class NetworkMatchController : NetworkBehaviour, IMatchController
    {
        #region Variables
        private GameController _gc;
        private Coroutine countdownCoroutine;
        public NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.Initial);

        public IPlatformController Pitcher { get; set; }
        public GameObject LastTouched { get; set; }
        public GameState GameState
        {
            get { return gameState.Value; }
            set { gameState.Value = value; }
        }
        #endregion
        private void Awake()
        {
            _gc = GetComponent<GameController>();
        }

        #region Connection handlers
        public void EnterMatch()
        {
            if (IsServer)
            {
                _gc.waitingForOpponentPanel.SetActive(true);
                _gc.LoadGameMode();
                gameState.Value = GameState.Initial;
            }
            if (!IsServer)
                ResetReadyState();
            _gc.menuPanel.SetActive(false);
            _gc.leaveButton.SetActive(true);
            _gc.ToggleControls(true);
        }

        public void OnBothPlayersConnected()
        {
            if (!NetworkManager.Singleton.IsServer) return;

            _gc.scoreHandler.InitScoreServer();
            GetComponent<PowerUpsManager>().SetUpPowerUpsTrigger();
            LastTouched = NetworkManager.Singleton.ConnectedClientsList[0].PlayerObject.gameObject;
            Pitcher = LastTouched.GetComponent<IPlatformController>();
            _gc.waitingForOpponentPanel.SetActive(false);
            ResetReadyState();

            // create network synced ball and find it on clients
            var ball = Instantiate(_gc.networkBallPrefab.GetComponent<NetworkObject>(), Pitcher.GetBallStartPosition(), Quaternion.identity);
            ball.Spawn();
            FindBallClientRpc();
            if (_gc.gameMode == GameMode.Accuracy)
            {
                ball.GetComponent<BallController>().ChangeMaterialClientRpc(LastTouched.tag);
            }

            gameState.Value = GameState.Prepare;
        }

        public void ResetReadyState()
        {
            NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId].PlayerObject.GetComponent<PlatformController>().IsReady.Value = false;
            _gc.readyButtonText.text = ReadyText;
            _gc.readyButton.SetActive(true);
        }

        [ClientRpc]
        public void FindBallClientRpc()
        {
            _gc.ballController = GameObject.FindGameObjectWithTag("Ball").GetComponent<BallController>();
        }

        public void ExitMatch()
        {
            if (!IsServer) return;
            GetComponent<PowerUpsManager>().ClearPowerUps();
            if (countdownCoroutine != null)
                StopCoroutine(countdownCoroutine);
            _gc.waitingForOpponentPanel.SetActive(false);
            _gc.startButton.SetActive(false);
        }
        #endregion

        #region Round handling
        public bool MovementAllowed()
        {
            return IsServer && gameState.Value == GameState.Play;
        }

        public void ReadyToStart(bool everyoneIsReady)
        {
            _gc.startButton.SetActive(everyoneIsReady);
        }

        /// <summary>
        /// Called on the server
        /// </summary>
        public void StartRound()
        {
            if (_gc.debugMode)
                return;
            foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
                _client.PlayerObject.GetComponent<IPlatformController>().ResetPlatform();
            _gc.ballController.ResetBall();
            StartCoroutine(StartAfterCountdown());
        }

        private IEnumerator StartAfterCountdown()
        {
            PrepareForRoundClientRpc();
            countdownCoroutine = StartCoroutine(_gc.countdownHandler.CountDown());
            yield return countdownCoroutine;

            StartAfterCountDownClientRpc();
            gameState.Value = GameState.Play;
            if (!_gc.debugMode)
            {
                _gc.ballController.LaunchBall();
            }
        }

        [ClientRpc]
        private void PrepareForRoundClientRpc()
        {
            _gc.readyButton.SetActive(false);
            _gc.startButton.SetActive(false);
        }

        [ClientRpc]
        private void StartAfterCountDownClientRpc()
        {
            _gc.ToggleControlsInteraction(true);
        }

        public void FinishRound()
        {
            FinishRoundClientRpc();
            _gc.ballController.StopBall();
            gameObject.GetComponent<PowerUpsManager>().ClearPowerUps();
            gameState.Value = GameState.Prepare;
        }

        [ClientRpc]
        public void FinishRoundClientRpc()
        {
            ResetReadyState();
            _gc.ToggleControlsInteraction(false);
            _gc.startButton.SetActive(false);
        }

        public void FinishMatch(string winnerName)
        {
            _gc.scoreHandler.ClearScores();
            WinnerNotificationClientRpc(winnerName);
        }

        [ClientRpc]
        public void WinnerNotificationClientRpc(string winner)
        {
            _gc.winnerPanel.GetComponentInChildren<TextMeshProUGUI>().text = $"{winner} won!";
            _gc.winnerPanel.SetActive(true);
        }
        #endregion

        #region Ingame handlers
        /// <summary>
        /// Handler for accuracy mode only
        /// </summary>
        public void PlatformTouchHandler(GameObject platform)
        {
            if (gameState.Value != GameState.Play) return;

            LastTouched = platform;
            if (_gc.gameMode == GameMode.Accuracy)
                _gc.ballController.ChangeMaterial(LastTouched.tag);
            GetComponent<PowerUpsManager>().TriggerPowerUp();
        }

        public void BackWallTouchHandler(string playerTag)
        {
            if (_gc.gameMode == GameMode.Classic)
            {
                FinishRound();
                bool hasWinner = _gc.scoreHandler.MUpdateScore(playerTag);
                foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    NetworkObject player = _client.PlayerObject;
                    if (!player.gameObject.CompareTag(playerTag))
                        Pitcher = player.GetComponent<IPlatformController>();
                    else if (hasWinner)
                    {
                        FinishMatch(player.GetComponent<PlatformController>().Name.Value);
                        break;
                    }
                }
            }
            else if (_gc.gameMode == GameMode.Accuracy)
            {
                foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    NetworkObject player = _client.PlayerObject;
                    if (player.gameObject.CompareTag(playerTag))
                    {
                        LastTouched = player.gameObject;
                        break;
                    }
                }
                _gc.ballController.ChangeMaterial(playerTag);
            }
        }

        public void PowerUpTouchHandler()
        {
            if (_gc.gameMode != GameMode.Accuracy) return;

            bool hasWinner = _gc.scoreHandler.MUpdateScore(LastTouched.tag);
            if (hasWinner)
            {
                FinishRound();
                foreach (var _client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    NetworkObject player = _client.PlayerObject;
                    if (player.gameObject.CompareTag(LastTouched.tag))
                    {
                        FinishMatch(player.GetComponent<PlatformController>().Name.Value);
                        break;
                    }
                }
            }
        }

        public void OnReadyButtonClicked()
        {
            var platform = NetworkManager.Singleton.ConnectedClients[NetworkManager.Singleton.LocalClientId]
                .PlayerObject.GetComponent<PlatformController>();
            _gc.readyButtonText.text = platform.IsReady.Value ? ReadyText : NotReadyText;
            platform.TogglePlayerReadyServerRpc();
        }
        #endregion
    }
}
