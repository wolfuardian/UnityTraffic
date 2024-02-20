using UnityEngine;

namespace TimetreeSample.Shaders
{
    public class BuildingTimer : MonoBehaviour
    {
        public Material _material;
        public float    _minY;
        public float    _maxY     = 2;
        public float    _duration = 5;
        public float    _progress0;
        public float    _progress1;




        private void Update()
        {
            var y = Mathf.Lerp(_minY, _maxY, Time.time / _duration);
            _material.SetFloat("_CutoffHeight", y);
        }
    }
}