using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Reflection : MonoBehaviour
{
    private SpriteRenderer _mySpriteRenderer, _daddySpriteRenderer;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        _mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _mySpriteRenderer.flipY = true;
        var spriteRenderers = GetComponentsInParent<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer.GetComponent<PlayerBrain>() != null)
            {
                _daddySpriteRenderer = spriteRenderer;                
            }
        }
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
