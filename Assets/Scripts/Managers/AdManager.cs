using UnityEngine;
using System;
using Game.Interfaces;

namespace Game.Managers
{
    /// <summary>
    /// Reklam yönetimi yapan sınıf. IAdService interface'ini implement eder.
    /// Şimdilik Debug.Log ile simüle edilir, gelecekte gerçek reklam servisi entegre edilecek.
    /// </summary>
    public class AdManager : MonoBehaviour, IAdService
    {
        #region Singleton
        
        private static AdManager _instance;
        
        /// <summary>
        /// AdManager'ın tek instance'ı
        /// </summary>
        public static AdManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AdManager>();
                    
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AdManager");
                        _instance = go.AddComponent<AdManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                
                return _instance;
            }
        }
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private bool _isInitialized = false;
        
        [SerializeField] private bool _isRewardedAdReady = true; // Simülasyon için varsayılan true
        
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
        
        #region IAdService Implementation
        
        /// <summary>
        /// Reklam servisini başlatır (Simüle edilmiş)
        /// </summary>
        public void InitializeAdService()
        {
            if (_isInitialized)
            {
                Debug.Log("AdManager: Reklam servisi zaten başlatılmış!");
                return;
            }
            
            Debug.Log("AdManager: Reklam servisi başlatılıyor... (Simüle edilmiş)");
            _isInitialized = true;
            Debug.Log("AdManager: Reklam servisi başlatıldı! (Simüle edilmiş)");
        }
        
        /// <summary>
        /// Banner reklamı gösterir (Simüle edilmiş)
        /// </summary>
        public void ShowBannerAd()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("AdManager: Reklam servisi henüz başlatılmadı! InitializeAdService() metodunu çağırın.");
                return;
            }
            
            Debug.Log("AdManager: Banner reklam gösteriliyor... (Simüle edilmiş)");
            // Gerçek implementasyonda: AdMob veya başka reklam SDK'sı kullanılacak
        }
        
        /// <summary>
        /// Interstitial (tam ekran) reklamı gösterir (Simüle edilmiş)
        /// </summary>
        public void ShowInterstitialAd()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("AdManager: Reklam servisi henüz başlatılmadı! InitializeAdService() metodunu çağırın.");
                return;
            }
            
            Debug.Log("AdManager: Interstitial reklam gösteriliyor... (Simüle edilmiş)");
            // Gerçek implementasyonda: AdMob veya başka reklam SDK'sı kullanılacak
        }
        
        /// <summary>
        /// Ödüllü reklamı gösterir (Simüle edilmiş)
        /// </summary>
        /// <param name="onComplete">Reklam izlendiğinde çağrılacak callback</param>
        public void ShowRewardedAd(Action onComplete)
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("AdManager: Reklam servisi henüz başlatılmadı! InitializeAdService() metodunu çağırın.");
                onComplete?.Invoke(); // Hata durumunda bile callback çağrılsın (fallback)
                return;
            }
            
            if (!IsRewardedAdReady())
            {
                Debug.LogWarning("AdManager: Ödüllü reklam hazır değil!");
                onComplete?.Invoke(); // Hata durumunda bile callback çağrılsın (fallback)
                return;
            }
            
            Debug.Log("AdManager: Ödüllü reklam gösteriliyor... (Simüle edilmiş)");
            
            // Simülasyon: Reklam izlendi olarak kabul edilip callback çağrılıyor
            // Gerçek implementasyonda: Reklam SDK'sının callback'i kullanılacak
            if (onComplete != null)
            {
                Debug.Log("AdManager: Ödüllü reklam izlendi, ödül veriliyor... (Simüle edilmiş)");
                onComplete.Invoke();
            }
        }
        
        /// <summary>
        /// Ödüllü reklam hazır mı kontrol eder (Simüle edilmiş)
        /// </summary>
        /// <returns>Ödüllü reklam hazır mı?</returns>
        public bool IsRewardedAdReady()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("AdManager: Reklam servisi henüz başlatılmadı! InitializeAdService() metodunu çağırın.");
                return false;
            }
            
            // Simülasyon için varsayılan olarak true döndürülüyor
            // Gerçek implementasyonda: Reklam SDK'sının hazır olup olmadığını kontrol edecek
            return _isRewardedAdReady;
        }
        
        #endregion
        
        #region Public Methods (Simülasyon için)
        
        /// <summary>
        /// Ödüllü reklamın hazır olup olmadığını manuel olarak ayarlar (Test amaçlı)
        /// </summary>
        /// <param name="isReady">Hazır mı?</param>
        public void SetRewardedAdReady(bool isReady)
        {
            _isRewardedAdReady = isReady;
            Debug.Log($"AdManager: Ödüllü reklam hazır durumu: {isReady} (Test amaçlı)");
        }
        
        #endregion
    }
}

