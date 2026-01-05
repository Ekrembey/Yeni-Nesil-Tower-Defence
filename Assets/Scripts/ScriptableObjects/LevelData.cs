using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.ScriptableObjects
{
    /// <summary>
    /// Level verilerini tutan ScriptableObject sınıfı.
    /// Unity Editor'da sağ tık > Create > Game > Level Data ile oluşturulabilir.
    /// </summary>
    [CreateAssetMenu(fileName = "New Level Data", menuName = "Game/Level Data", order = 4)]
    public class LevelData : ScriptableObject
    {
        #region Serialized Fields
        
        [SerializeField] private int _levelNumber = 1;
        
        [SerializeField] private int _startingGold = 100;
        
        [SerializeField] private List<WaveData> _waves = new List<WaveData>();
        
        #endregion
        
        #region Properties (Read-Only)
        
        /// <summary>
        /// Level numarası
        /// </summary>
        public int LevelNumber
        {
            get { return _levelNumber; }
        }
        
        /// <summary>
        /// Level başlangıç Altın miktarı
        /// </summary>
        public int StartingGold
        {
            get { return _startingGold; }
        }
        
        /// <summary>
        /// Level'daki dalga listesi
        /// </summary>
        public List<WaveData> Waves
        {
            get { return _waves; }
        }
        
        /// <summary>
        /// Toplam dalga sayısı
        /// </summary>
        public int WaveCount
        {
            get { return _waves != null ? _waves.Count : 0; }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Belirtilen index'teki dalgayı döndürür
        /// </summary>
        /// <param name="index">Dalga index'i (0'dan başlar)</param>
        /// <returns>WaveData veya null</returns>
        public WaveData GetWave(int index)
        {
            if (_waves == null || index < 0 || index >= _waves.Count)
            {
                Debug.LogWarning($"LevelData '{name}': Geçersiz wave index! Index: {index}, Toplam dalga: {WaveCount}");
                return null;
            }
            
            return _waves[index];
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
            
            if (_levelNumber <= 0)
            {
                Debug.LogWarning($"LevelData '{name}': LevelNumber 0'dan büyük olmalı! Şu anki değer: {_levelNumber}");
            }
            
            if (_startingGold < 0)
            {
                Debug.LogWarning($"LevelData '{name}': StartingGold negatif olamaz! Şu anki değer: {_startingGold}");
            }
            
            if (_waves == null || _waves.Count == 0)
            {
                Debug.LogError($"LevelData '{name}': En az bir dalga tanımlanmalı!");
                isValid = false;
            }
            else
            {
                for (int i = 0; i < _waves.Count; i++)
                {
                    if (_waves[i] == null)
                    {
                        Debug.LogError($"LevelData '{name}': Wave {i} null!");
                        isValid = false;
                    }
                    else if (!_waves[i].IsValid())
                    {
                        Debug.LogError($"LevelData '{name}': Wave {i} geçersiz!");
                        isValid = false;
                    }
                }
            }
            
            return isValid;
        }
        
        #endregion
    }
}

