﻿using System.Linq;
using UnityEngine;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class PathFollowController : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CrowdPathController crowdPathController;

        private CrowdPathFollow crowdPathFollow;

        #endregion

        #region Properties

        //

        #endregion

        #region Unity

        private void Awake()
        {
            crowdPathFollow = GetComponent<CrowdPathFollow>();
            if (crowdPathFollow == null)
            {
                Debug.LogWarning($"物件: {name} 找不到 PathFollow 腳本，請確認是否有設定。", this);
            }

            if (crowdPathController == null)
            {
                Debug.LogError($"[錯誤] 物件 '{name}' 上找不到 Path 組件。請在 PathFollowController 的檢視面板中指定一個 Path 組件。", this);
            }
        }

        private void Start()
        {
            crowdPathFollow.PointsSet = crowdPathController.PointsSet;
            crowdPathFollow.RadiusSet = crowdPathController.RadiusSet;
        }

        #endregion

        #region Public Methods

        // 

        #endregion

        #region Private Methods

        //

        #endregion

        #region Debug and Visualization Methods

        //

        #endregion
    }
}