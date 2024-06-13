using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class RenderController : MonoBehaviour
{
    static string SpritePath = "Assets/2DAssets/Sprites/";
    static string ModelPath = "Assets/3DAssets/Models/Machines/";
    static string MaterialPath = "Assets/Materials/Machines/";
    static string[] meshes = new string[] { "ReconstructorSueloOrganico", "Dispersor", "Inyector" };
    static string meshSufix = "_mesh";
    static string materialSufix = "V{0}_material";
    static string spriteSufix = "V{0}_sprite.png";
    [SerializeField] Transform RenderParent;

    public void RenderSprites()
    {
        Camera cam = Camera.main;
        GameObject newGameObject;
        MeshRenderer mr = null;
        MeshFilter mf = null;
        for (int i = 0; i < meshes.Length; i++)
        {
            newGameObject = new GameObject();
            mr = newGameObject.AddComponent<MeshRenderer>();
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newGameObject.transform.parent = RenderParent;
            newGameObject.transform.localPosition = Vector3.zero;
            mf = newGameObject.AddComponent<MeshFilter>();
            mf.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(ModelPath + meshes[i] + meshSufix + ".fbx");
            for (int j = 1; j <= 4; j++)
            {
                mr.material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath + meshes[i] + string.Format(materialSufix, j) + ".mat");
                RenderToSprite(512, 512, cam, SpritePath + meshes[i] + string.Format(spriteSufix, j), 512);
            }
            DestroyImmediate(newGameObject);
        }
    }

    public static Sprite RenderToSprite(int width, int height, Camera cam, string path, int maxTextureSize)
    {
        RenderTexture targetTexture = new RenderTexture(width, height, 24);

        cam.targetTexture = targetTexture;
        cam.Render();
        RenderTexture.active = targetTexture;
        Texture2D image = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGBAHalf, false);
        image.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
        image.Apply();
        byte[] bytes;
        bytes = image.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

        RenderTexture.active = null;
        cam.targetTexture = null;

        AssetDatabase.Refresh();

        RenderTexture.DestroyImmediate(targetTexture);

        var tImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (tImporter != null)
        {
            tImporter.textureType = TextureImporterType.Sprite;
            tImporter.maxTextureSize = maxTextureSize;
            if (!tImporter.isReadable)
            {
                tImporter.isReadable = true;
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
            }
        }

        return AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
    }


}

#if UNITY_EDITOR
    [CustomEditor(typeof(RenderController))]
    class RenderControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Render"))
            {
                ((RenderController)target).RenderSprites();
            }
        }
    }
#endif
