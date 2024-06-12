using System;
using UnityEngine;

public enum DecorationType
{
    Grass
}


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GroundDecorationsCreator : MonoBehaviour
{
    [SerializeField] DecorationType decorationType;
    [SerializeField] int decorationsCount = 5;
    [SerializeField, Range(0f, 1f)] float decorationsProbability = 0.1f;
    [SerializeField, Range(0f, 50f)] float maxRotation = 50f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 50f)] float minRotation = 30f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 1f)] float minScale = 0.5f; // Escala del ruido de Perlin
    [SerializeField, Range(1f, 2f)] float maxScale = 1.5f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 0.3f)] float xRandomPosition = 0.25f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 0.3f)] float zRandomPosition = 0.25f; // Escala del ruido de Perlin
    [SerializeField] MeshFilter filter;
    [SerializeField] MeshRenderer decorationRenderer;
    [SerializeField] MeshFilter decorationsFilterInstance;

    private Mesh grassTerrainMesh;
    MaterialPropertyBlock block;

    public DecorationType Type => decorationType;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrassDistribution();
        if(block == null) 
        { 
            block = new MaterialPropertyBlock();
        }
    }

    void CreateGrassDistribution()
    {
        if (UnityEngine.Random.Range(0, 1f) < decorationsProbability) return;

        CombineInstance[] combine = new CombineInstance[decorationsCount];
        Vector2[] combinedUVs = new Vector2[decorationsFilterInstance.mesh.uv.Length * decorationsCount];

        for (int i = 0; i < decorationsCount; i++)
        {
            MeshFilter newDecoration = Instantiate(decorationsFilterInstance, decorationRenderer.transform);
            newDecoration.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(-minRotation, -maxRotation));
            newDecoration.transform.Scale(UnityEngine.Random.Range(minScale, maxScale));
            newDecoration.transform.localPosition = new Vector3(UnityEngine.Random.Range(-xRandomPosition, xRandomPosition), newDecoration.transform.localPosition.y, UnityEngine.Random.Range(-zRandomPosition, zRandomPosition));
            combine[i].mesh = newDecoration.mesh;
            Matrix4x4 adjustedMatrix = newDecoration.transform.localToWorldMatrix;
            adjustedMatrix.m00 /= transform.localScale.x;
            adjustedMatrix.m01 /= transform.localScale.x;
            adjustedMatrix.m02 /= transform.localScale.x;

            adjustedMatrix.m10 /= transform.localScale.y;
            adjustedMatrix.m11 /= transform.localScale.y;
            adjustedMatrix.m12 /= transform.localScale.y;

            adjustedMatrix.m20 /= transform.localScale.z;
            adjustedMatrix.m21 /= transform.localScale.z;
            adjustedMatrix.m22 /= transform.localScale.z;

            // Restar la posición
            Vector3 position = decorationRenderer.transform.parent.position;
            adjustedMatrix.m03 -= position.x;
            adjustedMatrix.m13 -= position.y;
            adjustedMatrix.m23 -= position.z;

            combine[i].transform = adjustedMatrix;
            Array.Copy(decorationsFilterInstance.mesh.uv, 0, combinedUVs, i * decorationsFilterInstance.mesh.uv.Length, decorationsFilterInstance.mesh.uv.Length);
            newDecoration.gameObject.SetActive(false);
        }

        grassTerrainMesh = new Mesh();
        grassTerrainMesh.CombineMeshes(combine);
        grassTerrainMesh.uv = combinedUVs;
        filter.mesh = grassTerrainMesh;
        decorationRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    public void SetGrassColor(Color bottomGrassColor, Color topGrassColor)
    {
        if (block == null)
        {
            block = new MaterialPropertyBlock();
        }
        if (decorationRenderer != null)
        {
            decorationRenderer.GetPropertyBlock(block);
            block.SetColor("_BottomColor", bottomGrassColor);
            block.SetColor("_TopColor", topGrassColor);
            decorationRenderer.SetPropertyBlock(block);
        }
    }

}
