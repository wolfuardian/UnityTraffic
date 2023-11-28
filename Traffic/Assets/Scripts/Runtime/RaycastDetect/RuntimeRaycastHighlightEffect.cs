using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeRaycastHighlightEffect : MonoBehaviour, IRaycastChangeReceiver
{
    [SerializeField] private List<GameObject> highlights = new List<GameObject>();

    [Obsolete("Obsolete")]
    private void Start()
    {
        var raycastCheck = FindObjectOfType<RuntimeRaycastDetect>();
        if (raycastCheck == null) return;
        raycastCheck.onRaycastChange.AddListener(OnRaycastChanged);
        raycastCheck.onLeftMouseButtonClick.AddListener(OnLeftMouseButtonClicked);
        raycastCheck.onRightMouseButtonClick.AddListener(OnRightMouseButtonClicked);
        raycastCheck.onMiddleMouseButtonClick.AddListener(OnMiddleMouseButtonClicked);
    }

    public void OnRaycastChanged(GameObject hitObject)
    {
        // Debug.Log("OnRaycastChanged: " + hitObject.name);
    }

    public void OnLeftMouseButtonClicked(List<GameObject> selectObjects)
    {
        foreach (var highlight in highlights)
        {
            highlight.GetComponent<Renderer>().material.color = Color.white;
        }

        highlights.Clear();
        foreach (var selectObject in selectObjects)
        {
            selectObject.GetComponent<Renderer>().material.color = Color.red;
            highlights.Add(selectObject);
        }
    }

    public void OnRightMouseButtonClicked(List<GameObject> selectObjects)
    {
        foreach (var highlight in highlights)
        {
            highlight.GetComponent<Renderer>().material.color = Color.white;
        }

        highlights.Clear();
        foreach (var selectObject in selectObjects)
        {
            selectObject.GetComponent<Renderer>().material.color = Color.red;
            highlights.Add(selectObject);
        }
    }

    public void OnMiddleMouseButtonClicked(GameObject hitObject)
    {
        // Debug.Log("OnMiddleMouseButtonClicked");
    }
}