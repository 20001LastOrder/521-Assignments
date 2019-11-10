using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
	[SerializeField]
	private float _minSimulationSpeed = 0.25f;

	[SerializeField]
	private float _maxSimulationSpeed = 20;

	private float currentSimulationSpeed = 1;

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space)) {
			if(Time.timeScale > 0) {
				currentSimulationSpeed = Time.timeScale;
				UIManager.Instance.SetSimulationSpeedText(0);
				Time.timeScale = 0;
			} else {
				SetCurrentSimulationSpeed();
			}
		}else if (Input.GetKeyDown(KeyCode.Equals)) {
			if(Time.timeScale <= 0) {
				return;
			}

			currentSimulationSpeed = Time.timeScale * 2;
			if(currentSimulationSpeed <= _maxSimulationSpeed) {

				SetCurrentSimulationSpeed();
			}
		} else if (Input.GetKeyDown(KeyCode.Minus)) {
			if (Time.timeScale <= 0) {
				return;
			}

			currentSimulationSpeed = Time.timeScale / 2;
			if (currentSimulationSpeed >= _minSimulationSpeed) {
				SetCurrentSimulationSpeed();
			}
		}
	}

	private void SetCurrentSimulationSpeed() {
		UIManager.Instance.SetSimulationSpeedText(currentSimulationSpeed);
		Time.timeScale = currentSimulationSpeed;
	}
}
