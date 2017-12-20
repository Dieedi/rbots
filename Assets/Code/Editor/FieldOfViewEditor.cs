using UnityEngine;
using UnityEditor;

namespace Rbots.Characters
{
	[CustomEditor(typeof(FieldOfViewController))]
	public class FieldOfViewEditor : Editor
	{
		private void OnSceneGUI()
		{
			FieldOfViewController fovc = (FieldOfViewController)target;
			Handles.color = Color.white;
			Handles.DrawWireArc(fovc.transform.position, Vector3.up, Vector3.forward, 360, fovc.targettingRadius);
			//Vector3 viewAngleA = fovc.DirFromAngle(-fovc.targettingAngle / 2, false);
			//Vector3 viewAngleB = fovc.DirFromAngle(fovc.targettingAngle / 2, false);

			//Handles.DrawLine(fovc.transform.position, fovc.transform.position + viewAngleA * fovc.targettingRadius);
			//Handles.DrawLine(fovc.transform.position, fovc.transform.position + viewAngleB * fovc.targettingRadius);

			Handles.color = Color.red;
			foreach (Transform visibleTarget in fovc.visibleTargets) {
				Handles.DrawLine(fovc.transform.position, visibleTarget.position);
			}
		}

	}
}