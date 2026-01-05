using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using Game.ScriptableObjects;

namespace Game.Enemies
{
    /// <summary>
    /// Düşmanın waypoint path'i takip etmesini sağlayan sınıf.
    /// EnemyData'dan hız bilgisini alır ve waypoint'leri sırayla takip eder.
    /// </summary>
    public class EnemyMover : MonoBehaviour
    {
        #region Events
        
        /// <summary>
        /// Son waypoint'e ulaşıldığında tetiklenir
        /// </summary>
        public event Action OnReachedEnd;
        
        #endregion
        
        #region Serialized Fields
        
        [SerializeField] private WaypointPath _waypointPath;
        
        [SerializeField] private EnemyData _enemyData;
        
        #endregion
        
        #region Private Fields
        
        private int _currentWaypointIndex = 0;
        
        private bool _isMoving = false;
        
        private Coroutine _movementCoroutine;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Waypoint path referansı
        /// </summary>
        public WaypointPath WaypointPath
        {
            get { return _waypointPath; }
            set
            {
                if (_isMoving)
                {
                    StopMovement();
                }
                _waypointPath = value;
            }
        }
        
        /// <summary>
        /// Enemy data referansı
        /// </summary>
        public EnemyData EnemyData
        {
            get { return _enemyData; }
            set { _enemyData = value; }
        }
        
        /// <summary>
        /// Düşman hareket ediyor mu?
        /// </summary>
        public bool IsMoving
        {
            get { return _isMoving; }
            private set { _isMoving = value; }
        }
        
        /// <summary>
        /// Mevcut hız
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                if (_enemyData != null)
                {
                    return _enemyData.Speed;
                }
                return 0f;
            }
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void Awake()
        {
            // EnemyData yoksa uyarı ver
            if (_enemyData == null)
            {
                Debug.LogWarning($"EnemyMover '{name}': EnemyData atanmamış!");
            }
            
            // WaypointPath yoksa uyarı ver
            if (_waypointPath == null)
            {
                Debug.LogWarning($"EnemyMover '{name}': WaypointPath atanmamış!");
            }
        }
        
        private void Start()
        {
            // WaveManager'dan path Initialize() ile atanacak
            // Start() içinde path kontrolü yapmıyoruz çünkü Initialize() henüz çağrılmamış olabilir
            // Initialize() metodu zaten StartMovement() çağırıyor, bu yüzden burada bir şey yapmıyoruz
        }
        
        private void OnDestroy()
        {
            StopMovement();
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Hareketi başlatır
        /// </summary>
        public void StartMovement()
        {
            if (_isMoving)
            {
                Debug.LogWarning($"EnemyMover '{name}': Zaten hareket ediyor!");
                return;
            }
            
            if (_waypointPath == null)
            {
                Debug.LogError($"EnemyMover '{name}': WaypointPath atanmamış!");
                return;
            }
            
            if (!_waypointPath.IsValid())
            {
                Debug.LogError($"EnemyMover '{name}': WaypointPath geçersiz!");
                return;
            }
            
            if (_enemyData == null)
            {
                Debug.LogError($"EnemyMover '{name}': EnemyData atanmamış!");
                return;
            }
            
            _currentWaypointIndex = 0;
            IsMoving = true;
            
            // İlk waypoint pozisyonuna yerleştir (eğer zaten orada değilse)
            Transform firstWaypoint = _waypointPath.GetWaypoint(0);
            if (firstWaypoint != null)
            {
                // Sadece pozisyon farklıysa yerleştir (spawn pozisyonu zaten doğru olabilir)
                float distance = Vector3.Distance(transform.position, firstWaypoint.position);
                if (distance > 0.1f)
                {
                    transform.position = firstWaypoint.position;
                }
            }
            
            // Hareket coroutine'ini başlat
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
            }
            
            _movementCoroutine = StartCoroutine(MoveToWaypoints());
        }
        
        /// <summary>
        /// Hareketi durdurur
        /// </summary>
        public void StopMovement()
        {
            if (!_isMoving)
            {
                return;
            }
            
            IsMoving = false;
            
            if (_movementCoroutine != null)
            {
                StopCoroutine(_movementCoroutine);
                _movementCoroutine = null;
            }
        }
        
        /// <summary>
        /// Waypoint path'i ayarlar
        /// </summary>
        /// <param name="path">Takip edilecek waypoint path</param>
        public void Setup(WaypointPath path)
        {
            _waypointPath = path;
        }
        
        /// <summary>
        /// Waypoint path ve enemy data'yı ayarlar ve hareketi başlatır
        /// </summary>
        /// <param name="waypointPath">Takip edilecek waypoint path</param>
        /// <param name="enemyData">Düşman verisi (hız bilgisi için)</param>
        public void Initialize(WaypointPath waypointPath, EnemyData enemyData)
        {
            if (waypointPath == null)
            {
                Debug.LogError($"EnemyMover '{name}': WaypointPath null olamaz!");
                return;
            }
            
            if (enemyData == null)
            {
                Debug.LogError($"EnemyMover '{name}': EnemyData null olamaz!");
                return;
            }
            
            _waypointPath = waypointPath;
            _enemyData = enemyData;
            
            StartMovement();
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Waypoint'leri sırayla takip eden coroutine
        /// </summary>
        private IEnumerator MoveToWaypoints()
        {
            if (_waypointPath == null || _enemyData == null)
            {
                yield break;
            }
            
            int waypointCount = _waypointPath.WaypointCount;
            
            // İlk waypoint'ten başla (zaten oradayız)
            for (int i = 1; i < waypointCount; i++)
            {
                Transform targetWaypoint = _waypointPath.GetWaypoint(i);
                
                if (targetWaypoint == null)
                {
                    Debug.LogWarning($"EnemyMover '{name}': Waypoint {i} null!");
                    continue;
                }
                
                // Hedef waypoint'e kadar hareket et
                yield return StartCoroutine(MoveToPosition(targetWaypoint.position));
            }
            
            // Son waypoint'e ulaşıldı
            IsMoving = false;
            
            // Event tetikle
            if (OnReachedEnd != null)
            {
                OnReachedEnd.Invoke();
            }
        }
        
        /// <summary>
        /// Belirli bir pozisyona hareket eden coroutine
        /// </summary>
        private IEnumerator MoveToPosition(Vector3 targetPosition)
        {
            Vector3 startPosition = transform.position;
            float distance = Vector3.Distance(startPosition, targetPosition);
            
            if (distance <= 0.01f)
            {
                transform.position = targetPosition;
                yield break;
            }
            
            float speed = CurrentSpeed;
            if (speed <= 0f)
            {
                Debug.LogWarning($"EnemyMover '{name}': Hız 0 veya negatif! Hareket edilemiyor.");
                yield break;
            }
            
            float travelTime = distance / speed;
            float elapsedTime = 0f;
            
            while (elapsedTime < travelTime)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / travelTime);
                
                transform.position = Vector3.Lerp(startPosition, targetPosition, t);
                
                // Hedefe doğru bak
                if (targetPosition != transform.position)
                {
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    if (direction != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(direction);
                    }
                }
                
                yield return null;
            }
            
            // Son pozisyonu garanti et
            transform.position = targetPosition;
        }
        
        #endregion
    }
}

