using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// Düşman verilerini tutan ScriptableObject sınıfı.
    /// Unity Editor'da sağ tık > Create > Game > Enemy Data ile oluşturulabilir.
    /// </summary>
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Game/Enemy Data", order = 2)]
    public class EnemyData : ScriptableObject
    {
        #region Serialized Fields
        
        [SerializeField] private string _enemyName = "Enemy";
        
        [SerializeField] private GameObject _enemyPrefab;
        
        [SerializeField] private float _health = 100f;
        
        [SerializeField] private float _speed = 2f;
        
        [SerializeField] private int _goldReward = 10;
        
        [SerializeField] [Range(0f, 1f)] private float _diamondRewardChance = 0.1f; // 0-1 arası, %10 şans
        
        #endregion
        
        #region Properties (Read-Only)
        
        /// <summary>
        /// Düşman adı
        /// </summary>
        public string EnemyName
        {
            get { return _enemyName; }
        }
        
        /// <summary>
        /// Düşman prefab referansı
        /// </summary>
        public GameObject EnemyPrefab
        {
            get { return _enemyPrefab; }
        }
        
        /// <summary>
        /// Düşman canı
        /// </summary>
        public float Health
        {
            get { return _health; }
        }
        
        /// <summary>
        /// Düşman hızı (birim/saniye)
        /// </summary>
        public float Speed
        {
            get { return _speed; }
        }
        
        /// <summary>
        /// Öldürüldüğünde verilen Altın ödülü
        /// </summary>
        public int GoldReward
        {
            get { return _goldReward; }
        }
        
        /// <summary>
        /// Elmas ödülü alma şansı (0-1 arası, 0.1 = %10 şans)
        /// </summary>
        public float DiamondRewardChance
        {
            get { return Mathf.Clamp01(_diamondRewardChance); }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Elmas ödülü kazanıldı mı kontrol eder (rastgele)
        /// </summary>
        /// <returns>Elmas ödülü kazanıldı mı?</returns>
        public bool ShouldGiveDiamondReward()
        {
            if (DiamondRewardChance <= 0f)
            {
                return false;
            }
            
            float randomValue = Random.Range(0f, 1f);
            return randomValue <= DiamondRewardChance;
        }
        
        #endregion
        
        #region Validation
        
        /// <summary>
        /// Veri geçerliliğini kontrol eder
        /// </summary>
        /// <returns>Veri geçerli mi?</returns>
        public bool IsValid()
        {
            bool isValid = true;
            
            if (string.IsNullOrEmpty(_enemyName))
            {
                Debug.LogError("EnemyData: EnemyName boş olamaz!");
                isValid = false;
            }
            
            if (_enemyPrefab == null)
            {
                Debug.LogError($"EnemyData '{_enemyName}': EnemyPrefab atanmamış!");
                isValid = false;
            }
            
            if (_health <= 0f)
            {
                Debug.LogWarning($"EnemyData '{_enemyName}': Health 0'dan büyük olmalı! Şu anki değer: {_health}");
            }
            
            if (_speed <= 0f)
            {
                Debug.LogWarning($"EnemyData '{_enemyName}': Speed 0'dan büyük olmalı! Şu anki değer: {_speed}");
            }
            
            if (_goldReward < 0)
            {
                Debug.LogWarning($"EnemyData '{_enemyName}': GoldReward negatif olamaz! Şu anki değer: {_goldReward}");
            }
            
            if (_diamondRewardChance < 0f || _diamondRewardChance > 1f)
            {
                Debug.LogWarning($"EnemyData '{_enemyName}': DiamondRewardChance 0-1 arası olmalı! Şu anki değer: {_diamondRewardChance}");
            }
            
            return isValid;
        }
        
        #endregion
    }
}

