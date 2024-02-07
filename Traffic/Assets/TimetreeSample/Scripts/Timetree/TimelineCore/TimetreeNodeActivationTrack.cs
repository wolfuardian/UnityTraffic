using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TimetreeSample.Scripts.Timetree.TimelineCore
{
    [TrackColor(0.855f, 0.903f, 0.87f)]
    [TrackClipType(typeof(TimetreeNodeActivationPlayableAsset))]
    [TrackBindingType(typeof(TimetreeNode))]
    public class TimetreeNodeActivationTrack : TrackAsset
    {
        
        
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TimetreeNodeActivationBehaviour>.Create(graph, inputCount);
        }
    }
}