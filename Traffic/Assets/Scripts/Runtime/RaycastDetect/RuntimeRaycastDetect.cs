using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RuntimeRaycastDetect : MonoBehaviour
{
    private Camera _camera;
    private GameObject _hitObject;
    [SerializeField] private List<GameObject> raycastHits = new List<GameObject>();
    [SerializeField] private List<GameObject> selectObjects = new List<GameObject>();
    private bool _isCameraNull;
    private bool _isRaycastHitNull;
    private bool _isLeftMouseButtonClick;
    private bool _isRightMouseButtonClick;
    private bool _isMiddleMouseButtonClick;
    private bool _isShiftKey;
    public UnityEvent<GameObject> onRaycastChange;
    public UnityEvent<List<GameObject>> onLeftMouseButtonClick;
    public UnityEvent<List<GameObject>> onRightMouseButtonClick;
    public UnityEvent<GameObject> onMiddleMouseButtonClick;

    private void Start()
    {
        _camera = Camera.main;
        _isCameraNull = _camera == null;
    }


    private void Update()
    {
        if (_isCameraNull) return;

        var newHitObject = HandleRaycast(_camera);

        if (_hitObject != newHitObject)
        {
            _hitObject = newHitObject;
            onRaycastChange.Invoke(_hitObject);
        }

        CheckRaycastHit();
        CheckMouseInput();
        CheckKeyboardInput();
    }

    private void LateUpdate()
    {
        HandleRaycastSelection(_hitObject);

        // if (_isRaycastHitNull) return;

        HandleLeftMouseButtonClickSelection(_hitObject);
        HandleRightMouseButtonClickSelection(_hitObject);
        HandleMiddleMouseButtonClickSelection();
    }

    private static GameObject HandleRaycast(Camera cam)
    {
        var ray = cam.ScreenPointToRay(Input.mousePosition);

        return Physics.Raycast(ray, out var hit) ? HitObject(hit) : null;
    }

    private static GameObject HitObject(RaycastHit hit)
    {
        return hit.collider.gameObject;
    }

    private void CheckRaycastHit()
    {
        _isRaycastHitNull = raycastHits.Count == 1 && raycastHits[0] == null;
    }

    private void CheckMouseInput()
    {
        _isLeftMouseButtonClick = Input.GetMouseButtonDown(0);
        _isRightMouseButtonClick = Input.GetMouseButtonDown(1);
        _isMiddleMouseButtonClick = Input.GetMouseButtonDown(2);
    }

    private void CheckKeyboardInput()
    {
        _isShiftKey = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }


    private void HandleRaycastSelection(GameObject go)
    {
        AffectRaycastHit(go);
    }

    private void HandleLeftMouseButtonClickSelection(GameObject go)
    {
        if (!_isLeftMouseButtonClick) return;
        // AffectSelectAndFocus(go);
        onLeftMouseButtonClick.Invoke(selectObjects);
    }

    private void HandleRightMouseButtonClickSelection(GameObject go)
    {
        if (!_isRightMouseButtonClick) return;
        if (_isShiftKey)
        {
            AffectSelectAdditional(go);
        }
        else
        {
            AffectSelect(go);
        }

        onRightMouseButtonClick.Invoke(selectObjects);
    }

    private void HandleMiddleMouseButtonClickSelection()
    {
        if (!_isMiddleMouseButtonClick) return;
        // AffectCancelFocus();
        onMiddleMouseButtonClick.Invoke(_hitObject);
    }

    private void AffectRaycastHit(GameObject go)
    {
        raycastHits.Clear();
        if (go == null) return;
        raycastHits.Add(go);
    }

    private void AffectSelectAndFocus(GameObject go)
    {
        selectObjects.Clear();
        if (go == null) return;
        selectObjects.Add(go);
    }

    private void AffectSelect(GameObject go)
    {
        selectObjects.Clear();
        if (go == null) return;
        selectObjects.Add(go);
    }

    private void AffectSelectAdditional(GameObject go)
    {
        if (selectObjects.Contains(go))
        {
            selectObjects.Remove(go);
            return;
        }

        selectObjects.Add(go);
    }

    private void AffectCancelFocus()
    {
        selectObjects.Clear();
    }
}