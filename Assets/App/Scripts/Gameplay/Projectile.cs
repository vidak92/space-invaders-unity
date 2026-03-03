using System;
using SGSTools.Extensions;
using SGSTools.Util;
using UnityEngine;

namespace SpaceInvaders
{
    [Serializable]
    public enum ProjectileDirection
    {
        Up,
        Down
    }

    public class Projectile : MonoBehaviour
    {
        public Rigidbody2D Rigidbody;
        
        private ProjectileConfig _projectileConfig;
        private Vector3 _direction;
        
        private GameController GameController => ServiceLocator.Get<GameController>();
        
        public void Init(ProjectileConfig projectileConfig, Vector3 sourcePosition)
        {
            _projectileConfig = projectileConfig;
            
            var directionZ = _projectileConfig.Direction == ProjectileDirection.Up ? 1f : -1f;
            _direction = Vector3.up * directionZ;
            var angleZ = MathfExt.VectorXYToAngle(_direction) * Mathf.Rad2Deg - 90f;

            transform.position = sourcePosition + _direction * _projectileConfig.InitialVerticalOffset;
            transform.rotation = Quaternion.Euler(0f, 0f, angleZ);

            gameObject.layer = _projectileConfig.LayerMask.ToIndex();

            Rigidbody.linearVelocity = _direction * projectileConfig.MoveSpeed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage();
                GameController.DespawnProjectile(this);
            }
        }

        public void FixedUpdate()
        {
            if (Mathf.Abs(transform.position.y) > _projectileConfig.MaxPositionY)
            {
                Rigidbody.linearVelocity = Vector2.zero;
                GameController.DespawnProjectile(this);
            }
        }
    }
}