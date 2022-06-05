using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterReflection : MonoBehaviour
{
    private SpriteRenderer _mySpriteRenderer, _daddySpriteRenderer;
    [SerializeField] private GameObject objectToFollow;
    private Transform _myT, _daddyT;

    private void Start()
    {
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        _myT = transform;
        _mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _mySpriteRenderer.flipY = true;
        _daddySpriteRenderer = objectToFollow.GetComponent<SpriteRenderer>();
        _daddyT = objectToFollow.transform;
    }

    // Update is called once per frame
    void Update()
    {
        _mySpriteRenderer.flipX = _daddySpriteRenderer.flipX;
        _mySpriteRenderer.sprite = _daddySpriteRenderer.sprite;
        var newPos = _daddyT.position;
        newPos.y *= -1;
        _myT.position = newPos;
        _myT.localScale = _daddyT.localScale;
    }
}
