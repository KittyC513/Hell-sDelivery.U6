using System.Linq;
using UnityEngine;

public class RandomizeMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material[] materials;

    private void Start()
    {
        int rnd = Random.Range(0, materials.Length);
        meshRenderer.material = materials[rnd];
    }
}
