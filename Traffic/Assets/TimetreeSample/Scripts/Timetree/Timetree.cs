using System;
using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using System.Linq;
using TimetreeSample.Scripts.Timetree.TimelineCore;

namespace TimetreeSample.Scripts.Timetree
{
    public class Timetree : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private PlayableDirector m_playableDirector;
        [SerializeField] private List<GameObject> m_bindings = new List<GameObject>();

        #endregion

        #region Properties

        public PlayableDirector playableDirector => m_playableDirector;
        public List<GameObject> bindings         => m_bindings;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            FetchBindingGameObjects();
            SelectTime(1);
        }

        private void Start()
        {
            // 播放之後馬上停止播放，再跳到第零格
            playableDirector.playOnAwake = false;
            SelectTime(0);
        }

        private void Update()
        {
        }

        private void OnValidate()
        {
            FetchBindingGameObjects();
        }

        #endregion

        #region Public Methods

        public void SelectTime(int time)
        {
            playableDirector.time = time;
            playableDirector.Evaluate();
        }

        #endregion

        #region Internal Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        void FetchBindingGameObjects()
        {
            bindings.Clear();

            if (playableDirector != null && playableDirector.playableAsset != null)
            {
                foreach (var binding in playableDirector.playableAsset.outputs)
                {
                    // 檢查綁定的物體類型
                    if (playableDirector.GetGenericBinding(binding.sourceObject) is GameObject boundGameObject)
                    {
                        bindings.Add(boundGameObject);
                    }
                    else if (playableDirector.GetGenericBinding(binding.sourceObject) is Component boundComponent)
                    {
                        // 如果綁定的是 Component，則添加其 GameObject
                        bindings.Add(boundComponent.gameObject);
                    }
                }
            }
        }

        private void StopPlayableDirector()
        {
            // 停止播放並將時間重置
            if (playableDirector != null)
            {
                playableDirector.Stop();
                playableDirector.time = 0;
            }
        }

        private void InitializePlayableAsset(TimetreeNodeActivationPlayableAsset playableAsset)
        {
            // 解析 nodeToActivate，如果需要的話，這裡可以進行更多的初始化操作
            var node = playableAsset.nodeToActivate.Resolve(playableDirector.playableGraph.GetResolver());

            // 檢查 node 是否成功解析並做進一步的初始化或設置
            if (node != null)
            {
                // 這裡可以添加更多針對 node 的初始化代碼
                Debug.Log("TimetreeNode 解析成功: " + node.name);
            }
        }

        #endregion

        #region Unity Event Methods

        #endregion

        #region Implementation Methods

        #endregion

        #region Debug and Visualization Methods

        #endregion

        #region GUI Methods

        #endregion
    }
}