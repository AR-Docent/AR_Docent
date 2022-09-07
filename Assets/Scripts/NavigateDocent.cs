using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class NavigateDocent : MonoBehaviour
{
    public bool Navigating { get; private set; }

    [SerializeField]
    private GameObject navPlane;
    [SerializeField]
    private GameObject navAgent;

    private ARPlaneManager _arPlane;
    private RaycastClickEvent _clickEvent;

    private NavMeshAgent _agent;
    private NavMeshSurface _surface;

    //private Transform _groundTrans = null;
	private Transform _groundTrans;
    private List<ARPlane> _planeList;

    private Vector3 _dest;

	void Awake()
    {
        _arPlane = GetComponent<ARPlaneManager>();
        _clickEvent = GetComponent<RaycastClickEvent>();

        _surface = navPlane.GetComponent<NavMeshSurface>();
        _agent = navAgent.GetComponent<NavMeshAgent>();

        _planeList = new List<ARPlane>();
    }

    private void Start()
    {
        navAgent.transform.position = Vector3.zero;
        Navigating = false;

        _groundTrans = navPlane.transform;
    }

    void OnEnable()
    {
        _arPlane.planesChanged += OnChanged;
        _clickEvent.clickEvent += OnClick;
    }

    void OnDisable()
    {
        _arPlane.planesChanged -= OnChanged;
        _clickEvent.clickEvent -= OnClick;
    }

    void Update()
    {
        if (Navigating == true)
        {
            _agent.Move(_agent.desiredVelocity);
            //Debug.Log("moving: " + _agent.remainingDistance);
            if (_agent.remainingDistance <= _agent.stoppingDistance)
            {
                Navigating = false;
                _agent.isStopped = true;
                Debug.Log("Stop");
            }

        }
    }

    void PlacePlane(Vector3 targetPos)
    {
        //move plane to real ground position y
        navPlane.transform.position = targetPos;
    }

    void PlaceAgent(Vector3 targetPos)
    {
        Debug.Log(targetPos);
        //move plane to real ground position y
        if (_agent.isOnNavMesh == true)
        {
            if (_agent.isStopped == false)
            {
                _agent.ResetPath();
                Navigating = false;
                Debug.Log("reset");
            }
        }
        _agent.Warp(targetPos);
    }

    void Navigate(Transform target)
    {
        Vector3 g_targetPos = new(target.position.x, _groundTrans.position.y, target.position.z);
        Vector3 g_startPos = new (navAgent.transform.position.x, _groundTrans.position.y, navAgent.transform.position.z);

        //set navPlane scale
        float dist = Vector3.Distance(g_startPos, g_targetPos) * 0.1f + 0.2f;
        Debug.Log("size" + dist);
        navPlane.transform.localScale = new Vector3(dist, dist, dist);

        //set pos and angle
        Vector3 diffVec = g_targetPos - g_startPos;

        //fix agent pos
        PlaceAgent(g_startPos);
        // move navPlane to center of target and startpos    
        PlacePlane(g_startPos + diffVec / 2);
        //set dest
        _dest = g_targetPos;
        //navmesh
        StartNavMesh();
    }

    void StartNavMesh()
    {
        //activate navMesh
        StartCoroutine(BuildNav());
    }

    IEnumerator BuildNav()
    {
        _surface.BuildNavMesh();
        while (_surface.navMeshData == null)
        {
            yield return new WaitForEndOfFrame();
        }
        _agent.SetDestination(_dest);
        while (_agent.destination == null)
        {
            yield return new WaitForEndOfFrame();
        }
        Debug.Log("start");
        Navigating = true;
    }
    
    void OnClick(Transform t, Vector3 p)
    {
        if (t.CompareTag("Product"))
        {
            //Set obj status
            GuidButton objGuid = t.GetComponent<GuidButton>();
            objGuid?.OnClick();

            if (objGuid.Selecting)
            {
                //set docent position y
                //set targetPos
                Navigate(t);
            }
        }
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
