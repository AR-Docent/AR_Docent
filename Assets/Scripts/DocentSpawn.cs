using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocentSpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject docentPrefab;
    [SerializeField]
    private RectTransform _canvas;

    private Camera _mainCam;
    private Ray _ray;
    private RaycastHit _hit;
    private int _layer = 1 << 7;

    void Awake()
    {
        _mainCam = Camera.main;
        Debug.Log(_canvas.rect.width * 0.5f);
        Debug.Log(_canvas.rect.height * 0.5f);
    }

    public void SpawnDocent()
    {
        _ray = _mainCam.ScreenPointToRay(new Vector3(_canvas.rect.width * 0.5f, _canvas.rect.height * 0.5f, 0));
        if (Physics.Raycast(_ray, out _hit, 100, _layer))
        {
            Instantiate(docentPrefab, _hit.point, Quaternion.identity);
        }
    }
}
