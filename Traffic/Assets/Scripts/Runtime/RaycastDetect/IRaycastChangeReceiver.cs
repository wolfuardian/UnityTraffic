using System.Collections.Generic;
using UnityEngine;

public interface IRaycastChangeReceiver
{
    void OnRaycastChanged(GameObject hitObject);
    void OnLeftMouseButtonClicked(List<GameObject> selectObjects);
    void OnRightMouseButtonClicked(List<GameObject> selectObjects);
    void OnMiddleMouseButtonClicked(GameObject hitObject);
}