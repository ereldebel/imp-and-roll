using UnityEngine;

namespace UI
{
    public class Destroyable : MonoBehaviour
    {
        private void AnimatorDestroy()
        {
            Destroy(gameObject);
        }
    }
}
