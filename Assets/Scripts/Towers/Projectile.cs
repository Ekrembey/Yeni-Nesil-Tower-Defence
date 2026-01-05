using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Enemies;
using Game.Core;
using Game.Interfaces;

namespace Game.Towers
{
    /// <summary>
    /// Kulelerden fırlatılan mermi. Hedefe doğru hareket eder ve çarpınca hasar verir.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _speed = 10f;

        [SerializeField] private float _lifeTime = 5f;

        [SerializeField] private float _damage = 10f;

        #endregion

        #region Private Fields

        private Enemy _target;

        #endregion

        #region Unity Lifecycle

        private void Start()
        {
            // Güvenlik için maksimum yaşam süresi
            StartCoroutine(DestroyAfterTime());
        }

        private void Update()
        {
            if (_target == null || _target.Equals(null))
            {
                Destroy(gameObject);
                return;
            }

            Vector3 direction = _target.transform.position - transform.position;
            float distanceThisFrame = _speed * Time.deltaTime;

            if (direction.magnitude <= distanceThisFrame)
            {
                HitTarget();
                return;
            }

            transform.position += direction.normalized * distanceThisFrame;
            transform.rotation = Quaternion.LookRotation(direction);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Merminin takip edeceği hedefi ayarlar.
        /// </summary>
        /// <param name="enemy">Hedef düşman.</param>
        public void Initialize(Enemy enemy)
        {
            _target = enemy;
        }

        #endregion

        #region Private Methods

        private void HitTarget()
        {
            if (_target != null && !_target.Equals(null))
            {
                // IDamageable interface'i ile hasar ver
                IDamageable damageable = _target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(_damage);
                }
                else
                {
                    // IDamageable yoksa Health component'ini kontrol et
                    Health health = _target.GetComponent<Health>();
                    if (health != null && !health.IsDead)
                    {
                        health.TakeDamage(_damage);
                    }
                    else
                    {
                        // Hiçbiri yoksa doğrudan düşmanı imha et
                        _target.HandleClick();
                    }
                }
            }

            Destroy(gameObject);
        }
        
        /// <summary>
        /// Collider-based collision için OnTriggerEnter (fizik tabanlı çarpışma)
        /// </summary>
        private void OnTriggerEnter(Collider other)
        {
            // Hedef düşmana çarptıysa hasar ver
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy == _target)
            {
                HitTarget();
                return;
            }
            
            // Herhangi bir IDamageable objeye çarptıysa hasar ver
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }

        private IEnumerator DestroyAfterTime()
        {
            yield return new WaitForSeconds(_lifeTime);
            if (this != null)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}


