using System;
using System.Collections.Generic;
using DG.Tweening;
using SGSTools.Common;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    public class Player : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;
        public Collider2D[] Colliders;
        public Health Health;
        public SpriteRenderer SpriteRenderer;
        public ParticleSystem Particles;
        
        private float _moveDirectionX;
        private Timer _shotCooldownTimer;
        
        private bool _isInvincible;
        private Timer _invincibleTimer;
        private bool _isKilled;
        
        public bool IsActive { get; private set; }

        private AppController AppController => ServiceLocator.Get<AppController>();
        private GameController GameController => ServiceLocator.Get<GameController>();
        private AudioController AudioController => ServiceLocator.Get<AudioController>();
        
        private GameConfig GameConfig => ServiceLocator.Get<GameConfig>();
        private PlayerConfig PlayerConfig => GameConfig.PlayerConfig;
        private GameplayBounds GridBounds => GameConfig.EnemiesConfig.FormationConfig.GridBounds;

        public Action OnPlayerKilled { get; set; }
        
        public void Init()
        {
            ResetState();
            Health.OnDamageTaken += OnDamageTaken;
        }

        private void OnDamageTaken()
        {
            if (!_isInvincible)
            {
                _isInvincible = true;
                _invincibleTimer.Reset();
                GameController.OnPlayerShot();
            }
        }
        
        public void SetActive(bool active)
        {
            IsActive = active;
            gameObject.SetActive(active);
        }

        public void ResetState()
        {
            transform.position = PlayerConfig.StartPosition;
            ResetColor();

            _moveDirectionX = 0f;
            // @TODO first cooldown after new wave should be equal to enemy formation spawn duration
            _shotCooldownTimer.Init(PlayerConfig.ShotCooldownDuration); 

            _isInvincible = false;
            _invincibleTimer.Init(PlayerConfig.InvincibilityDuration);
            _isKilled = false;

            SetActive(false);
            SetCollidersEnabled(true);
        }

        public void OnUpdate(PlayerInput playerInput)
        {
            if (_isKilled) return;
            
            var dt = Time.deltaTime;
            _moveDirectionX = playerInput.MoveDirectionX;

            // shot
            _shotCooldownTimer.Update(dt);
            if (playerInput.ShouldShoot && _shotCooldownTimer.IsDone)
            {
                _shotCooldownTimer.Reset();
                GameController.SpawnProjectile(PlayerConfig.ProjectileConfig, transform.position);
                AudioController.PlaySound(AudioController.PlayerLaserSound);
            }

            // invincibility
            if (_isInvincible)
            {
                var t = Mathf.Cos(Time.time * PlayerConfig.InvincibilityBlinkSpeed);
                t = Mathf.Abs(t);
                var color = Color.Lerp(PlayerConfig.InvincibilityColorTint, Color.white, t);
                SetColorMultiplier(color);
                var vignettePower = GameConfig.VignettePowerRange.GetValueAt(t);
                AppController.UpdateVignette(vignettePower, isAdditive: true);
                
                if (_invincibleTimer.Update(dt))
                {
                    _isInvincible = false;
                    _invincibleTimer.Reset();
                    ResetColor();
                    AppController.ResetVignette();
                }
            }
        }

        public void OnFixedUpdate()
        {
            var dt = Time.deltaTime;
            var velocity = Rigidbody.linearVelocity;
            var acceleration = PlayerConfig.Acceleration * dt;
            var maxMoveSpeed = PlayerConfig.MaxMoveSpeed;
            
            if (Mathf.Abs(_moveDirectionX) > 0f)
            {
                // accelerate to move direction
                velocity.x += acceleration * Mathf.Sign(_moveDirectionX);
                velocity.x = Mathf.Clamp(velocity.x, -maxMoveSpeed, maxMoveSpeed);
            }
            else if (velocity.x > 0f)
            {
                // decelerate to left
                velocity.x = Mathf.Clamp(velocity.x - acceleration, 0f, maxMoveSpeed);
            }
            else if (velocity.x < 0f)
            {
                // decelerate to right
                velocity.x = Mathf.Clamp(velocity.x + acceleration, -maxMoveSpeed, 0f);
            }
            
            // Debug.Log($"input x: {_moveDirectionX}, velocity x: {velocity.x}");
            Rigidbody.linearVelocity = velocity;

            // clamp position
            var position = Rigidbody.position;
            position.x = Mathf.Clamp(position.x, GridBounds.Left, GridBounds.Right);
            Rigidbody.position = position;
        }

        private void ResetColor()
        {
            SetColorMultiplier(Color.white);
        }

        private void SetColorMultiplier(Color color)
        {
            SpriteRenderer.color = color;
        }
        
        public void OnLifeLost(int remainingLives)
        {
            if (remainingLives == 0)
            {
                // @NOTE killed
                _isKilled = true;
                
                SetCollidersEnabled(false);
                SpriteRenderer.enabled = false;
                Particles.Play();

                var particleLifetime = Particles.main.startLifetime.constantMax;
                DOVirtual.DelayedCall(particleLifetime, () =>
                {
                    SetCollidersEnabled(true);
                    SpriteRenderer.enabled = true;
                    SetActive(false);
                    OnPlayerKilled?.Invoke();
                });
            }
        }

        private void SetCollidersEnabled(bool enabled)
        {
            foreach (var collider in Colliders)
            {
                collider.enabled = enabled;
            }
        }
    }
}