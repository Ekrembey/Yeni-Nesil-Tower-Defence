using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Game.Towers;

namespace Game.Managers
{
    /// <summary>
    /// Kule yerleştirme ve yönetiminden sorumlu sınıf.
    /// Plane (zemin) üzerine tıklanınca Tower_Prefab instantiate eder.
    /// </summary>
    public class TowerManager : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private Camera _camera;

        [SerializeField] private GameObject _towerPrefab;

        [SerializeField] private float _minTowerSpacing = 2f;

        [SerializeField] private string _groundObjectName = "Plane";

        #endregion

        #region Private Fields

        private readonly List<Transform> _spawnedTowers = new List<Transform>();

        private Transform _groundTransform;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_camera == null)
            {
                Debug.LogError("TowerManager: Kamera bulunamadı! Lütfen _camera alanını atayın.");
            }

            if (_towerPrefab == null)
            {
                Debug.LogWarning("TowerManager: Tower_Prefab atanmadı! Lütfen Inspector'dan atayın.");
            }

            GameObject ground = GameObject.Find(_groundObjectName);
            if (ground != null)
            {
                _groundTransform = ground.transform;
                Debug.Log($"TowerManager: Zemin objesi bulundu: {_groundTransform.name}");
            }
            else
            {
                Debug.LogWarning($"TowerManager: '{_groundObjectName}' isimli zemin objesi bulunamadı. Raycast yine de çalışacak, ancak isim kontrolü başarısız olabilir.");
            }
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
        
        private void Update()
        {
            if (_camera == null || _towerPrefab == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                TryHandlePlacementClick();
            }
        }
        
        /// <summary>
        /// Sahne yüklendiğinde çağrılır. Referansları yeniden bulur.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Camera referansını yeniden bul
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera != null)
                {
                    Debug.Log($"TowerManager: Sahne yüklendi - Camera yeniden bulundu: {_camera.name}");
                }
                else
                {
                    Debug.LogWarning("TowerManager: Sahne yüklendi ancak Camera.main bulunamadı!");
                }
            }
            else
            {
                // Referans var ama geçersiz olabilir (Missing durumu)
                if (_camera == null || _camera.Equals(null))
                {
                    _camera = Camera.main;
                    if (_camera != null)
                    {
                        Debug.Log($"TowerManager: Sahne yüklendi - Camera referansı yenilendi: {_camera.name}");
                    }
                }
            }
            
            // Zemin objesini de yeniden bul
            GameObject ground = GameObject.Find(_groundObjectName);
            if (ground != null)
            {
                _groundTransform = ground.transform;
                Debug.Log($"TowerManager: Sahne yüklendi - Zemin objesi yeniden bulundu: {_groundTransform.name}");
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Verilen dünyasal pozisyona kule yerleştirmeye çalışır.
        /// </summary>
        /// <param name="worldPosition">Dünyasal pozisyon.</param>
        /// <returns>Kule yerleştirildiyse true.</returns>
        public bool TryPlaceTowerAt(Vector3 worldPosition)
        {
            if (!CanPlaceAt(worldPosition))
            {
                Debug.Log("TowerManager: Bu noktaya kule yerleştirilemez, başka kuleye çok yakın.");
                return false;
            }

            Quaternion rotation = Quaternion.identity;
            GameObject towerInstance = Instantiate(_towerPrefab, worldPosition, rotation);

            if (towerInstance == null)
            {
                Debug.LogError("TowerManager: Kule instantiate edilemedi!");
                return false;
            }

            _spawnedTowers.Add(towerInstance.transform);
            return true;
        }

        #endregion

        #region Private Methods

        private void TryHandlePlacementClick()
        {
            // UI üzerine tıklanmışsa kule yerleştirme
            if (UnityEngine.EventSystems.EventSystem.current != null && 
                UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("TowerManager: UI üzerine tıklandı, kule yerleştirilmedi.");
                return;
            }

            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            
            // Layer mask kullanmadan tüm objeleri kontrol et
            int layerMask = ~0; // Tüm layer'lar
            if (!Physics.Raycast(ray, out hitInfo, 1000f, layerMask))
            {
                Debug.Log($"TowerManager: Raycast hiçbir şeye çarpmadı. Mouse pozisyon: {Input.mousePosition}");
                return;
            }

            if (hitInfo.collider == null)
            {
                Debug.Log("TowerManager: Raycast collider buldu ama collider null geldi.");
                return;
            }

            Debug.Log($"TowerManager: Raycast '{hitInfo.collider.name}' objesine çarptı. Hit point: {hitInfo.point}, World position: {hitInfo.point}");

            // Sadece Plane üzerinde yerleştir
            if (!hitInfo.collider.name.Equals(_groundObjectName))
            {
                Debug.Log($"TowerManager: Raycast '{hitInfo.collider.name}' objesine çarptı, zemin '{_groundObjectName}' olmadığı için kule yerleştirilmedi.");
                return;
            }

            // Raycast'in döndürdüğü pozisyonu kullan (bu zaten dünya koordinatlarında)
            Vector3 placePosition = hitInfo.point;

            // Kule yüksekliği için hafif offset ekle (küplerin yarı yüksekliği)
            placePosition.y = 0.5f; // Sabit yükseklik

            Debug.Log($"TowerManager: Kule yerleştirme denemesi. Pozisyon: {placePosition}, Mevcut kule sayısı: {_spawnedTowers.Count}");

            bool placed = TryPlaceTowerAt(placePosition);
            if (placed)
            {
                Debug.Log($"TowerManager: Kule başarıyla yerleştirildi. Pozisyon: {placePosition}");
            }
            else
            {
                Debug.Log($"TowerManager: Kule yerleştirilemedi. Pozisyon: {placePosition}");
            }
        }

        private bool CanPlaceAt(Vector3 worldPosition)
        {
            // Null referansları temizle
            for (int i = _spawnedTowers.Count - 1; i >= 0; i--)
            {
                if (_spawnedTowers[i] == null)
                {
                    _spawnedTowers.RemoveAt(i);
                }
            }

            for (int i = 0; i < _spawnedTowers.Count; i++)
            {
                Transform tower = _spawnedTowers[i];
                if (tower == null)
                {
                    continue;
                }

                float distance = Vector3.Distance(worldPosition, tower.position);
                Debug.Log($"TowerManager: Mesafe kontrolü - Yeni pozisyon: {worldPosition}, Mevcut kule: {tower.position}, Mesafe: {distance:F2}, Min spacing: {_minTowerSpacing}");
                
                if (distance < _minTowerSpacing)
                {
                    Debug.Log($"TowerManager: Kule yerleştirilemez - Mevcut kuleye çok yakın (Mesafe: {distance:F2} < {_minTowerSpacing})");
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}


