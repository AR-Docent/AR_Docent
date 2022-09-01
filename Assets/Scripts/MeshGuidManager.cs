using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class MeshGuidManager : MonoBehaviour
{
    ARMeshManager _meshManager;

    void Awake()
    {
        _meshManager = GetComponent<ARMeshManager>();
    }

    void OnEnable()
    {
        _meshManager.meshesChanged += OnChanged;
    }

    void OnDisable()
    {
        _meshManager.meshesChanged += OnChanged;
    }

    private void OnChanged(ARMeshesChangedEventArgs eventArgs)
    {
        foreach (var addMesh in eventArgs.added)
        {
            Debug.Log(addMesh.name + "add");
            addMesh.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        foreach (var updatedMesh in eventArgs.updated)
        {
            Debug.Log(updatedMesh.name + "updated");
            updatedMesh.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
        foreach (var deletedMesh in eventArgs.removed)
        {
            //
        }
    }
}
