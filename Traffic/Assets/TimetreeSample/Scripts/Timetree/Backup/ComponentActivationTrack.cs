// using UnityEngine;
// using UnityEngine.Playables;
// using UnityEngine.Timeline;
//
// namespace TimetreeSample.Scripts.Timetree
// {
//     [TrackColor(0.855f, 0.903f, 0.87f)]
//     [TrackClipType(typeof(ComponentActivationPlayableAsset))]
//     [TrackBindingType(typeof(Behaviour))]
//     public class ComponentActivationTrack : TrackAsset
//     {
//         public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
//         {
//             return ScriptPlayable<ComponentActivationBehaviour>.Create(graph, inputCount);
//         }
//     }
// }