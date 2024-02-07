using UnityEngine;

namespace TimetreeSample.Scripts.Timetree
{
    public class TimetreeNode : MonoBehaviour
    {
        private const string FADE_IN     = "Fade_in";
        private const string FADE_OUT    = "Fade_out";
        private const string MOVE_IN     = "Move_in";
        private const string MOVE_OUT    = "Move_out";
        private const string VISIBLE_ON  = "Visible_on";
        private const string VISIBLE_OFF = "Visible_off";

        #region Field Declarations

        #endregion

        #region Properties

        #endregion

        #region Unity Methods

        private void Awake()
        {
            VisibleOff();
        }

        private void OnEnable()
        {
            VisibleOn();
        }

        private void OnDisable()
        {
            VisibleOff();
        }

        private void FadeIn()
        {
            Fade(FADE_IN, 1f);
        }

        private void FadeOut()
        {
            Fade(FADE_OUT, 1f);
        }

        private void MoveIn()
        {
            Move(MOVE_IN, 1f);
        }

        private void MoveOut()
        {
            Move(MOVE_OUT, 1f);
        }

        private void VisibleOn()
        {
            Visible(VISIBLE_ON);
        }

        private void VisibleOff()
        {
            Visible(VISIBLE_OFF);
        }


        private void Move(string moveIn, float duration)
        {
        }

        private void Fade(string state, float duration)
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                foreach (var material in meshRenderer.materials)
                {
                    if (state == FADE_IN)
                    {
                        // material.DOFade(1f, duration);
                    }

                    if (state == FADE_OUT)
                    {
                        // material.DOFade(0f, duration);
                    }
                }
            }
        }

        private void Visible(string state)
        {
            Debug.Log("Visible");
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                meshRenderer.enabled = state == VISIBLE_ON;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Internal Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #endregion

        #region Unity Event Methods

        #endregion

        #region Implementation Methods

        #endregion

        #region Debug and Visualization Methods

        #endregion

        #region GUI Methods

        #endregion
    }
}