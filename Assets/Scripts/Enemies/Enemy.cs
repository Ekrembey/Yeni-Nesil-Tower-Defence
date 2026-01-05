using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.ScriptableObjects;
using Game.Managers;
using Game.Interfaces;

namespace Game.Enemies
{
    /// <summary>
    /// Düşmanın ana kontrolcüsü. Sağlık, hareket ve ödül sistemini yönetir.
    /// </summary>
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(EnemyMover))]
    public class Enemy : MonoBehaviour, IDamageable
    {
        #region Serialized Fields

        [SerializeField] private EnemyData _enemyData;

        #endregion

        #region Private Fields

        private Health _health;

        private EnemyMover _enemyMover;

        #endregion

        #region Properties

        /// <summary>
        /// Bu düşmana ait EnemyData referansı.
        /// </summary>
        public EnemyData EnemyData
        {
            get { return _enemyData; }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _health = GetComponent<Health>();
            if (_health == null)
            {
                Debug.LogError($"Enemy '{name}': Health bileşeni bulunamadı!");
            }

            _enemyMover = GetComponent<EnemyMover>();
            if (_enemyMover == null)
            {
                Debug.LogError($"Enemy '{name}': EnemyMover bileşeni bulunamadı!");
            }
        }

        private void OnEnable()
        {
            if (_health != null)
            {
                _health.OnDeath += OnHealthDeathHandler;
            }

            if (_enemyMover != null)
            {
                _enemyMover.OnReachedEnd += OnReachedEndHandler;
            }
        }

        private void Start()
        {
            if (_enemyData == null)
            {
                Debug.LogWarning($"Enemy '{name}': EnemyData atanmamış!");
            }
            else if (_health != null)
            {
                // EnemyData sağlık değeri ile Health bileşenini başlat
                _health.Initialize(_enemyData.Health);
            }
        }

        private void OnDisable()
        {
            if (_health != null)
            {
                _health.OnDeath -= OnHealthDeathHandler;
            }

            if (_enemyMover != null)
            {
                _enemyMover.OnReachedEnd -= OnReachedEndHandler;
            }
        }

        #endregion

        #region Public Methods
        
        /// <summary>
        /// IDamageable interface'i için hasar alma metodu.
        /// </summary>
        /// <param name="damage">Alınacak hasar miktarı.</param>
        public void TakeDamage(float damage)
        {
            if (_health != null && !_health.IsDead)
            {
                _health.TakeDamage(damage);
            }
            else
            {
                // Health yoksa direkt öldür
                Die();
            }
        }
        
        /// <summary>
        /// Düşmanı öldürür. Ödül verir ve WaveManager'a bildirir.
        /// </summary>
        public void Die()
        {
            if (_health != null && _health.IsDead)
            {
                // Zaten öldü, tekrar çağrılmasın
                return;
            }
            
            EnemyData data = _enemyData;
            if (data == null)
            {
                Debug.LogWarning($"Enemy '{name}': EnemyData null, ödül verilemiyor.");
            }
            else
            {
                // CurrencyManager üzerinden ödül ver
                CurrencyManager currencyManager = null;

                if (GameManager.Instance != null)
                {
                    currencyManager = GameManager.Instance.CurrencyManager;
                }

                if (currencyManager != null)
                {
                    if (data.GoldReward > 0)
                    {
                        currencyManager.AddGold(data.GoldReward);
                        Debug.Log($"Enemy '{name}': {data.GoldReward} altın ödülü verildi.");
                    }

                    if (data.ShouldGiveDiamondReward())
                    {
                        currencyManager.AddDiamond(1);
                        Debug.Log($"Enemy '{name}': 1 elmas ödülü verildi.");
                    }
                }
                else
                {
                    Debug.LogWarning($"Enemy '{name}': CurrencyManager bulunamadı, ödül verilemedi.");
                }
            }

            // WaveManager'a düşmanın öldüğünü bildir
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled();
            }
            else
            {
                Debug.LogWarning($"Enemy '{name}': WaveManager bulunamadı, OnEnemyKilled çağrılamadı.");
            }

            // Düşman objesini yok et
            Destroy(gameObject);
        }

        /// <summary>
        /// Düşmana tıklandığında çalıştırılacak ortak mantık.
        /// </summary>
        public void HandleClick()
        {
            Debug.Log("Düşman imha edildi!");

            // Dalga sayacı için WaveManager'a haber ver
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled();
            }

            // Düşmanı direkt yok et
            Destroy(gameObject);
        }

        /// <summary>
        /// Legacy OnMouseDown eventi (destek varsa çalışır).
        /// </summary>
        private void OnMouseDown()
        {
            HandleClick();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Düşman öldüğünde çağrılır. Die() metodunu çağırır.
        /// </summary>
        private void OnHealthDeathHandler()
        {
            Die();
        }

        /// <summary>
        /// Düşman yolun sonuna ulaştığında çağrılır.
        /// </summary>
        private void OnReachedEndHandler()
        {
            Debug.Log($"Enemy '{name}': Yolun sonuna ulaştı, oyuncu can kaybetmeli (şimdilik sadece log).");

            // Düşman dalga açısından artık sahnede değil, WaveManager bilgilendirilebilir
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemyKilled();
            }

            Destroy(gameObject);
        }

        #endregion
    }
}


