using UnityEngine;

public class OpeningSceneManager : MonoBehaviour
{
    [SerializeField] private Ball.Ball ball;

    private void OnDestroy()
    {
        if (ball)
            Destroy(ball.gameObject);
    }

}
