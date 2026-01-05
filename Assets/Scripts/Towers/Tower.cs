using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Game.Enemies;

namespace Game.Towers
{
    /// <summary>
    /// Basit kule bileşeni.
    /// - Menzilini sahnede gösterir.
    /// - En yakın düşmanı takip eder ve belirli aralıklarla mermi fırlatır.
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    public class Tower : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private float _range = 3f;

        [SerializeField] private float _fireRate = 1f;

        [SerializeField] private Transform _firePoint;

        [SerializeField] private Projectile _projectilePrefab;

        #endregion

        #region Private Fields

        private float _fireCooldown;
        private Enemy _currentTarget;

        #endregion

        #region Properties

        /// <summary>
        /// Kule menzili.
        /// </summary>
        public float Range
        {
            get { return _range; }
        }

        /// <summary>
        /// Kule ateş hızı (saniyedeki atış sayısı).
        /// </summary>
        public float FireRate
        {
            get { return _fireRate; }
        }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            if (_firePoint == null)
            {
                _firePoint = transform;
                Debug.LogWarning($"Tower '{name}': FirePoint atanmamış, kule merkezinden ateş ediyor.");
            }

            if (_projectilePrefab == null)
            {
                Debug.LogWarning($"Tower '{name}': Projectile prefab atanmamış. Lütfen Inspector'dan atayın.");
            }
        }

        private void Update()
        {
            // Master Tower (Tower_Prefab_Source) ateş etmesin
            if (name.Contains("Tower_Prefab_Source"))
            {
                return;
            }

            // Hedefi bul
            UpdateTarget();

            // Ateş etme zamanını kontrol et
            if (_currentTarget != null && !_currentTarget.Equals(null))
            {
                if (_fireCooldown <= 0f)
                {
                    Shoot();
                    _fireCooldown = 1f / Mathf.Max(_fireRate, 0.01f);
                }
            }

            if (_fireCooldown > 0f)
            {
                _fireCooldown -= Time.deltaTime;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Menzil içindeki en yakın düşmanı bulur.
        /// </summary>
        private void UpdateTarget()
        {
            float shortestDistance = Mathf.Infinity;
            Enemy nearestEnemy = null;

            // Tüm düşmanları bul (OverlapSphere yerine FindObjectsOfType kullan)
            Enemy[] allEnemies = FindObjectsOfType<Enemy>();
            
            foreach (Enemy enemy in allEnemies)
            {
                if (enemy == null || enemy.Equals(null))
                {
                    continue;
                }

                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                
                // Menzil kontrolü
                if (distanceToEnemy <= _range)
                {
                    if (distanceToEnemy < shortestDistance)
                    {
                        shortestDistance = distanceToEnemy;
                        nearestEnemy = enemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                if (_currentTarget != nearestEnemy)
                {
                    _currentTarget = nearestEnemy;
                    Debug.Log($"Tower '{name}': Yeni hedef bulundu - {nearestEnemy.name} (Mesafe: {shortestDistance:F2})");
                }
            }
            else
            {
                if (_currentTarget != null)
                {
                    _currentTarget = null;
                }
            }

            // Hedefe doğru dön
            if (_currentTarget != null && !_currentTarget.Equals(null))
            {
                Vector3 dir = _currentTarget.transform.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion lookRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
                }
            }
        }

        /// <summary>
        /// Mermi fırlatır.
        /// </summary>
        private void Shoot()
        {
            if (_projectilePrefab == null)
            {
                Debug.LogWarning($"Tower '{name}': Projectile prefab null! Mermi atılamıyor.");
                return;
            }

            if (_firePoint == null)
            {
                Debug.LogWarning($"Tower '{name}': FirePoint null! Mermi atılamıyor.");
                return;
            }

            if (_currentTarget == null || _currentTarget.Equals(null))
            {
                Debug.LogWarning($"Tower '{name}': Hedef null! Mermi atılamıyor.");
                return;
            }

            Debug.Log($"Tower '{name}': Mermi fırlatılıyor! Hedef: {_currentTarget.name}");
            Projectile projectileInstance = Instantiate(_projectilePrefab, _firePoint.position, _firePoint.rotation);
            if (projectileInstance != null)
            {
                projectileInstance.Initialize(_currentTarget);
                Debug.Log($"Tower '{name}': Mermi oluşturuldu ve hedefe atandı.");
            }
            else
            {
                Debug.LogError($"Tower '{name}': Mermi instantiate edilemedi!");
            }
        }

        #endregion

        #region Gizmos

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _range);
        }

        #endregion
    }
}
