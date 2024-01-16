using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CrowdSample.Scripts.Runtime.Crowd
{
    public class UIAgentAlarmListHandler : MonoBehaviour
    {
        public           GameObject       alarmListContent;
        public           GameObject       alarmListItemPrefab;
        private readonly List<GameObject> _alarmListItems = new List<GameObject>();

        public void UpdateAlarmListItem(bool onAlarm, AgentEntity entity)
        {
            if (onAlarm)
            {
                AddItem(entity);
            }
            else
            {
                RemoveItem(entity);
            }
        }

        private void AddItem(AgentEntity entity)
        {
            var alarmListItem     = Instantiate(alarmListItemPrefab, alarmListContent.transform);
            var alarmListItemText = alarmListItem.GetComponent<Text>();
            alarmListItemText.text = entity.LicensePlateNumber;
            _alarmListItems.Add(alarmListItem);
        }

        private void RemoveItem(AgentEntity entity)
        {
            var alarmListItem =
                _alarmListItems.Find(item => item.GetComponent<Text>().text == entity.LicensePlateNumber);
            if (alarmListItem == null) return;
            _alarmListItems.Remove(alarmListItem);
            Destroy(alarmListItem);
        }
    }
}