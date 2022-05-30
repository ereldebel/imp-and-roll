using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private float maxHeight;
    private Transform _transform;
    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        StartCoroutine(Rise(maxHeight));
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    
    private IEnumerator Rise(float maxHeight)
    {
        var t = _transform.position;
        while (_transform.position.y < maxHeight)
        {
            t.y += Time.deltaTime;
            _transform.position = t.y<maxHeight? t:new Vector3(t.x,maxHeight,t.z);
            yield return 0;
        }
    }
}
