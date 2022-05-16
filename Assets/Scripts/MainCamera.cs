using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private static MainCamera _instance;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 eulerAngles;
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void ToGamePosition()
    {
        var instanceTransform = _instance.transform;
        instanceTransform.position = _instance.position;
        instanceTransform.rotation = Quaternion.Euler(_instance.eulerAngles);
    }
}
