// using UnityEngine;
// using UnityEngine.Playables;
//
// namespace TimetreeSample.Scripts.Timetree
// {
//     // [CreateAssetMenu(fileName = "ComponentActivationPlayable", menuName = "Timeline/ComponentActivationPlayable")]
//     public class ComponentActivationPlayableAsset : PlayableAsset
//     {
//         public ExposedReference<Behaviour> componentToActivate;
//
//         public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
//         {
//             var playable                     = ScriptPlayable<ComponentActivationBehaviour>.Create(graph);
//             var componentActivationBehaviour = playable.GetBehaviour();
//             componentActivationBehaviour.targetComponent = componentToActivate.Resolve(graph.GetResolver());
//             return playable;
//         }
//     }
// }