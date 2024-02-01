using UnityEngine;
using UnityEngine.UI;

namespace CrowdSample.Scripts.Crowd
{
    public class CrowdAgentActionLicensePlateGUIReceiver : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private Text   m_licensePlateNumberText;
        [SerializeField] private Toggle m_allowToggleGroup;
        [SerializeField] private Toggle m_denyToggleGroup;
        [SerializeField] private Image  m_highlightImage;

        #endregion

        #region Properties

        public Text   licensePlateNumberText => m_licensePlateNumberText;
        public Toggle allowToggleGroup       => m_allowToggleGroup;
        public Toggle denyToggleGroup        => m_denyToggleGroup;
        public Image  highlightImage         => m_highlightImage;

        #endregion

        #region Public Methods

        public void SetAccess(bool isAllow)
        {
            if (isAllow)
            {
                allowToggleGroup.isOn = true;
                denyToggleGroup.isOn  = false;
            }
            else
            {
                allowToggleGroup.isOn = false;
                denyToggleGroup.isOn  = true;
            }
        }

        public void SetHighlight(bool isHighlight)
        {
            var color = highlightImage.color;
            if (isHighlight)
            {
                color.a              = 1f;
                highlightImage.color = color;
            }
            else
            {
                color.a              = 0f;
                highlightImage.color = color;
            }
        }

        #endregion
    }
}