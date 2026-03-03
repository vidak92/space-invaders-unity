using System;
using DG.Tweening;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    [Serializable]
    public enum EnemyType
    {
        Enemy1,
        Enemy2,
        Enemy3,
        UFO
    }

    public class Enemy : MonoBehaviour
    {
        public Collider2D Collider;
        public Health Health;
        public SpriteRenderer SpriteRenderer;
        public ParticleSystem Particles;
        
        [Space]
        public EnemyType Type;
        
        public bool IsActive { get; private set; }
        public bool IsKilled { get; private set; }
        
        private GameController GameController => ServiceLocator.Get<GameController>();
        private GameConfig GameConfig => ServiceLocator.Get<GameConfig>();
        private EnemiesConfig EnemiesConfig => GameConfig.EnemiesConfig;

        public void Init(string name)
        {
            gameObject.name = name;
            transform.localPosition = Vector3.zero;
            
            Health.OnDamageTaken += OnDamageTaken;
            SetActive(false);
        }

        private void OnDamageTaken()
        {
            IsKilled = true;
            Collider.enabled = false;
            SpriteRenderer.enabled = false;
            
            // @TODO pool particles instead of having one per enemy
            Particles.Play();
            Particles.transform.SetParent(null, worldPositionStays: true);
            Particles.transform.SetLocalScale(1f);
            
            var particleLifetime = Particles.main.startLifetime.constantMax;
            DOVirtual.DelayedCall(particleLifetime, () =>
            {
                Particles.transform.SetParent(transform, worldPositionStays: true);
                Particles.transform.localPosition = Vector3.zero;
                Particles.transform.SetLocalScale(1f);
                Collider.enabled = true;
                SpriteRenderer.enabled = true;
                SetActive(false);
            });
            
            GameController.OnEnemyShot(Type);
        }
        
        public void SetActive(bool active)
        {
            IsActive = active;
            gameObject.SetActive(active);
        }

        public void Spawn(Vector3 position, float delay, float duration)
        {
            IsKilled = false;
            transform.DOKill();
            transform.localPosition = position;
            transform.localScale = Vector3.zero;
            Collider.enabled = false;
            SetActive(true);

            transform.DOScale(EnemiesConfig.EnemyScale, duration)
                .SetDelay(delay)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    Collider.enabled = true;
                });
        }
    }
}