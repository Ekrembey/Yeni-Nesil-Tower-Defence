using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Interfaces;

namespace Game.Core
{
    /// <summary>
    /// Ortak sağlık ve hasar yönetimi bileşeni.
    /// IDamageable arayüzünü uygular ve ölüm/sağlık değişimi event'leri sağlar.
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        #region Events

        /// <summary>
        /// Nesne öldüğünde tetiklenir.
        /// </summary>
        public event Action OnDeath;

        /// <summary>
        /// Sağlık değeri değiştiğinde tetiklenir. (current, max)
        /// </summary>
        public event Action<float, float> OnHealthChanged;

        #endregion

        #region Serialized Fields

        [SerializeField] private float _maxHealth = 100f;

        #endregion

        #region Private Fields

        private float _currentHealth;

        private bool _isDead = false;

        #endregion

        #region Properties

        /// <summary>
        /// Maksimum sağlık değeri.
        /// </summary>
        public float MaxHealth
        {
            get { return _maxHealth; }
            private set { _maxHealth = Mathf.Max(1f, value); }
        }

        /// <summary>
        /// Mevcut sağlık değeri.
        /// </summary>
        public float CurrentHealth
        {
            get { return _currentHealth; }
            private set
            {
                float clamped = Mathf.Clamp(value, 0f, MaxHealth);
                if (Mathf.Approximately(clamped, _currentHealth))
                {
                    _currentHealth = clamped;
                    return;
                }

                _currentHealth = clamped;

                if (OnHealthChanged != null)
                {
                    OnHealthChanged.Invoke(_currentHealth, MaxHealth);
                }
            }
        }

        /// <summary>
        /// Nesne ölü mü?
        /// </summary>
        public bool IsDead
        {
            get { return _isDead; }
            private set { _isDead = value; }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_maxHealth <= 0f)
            {
                Debug.LogWarning($"Health '{name}': MaxHealth 0 veya negatif, 1 olarak ayarlanıyor.");
                _maxHealth = 1f;
            }

            _currentHealth = _maxHealth;

            if (OnHealthChanged != null)
            {
                OnHealthChanged.Invoke(_currentHealth, MaxHealth);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sağlık bileşenini verilen maksimum sağlık değeriyle başlatır.
        /// </summary>
        /// <param name="maxHealth">Maksimum sağlık değeri.</param>
        public void Initialize(float maxHealth)
        {
            if (maxHealth <= 0f)
            {
                Debug.LogWarning($"Health '{name}': Initialize için geçersiz maxHealth ({maxHealth}). 1 olarak ayarlanıyor.");
                maxHealth = 1f;
            }

            MaxHealth = maxHealth;
            IsDead = false;
            CurrentHealth = MaxHealth;
        }

        /// <summary>
        /// Nesnenin hasar almasını sağlar.
        /// </summary>
        /// <param name="damage">Verilecek hasar miktarı.</param>
        public void TakeDamage(float damage)
        {
            if (IsDead)
            {
                return;
            }

            if (damage <= 0f)
            {
                Debug.LogWarning($"Health '{name}': Geçersiz hasar miktarı ({damage}). Hasar 0'dan büyük olmalı.");
                return;
            }

            CurrentHealth -= damage;

            if (CurrentHealth <= 0f && !IsDead)
            {
                HandleDeath();
            }
        }

        /// <summary>
        /// Nesneyi iyileştirir.
        /// </summary>
        /// <param name="amount">İyileştirme miktarı.</param>
        public void Heal(float amount)
        {
            if (IsDead)
            {
                return;
            }

            if (amount <= 0f)
            {
                Debug.LogWarning($"Health '{name}': Geçersiz iyileştirme miktarı ({amount}). Miktar 0'dan büyük olmalı.");
                return;
            }

            CurrentHealth += amount;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ölüm işlemini gerçekleştirir ve OnDeath event'ini tetikler.
        /// </summary>
        private void HandleDeath()
        {
            if (IsDead)
            {
                return;
            }

            IsDead = true;
            CurrentHealth = 0f;

            Debug.Log($"Health '{name}': Nesne öldü.");

            if (OnDeath != null)
            {
                try
                {
                    OnDeath.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Health '{name}': OnDeath event çağrısında hata: {ex.Message}");
                }
            }
        }

        #endregion
    }
}


