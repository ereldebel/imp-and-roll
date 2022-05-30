using System.Collections;
using System.Collections.Generic;
using Environment;
using Managers;
using Player;
using UnityEngine;

public class Reflection : MonoBehaviour
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
        // var spriteRenderers = GetComponentsInParent<SpriteRenderer>();
        _daddySpriteRenderer = GameManager.Players[playerToFollow].GetComponent<SpriteRenderer>();
        transform.SetParent(_daddySpriteRenderer.transform);
        transform.localPosition = new Vector3(0, -2, 0);
        // transform.Rotate(Vector3.left,180);
        // foreach (var spriteRenderer in spriteRenderers)
        // {
        //     if (spriteRenderer.GetComponent<PlayerBrain>() != null)
        //     {
        //         _daddySpriteRenderer = spriteRenderer;                
        //     }
        // }
        if (_daddySpriteRenderer == _mySpriteRenderer)
        {
            print("Same Sprite Renderer");
        }
    }

    // Update is called once per frame
    void Update()
    {
        _mySpriteRenderer.sprite = _daddySpriteRenderer.sprite;
    }
}
