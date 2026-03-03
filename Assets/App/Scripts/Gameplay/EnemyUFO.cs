using SGSTools.Common;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SpaceInvaders
{
    public class EnemyUFO : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;
        public Enemy Enemy;
        
        private MoveDirection _moveDirection;
        private Timer _spawnTimer;
        private FloatRange _spawnDurationRange;

        private GameConfig GameConfig => ServiceLocator.Get<GameConfig>();
        private UFOConfig UFOConfig => GameConfig.EnemiesConfig.UFOConfig;
        private GameplayBounds GridBounds => GameConfig.EnemiesConfig.FormationConfig.GridBounds;

        private float _formationSpeedT;

        public void Init()
        {
            _spawnDurationRange = new FloatRange(UFOConfig.SpawnDurationRangeMin.Min, UFOConfig.SpawnDurationRangeMin.Max);
            var spawnDuration = _spawnDurationRange.GetRandomValue();
            _spawnTimer.Init(spawnDuration);
            _spawnTimer.Reset();

            Enemy.Init($"{EnemyType.UFO}");
            _formationSpeedT = 0f;
        }

        public void OnWaveStart()
        {
            _spawnTimer.Reset();
        }

        public void OnUpdate(float formationSpeedT)
        {
            var dt = Time.deltaTime;
            _formationSpeedT = formationSpeedT;
            
            if (Enemy.IsActive)
            {
                // fade
                var fadeOutPositionXRight = UFOConfig.BoundsRight - UFOConfig.FadeOutDistance;
                var fadeOutPositionXLeft = UFOConfig.BoundsLeft + UFOConfig.FadeOutDistance;
                var fadeOutPositionXDiff = 0f;
                if (transform.position.x > fadeOutPositionXRight)
                {
                    fadeOutPositionXDiff = transform.position.x - fadeOutPositionXRight;
                }
                else if (transform.position.x < fadeOutPositionXLeft)
                {
                    fadeOutPositionXDiff = fadeOutPositionXLeft - transform.position.x;
                }
                var alpha = 1f - Mathf.Clamp01(fadeOutPositionXDiff / UFOConfig.FadeOutDistance);
                Enemy.SpriteRenderer.SetAlpha(alpha);

                // bounds check
                if (transform.position.x < UFOConfig.BoundsLeft || transform.position.x > UFOConfig.BoundsRight)
                {
                    // despawn
                    Enemy.SetActive(false);
                    _spawnDurationRange.Min = UFOConfig.SpawnDurationRangeMin.GetValueAt(_formationSpeedT);
                    _spawnDurationRange.Max = UFOConfig.SpawnDurationRangeMax.GetValueAt(_formationSpeedT);
                    var spawnDuration = _spawnDurationRange.GetRandomValue();
                    _spawnTimer.Init(spawnDuration);
                    _spawnTimer.Reset();
                }
            }
            else
            {
                if (_spawnTimer.Update(dt))
                {
                    // spawn
                    Enemy.SetActive(true);
                    float positionX;
                    if (Random.Range(-1f, 1f) > 0f)
                    {
                        _moveDirection = MoveDirection.Right;
                        positionX = UFOConfig.BoundsLeft;
                    }
                    else
                    {
                        _moveDirection = MoveDirection.Left;
                        positionX = UFOConfig.BoundsRight;
                    }
                    
                    var topOffsetY = 2.2f; // @TODO config
                    transform.position = new Vector3(positionX, GridBounds.Top + topOffsetY, 0f);
                }
            }
        }

        public void OnFixedUpdate()
        {
            if (Enemy.IsActive)
            {
                var directionX = _moveDirection == MoveDirection.Right ? 1f : -1f;
                var moveSpeed = UFOConfig.MoveSpeedRange.GetValueAt(_formationSpeedT);
                Rigidbody.linearVelocity = new Vector2(moveSpeed * directionX, 0f);
            }
        }
    }
}