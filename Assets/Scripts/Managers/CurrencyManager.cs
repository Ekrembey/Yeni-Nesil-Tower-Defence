using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Managers
{
    /// <summary>
    /// Para yönetimi yapan sınıf. Altın ve Elmas miktarını tutar ve yönetir.
    /// </summary>
    public class CurrencyManager : MonoBehaviour
    {
        #region Events
        
        /// <summary>
        /// Altın miktarı değiştiğinde tetiklenir (eskiMiktar, yeniMiktar)
        /// </summary>
        public event Action<int, int> OnGoldChanged;
        
        /// <summary>
        /// Elmas miktarı değiştiğinde tetiklenir (eskiMiktar, yeniMiktar)
        /// </summary>
        public event Action<int, int> OnDiamondChanged;
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private int _initialGold = 100;
        
        [SerializeField] private int _initialDiamond = 10;
        
        #endregion
        
        #region Private Fields
        
        private int _currentGold;
        
        private int _currentDiamond;
        
        private bool _isInitialized = false;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Mevcut Altın miktarı
        /// </summary>
        public int CurrentGold
        {
            get { return _currentGold; }
            private set
            {
                int oldValue = _currentGold;
                _currentGold = Mathf.Max(0, value);
                
                if (oldValue != _currentGold && OnGoldChanged != null)
                {
                    OnGoldChanged.Invoke(oldValue, _currentGold);
                }
            }
        }
        
        /// <summary>
        /// Mevcut Elmas miktarı
        /// </summary>
        public int CurrentDiamond
        {
            get { return _currentDiamond; }
            private set
            {
                int oldValue = _currentDiamond;
                _currentDiamond = Mathf.Max(0, value);
                
                if (oldValue != _currentDiamond && OnDiamondChanged != null)
                {
                    OnDiamondChanged.Invoke(oldValue, _currentDiamond);
                }
            }
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // Eğer başka bir CurrencyManager varsa, bu instance'ı yok et
            CurrencyManager[] managers = FindObjectsOfType<CurrencyManager>();
            if (managers.Length > 1)
            {
                Debug.LogWarning("Birden fazla CurrencyManager bulundu! Fazla olan siliniyor.");
                Destroy(gameObject);
                return;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// CurrencyManager'ı başlatır. Başlangıç değerlerini ayarlar.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("CurrencyManager zaten başlatılmış!");
                return;
            }
            
            _currentGold = _initialGold;
            _currentDiamond = _initialDiamond;
            _isInitialized = true;
            
            Debug.Log($"CurrencyManager başlatıldı. Başlangıç Altın: {_currentGold}, Başlangıç Elmas: {_currentDiamond}");
        }
        
        /// <summary>
        /// Altın ekler
        /// </summary>
        /// <param name="amount">Eklenecek Altın miktarı</param>
        /// <returns>İşlem başarılı mı?</returns>
        public bool AddGold(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            if (amount <= 0)
            {
                Debug.LogWarning($"Geçersiz Altın miktarı: {amount}. Miktar 0'dan büyük olmalı.");
                return false;
            }
            
            CurrentGold += amount;
            Debug.Log($"{amount} Altın eklendi. Yeni miktar: {CurrentGold}");
            
            return true;
        }
        
        /// <summary>
        /// Altın çıkarır
        /// </summary>
        /// <param name="amount">Çıkarılacak Altın miktarı</param>
        /// <returns>İşlem başarılı mı? (Yeterli para varsa true)</returns>
        public bool SpendGold(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            if (amount <= 0)
            {
                Debug.LogWarning($"Geçersiz Altın miktarı: {amount}. Miktar 0'dan büyük olmalı.");
                return false;
            }
            
            if (CurrentGold < amount)
            {
                Debug.LogWarning($"Yetersiz Altın! Mevcut: {CurrentGold}, İstenen: {amount}");
                return false;
            }
            
            CurrentGold -= amount;
            Debug.Log($"{amount} Altın harcandı. Kalan miktar: {CurrentGold}");
            
            return true;
        }
        
        /// <summary>
        /// Elmas ekler
        /// </summary>
        /// <param name="amount">Eklenecek Elmas miktarı</param>
        /// <returns>İşlem başarılı mı?</returns>
        public bool AddDiamond(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            if (amount <= 0)
            {
                Debug.LogWarning($"Geçersiz Elmas miktarı: {amount}. Miktar 0'dan büyük olmalı.");
                return false;
            }
            
            CurrentDiamond += amount;
            Debug.Log($"{amount} Elmas eklendi. Yeni miktar: {CurrentDiamond}");
            
            return true;
        }

        /// <summary>
        /// Elmas ekler (AddDiamond için yardımcı yöntem).
        /// Enemy gibi sistemler tarafından kullanılabilir.
        /// </summary>
        /// <param name="amount">Eklenecek Elmas miktarı</param>
        /// <returns>İşlem başarılı mı?</returns>
        public bool AddDiamonds(int amount)
        {
            return AddDiamond(amount);
        }
        
        /// <summary>
        /// Elmas çıkarır
        /// </summary>
        /// <param name="amount">Çıkarılacak Elmas miktarı</param>
        /// <returns>İşlem başarılı mı? (Yeterli para varsa true)</returns>
        public bool SpendDiamond(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            if (amount <= 0)
            {
                Debug.LogWarning($"Geçersiz Elmas miktarı: {amount}. Miktar 0'dan büyük olmalı.");
                return false;
            }
            
            if (CurrentDiamond < amount)
            {
                Debug.LogWarning($"Yetersiz Elmas! Mevcut: {CurrentDiamond}, İstenen: {amount}");
                return false;
            }
            
            CurrentDiamond -= amount;
            Debug.Log($"{amount} Elmas harcandı. Kalan miktar: {CurrentDiamond}");
            
            return true;
        }
        
        /// <summary>
        /// Yeterli Altın var mı kontrol eder
        /// </summary>
        /// <param name="amount">Kontrol edilecek miktar</param>
        /// <returns>Yeterli Altın var mı?</returns>
        public bool HasEnoughGold(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            return CurrentGold >= amount;
        }
        
        /// <summary>
        /// Yeterli Elmas var mı kontrol eder
        /// </summary>
        /// <param name="amount">Kontrol edilecek miktar</param>
        /// <returns>Yeterli Elmas var mı?</returns>
        public bool HasEnoughDiamond(int amount)
        {
            if (!_isInitialized)
            {
                Debug.LogError("CurrencyManager henüz başlatılmadı! Initialize() metodunu çağırın.");
                return false;
            }
            
            return CurrentDiamond >= amount;
        }
        
        /// <summary>
        /// Para miktarlarını sıfırlar (Test amaçlı)
        /// </summary>
        public void ResetCurrency()
        {
            CurrentGold = _initialGold;
            CurrentDiamond = _initialDiamond;
            Debug.Log("Para miktarları sıfırlandı.");
        }
        
        #endregion
        
        #region IAP & Ad Service Interface Placeholders
        
        // Gelecekte IAP ve Ad sistemleri için hazır interface yapısı
        // Şimdilik boş bırakılıyor, ileride implement edilecek
        
        /// <summary>
        /// IAP servisi için placeholder (gelecekte implement edilecek)
        /// </summary>
        private void OnPurchaseCompleted(string productId)
        {
            // TODO: IAP servisi implement edildiğinde buraya kod eklenecek
            Debug.Log($"Satın alma tamamlandı: {productId}");
        }
        
        /// <summary>
        /// Reklam servisi için placeholder (gelecekte implement edilecek)
        /// </summary>
        private void OnAdRewardReceived(int rewardAmount, string rewardType)
        {
            // TODO: Ad servisi implement edildiğinde buraya kod eklenecek
            Debug.Log($"Reklam ödülü alındı: {rewardAmount} {rewardType}");
        }
        
        #endregion
    }
}

