using System.Collections;
using UnityEngine;

namespace Runtime
{
    public class SimpleJump : MonoBehaviour
    {
        [Range(0.5f, 5f)] public float jumpHeight = 2f;
        [Range(0.5f, 5f)] public float jumpDuration = 1f;
        [Range(1f, 10f)] public float jumpInterval = 3f;
        [Range(1f, 15f)] public float maxRandomInterval = 10f;

        [Header("Debug")] [SerializeField] private float baseHeight;

        private Transform _transform;
        private bool _isJumping;
        private float _elapsedTime;
        private float _currentRandomWaitTime;
        private Coroutine _jumpRoutine;

        public bool isJumpRoutineRunningForce = true;

        private void Awake()
        {
            _transform = transform;
            baseHeight = _transform.position.y;
        }

        private void OnEnable()
        {
            StartJumping();
        }

        private void OnDisable()
        {
            StopJumping();
        }

        private void Update()
        {
            if (!_isJumping) return;

            _elapsedTime += Time.deltaTime;
            var height = CalculateJumpHeight(_elapsedTime);
            _transform.position = new Vector3(transform.position.x, baseHeight + height, _transform.position.z);

            if (_elapsedTime >= jumpDuration)
            {
                _isJumping = false;
            }
        }

        private void StartJumping()
        {
            _jumpRoutine ??= StartCoroutine(JumpRoutine());
        }

        private void StopJumping()
        {
            if (_jumpRoutine == null) return;

            StopCoroutine(_jumpRoutine);
            _jumpRoutine = null;

            _elapsedTime = 0f;
            _transform.position = new Vector3(transform.position.x, baseHeight, _transform.position.z);
        }

        private IEnumerator JumpRoutine()
        {
            while (isJumpRoutineRunningForce)
            {
                yield return new WaitForSeconds(jumpDuration);
                yield return new WaitForSeconds(jumpInterval - jumpDuration);

                _currentRandomWaitTime = Random.Range(jumpInterval, maxRandomInterval);
                yield return new WaitForSeconds(_currentRandomWaitTime - jumpDuration);
                StartJump();
            }
        }

        private void StartJump()
        {
            if (_isJumping) return;

            _isJumping = true;
            _elapsedTime = 0f;
        }

        private float CalculateJumpHeight(float time)
        {
            var progress = time / jumpDuration;
            var height = jumpHeight * 4 * progress * (1 - progress);
            return height;
        }
    }
}