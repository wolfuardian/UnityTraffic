using UnityEngine;
using System.Collections.Generic;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdAgentActionLicensePlateGUIEmitter : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private GameObject m_scrollViewContent;
        [SerializeField] private GameObject m_guiPrefab;
        [SerializeField] private int        m_maxItemCount = 25;

        private readonly Dictionary<GameObject, GameObject> agentGuiMap = new Dictionary<GameObject, GameObject>();

        #endregion

        #region Properties

        public GameObject scrollViewContent => m_scrollViewContent;

        public GameObject guiPrefab => m_guiPrefab;

        public int maxItemCount => m_maxItemCount;

        #endregion

        #region Public Methods

        public void AddItem(CrowdAgent agent)
        {
            if (!agent.hasAddonLicensePlate) return;
            var guiItem = Instantiate(guiPrefab, scrollViewContent.transform);

            PlaceNewestItemAtTop();

            var receiver          = guiItem.GetComponent<CrowdAgentActionLicensePlateGUIReceiver>();
            var addonLicensePlate = agent.GetComponent<CrowdAgentAddonLicensePlate>();
            if (addonLicensePlate != null)
            {
                receiver.licensePlateNumberText.text = addonLicensePlate.licensePlateNumber;
                receiver.SetAccess(addonLicensePlate.licensePlateAuthStates == LicensePlateStates.Vip ||
                                   addonLicensePlate.licensePlateAuthStates == LicensePlateStates.Guest);
            }

            var target = agent.gameObject;
            agentGuiMap.Add(target, guiItem);

            if (agentGuiMap.Count > maxItemCount)
            {
                RemoveOldestItem();
            }
        }

        public void RemoveItem(CrowdAgent agent)
        {
        }

        public void SetGUIHighlightOn(CrowdAgent agent)
        {
            if (agentGuiMap.TryGetValue(agent.gameObject, out var guiItem))
            {
                var receiver = guiItem.GetComponent<CrowdAgentActionLicensePlateGUIReceiver>();
                receiver.SetHighlight(true);
            }
        }

        public void SetGUIHighlightOff(CrowdAgent agent)
        {
            if (agentGuiMap.TryGetValue(agent.gameObject, out var guiItem))
            {
                var receiver = guiItem.GetComponent<CrowdAgentActionLicensePlateGUIReceiver>();
                receiver.SetHighlight(false);
            }
        }


        public void PlaceNewestItemAtTop()
        {
            var children = new List<Transform>();
            foreach (Transform child in scrollViewContent.transform)
            {
                children.Add(child);
            }

            if (children.Count > 0)
            {
                var lastChild = children[children.Count - 1];
                children.RemoveAt(children.Count - 1);
                children.Insert(0, lastChild);
            }

            for (int i = 0; i < children.Count; i++)
            {
                children[i].SetSiblingIndex(i);
            }
        }

        public void RemoveOldestItem()
        {
            var oldestItem = scrollViewContent.transform.GetChild(scrollViewContent.transform.childCount - 1);
            agentGuiMap.Remove(oldestItem.gameObject);
            Destroy(oldestItem.gameObject);
        }

        #endregion
    }
}