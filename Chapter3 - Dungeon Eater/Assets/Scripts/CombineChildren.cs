using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineChildren : MonoBehaviour
{
    /// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
	public bool generateTriangleStrips = true;

    /// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    public void Combine()
    {
        var meshFilters = GetComponentsInChildren<MeshFilter>();
        
        var materialToMesh = new Dictionary<Material, List<CombineInstance>>();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            var filter = meshFilters[i];
            var curRenderer = filter.GetComponent<Renderer>();
            var instance = new CombineInstance
            {
                mesh = meshFilters[i].sharedMesh,
                transform = meshFilters[i].transform.localToWorldMatrix
            };

            if (curRenderer != null && curRenderer.enabled && instance.mesh != null)
            {
                int m = 0;
                foreach(var material in curRenderer.sharedMaterials)
                {
                    instance.subMeshIndex = Mathf.Min(m, instance.mesh.subMeshCount - 1);
                    List<CombineInstance> instances;
                    if (materialToMesh.TryGetValue(material, out instances))
                        instances.Add(instance);
                    else
                    {
                        instances = new List<CombineInstance>();
                        instances.Add(instance);
                        materialToMesh.Add(material, instances);
                    }
                    m++;
                }
                curRenderer.enabled = false;
            }
        }

        foreach (var de in materialToMesh)
        {
            var instances = de.Value.ToArray();

            // We have a maximum of one material, so just attach the mesh to our own game object
            if (materialToMesh.Count == 1)
            {
                // Make sure we have a mesh filter & renderer
                if (GetComponent<MeshFilter>() == null)
                    gameObject.AddComponent<MeshFilter>();
                if (GetComponent<MeshRenderer>() == null)
                    gameObject.AddComponent<MeshRenderer>();

                var filter = GetComponent<MeshFilter>();
                filter.mesh.CombineMeshes(instances, generateTriangleStrips);
                GetComponent<Renderer>().material = de.Key;
                GetComponent<Renderer>().enabled = true;
            }
            // We have multiple materials to take care of, build one mesh / gameobject for each material
            // and parent it to this object
            else
            {
                GameObject go = new GameObject("Combined mesh");
                go.transform.parent = transform;
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                go.GetComponent<Renderer>().material = (Material)de.Key;
                MeshFilter filter = go.GetComponent<MeshFilter>();
                filter.mesh.CombineMeshes(instances, generateTriangleStrips);
            }
        }
    }
}