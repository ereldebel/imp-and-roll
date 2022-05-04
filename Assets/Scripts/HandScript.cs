using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float ymaxVal;
    [SerializeField] private bool followX;
    [SerializeField] private bool followZ;
    [SerializeField] private GameObject object_to_follow;
    [SerializeField] private string arenaMaterialBorderYValueName = "BorderY";
    
    #endregion

    private Vector3 _startingPos;
    private Material _handMaterial;
    private float _yminVal;
    private int _shaderBorderYVar;
    private Collider _myCollider;

    private float Lerp(float max, float min, float t)
    {
        return min * t +  max* (1 - t);
    }
    private void Awake()
    {
        _startingPos = transform.position;
        _yminVal = _startingPos.y;
        _handMaterial = GetComponent<Renderer>().material;
        _shaderBorderYVar = Shader.PropertyToID(arenaMaterialBorderYValueName);
        _myCollider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        Vector3 pos = _startingPos;
        if (followX)
        {
            pos.x = object_to_follow.transform.position.x;
            
            if (Math.Abs(transform.position.z - object_to_follow.transform.position.z)<5)
            {
                // print(Math.Abs(transform.position.z - object_to_follow.transform.position.z));
                pos.y = Lerp(ymaxVal, _yminVal, Math.Abs(transform.position.z - object_to_follow.transform.position.z)/5);
                _handMaterial.SetFloat(_shaderBorderYVar, 1-Math.Abs(transform.position.z - object_to_follow.transform.position.z)/5);
            }
        }
        else
        {
            pos.z = object_to_follow.transform.position.z;   
        }
        transform.position = pos;
    }
}
