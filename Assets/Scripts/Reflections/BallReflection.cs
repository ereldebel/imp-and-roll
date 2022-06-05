using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

public class BallReflection : MonoBehaviour
{
    private MeshRenderer _myMeshRenderer, _daddyMeshRenderer;
    private MeshFilter _myMeshFilter, _daddyMeshFilter;
    private Transform _myT, _daddyT;
    void OnEnable()
    {
        _myT = transform;
        _myMeshRenderer = GetComponent<MeshRenderer>();
        _myMeshFilter = GetComponent<MeshFilter>();
        _daddyMeshRenderer = MatchManager.BallTransform.GetComponent<MeshRenderer>();
        _daddyT = _daddyMeshRenderer.transform;
        _daddyMeshFilter = _daddyT.GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        var newPos = _daddyT.position;
        newPos.y *= -1;
        _myT.position = newPos;
        _myT.localScale = _daddyT.localScale;
        if (_daddyMeshRenderer.material != _myMeshRenderer.material)
        {
            _myMeshRenderer.material = _daddyMeshRenderer.material;
        }
        _myMeshFilter.mesh = _daddyMeshFilter.mesh;
        if (GameManager.CurScene != 2)
        {
            Destroy(gameObject);
        }
    }
}
