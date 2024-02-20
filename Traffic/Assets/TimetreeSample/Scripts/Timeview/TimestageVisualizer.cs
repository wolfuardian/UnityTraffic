using UnityEngine;
using System.Collections;

namespace TimetreeSample.Scripts.Timestage
{
    public class TimestageVisualizer : MonoBehaviour
    {
        [SerializeField] public StageConfig _config;

        private MeshFilter[] meshFilterInChildren;

        private void Awake()
        {
            meshFilterInChildren = GetComponentsInChildren<MeshFilter>();
        }

        public void OnShow()
        {
            if (_config._animEachChildren)
            {
                ShowEachChildren(_config._useSequential);
            }
            else
            {
                ShowTopLevelChildren(_config._useSequential);
            }
        }

        public void OnHide()
        {
            if (_config._animEachChildren)
            {
                HideEachChildren(_config._useSequential);
            }
            else
            {
                HideTopLevelChildren(_config._useSequential);
            }
        }

        private void ShowEachChildren(bool sequential)
        {
            var delay = 0f;
            foreach (var meshFilter in meshFilterInChildren)
            {
                var target = meshFilter.transform;
                StartCoroutine(ShowWithDelay(target, delay));

                if (sequential)
                    delay += _config._sequentialDelta;
            }
        }

        private void ShowTopLevelChildren(bool sequential)
        {
            var delay = 0f;
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) == transform)
                {
                    continue;
                }

                var target = transform.GetChild(i);
                StartCoroutine(ShowWithDelay(target, delay));

                if (sequential)
                    delay += _config._sequentialDelta;
            }
        }

        private void HideEachChildren(bool sequential)
        {
            var delay = 0f;
            foreach (var meshFilter in meshFilterInChildren)
            {
                var target = meshFilter.transform;
                StartCoroutine(HideWithDelay(target, delay));

                if (sequential)
                    delay += _config._sequentialDelta;
            }
        }

        private void HideTopLevelChildren(bool sequential)
        {
            var delay = 0f;
            for (var i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) == transform)
                {
                    continue;
                }

                var target = transform.GetChild(i);
                StartCoroutine(HideWithDelay(target, delay));

                if (sequential)
                    delay += _config._sequentialDelta;
            }
        }

        private IEnumerator ShowWithDelay(Component target, float delay)
        {
            yield return new WaitForSeconds(delay);
            Show(target);
        }

        private IEnumerator HideWithDelay(Component target, float delay)
        {
            yield return new WaitForSeconds(delay);
            Hide(target);
        }

        private void Show(Component target)
        {
            // 當打開立即顯示的選項，額外特效已經沒有展示上的意義，所以略過其餘的邏輯
            if (_config._useImmediatelyVisible)
            {
                target.gameObject.SetActive(true);
                return;
            }

            if (_config._useDissolve)
            {
                var effect           = target.gameObject.AddComponent<VisualizerEffects.Dissolve>();
                var materialInstance = new Material(target.GetComponent<Renderer>().material);
                var config           = _config._dissolveConfig;
                // effect.Show(target, materialInstance, config);
            }

            if (_config._useMoGraph)
            {
                // MoGraph 相關邏輯
            }
        }

        private void Hide(Component target)
        {
            // 當打開立即顯示的選項，額外特效已經沒有展示上的意義，所以略過其餘的邏輯
            if (_config._useImmediatelyVisible)
            {
                target.gameObject.SetActive(false);
                return;
            }

            if (_config._useDissolve)
            {
                // 溶解效果邏輯
            }

            if (_config._useMoGraph)
            {
                // MoGraph 相關邏輯
            }
        }
    }
}