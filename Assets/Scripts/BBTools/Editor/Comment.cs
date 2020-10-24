using UnityEngine;

public class Comment : MonoBehaviour
{
	#if UNITY_EDITOR
	[TextAreaAttribute (12, 3000)]
	public string comment;
	#endif
}
