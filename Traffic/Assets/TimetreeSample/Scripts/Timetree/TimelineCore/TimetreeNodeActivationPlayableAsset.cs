using UnityEngine;
using UnityEngine.Playables;

// [CreateAssetMenu(fileName = "TimetreeNodeActivationPlayable", menuName = "Timeline/TimetreeNodeActivationPlayable")]
namespace TimetreeSample.Scripts.Timetree.TimelineCore
{
    public class TimetreeNodeActivationPlayableAsset : PlayableAsset
    {
        public ExposedReference<TimetreeNode> nodeToActivate;
        
        // 遊戲開始時初始化
        

        
        
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable                        = ScriptPlayable<TimetreeNodeActivationBehaviour>.Create(graph);
            var timetreeNodeActivationBehaviour = playable.GetBehaviour();
            timetreeNodeActivationBehaviour.targetNode = nodeToActivate.Resolve(graph.GetResolver());
            return playable;
        }
    }
}