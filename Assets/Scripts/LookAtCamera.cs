using UnityEngine;

[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
	[SerializeField] private Quaternion offset;

	private void OnValidate()
	{
		transform.LookAt(Camera.main.transform.position);
		transform.rotation = offset * transform.rotation;
		print(offset);
		print(offset.eulerAngles);
		print(Quaternion.Inverse(transform.rotation));
		print(Quaternion.Inverse(transform.rotation).eulerAngles);
		print(Quaternion.Euler(50, 0, 0) * Quaternion.Inverse(transform.rotation));
		print((Quaternion.Euler(50, 0, 0) * Quaternion.Inverse(transform.rotation)).eulerAngles);
	}
}