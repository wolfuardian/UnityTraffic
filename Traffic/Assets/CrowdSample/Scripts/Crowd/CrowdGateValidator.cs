// using System;
// using System.IO;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace CrowdSample.Scripts.Crowd
// {
//     public class CrowdGateValidator : MonoBehaviour
//     {
//         #region Field Declarations
//
//         public enum PermissionID
//         {
//             Authorized,
//             Denied
//         }
//
//         private LicensePlateRegistry   licensePlateRegistry;
//         public  UnityEvent<CrowdAgent> onAgentEnterAccess;
//         public  UnityEvent<CrowdAgent> onAgentEnterDenied;
//         public  UnityEvent<CrowdAgent> onAgentExitDetect;
//
//         #endregion
//
//         #region Properties
//
//         #endregion
//
//         #region Unity Methods
//
//         private void Start()
//         {
//             licensePlateRegistry = new LicensePlateRegistry("Assets/AGVSample/Data/LicensePlate.csv");
//
//             // plateRegistry = new LicensePlateRegistry("Assets/AGVSample/Data/PlateNumber.csv");
//             // Debug.Log(plateRegistry.GetRandomPlateData().plateNumber);
//         }
//
//         private void OnTriggerEnter(Collider other)
//         {
//             var agentEntity = other.GetComponent<CrowdAgent>();
//
//             if (agentEntity == null) return;
//
//             var uid = agentEntity.userIdentity;
//
//             var plateData            = licensePlateRegistry.CheckPlateAuthorization(uid);
//
//             if (!Enum.TryParse(agentEntity.userIdentity, out PermissionID permissionId)) return;
//
//             switch (permissionId)
//             {
//                 case PermissionID.Authorized:
//                     onAgentEnterAccess?.Invoke(agentEntity);
//                     break;
//                 case PermissionID.Denied:
//                     onAgentEnterDenied?.Invoke(agentEntity);
//                     break;
//             }
//         }
//
//         private void OnTriggerExit(Collider other)
//         {
//             // var agentEntity = other.GetComponent<AgentEntity>();
//             // onAgentExitDetect?.Invoke(agentEntity);
//         }
//
//         #endregion
//
//         #region Public Methods
//
//         #endregion
//
//         #region Internal Methods
//
//         #endregion
//
//         #region Protected Methods
//
//         #endregion
//
//         #region Private Methods
//
//         #endregion
//
//         #region Unity Event Methods
//
//         #endregion
//
//         #region Implementation Methods
//
//         #endregion
//
//         #region Debug and Visualization Methods
//
//         #endregion
//
//         #region GUI Methods
//
//         #endregion
//     }
// }