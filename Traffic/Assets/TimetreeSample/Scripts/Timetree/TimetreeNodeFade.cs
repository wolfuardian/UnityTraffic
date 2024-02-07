using UnityEngine;

namespace TimetreeSample.Scripts.Timetree
{
    public class TimetreeNodeFade : MonoBehaviour
    {
        private const string FADE_IN  = "Fade_in";
        private const string FADE_OUT = "Fade_out";

        #region Field Declarations

        #endregion

        #region Properties

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            FadeIn();
            Debug.Log("FadeIn");
        }

        private void OnDisable()
        {
            FadeOut();
            Debug.Log("FadeOut");
        }

        private void FadeIn()
        {
            Fade(FADE_IN, 1f);
        }

        private void FadeOut()
        {
            Fade(FADE_OUT, 1f);
        }

        private void Fade(string state, float f)
        {
            var meshFilters = GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                var meshRenderer = meshFilter.GetComponent<MeshRenderer>();
                foreach (var material in meshRenderer.materials)
                {
                    if (state == FADE_IN)
                    {
                        // material.DOFade(1f, f);
                    }

                    if (state == FADE_OUT)
                    {
                        // material.DOFade(0f, f);
                    }
                }
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