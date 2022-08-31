using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DocentBehaviour : MonoBehaviour
{
    RaycastClickEvent _click;
    NavMeshAgent _agent;

    void Awake()
    {
        _click = GetComponent<RaycastClickEvent>();
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
    }

    void OnEnable()
    {
        _click.clickEvent += OnClick;
    }

    void OnDisable()
    {
        _click.clickEvent -= OnClick;
    }

    void OnClick(Transform t)
    {
        if (t.CompareTag("Product"))
        {
            _agent.SetDestination(t.position);
        }
        else
        {
            this.transform.position = t.position;
        }
    }

    public void SetDest(Vector3 pos)
    {
        _agent.SetDestination(pos);
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
