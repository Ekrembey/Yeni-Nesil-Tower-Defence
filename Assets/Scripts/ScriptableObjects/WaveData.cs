using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// Dalga verilerini tutan ScriptableObject sınıfı.
    /// Unity Editor'da sağ tık > Create > Game > Wave Data ile oluşturulabilir.
    /// </summary>
    [CreateAssetMenu(fileName = "New Wave Data", menuName = "Game/Wave Data", order = 3)]
    public class WaveData : ScriptableObject
    {
        #region Serialized Fields
        
        [SerializeField] private EnemyData _enemyData;
        
        [SerializeField] private int _count = 10;
        
        [SerializeField] private float _spawnInterval = 1f; // Saniye cinsinden
        
        #endregion
        
        #region Properties (Read-Only)
        
        /// <summary>
        /// Bu dalgada spawn edilecek düşman verisi
        /// </summary>
        public EnemyData EnemyData
        {
            get { return _enemyData; }
        }
        
        /// <summary>
        /// Bu dalgada kaç düşman spawn edilecek
        /// </summary>
        public int Count
        {
            get { return _count; }
        }
        
        /// <summary>
        /// Düşmanlar arası spawn aralığı (saniye cinsinden)
        /// </summary>
        public float SpawnInterval
        {
            get { return _spawnInterval; }
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
            
            if (_enemyData == null)
            {
                Debug.LogError($"WaveData '{name}': EnemyData atanmamış!");
                isValid = false;
            }
            else if (!_enemyData.IsValid())
            {
                Debug.LogError($"WaveData '{name}': EnemyData geçersiz!");
                isValid = false;
            }
            
            if (_count <= 0)
            {
                Debug.LogWarning($"WaveData '{name}': Count 0'dan büyük olmalı! Şu anki değer: {_count}");
            }
            
            if (_spawnInterval <= 0f)
            {
                Debug.LogWarning($"WaveData '{name}': SpawnInterval 0'dan büyük olmalı! Şu anki değer: {_spawnInterval}");
            }
            
            return isValid;
        }
        
        #endregion
    }
}

