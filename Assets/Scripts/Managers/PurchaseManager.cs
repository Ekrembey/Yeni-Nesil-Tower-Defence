using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Interfaces;

namespace Game.Managers
{
    /// <summary>
    /// Satın alma yönetimi yapan sınıf. IPurchaseService interface'ini implement eder.
    /// Şimdilik Debug.Log ile simüle edilir, gelecekte gerçek IAP servisi entegre edilecek.
    /// </summary>
    public class PurchaseManager : MonoBehaviour, IPurchaseService
    {
        #region Singleton
        
        private static PurchaseManager _instance;
        
        /// <summary>
        /// PurchaseManager'ın tek instance'ı
        /// </summary>
        public static PurchaseManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PurchaseManager>();
                    
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("PurchaseManager");
                        _instance = go.AddComponent<PurchaseManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                
                return _instance;
            }
        }
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private bool _isInitialized = false;
        
        #endregion
        
        #region Private Fields
        
        // Simülasyon için sahip olunan ürünleri tutar
        private HashSet<string> _ownedProducts = new HashSet<string>();
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        #endregion
        
        #region IPurchaseService Implementation
        
        /// <summary>
        /// Satın alma servisini başlatır (Simüle edilmiş)
        /// </summary>
        public void InitializePurchaseService()
        {
            if (_isInitialized)
            {
                Debug.Log("PurchaseManager: Satın alma servisi zaten başlatılmış!");
                return;
            }
            
            Debug.Log("PurchaseManager: Satın alma servisi başlatılıyor... (Simüle edilmiş)");
            _isInitialized = true;
            _ownedProducts.Clear();
            Debug.Log("PurchaseManager: Satın alma servisi başlatıldı! (Simüle edilmiş)");
        }
        
        /// <summary>
        /// Ürün satın alır (Simüle edilmiş)
        /// </summary>
        /// <param name="productId">Satın alınacak ürün ID'si</param>
        /// <param name="onComplete">Satın alma tamamlandığında çağrılacak callback (başarılı: true, başarısız: false)</param>
        public void PurchaseProduct(string productId, Action<bool> onComplete)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("PurchaseManager: Satın alma servisi henüz başlatılmadı! InitializePurchaseService() metodunu çağırın.");
                onComplete?.Invoke(false);
                return;
            }
            
            if (string.IsNullOrEmpty(productId))
            {
                Debug.LogError("PurchaseManager: ProductId boş olamaz!");
                onComplete?.Invoke(false);
                return;
            }
            
            if (_ownedProducts.Contains(productId))
            {
                Debug.LogWarning($"PurchaseManager: '{productId}' ürünü zaten satın alınmış!");
                onComplete?.Invoke(true);
                return;
            }
            
            Debug.Log($"PurchaseManager: '{productId}' ürünü satın alınıyor... (Simüle edilmiş)");
            
            // Simülasyon: Satın alma başarılı olarak kabul ediliyor
            // Gerçek implementasyonda: Unity IAP veya başka IAP SDK'sı kullanılacak
            _ownedProducts.Add(productId);
            Debug.Log($"PurchaseManager: '{productId}' ürünü başarıyla satın alındı! (Simüle edilmiş)");
            
            if (onComplete != null)
            {
                onComplete.Invoke(true);
            }
        }
        
        /// <summary>
        /// Ürün sahip olunan ürünler arasında mı kontrol eder (Simüle edilmiş)
        /// </summary>
        /// <param name="productId">Kontrol edilecek ürün ID'si</param>
        /// <returns>Ürün sahip olunan ürünler arasında mı?</returns>
        public bool IsProductOwned(string productId)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("PurchaseManager: Satın alma servisi henüz başlatılmadı! InitializePurchaseService() metodunu çağırın.");
                return false;
            }
            
            if (string.IsNullOrEmpty(productId))
            {
                Debug.LogWarning("PurchaseManager: ProductId boş olamaz!");
                return false;
            }
            
            bool isOwned = _ownedProducts.Contains(productId);
            Debug.Log($"PurchaseManager: '{productId}' ürünü sahip olunan ürünler arasında mı? {isOwned} (Simüle edilmiş)");
            
            return isOwned;
        }
        
        #endregion
        
        #region Public Methods (Simülasyon için)
        
        /// <summary>
        /// Tüm sahip olunan ürünleri temizler (Test amaçlı)
        /// </summary>
        public void ClearOwnedProducts()
        {
            _ownedProducts.Clear();
            Debug.Log("PurchaseManager: Tüm sahip olunan ürünler temizlendi (Test amaçlı)");
        }
        
        /// <summary>
        /// Manuel olarak ürün ekler (Test amaçlı)
        /// </summary>
        /// <param name="productId">Eklenecek ürün ID'si</param>
        public void AddOwnedProduct(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                Debug.LogWarning("PurchaseManager: ProductId boş olamaz!");
                return;
            }
            
            _ownedProducts.Add(productId);
            Debug.Log($"PurchaseManager: '{productId}' ürünü manuel olarak eklendi (Test amaçlı)");
        }
        
        #endregion
    }
}

