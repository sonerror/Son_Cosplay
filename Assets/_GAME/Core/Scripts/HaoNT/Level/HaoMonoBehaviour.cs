using UnityEngine;

namespace HaoNT
{
    public class HaoMonoBehaviour : MonoBehaviour
    {
        private Transform _tf;

        public Transform Tf => _tf ? _tf : _tf = transform;
    }
}