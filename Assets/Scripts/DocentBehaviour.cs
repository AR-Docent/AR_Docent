using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class DocentBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _text;
    
    private RaycastClickEvent _click;
    private NavMeshAgent _agent;
    
    private int _layer = 1 << 7;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void OnEnable()
    {
        _click.clickEvent += OnClick;
    }

    void OnDisable()
    {
        _click.clickEvent -= OnClick;
    }

    void OnClick(Transform t, Vector3 p)
    {
        if (t.CompareTag("Product"))
        {
            SetDest(p);
        }
    }

    public void SetDest(Vector3 p)
    {
        RaycastHit _hit;
        Ray _ray = new Ray(p, Vector3.down);

        if (Physics.Raycast(_ray, out _hit, 100f, _layer))
        {
            _agent.SetDestination(_hit.point);
            _text.text = "tracking";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_agent.remainingDistance > _agent.stoppingDistance)
        {
            _agent.Move(_agent.desiredVelocity);
        }
        else
        {
            _agent.Move(Vector3.zero);
        }
    }

}
