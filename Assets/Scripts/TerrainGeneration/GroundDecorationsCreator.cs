using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GroundDecorationsCreator : MonoBehaviour
{
    [SerializeField] int decorationsCount = 5;
    [SerializeField, Range(0f, 1f)] float decorationsProbability = 0.1f;
    [SerializeField, Range(0f, 50f)] float maxRotation = 50f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 50f)] float minRotation = 30f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 1f)] float minScale = 0.5f; // Escala del ruido de Perlin
    [SerializeField, Range(1f, 2f)] float maxScale = 1.5f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 0.3f)] float xRandomPosition = 0.25f; // Escala del ruido de Perlin
    [SerializeField, Range(0f, 0.3f)] float zRandomPosition = 0.25f; // Escala del ruido de Perlin
    [SerializeField] MeshFilter filter;
    [SerializeField] MeshRenderer renderer;
    [SerializeField] MeshFilter decorationsFilterInstance;

    private Mesh grassTerrainMesh;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrassDistribution();
    }

    void CreateGrassDistribution()
    {
        if (Random.Range(0, 1f) < decorationsProbability) return;

        CombineInstance[] combine = new CombineInstance[decorationsCount];

        for(int i = 0; i < decorationsCount; i++)
        {
            MeshFilter newDecoration = Instantiate(decorationsFilterInstance, renderer.transform);
            newDecoration.transform.Rotate(Vector3.forward * Random.Range(-minRotation, -maxRotation));
            newDecoration.transform.Scale(Random.Range(minScale, maxScale));
            newDecoration.transform.localPosition = new Vector3(Random.Range(-xRandomPosition, xRandomPosition), newDecoration.transform.localPosition.y, Random.Range(-zRandomPosition, zRandomPosition));
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
            Vector3 position = renderer.transform.parent.position;
            adjustedMatrix.m03 -= position.x;
            adjustedMatrix.m13 -= position.y;
            adjustedMatrix.m23 -= position.z;

            combine[i].transform = adjustedMatrix;
            newDecoration.gameObject.SetActive(false);
        }

        grassTerrainMesh = new Mesh();
        grassTerrainMesh.CombineMeshes(combine);
        filter.mesh = grassTerrainMesh;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

}
