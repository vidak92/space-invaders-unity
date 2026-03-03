using SGSTools.Components;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Paused,
        GameOver,
        HighScores,
        Controls
    }

    public class GameController : MonoBehaviour
    {
        public EnemyFormation EnemyFormation;
        
        private ObjectPool<Projectile> _projectilePool;
        
        private GameState _currentState;
        private Player _player;
        
        private int _score;
        private int _wave;
        private int _lives;

        private AppController AppController => ServiceLocator.Get<AppController>();
        private AudioController AudioController => ServiceLocator.Get<AudioController>();
        private InputService InputService => ServiceLocator.Get<InputService>();
        private UIController UIController => ServiceLocator.Get<UIController>();

        private GameConfig GameConfig => ServiceLocator.Get<GameConfig>();

        public void Init()
        {
            _projectilePool = ObjectPool<Projectile>.CreateWithGameObject(GameConfig.ProjectilePrefab, 100, "ProjectilePool");
            // _projectilePool.Parent.parent = transform;

            _player = Instantiate(GameConfig.PlayerPrefab, transform, true);
            _player.OnPlayerKilled += GameOver;
            _player.Init();

            EnemyFormation.transform.parent = transform;
            EnemyFormation.Init();

            EnemyFormation.UFOTransform.parent = transform;

            ShowMainMenu();
        }

        public void OnUpdate()
        {
            switch (_currentState)
            {
                case GameState.MainMenu:
                {
                    break;
                }
                case GameState.Gameplay:
                {
                    if (AppController.IsMobilePlatform())
                    {
                        var directionX = UIController.GameplayScreen.MoveJoystick.Direction.x;
                        InputService.SetInputAction(InputAction.MoveRight, directionX > 0f);
                        InputService.SetInputAction(InputAction.MoveLeft, directionX < 0f);
                    }
                    else
                    {
                        InputService.UpdateStandaloneActions();
                    }
                    
                    if (_player.IsActive)
                    {
                        var playerInput = InputService.GetPlayerInput();
                        _player.OnUpdate(playerInput);
                        EnemyFormation.OnUpdate();
                    }
                    break;
                }
                case GameState.GameOver:
                {
                    break;
                }
                case GameState.HighScores:
                {
                    break;
                }
                case GameState.Controls:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public void OnFixedUpdate()
        {
            switch (_currentState)
            {
                case GameState.MainMenu:
                {
                    break;
                }
                case GameState.Gameplay:
                {
                    if (_player.IsActive)
                    {
                        _player.OnFixedUpdate();
                        EnemyFormation.OnFixedUpdate();
                    }
                    break;
                }
                case GameState.GameOver:
                {
                    break;
                }
                case GameState.HighScores:
                {
                    break;
                }
                case GameState.Controls:
                {
                    break;
                }
                default:
                {
                    break;
                }
            }
        }

        public void ShowMainMenu()
        {
            Time.timeScale = 1f;
            UIController.StartScreenTransition(UIController.MainMenuScreen, onComplete: () =>
            {
                _currentState = GameState.MainMenu;
            });
        }

        public void ShowHighScores()
        {
            UIController.StartScreenTransition(UIController.HighScoresScreen, onComplete: () =>
            {
                _currentState = GameState.HighScores;
            });
        }
        
        public void ShowControls()
        {
            UIController.StartScreenTransition(UIController.ControlsScreen, onComplete: () =>
            {
                _currentState = GameState.Controls;
            });
        }

        public void StartGame()
        {
            Time.timeScale = 1f;
            UIController.StartScreenTransition(UIController.GameplayScreen, onComplete: () =>
            {
                _currentState = GameState.Gameplay;
                
                _score = 0;
                _wave = 0;
                _lives = GameConfig.PlayerConfig.StartLives;
            
                _player.ResetState();
                _player.SetActive(true);
                EnemyFormation.StartWave();
            });
        }

        public void PauseGame()
        {
            Time.timeScale = 0f;
            _currentState = GameState.Paused;
            UIController.SetPauseOverlayVisible(true);
        }

        public void ResumeGame()
        {
            _currentState = GameState.Gameplay;
            Time.timeScale = 1f;
            UIController.SetPauseOverlayVisible(false);
        }

        private void GameOver()
        {
            AudioController.PlaySound(AudioController.GameOverSound);
            UIController.StartScreenTransition(UIController.GameOverScreen, onComplete: () =>
            {
                _currentState = GameState.GameOver;
                _player.SetActive(false);
                EnemyFormation.EndWave();
                _projectilePool.ReturnAllActiveObjects();
                var highScoreStats = new HighScoreStats(_score, _wave);
                AppController.HighScoreService.AddNewHighScore(highScoreStats);
                UIController.GameOverScreen.SetStats(_score, _wave);
                AppController.ResetVignette();
            });
        }

        public void ExitGame()
        {
            EnemyFormation.EndWave();
            _player.SetActive(false);
            _projectilePool.ReturnAllActiveObjects();
            ShowMainMenu();
            AppController.ResetVignette();
        }

        public void OnPlayerShot()
        {
            _lives--;
            _player.OnLifeLost(_lives);
            UIController.GameplayScreen.SetGameStats(_score, _wave, _lives);
            AppController.CameraShaker.StartShake(1f); // @TODO config
            AudioController.PlaySound(AudioController.ExplosionSound, volume: 0.1f); // @TODO config
        }

        public void OnEnemyShot(EnemyType enemyType)
        {
            _score += GameConfig.EnemiesConfig.GetScoreValueForEnemyType(enemyType);
            UIController.GameplayScreen.SetGameStats(_score, _wave, _lives);
            AppController.CameraShaker.StartShake(enemyType == EnemyType.UFO ? 0.5f : 0f); // @TODO config
            AudioController.PlaySound(AudioController.ExplosionSound, volume: 0.1f); // @TODO config
        }

        public void OnWaveStart()
        {
            _wave++;
            UIController.GameplayScreen.SetGameStats(_score, _wave, _lives);
            AudioController.PlaySound(AudioController.NewWaveSound);
        }

        public void SpawnProjectile(ProjectileConfig projectileConfig, Vector3 position)
        {
            var projectile = _projectilePool.Get();
            projectile.Init(projectileConfig, position);
        }
        
        public void DespawnProjectile(Projectile projectile)
        {
            _projectilePool.Return(projectile);
        }
    }
}