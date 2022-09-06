using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class NavigateDocent : MonoBehaviour
{
    [SerializeField]
    private GameObject navPlane;
    
    private Transform _groundTrans = null;
    private ARPlaneManager _arPlane;
    private List<ARPlane> _planeList;

    void Awake()
    {
        _arPlane = GetComponent<ARPlaneManager>();
        _planeList = new List<ARPlane>();
    }

    void OnEnable()
    {
        _arPlane.planesChanged += OnChanged;
    }

    void OnDisable()
    {
        _arPlane.planesChanged -= OnChanged;
    }

    public void PlacePlane(Transform targetPos)
    {
        float _y = 0f;

        if (_groundTrans)
        {
            if (navPlane.transform.position.y - _groundTrans.transform.position.y > 0.1f
                || navPlane.transform.position.y - _groundTrans.transform.position.y < -0.1f)
            {
                _y = _groundTrans.transform.position.y;
            }
        }
        //move plane to real ground position y
        navPlane.transform.position = new Vector3(targetPos.position.x, _y, targetPos.position.z);
        //activate navMesh
        //navPlane.GetComponent<NavMeshSurface>()?.BuildNavMesh();
    }

    public void BakeNavInPlane()
    {
        //activate navMesh
        navPlane.GetComponent<NavMeshSurface>()?.BuildNavMesh();
    }

    private void OnChanged(ARPlanesChangedEventArgs eventArgs)
    {
        foreach (var addPlane in eventArgs.added)
        {
            _planeList.Add(addPlane);
        }
        foreach (var removedPlane in eventArgs.removed)
        {
            _planeList.Remove(removedPlane);
        }
        _planeList.Sort((a, b) => a.transform.position.y.CompareTo(b.transform.position.y));
        _groundTrans = _planeList?[0].transform;
    }
}
