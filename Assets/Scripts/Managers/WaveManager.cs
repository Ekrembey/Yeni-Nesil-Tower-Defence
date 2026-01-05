using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.ScriptableObjects;
using Game.Core;
using Game.Enemies;

namespace Game.Managers
{
    /// <summary>
    /// Dalga yönetimi yapan sınıf. Singleton pattern ile çalışır.
    /// Level ve dalga sistemini yönetir, düşman spawn işlemlerini kontrol eder.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Singleton
        
        private static WaveManager _instance;
        
        /// <summary>
        /// WaveManager'ın tek instance'ı
        /// </summary>
        public static WaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<WaveManager>();
                    
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("WaveManager");
                        _instance = go.AddComponent<WaveManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                
                return _instance;
            }
        }
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Dalga başladığında tetiklenir (dalga index'i)
        /// </summary>
        public event Action<int> OnWaveStarted;
        
        /// <summary>
        /// Dalga tamamlandığında tetiklenir (dalga index'i)
        /// </summary>
        public event Action<int> OnWaveCompleted;
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private LevelData _currentLevelData;
        
        [SerializeField] private WaypointPath _waypointPath;
        
        #endregion
        
        #region Private Fields
        
        private int _currentWaveIndex = -1;
        
        private int _spawnedEnemyCount = 0;
        
        private int _killedEnemyCount = 0;
        
        private bool _isWaveActive = false;
        
        private Coroutine _spawnCoroutine;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Mevcut level verisi
        /// </summary>
        public LevelData CurrentLevelData
        {
            get { return _currentLevelData; }
            private set { _currentLevelData = value; }
        }
        
        /// <summary>
        /// Waypoint path referansı
        /// </summary>
        public WaypointPath WaypointPath
        {
            get { return _waypointPath; }
            set { _waypointPath = value; }
        }
        
        /// <summary>
        /// Mevcut dalga index'i (-1 = dalga başlamadı)
        /// </summary>
        public int CurrentWaveIndex
        {
            get { return _currentWaveIndex; }
            private set { _currentWaveIndex = value; }
        }
        
        /// <summary>
        /// Dalga aktif mi?
        /// </summary>
        public bool IsWaveActive
        {
            get { return _isWaveActive; }
            private set { _isWaveActive = value; }
        }
        
        /// <summary>
        /// Toplam spawn edilen düşman sayısı (mevcut dalga için)
        /// </summary>
        public int SpawnedEnemyCount
        {
            get { return _spawnedEnemyCount; }
            private set { _spawnedEnemyCount = value; }
        }
        
        /// <summary>
        /// Toplam öldürülen düşman sayısı (mevcut dalga için)
        /// </summary>
        public int KilledEnemyCount
        {
            get { return _killedEnemyCount; }
            private set { _killedEnemyCount = value; }
        }
        
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
            
            // Eğer WaypointPath atanmamışsa, sahnedeki Path objesini otomatik bul
            if (_waypointPath == null)
            {
                _waypointPath = FindObjectOfType<WaypointPath>();
                if (_waypointPath != null)
                {
                    Debug.Log($"WaveManager: WaypointPath otomatik olarak bulundu - {_waypointPath.name}");
                }
                else
                {
                    Debug.LogWarning("WaveManager: Sahne'de WaypointPath component'i bulunamadı! Lütfen Path objesini Inspector'da atayın.");
                }
            }
        }

        private void Start()
        {
            if (_currentLevelData != null)
            {
                StartLevel(_currentLevelData);
            }

            StartNextWave();
        }
        
        private void OnEnable()
        {
            // Sahne yüklendiğinde referansları yeniden bulmak için event'e abone ol
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            // Event'ten aboneliği kaldır
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void OnDestroy()
        {
            // Event'ten aboneliği kaldır (güvenlik için)
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            // Coroutine'i durdur
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
        }
        
        /// <summary>
        /// Sahne yüklendiğinde çağrılır. Referansları yeniden bulur ve oyunu başlatır.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // WaypointPath referansını yeniden bul
            if (_waypointPath == null)
            {
                _waypointPath = FindObjectOfType<WaypointPath>();
                if (_waypointPath != null)
                {
                    Debug.Log($"WaveManager: Sahne yüklendi - WaypointPath yeniden bulundu: {_waypointPath.name}");
                }
                else
                {
                    Debug.LogWarning("WaveManager: Sahne yüklendi ancak WaypointPath bulunamadı!");
                }
            }
            else
            {
                // Referans var ama geçersiz olabilir (Missing durumu)
                if (_waypointPath == null || _waypointPath.Equals(null))
                {
                    _waypointPath = FindObjectOfType<WaypointPath>();
                    if (_waypointPath != null)
                    {
                        Debug.Log($"WaveManager: Sahne yüklendi - WaypointPath referansı yenilendi: {_waypointPath.name}");
                    }
                }
            }
            
            // Oyun durumunu sıfırla
            CurrentWaveIndex = -1;
            SpawnedEnemyCount = 0;
            KilledEnemyCount = 0;
            IsWaveActive = false;
            
            // Coroutine'i durdur (eğer çalışıyorsa)
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            
            // Level'i ve dalgayı yeniden başlat (Start() metodundaki mantık)
            if (_currentLevelData != null)
            {
                StartLevel(_currentLevelData);
            }
            
            // İlk dalgayı başlat
            StartNextWave();
            
            Debug.Log("WaveManager: Sahne yüklendi - Oyun durumu sıfırlandı ve dalga başlatıldı.");
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Level'i başlatır
        /// </summary>
        /// <param name="levelData">Başlatılacak level verisi</param>
        public void StartLevel(LevelData levelData)
        {
            if (levelData == null)
            {
                Debug.LogError("WaveManager: LevelData null olamaz!");
                return;
            }
            
            if (!levelData.IsValid())
            {
                Debug.LogError($"WaveManager: LevelData '{levelData.name}' geçersiz!");
                return;
            }
            
            CurrentLevelData = levelData;
            CurrentWaveIndex = -1;
            SpawnedEnemyCount = 0;
            KilledEnemyCount = 0;
            IsWaveActive = false;
            
            // Başlangıç altınını ayarla
            if (GameManager.Instance != null && GameManager.Instance.CurrencyManager != null)
            {
                // CurrencyManager'ın ResetCurrency metodunu kullanabiliriz veya direkt set edebiliriz
                // Şimdilik sadece log basıyoruz
                Debug.Log($"WaveManager: Level {levelData.LevelNumber} başlatıldı. Başlangıç Altın: {levelData.StartingGold}");
            }
            
            Debug.Log($"WaveManager: Level {levelData.LevelNumber} başlatıldı. Toplam {levelData.WaveCount} dalga var.");
        }
        
        /// <summary>
        /// Sonraki dalgayı başlatır
        /// </summary>
        public void StartNextWave()
        {
            if (CurrentLevelData == null)
            {
                Debug.LogError("WaveManager: LevelData atanmamış! StartLevel() metodunu çağırın.");
                return;
            }
            
            if (IsWaveActive)
            {
                Debug.LogWarning("WaveManager: Zaten aktif bir dalga var! Önce mevcut dalga bitmeli.");
                return;
            }
            
            CurrentWaveIndex++;
            
            if (CurrentWaveIndex >= CurrentLevelData.WaveCount)
            {
                Debug.Log($"WaveManager: Tüm dalgalar tamamlandı! Level {CurrentLevelData.LevelNumber} bitti.");
                return;
            }
            
            WaveData waveData = CurrentLevelData.GetWave(CurrentWaveIndex);
            
            if (waveData == null)
            {
                Debug.LogError($"WaveManager: Wave {CurrentWaveIndex} bulunamadı!");
                return;
            }
            
            if (!waveData.IsValid())
            {
                Debug.LogError($"WaveManager: Wave {CurrentWaveIndex} geçersiz!");
                return;
            }
            
            IsWaveActive = true;
            SpawnedEnemyCount = 0;
            KilledEnemyCount = 0;
            
            Debug.Log($"WaveManager: Dalga {CurrentWaveIndex + 1} başlatıldı. {waveData.Count} düşman spawn edilecek.");
            
            // Event tetikle
            if (OnWaveStarted != null)
            {
                OnWaveStarted.Invoke(CurrentWaveIndex);
            }
            
            // Spawn coroutine'ini başlat
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            
            _spawnCoroutine = StartCoroutine(SpawnWaveCoroutine(waveData));
        }
        
        /// <summary>
        /// Düşman öldürüldüğünde çağrılır
        /// </summary>
        public void OnEnemyKilled()
        {
            if (!IsWaveActive)
            {
                return;
            }
            
            KilledEnemyCount++;
            
            Debug.Log($"WaveManager: Düşman öldürüldü. Öldürülen: {KilledEnemyCount}/{SpawnedEnemyCount}");
            
            // Dalga bitiş kontrolü
            CheckWaveCompletion();
        }
        
        /// <summary>
        /// Mevcut dalgayı durdurur
        /// </summary>
        public void StopCurrentWave()
        {
            if (!IsWaveActive)
            {
                return;
            }
            
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
                _spawnCoroutine = null;
            }
            
            IsWaveActive = false;
            Debug.Log($"WaveManager: Dalga {CurrentWaveIndex + 1} durduruldu.");
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Dalga spawn coroutine'i
        /// </summary>
        private IEnumerator SpawnWaveCoroutine(WaveData waveData)
        {
            for (int i = 0; i < waveData.Count; i++)
            {
                SpawnEnemy(waveData.EnemyData);
                SpawnedEnemyCount++;
                
                // Son düşman değilse bekle
                if (i < waveData.Count - 1)
                {
                    yield return new WaitForSeconds(waveData.SpawnInterval);
                }
            }
            
            Debug.Log($"WaveManager: Tüm düşmanlar spawn edildi. Toplam: {SpawnedEnemyCount}");
        }
        
        /// <summary>
        /// Düşman spawn eder
        /// </summary>
        /// <param name="enemyData">Spawn edilecek düşman verisi</param>
        private void SpawnEnemy(EnemyData enemyData)
        {
            if (enemyData == null)
            {
                Debug.LogError("WaveManager: EnemyData null olamaz!");
                return;
            }
            
            if (enemyData.EnemyPrefab == null)
            {
                Debug.LogError($"WaveManager: EnemyData '{enemyData.EnemyName}' için prefab atanmamış!");
                return;
            }
            
            // Eğer path atanmamışsa, tekrar dene (Awake'te bulunamadıysa)
            if (_waypointPath == null)
            {
                _waypointPath = FindObjectOfType<WaypointPath>();
                if (_waypointPath == null)
                {
                    Debug.LogError("WaveManager: WaypointPath atanmamış ve sahnede bulunamadı! Düşman spawn edilemiyor.");
                    return;
                }
                Debug.Log($"WaveManager: WaypointPath SpawnEnemy() içinde bulundu - {_waypointPath.name}");
            }
            
            if (!_waypointPath.IsValid())
            {
                Debug.LogError("WaveManager: WaypointPath geçersiz! Düşman spawn edilemiyor.");
                return;
            }
            
            // Spawn pozisyonu: İlk waypoint pozisyonu
            Vector3 spawnPosition = _waypointPath.StartPosition;
            
            // Düşman prefab'ını spawn et
            GameObject yeniDusman = Instantiate(enemyData.EnemyPrefab, spawnPosition, Quaternion.identity);
            
            if (yeniDusman == null)
            {
                Debug.LogError($"WaveManager: Düşman spawn edilemedi! Prefab: {enemyData.EnemyName}");
                return;
            }
            
            // Düşmanın materyalini kırmızı yap
            MeshRenderer renderer = yeniDusman.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // Materyalin rengini kırmızı yap
                Material mat = renderer.material;
                if (mat != null)
                {
                    mat.color = Color.red;
                    Debug.Log($"WaveManager: Düşman rengi kırmızı olarak ayarlandı.");
                }
            }
            
            // EnemyMover component'ini al veya ekle
            EnemyMover enemyMover = yeniDusman.GetComponent<EnemyMover>();
            
            if (enemyMover == null)
            {
                enemyMover = yeniDusman.AddComponent<EnemyMover>();
            }
            
            // EnemyMover'ı initialize et (WaypointPath ve EnemyData referanslarını ver ve hareketi başlat)
            // Initialize() metodu zaten Setup()'ı da içeriyor, bu yüzden direkt Initialize() çağırıyoruz
            if (enemyMover != null)
            {
                enemyMover.Initialize(_waypointPath, enemyData);
                
                // Düşman son waypoint'e ulaştığında bildirim al
                enemyMover.OnReachedEnd += () =>
                {
                    // Düşman sona ulaştı (oyuncu can kaybetti)
                    Debug.LogWarning($"WaveManager: Düşman sona ulaştı! {enemyData.EnemyName}");
                    
                    // Oyuncu canını azalt
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.TakeDamage(1);
                    }
                    else
                    {
                        Debug.LogError("WaveManager: GameManager.Instance null! Can azaltılamadı.");
                    }
                    
                    // Düşman objesini yok et (artık işi bitti)
                    if (yeniDusman != null)
                    {
                        Destroy(yeniDusman);
                    }
                };
            }
            
            Debug.Log($"WaveManager: Düşman spawn edildi - {enemyData.EnemyName} (Can: {enemyData.Health}, Hız: {enemyData.Speed})");
        }
        
        /// <summary>
        /// Dalga tamamlanma kontrolü yapar
        /// </summary>
        private void CheckWaveCompletion()
        {
            if (!IsWaveActive)
            {
                return;
            }
            
            // Tüm düşmanlar spawn edildi ve öldürüldü mü?
            WaveData currentWave = CurrentLevelData.GetWave(CurrentWaveIndex);
            
            if (currentWave != null && SpawnedEnemyCount >= currentWave.Count && KilledEnemyCount >= currentWave.Count)
            {
                CompleteWave();
            }
        }
        
        /// <summary>
        /// Dalgayı tamamlar
        /// </summary>
        private void CompleteWave()
        {
            IsWaveActive = false;
            
            Debug.Log($"WaveManager: Dalga {CurrentWaveIndex + 1} tamamlandı!");
            
            // Event tetikle
            if (OnWaveCompleted != null)
            {
                OnWaveCompleted.Invoke(CurrentWaveIndex);
            }
        }
        
        #endregion
    }
}

