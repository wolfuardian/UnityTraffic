using UnityEngine.Playables;

namespace TimetreeSample.Scripts.Timetree.TimelineCore
{
    public class TimetreeNodeActivationBehaviour : PlayableBehaviour
    {
        public  TimetreeNode targetNode;
        private bool         previousState;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (targetNode != null)
            {
                // 在 Clip 開始播放時記錄原始狀態並啟用 TimetreeNode
                previousState      = targetNode.enabled;
                targetNode.enabled = true;
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            if (targetNode != null)
            {
                // 在 Clip 停止播放時恢復原始狀態
                targetNode.enabled = previousState;
            }
        }
    }
}