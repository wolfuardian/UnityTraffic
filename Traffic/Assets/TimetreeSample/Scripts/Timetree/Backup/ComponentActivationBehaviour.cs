// using UnityEngine;
// using UnityEngine.Playables;
//
// namespace TimetreeSample.Scripts.Timetree
// {
//     public class ComponentActivationBehaviour : PlayableBehaviour
//     {
//         public  Behaviour targetComponent;
//         private bool      previousState;
//
//         public override void OnBehaviourPlay(Playable playable, FrameData info)
//         {
//             if (targetComponent != null)
//             {
//                 // 在 Clip 開始播放時記錄原始狀態並啟用 Component
//                 previousState           = targetComponent.enabled;
//                 targetComponent.enabled = true;
//             }
//         }
//
//         public override void OnBehaviourPause(Playable playable, FrameData info)
//         {
//             if (targetComponent != null)
//             {
//                 // 在 Clip 停止播放時恢復原始狀態
//                 targetComponent.enabled = previousState;
//             }
//         }
//     }
// }