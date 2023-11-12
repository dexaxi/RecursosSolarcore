using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct HighlightableMaterial
{
    [SerializeField] public string name;
    [SerializeField] public Material material;
}

public class Highlightable : MonoBehaviour
{
    [Header("References")]
    //TODO: Idealmente esto sería un serializable dictionary y santas pascuas
    [SerializeField] List<HighlightableMaterial> HighlightableMaterials;

    private Renderer[] _renderers;
    private Material[] _originalMaterials;

    private void Awake()
    {
        UpdateOriginalMaterials();
    }

    public void UpdateOriginalMaterials() 
    {
        List<Renderer> rendererList = new List<Renderer>();

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            //Filter renderers so no text mesh is included. Cant compare with GetType because TextMeshPro is no renderer
            if (renderer.GetComponent<TextMeshPro>() == false)
            {
                rendererList.Add(renderer);
            }
        }

        _renderers = rendererList.ToArray();

        if (_renderers != null && _renderers.Length != 0)
        {
            _originalMaterials = new Material[_renderers.Length];
            for (int i = 0; i < _renderers.Length; i++)
            {
                _originalMaterials[i] = _renderers[i].material;
            }
        }
    }

    public Material GetMaterial(string matName)
    {
        foreach (var mat in HighlightableMaterials) { if (mat.name.ToLower().Equals(matName.ToLower())) return mat.material; }
        Debug.LogWarning("WARNING: NO MATERIAL FOUND WITH PROVIDED NAME: " + matName);
        return null;
    }

    public void Highlight(string matName) 
    {
        SetMaterial(GetMaterial(matName));
    }

    [ContextMenu("Hightlight")]
    public void HighlightMouseEnter()
    {
        Highlight("MouseEnter");
    }

    [ContextMenu("Unhightlight")]
    public void Unhighlight()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i].material != null) _renderers[i].material = _originalMaterials[i];
        }
    }

    private void SetMaterial(Material material)
    {
        foreach (var render in _renderers)
        {
            render.material = material;
        }
    }


}
