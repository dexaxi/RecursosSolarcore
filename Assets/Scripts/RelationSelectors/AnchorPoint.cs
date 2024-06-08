using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnchorPoint : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private LineSpawner _spawner;

    private bool _performDraw;

    private RectTransform _rect;
    private void Awake()
    {
        _spawner = GetComponentInParent<LineSpawner>();
        _rect = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Mouse Down");
        GetComponentInChildren<Image>().color = Color.green;
        DrawLine();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Mouse up");
        StopDraw();
    }

    private void DrawLine() 
    {
        LineRenderer line = _spawner.SpawnLine(transform.position, this);
        _performDraw = true;
        DrawLoop(line).Forget();
    }

    private void StopDraw() { _performDraw = false; }

    private async UniTask DrawLoop(LineRenderer line) 
    {
        await UniTask.Delay(0);
        
        if (!_performDraw) 
        {
            HandleMouseUp(line);
            return;
        }

        Vector3 screenPos = new(Input.mousePosition.x, Input.mousePosition.y, CameraUtils.MainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 pos = CameraUtils.MainCamera.ScreenToWorldPoint(screenPos);
        Vector3 anchorPos = GetAnchorWorldPos();
        line.positionCount = 2;
        line.SetPosition(0, anchorPos);
        line.SetPosition(1, pos);
        DrawLoop(line).Forget();
    }

    private Vector3 GetAnchorWorldPos() 
    {
       return new Vector3(_rect.position.x, _rect.position.y, _spawner.transform.position.z - 2);
    }

    private void HandleMouseUp(LineRenderer line) 
    {
        GetComponentInChildren<Image>().color = Color.white;
        Destroy(line.gameObject);
    }
}
