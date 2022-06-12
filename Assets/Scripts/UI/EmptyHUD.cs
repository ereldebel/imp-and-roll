using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EmptyHUD : MonoBehaviour
    {

        private Animator _animator;
        private Image _image;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _image = GetComponent<Image>();
        }

        private void OnEnable()
        {
            _animator.enabled = false;
            var color = _image.color;
            color.a = 1;
            _image.color = color;
        }

        private void AnimatorDisable()
        {
            gameObject.SetActive(false);
        }
    }
}
