using System;
using System.Collections.Generic;
using SGSTools.Common;
using UnityEngine;

namespace SpaceInvaders
{
    [Serializable]
    public class GameplayBounds
    {
        public float Left;
        public float Right;
        public float Bottom;
        public float Top;
    }

    [Serializable]
    public class ProjectileConfig
    {
        public float MoveSpeed;
        public float InitialVerticalOffset;
        public ProjectileDirection Direction;
        public LayerMask LayerMask;
        public float MaxPositionY;
    }

    [Serializable]
    public class PlayerConfig
    {
        public float MaxMoveSpeed;
        public float Acceleration;
        
        [Space]
        public float ShotCooldownDuration;
        
        [Space]
        public float InvincibilityDuration;
        public float InvincibilityBlinkSpeed;
        public Color InvincibilityColorTint;

        [Space]
        [Range(1, 5)]
        public int StartLives;
        public Vector3 StartPosition;

        [Space]
        public ProjectileConfig ProjectileConfig;
    }

    [Serializable]
    public class UFOConfig
    {
        [Space]
        public FloatRange MoveSpeedRange;
        
        [Space]
        public float BoundsLeft;
        public float BoundsRight;
        public float FadeOutDistance;

        [Space]
        public FloatRange SpawnDurationRangeMin;
        public FloatRange SpawnDurationRangeMax;
    }

    [Serializable]
    public class EnemiesConfig
    {
        public int Enemy1ScoreValue;
        public int Enemy2ScoreValue;
        public int Enemy3ScoreValue;
        public int EnemyUFOScoreValue;

        [Space]
        public float EnemyScale;
        
        [Space]
        public ProjectileConfig ProjectileConfig;
        
        [Space]
        public UFOConfig UFOConfig;

        [Space]
        public FormationConfig FormationConfig;
        
        public int GetScoreValueForEnemyType(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Enemy1:
                    return Enemy1ScoreValue;
                case EnemyType.Enemy2:
                    return Enemy2ScoreValue;
                case EnemyType.Enemy3:
                    return Enemy3ScoreValue;
                case EnemyType.UFO:
                    return EnemyUFOScoreValue;
                default:
                    break;
            }
            Debug.LogError($"GameConfig: enemy type {enemyType} not handled");
            return 0;
        }
    }

    [Serializable]
    public class FormationConfig
    {
        public float NewWaveDelay;
        public float SpawnDuration;
        public float FormationSpeedTPower;
        
        [Space]
        public FloatRange StepDurationRange;
        public FloatRange IdleDurationRange;
        
        [Space]
        public FloatRange ShotDelayRangeMin;
        public FloatRange ShotDelayRangeMax;

        [Header("Grid")]
        public GameplayBounds GridBounds;
        public Vector2Int GridSize;
        public int EnemiesPerRow;
        public List<EnemyType> EnemyRows = new List<EnemyType>();
        public Vector3 StartPosition;
    }

    [CreateAssetMenu(menuName = "Config/Gameplay Config")]
    public class GameConfig : ScriptableObject
    {
        public FloatRange CameraSizeRange;
        public FloatRange CanvasScaleRange;
        
        [Header("Prefabs")]
        public Player PlayerPrefab;
        public Enemy Enemy1Prefab;
        public Enemy Enemy2Prefab;
        public Enemy Enemy3Prefab;
        public EnemyUFO UFOPrefab;
        public Projectile ProjectilePrefab;

        [Header("Vignette")]
        public FloatRange VignettePowerRange;
        public Color VignetteColor;

        [Header("Player")]
        public PlayerConfig PlayerConfig;
        
        [Header("Enemies")]
        public EnemiesConfig EnemiesConfig;
        
        public Enemy GetEnemyPrefab(EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Enemy1:
                    return Enemy1Prefab;
                case EnemyType.Enemy2:
                    return Enemy2Prefab;
                case EnemyType.Enemy3:
                    return Enemy3Prefab;
                default:
                    Debug.LogError($"GameConfig: enemy type {enemyType} not handled");
                    return null;
            }
        }
    }
}