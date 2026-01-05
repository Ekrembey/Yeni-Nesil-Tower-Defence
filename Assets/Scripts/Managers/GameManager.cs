using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Game.Managers
{
    /// <summary>
    /// Oyunun ana yönetici sınıfı. Singleton pattern ile çalışır.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        
        private static GameManager _instance;
        
        /// <summary>
        /// GameManager'ın tek instance'ı
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<GameManager>();
                    
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                
                return _instance;
            }
        }
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private CurrencyManager _currencyManager;
        
        [SerializeField] private bool _isGamePaused = false;
        
        [SerializeField] private bool _isGameStarted = false;
        
        [SerializeField] private int _startingLives = 3;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Can değiştiğinde tetiklenir (mevcut can)
        /// </summary>
        public event Action<int> OnLivesChanged;
        
        #endregion
        
        #region Private Fields
        
        private int _currentLives;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Para yöneticisi referansı
        /// </summary>
        public CurrencyManager CurrencyManager
        {
            get
            {
                if (_currencyManager == null)
                {
                    _currencyManager = FindObjectOfType<CurrencyManager>();
                    
                    if (_currencyManager == null)
                    {
                        Debug.LogError("CurrencyManager bulunamadı! Lütfen sahneye ekleyin.");
                    }
                }
                
                return _currencyManager;
            }
        }
        
        /// <summary>
        /// Oyun durduruldu mu?
        /// </summary>
        public bool IsGamePaused
        {
            get { return _isGamePaused; }
            private set { _isGamePaused = value; }
        }
        
        /// <summary>
        /// Oyun başladı mı?
        /// </summary>
        public bool IsGameStarted
        {
            get { return _isGameStarted; }
            private set { _isGameStarted = value; }
        }
        
        /// <summary>
        /// Mevcut can sayısı
        /// </summary>
        public int CurrentLives
        {
            get { return _currentLives; }
            private set { _currentLives = value; }
        }
        
        /// <summary>
        /// Başlangıç can sayısı
        /// </summary>
        public int StartingLives
        {
            get { return _startingLives; }
            private set { _startingLives = value; }
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeManagers();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Can sistemini başlat
            CurrentLives = StartingLives;
            
            // Manager'ları başlat
            if (CurrencyManager != null)
            {
                CurrencyManager.Initialize();
            }
            
            // Event tetikle (başlangıç canı için)
            if (OnLivesChanged != null)
            {
                OnLivesChanged.Invoke(CurrentLives);
            }
        }
        
        private void Update()
        {
            // Test için: R tuşuna basıldığında oyunu yeniden başlat
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Manager'ları başlatır
        /// </summary>
        private void InitializeManagers()
        {
            // CurrencyManager kontrolü - önce aynı GameObject'te ara, sonra sahnede ara
            if (_currencyManager == null)
            {
                // Önce aynı GameObject'teki component'i kontrol et
                _currencyManager = GetComponent<CurrencyManager>();
                
                // Bulunamazsa sahnede ara
                if (_currencyManager == null)
                {
                    _currencyManager = FindObjectOfType<CurrencyManager>();
                }
                
                if (_currencyManager != null)
                {
                    Debug.Log($"GameManager: CurrencyManager otomatik olarak bulundu - {_currencyManager.name}");
                }
                else
                {
                    Debug.LogWarning("GameManager: CurrencyManager bulunamadı! Lütfen sahneye ekleyin.");
                }
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Oyunu başlatır
        /// </summary>
        public void StartGame()
        {
            if (IsGameStarted)
            {
                Debug.LogWarning("Oyun zaten başlamış durumda!");
                return;
            }
            
            IsGameStarted = true;
            IsGamePaused = false;
            
            Debug.Log("Oyun başlatıldı!");
        }
        
        /// <summary>
        /// Oyunu duraklatır
        /// </summary>
        public void PauseGame()
        {
            if (!IsGameStarted)
            {
                Debug.LogWarning("Oyun henüz başlamadı!");
                return;
            }
            
            IsGamePaused = true;
            Time.timeScale = 0f;
            
            Debug.Log("Oyun duraklatıldı!");
        }
        
        /// <summary>
        /// Oyunu devam ettirir
        /// </summary>
        public void ResumeGame()
        {
            if (!IsGameStarted)
            {
                Debug.LogWarning("Oyun henüz başlamadı!");
                return;
            }
            
            IsGamePaused = false;
            Time.timeScale = 1f;
            
            Debug.Log("Oyun devam ediyor!");
        }
        
        /// <summary>
        /// Oyunu sonlandırır
        /// </summary>
        public void EndGame()
        {
            IsGameStarted = false;
            IsGamePaused = false;
            Time.timeScale = 1f;
            
            Debug.Log("Oyun sonlandırıldı!");
        }
        
        /// <summary>
        /// Oyuncu canını azaltır
        /// </summary>
        /// <param name="amount">Azaltılacak can miktarı</param>
        public void TakeDamage(int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"GameManager: TakeDamage() negatif veya sıfır miktar ile çağrıldı: {amount}");
                return;
            }
            
            CurrentLives -= amount;
            
            // Can 0 veya altına düştüyse oyunu bitir
            if (CurrentLives <= 0)
            {
                CurrentLives = 0;
                GameOver();
            }
            
            // Event tetikle (UI güncellemesi için)
            if (OnLivesChanged != null)
            {
                OnLivesChanged.Invoke(CurrentLives);
            }
            
            Debug.Log($"GameManager: Can azaldı. Kalan Can: {CurrentLives}");
        }
        
        /// <summary>
        /// Oyun bittiğinde çağrılır
        /// </summary>
        private void GameOver()
        {
            Debug.LogWarning("GameManager: OYUN BİTTİ! Tüm canlar tükendi.");
            
            // Oyunu fiziksel olarak durdur (tüm hareket ve fizik olayları donacak)
            Time.timeScale = 0f;
            
            // TODO: Game Over UI göster, skor kaydet, vb.
        }
        
        /// <summary>
        /// Oyunu yeniden başlatır
        /// </summary>
        public void RestartGame()
        {
            // Önce zamanı tekrar normal hıza al
            Time.timeScale = 1f;
            
            // Oyun durumunu sıfırla
            IsGameStarted = false;
            IsGamePaused = false;
            
            // Mevcut sahneyi baştan yükle
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            
            Debug.Log("GameManager: Oyun yeniden başlatıldı!");
        }
        
        #endregion
    }
}

