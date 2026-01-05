using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Game.Core
{
    /// <summary>
    /// Sahnedeki waypoint noktalarını tutan ve yolu görselleştiren sınıf.
    /// Düşmanların takip edeceği yolu belirler.
    /// </summary>
    public class WaypointPath : MonoBehaviour
    {
        #region Serialized Fields
        
        [SerializeField] private List<Transform> _waypoints = new List<Transform>();
        
        [SerializeField] private Color _pathColor = Color.yellow;
        
        [SerializeField] private float _waypointSize = 0.5f;
        
        #endregion
        
        #region Properties
        
        /// <summary>
        /// Waypoint listesi (sadece okunabilir)
        /// </summary>
        public List<Transform> Waypoints
        {
            get { return _waypoints; }
        }
        
        /// <summary>
        /// Waypoint sayısı
        /// </summary>
        public int WaypointCount
        {
            get { return _waypoints != null ? _waypoints.Count : 0; }
        }
        
        /// <summary>
        /// İlk waypoint pozisyonu (spawn pozisyonu)
        /// </summary>
        public Vector3 StartPosition
        {
            get
            {
                if (_waypoints == null || _waypoints.Count == 0 || _waypoints[0] == null)
                {
                    return transform.position;
                }
                return _waypoints[0].position;
            }
        }
        
        /// <summary>
        /// Son waypoint pozisyonu (hedef pozisyonu)
        /// </summary>
        public Vector3 EndPosition
        {
            get
            {
                if (_waypoints == null || _waypoints.Count == 0)
                {
                    return transform.position;
                }
                
                Transform lastWaypoint = _waypoints[_waypoints.Count - 1];
                if (lastWaypoint == null)
                {
                    return transform.position;
                }
                
                return lastWaypoint.position;
            }
        }
        
        #endregion
        
        #region Unity Lifecycle
        
        private void OnDrawGizmos()
        {
            if (_waypoints == null || _waypoints.Count < 2)
            {
                return;
            }
            
            // Yolu çiz
            Gizmos.color = _pathColor;
            
            for (int i = 0; i < _waypoints.Count - 1; i++)
            {
                if (_waypoints[i] != null && _waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
                }
            }
            
            // Waypoint'leri göster
            Gizmos.color = Color.red;
            foreach (Transform waypoint in _waypoints)
            {
                if (waypoint != null)
                {
                    Gizmos.DrawSphere(waypoint.position, _waypointSize);
                }
            }
            
            // Başlangıç noktasını yeşil göster
            if (_waypoints.Count > 0 && _waypoints[0] != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_waypoints[0].position, _waypointSize * 1.2f);
            }
            
            // Bitiş noktasını kırmızı göster
            if (_waypoints.Count > 1)
            {
                Transform lastWaypoint = _waypoints[_waypoints.Count - 1];
                if (lastWaypoint != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(lastWaypoint.position, _waypointSize * 1.2f);
                }
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Belirtilen index'teki waypoint'i döndürür
        /// </summary>
        /// <param name="index">Waypoint index'i</param>
        /// <returns>Transform veya null</returns>
        public Transform GetWaypoint(int index)
        {
            if (_waypoints == null || index < 0 || index >= _waypoints.Count)
            {
                Debug.LogWarning($"WaypointPath '{name}': Geçersiz waypoint index! Index: {index}, Toplam waypoint: {WaypointCount}");
                return null;
            }
            
            return _waypoints[index];
        }
        
        /// <summary>
        /// Waypoint path'inin geçerli olup olmadığını kontrol eder
        /// </summary>
        /// <returns>Path geçerli mi?</returns>
        public bool IsValid()
        {
            if (_waypoints == null || _waypoints.Count < 2)
            {
                Debug.LogWarning($"WaypointPath '{name}': En az 2 waypoint olmalı!");
                return false;
            }
            
            foreach (Transform waypoint in _waypoints)
            {
                if (waypoint == null)
                {
                    Debug.LogError($"WaypointPath '{name}': Null waypoint bulundu!");
                    return false;
                }
            }
            
            return true;
        }
        
        #endregion
    }
}

