using UnityEngine;
using UnityEngine.Playables;
using TimetreeSample.Scripts.Timetree.TimelineCore;

namespace TimetreeSample.Scripts.Timetree
{
    public class Timetree : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private PlayableDirector m_playableDirector;
        public                   double           playableLength = 6; // 定義播放的時間長度，比如 6 帧

        #endregion

        #region Properties

        public PlayableDirector playableDirector => m_playableDirector;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            SelectTime(1);
        }

        private void Start()
        {
            // 播放之後馬上停止播放，再跳到第零格
            m_playableDirector.playOnAwake = false;
            SelectTime(0);
        }

        private void Update()
        {
        }

        #endregion

        #region Public Methods

        public void SelectTime(int time)
        {
            m_playableDirector.time = time;
            playableDirector.Evaluate();
        }

        #endregion

        #region Internal Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

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