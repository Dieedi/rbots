using Rbots.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rbots.CameraUI
{
	public class CameraController : MonoBehaviour
	{
		[SerializeField] Cinemachine.CinemachineFreeLook CombatCam;

		Player c_Player;

		private void Awake()
		{
			c_Player = FindObjectOfType<Player>();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (c_Player.myTarget) {
				Debug.Log("Got target");
				CombatCam.Priority = 20;
				CombatCam.m_Orbits[0].m_Height = 6;
			} else {
				Debug.Log("lost target");
				CombatCam.Priority = 0;
				CombatCam.m_Orbits[0].m_Height = 4;
			}
		}
	}
}
