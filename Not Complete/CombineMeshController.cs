using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace TopeBox.Util
{
    public class CombineMeshController : MonoBehaviour
    {
        public Transform CloneFromObject;
        public GameObject[] CombineTargets; 
        
        [CanBeNull]
        private MeshFilter[] _TargetMeshFilters;
        
        private Transform _CombinedObject;
        private CombineInstance[] _CombineInstances;

        private Dictionary<string, Material> _TargetMaterials;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Combine();
            }
        }

        public void Combine()
        {
            GenerateCombinedObjectTransform();
            GetAllMeshRenderer();
            GetMaterials();
            CombineMeshes();
            DisableTargetFilters();
        }

        private void GenerateCombinedObjectTransform()
        {
            GameObject go = new GameObject(Utils.CreateStrings(CloneFromObject.name, "_CombineMesh"));
            _CombinedObject = go.transform;
            _CombinedObject.SetParent(CloneFromObject.parent);
            _CombinedObject.localPosition = CloneFromObject.localPosition;
            
            _CombinedObject.gameObject.AddComponent<MeshFilter>();
            _CombinedObject.gameObject.AddComponent<MeshRenderer>();
        }
        
        private void GetAllMeshRenderer()
        {
            List<MeshFilter> meshFilters = new List<MeshFilter>();
            
            int length = CombineTargets.Length;
            for (int i = 0; i < length; i++)
            {
                GameObject target = CombineTargets[i];
                meshFilters.AddRange(target.GetComponentsInChildren<MeshFilter>());
            }

            _TargetMeshFilters = meshFilters.ToArray();
            meshFilters = null;
        }
        
        private void GetMaterials()
        {
            _TargetMaterials = new Dictionary<string, Material>();
            
            int length = _TargetMeshFilters.Length;
            for (int i = 0; i < length; i++)
            {
                Material material = _TargetMeshFilters[i].GetComponent<Renderer>().material;
                string key = material.name;
                if (!_TargetMaterials.ContainsKey(key))
                {
                    _TargetMaterials.Add(key, material);    
                }
            }
        }
        
        private void CombineMeshes()
        {
            // Setup collection for each material combine instance 
            int collectionLength = _TargetMaterials.Count;
            (string, List<CombineInstance>)[] combineInstanceCollections = new (string, List<CombineInstance>)[collectionLength];
            int setupIndex = 0;
            foreach (KeyValuePair<string,Material> pair in _TargetMaterials)
            {
                string keyMaterialName = pair.Value.name;
                combineInstanceCollections[setupIndex].Item1 = keyMaterialName;
                combineInstanceCollections[setupIndex].Item2 = new List<CombineInstance>();

                setupIndex++;
            }
            
            // Fill combine instance data into correct collections
            int length = _TargetMeshFilters.Length;
            for (int meshFilterIndex = 0; meshFilterIndex < length; meshFilterIndex++)
            {
                // cache 
                MeshFilter meshFilter = _TargetMeshFilters[meshFilterIndex]; 
                
                // Create new combine instance
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = meshFilter.sharedMesh;
                combineInstance.transform = meshFilter.transform.localToWorldMatrix;
                
                // add combine instance into collection base on Material name
                string materialName = meshFilter.gameObject.GetComponent<Renderer>().material.name;
                for (int collectionIndex = 0; collectionIndex < collectionLength; collectionIndex++)
                {
                    string keyMaterialName = combineInstanceCollections[collectionIndex].Item1;
                    List<CombineInstance> combineInstances = combineInstanceCollections[collectionIndex].Item2;
                    if (materialName.Equals(keyMaterialName))
                    {
                        combineInstances.Add(combineInstance);
                        break;
                    }
                }   
            }
            
            // Combine instances to mesh. add mesh into total mesh with order by cached material
            CombineInstance[] totalCombineInstances = new CombineInstance[collectionLength];
            for (int i = 0; i < collectionLength; i++)
            {
                // Combine instance to mesh
                List<CombineInstance> combineInstances = combineInstanceCollections[i].Item2;
                Mesh combineMesh = new Mesh();
                combineMesh.CombineMeshes(combineInstances.ToArray());
                
                // Add the submeshes in the same order as the material is set in the combined mesh
                CombineInstance combineInstance = new CombineInstance();
                combineInstance.mesh = combineMesh;
                combineInstance.transform = _CombinedObject.localToWorldMatrix;
                combineInstance.subMeshIndex = i;

                totalCombineInstances[i] = combineInstance;
            }
            
            // Create the final combined mesh
            Mesh combinedAllMesh = new Mesh();
            //Make sure it's set to false to get 2 separate meshes
            combinedAllMesh.CombineMeshes(totalCombineInstances, false);
            
            // Assgin data to Target combined object
            MeshFilter combinedMeshFilter = _CombinedObject.GetComponent<MeshFilter>();
            combinedMeshFilter.mesh = combinedAllMesh;
            combinedMeshFilter.mesh.name = "CombineMesh_XX";
            combinedMeshFilter.gameObject.SetActive(true);
        }

        private void DisableTargetFilters()
        {
            int length = _TargetMeshFilters.Length;
            for (int i = 0; i < length; i++)
            {
                _TargetMeshFilters[i].gameObject.SetSafeActive(false);
            }
        }
    }
}