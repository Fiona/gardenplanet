using UnityEngine;

namespace StompyBlondie
{

	/*
	 * Behaviour that constantly "looks" at the camera. Useful
	 * for billboarded objects.
	 */
	public class FaceCamera: MonoBehaviour
	{
		void Awake()
		{
			Update();
		}
		void Update()
		{
			transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
				Camera.main.transform.rotation * Vector3.up);
		}
	}
}

