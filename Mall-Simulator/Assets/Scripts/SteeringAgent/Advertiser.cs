using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Advertiser : SteeringAgent {
	[SerializeField]
	private float _advertiserDislikeDistance;

	[SerializeField]
	private float _wanderTargetXMin = 16;
	[SerializeField]
	private float _wanderTargetXMax = 17;
	[SerializeField]
	private float _wanderTargetChangeTime = 5;

	[SerializeField]
	private float _wanderTargetYMax= 8;

	[SerializeField]
	private float _advertiseFreq = 5;

	[SerializeField]
	private float _advertiseProb = 0.5f;

	[SerializeField]
	private GameObject _flyerPrefab;

	private float _counter;
	private float _advertiseCounter;

	private enum AdvertiserState {
		Wandering
	}
	private AdvertiserState _state;

	public SteeringAgent PursueTarget;
	public SteeringAgent FleeTarget;

	protected override void Start() {
		base.Start();
		_state = AdvertiserState.Wandering;
		_seekingTarget = GetNewSeekTarget();
	}

	protected override void ActionSelection() {
		switch (_state) {
			case AdvertiserState.Wandering:
				_counter += Time.deltaTime; 
				if(_counter > _wanderTargetChangeTime) {
					_counter = 0;
					_seekingTarget = GetNewSeekTarget();
				}

				_advertiseCounter += Time.deltaTime;
				if(_advertiseCounter > _advertiseFreq) {
					_advertiseCounter = 0;
					CheckToPutFlyer();
				}

				break;
		}
	}

	private void CheckToPutFlyer() {
		var shouldAdvertise = Utils.RandomFloat() < _advertiseProb;
		if (shouldAdvertise) {
			Instantiate(_flyerPrefab, transform.position, Quaternion.identity);
		}
	}

	protected override void Steering() {

		switch (_state) {
			case AdvertiserState.Wandering:
				SteeringManager.Instance.Seek(this);
				foreach(var a in AdvertiserManager.Instance.Advertisers) {
					if(Vector3.Distance(transform.position, a.transform.position) < _advertiserDislikeDistance && a != this) {
						SteeringManager.Instance.Flee(this, a);
					}
				}
				SteeringManager.Instance.ObstacleAvoidance(this);
				break;
		}
	}

	private Vector3 GetNewSeekTarget() {
		// Decide whether we set target on the left or on the right
		var multiplier = Utils.RandomFloat() > 0.5 ? -1 : 1;
		var x = multiplier * Utils.RandomFloat(_wanderTargetXMin, _wanderTargetXMax);
		var y = Utils.RandomFloat(-_wanderTargetYMax, _wanderTargetYMax);
		var destination =  new Vector3(x, y, -1);
		Debug.Log(destination);
		return destination;
	}
}
