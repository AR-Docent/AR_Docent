using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR.ARFoundation;

public class NavigateDocent : MonoBehaviour
{
    public bool Navigating
    {
        get { return _navigate; }
        private set
        {
            _navigate = value;
            _agentAnimater.walk = _navigate;
        }
    }

    [SerializeField]
    private GameObject navPlane;
    [SerializeField]
    private GameObject navAgent;

    private ARPlaneManager _arPlane;
    private RaycastClickEvent _clickEvent;

    private NavMeshAgent _agent;
    private NavMeshSurface _surface;
    private DocentAnimater _agentAnimater;

	private Transform _groundTrans = null;
    private List<ARPlane> _planeList;

    private Vector3 _dest;
    private bool _navigate;

    private Vector3 _pastPos;

    private Coroutine _bakeCoroutine = null;

    void Awake()
    {
        _arPlane = GetComponent<ARPlaneManager>();
        _clickEvent = GetComponent<RaycastClickEvent>();

        _surface = navPlane.GetComponent<NavMeshSurface>();

        _agent = navAgent.GetComponent<NavMeshAgent>();
        _agentAnimater = navAgent.GetComponentInChildren<DocentAnimater>();

        _planeList = new List<ARPlane>();
    }

    private void Start()
    {
        Navigating = false;
        navAgent.transform.position = Vector3.zero;
        //set nav disabled
        navPlane.SetActive(false);
        navAgent.SetActive(false);
        // set rotation will be controlled manualy
        _agent.updateRotation = false;

        StartCoroutine(InitAgent(Camera.main.transform.position));
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
        if (Navigating != true)
            return;
        //move agent
        _agent.Move(_agent.desiredVelocity);
        //rotate agent
        navAgent.transform.rotation = Quaternion.LookRotation(navAgent.transform.position - _pastPos, navPlane.transform.up);
        //save pos
        _pastPos = navAgent.transform.position;
        //Debug.Log("moving: " + _agent.remainingDistance);
        if (_agent.remainingDistance <= _agent.stoppingDistance)
        {
            //arrive
            Navigating = false;
            _agent.isStopped = true;
            //rotate agent
            navAgent.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - navAgent.transform.position, navPlane.transform.up);
            Debug.Log("Stop");
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

    IEnumerator InitAgent(Vector3 startPos)
    {
        while (_groundTrans == null)
        {
            yield return new WaitForEndOfFrame();
        }
        startPos += Vector3.forward;

        Vector3 _ground = new Vector3(startPos.x, _groundTrans.position.y, startPos.z);

        //set active
        navAgent.SetActive(true);
        navPlane.SetActive(true);
        //move
        PlacePlane(_ground);
        PlaceAgent(_ground);
        //rotate
        navAgent.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - navAgent.transform.position, navPlane.transform.up);
        //init pastPos
        _pastPos = navAgent.transform.position;
    }

    IEnumerator BuildNav()
    {
        //wait another buildNav coroutine running
        while (_bakeCoroutine != null)
        {
            yield return new WaitForEndOfFrame();
        }
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
        //finish
        Navigating = true;
        _bakeCoroutine = null;
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
        //activate navMesh
        _bakeCoroutine = StartCoroutine(BuildNav());
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
        //
        _groundTrans.transform.position += Vector3.up * 0.01f;
    }
}
