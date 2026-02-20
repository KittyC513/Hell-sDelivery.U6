using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class OutlineShaderBlend : MonoBehaviour
{
    [SerializeField] private MeshFilter[] meshes;

    [SerializeField] private Color colorA;
    [SerializeField] private Color colorB;
    [SerializeField] private float flashSpeed = 1;

    private float percent;
    private float t;
    
    

    public void Update()
    {
        if (t >= 1)
        {
            t = 0;
        }
        else
        {
            t += flashSpeed * Time.deltaTime;
        }
        

        percent = t / 1;

        for (int i = 0; i < meshes.Length; i++)
        {
            Vector3[] vertices = meshes[i].mesh.vertices;
            Color32[] colors32 = new Color32[vertices.Length];

            for (int v = 0; v < vertices.Length; v++)
            {
                colors32[v] = Color.Lerp(colorA, colorB, percent);
            }

            meshes[i].mesh.colors32 = colors32;
        }
    }
}
