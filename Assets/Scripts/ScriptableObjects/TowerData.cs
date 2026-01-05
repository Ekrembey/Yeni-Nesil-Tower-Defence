using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// Kule verilerini tutan ScriptableObject sınıfı.
    /// Unity Editor'da sağ tık > Create > Game > Tower Data ile oluşturulabilir.
    /// </summary>
    [CreateAssetMenu(fileName = "New Tower Data", menuName = "Game/Tower Data", order = 1)]
    public class TowerData : ScriptableObject
    {
        #region Serialized Fields
        
        [SerializeField] private string _towerName = "Tower";
        
        [SerializeField] private GameObject _towerPrefab;
        
        [SerializeField] private int _cost = 50;
        
        [SerializeField] private float _damage = 10f;
        
        [SerializeField] private float _range = 5f;
        
        [SerializeField] private float _fireRate = 1f; // Saniyede kaç atış
        
        #endregion
        
        #region Properties (Read-Only)
        
        /// <summary>
        /// Kule adı
        /// </summary>
        public string TowerName
        {
            get { return _towerName; }
        }
        
        /// <summary>
        /// Kule prefab referansı
        /// </summary>
        public GameObject TowerPrefab
        {
            get { return _towerPrefab; }
        }
        
        /// <summary>
        /// Kule maliyeti (Altın cinsinden)
        /// </summary>
        public int Cost
        {
            get { return _cost; }
        }
        
        /// <summary>
        /// Kule hasarı
        /// </summary>
        public float Damage
        {
            get { return _damage; }
        }
        
        /// <summary>
        /// Kule menzili
        /// </summary>
        public float Range
        {
            get { return _range; }
        }
        
        /// <summary>
        /// Kule ateş hızı (saniyede atış sayısı)
        /// </summary>
        public float FireRate
        {
            get { return _fireRate; }
        }
        
        /// <summary>
        /// Atışlar arası süre (FireRate'in tersi)
        /// </summary>
        public float FireInterval
        {
            get
            {
                if (_fireRate <= 0f)
                {
                    Debug.LogWarning($"TowerData '{_towerName}' için FireRate 0 veya negatif! Varsayılan 1f kullanılıyor.");
                    return 1f;
                }
                return 1f / _fireRate;
            }
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
            
            if (string.IsNullOrEmpty(_towerName))
            {
                Debug.LogError("TowerData: TowerName boş olamaz!");
                isValid = false;
            }
            
            if (_towerPrefab == null)
            {
                Debug.LogError($"TowerData '{_towerName}': TowerPrefab atanmamış!");
                isValid = false;
            }
            
            if (_cost < 0)
            {
                Debug.LogWarning($"TowerData '{_towerName}': Cost negatif olamaz! Şu anki değer: {_cost}");
            }
            
            if (_damage < 0f)
            {
                Debug.LogWarning($"TowerData '{_towerName}': Damage negatif olamaz! Şu anki değer: {_damage}");
            }
            
            if (_range <= 0f)
            {
                Debug.LogWarning($"TowerData '{_towerName}': Range 0'dan büyük olmalı! Şu anki değer: {_range}");
            }
            
            if (_fireRate <= 0f)
            {
                Debug.LogWarning($"TowerData '{_towerName}': FireRate 0'dan büyük olmalı! Şu anki değer: {_fireRate}");
            }
            
            return isValid;
        }
        
        #endregion
    }
}

