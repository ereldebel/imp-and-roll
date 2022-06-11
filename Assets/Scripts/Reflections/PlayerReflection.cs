using System.Collections;
using System.Collections.Generic;
using Environment;
using Managers;
using Player;
using UnityEngine;

public class PlayerReflection : MonoBehaviour
{
    private SpriteRenderer _mySpriteRenderer, _daddySpriteRenderer;
    [SerializeField] private int playerToFollow;

    private void Start()
    {
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        _mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _mySpriteRenderer.flipY = true;
        _daddySpriteRenderer = GameManager.Players[playerToFollow].GetComponent<SpriteRenderer>();
        transform.SetParent(_daddySpriteRenderer.transform);
        transform.localPosition = new Vector3(0, -2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerToFollow == 1)
        {
            _mySpriteRenderer.flipX = !_daddySpriteRenderer.flipX;            
        }
        else
        {
            _mySpriteRenderer.flipX = _daddySpriteRenderer.flipX;
        }

        _mySpriteRenderer.sprite = _daddySpriteRenderer.sprite;
        if (GameManager.CurScene != 2)
        {
            Destroy(gameObject);
        }
    }
}
