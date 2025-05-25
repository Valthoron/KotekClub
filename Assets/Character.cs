using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
	// ********************************************************************************
	// Structs/enums
	private struct StateInfo
	{
		public string Name;
		public Action Handler;
	}

	private struct AnimationInfo
	{
		public string Name;
		public float Speed;
	}

	// ********************************************************************************
	// Members
	private Dictionary<int, StateInfo> _states = new ();
	private Dictionary<int, AnimationInfo> _animations = new ();

	private int _currentStateIndex = -1;
	private int _currentAnimationIndex = -1;

	// ********************************************************************************
	// Properties
	protected int State { get { return _currentStateIndex; } }
	protected int Animation { get { return _currentAnimationIndex; } }

	public SpriteRenderer SpriteRenderer;
	public Animator Animator;

	// ********************************************************************************
	// Unity messages
	private void Start()
    {
		OnStart();
    }

    
    private void Update()
    {
        if (_currentStateIndex != -1)
		{
			var state = _states[_currentStateIndex];
			state.Handler();
		}

		OnUpdate();
    }

	protected abstract void OnStart();
	protected abstract void OnUpdate();

	// ********************************************************************************
	// Initialization utilities
	protected void RegisterState(int index, string name, Action handler)
	{
		_states[index] = new StateInfo() { Name = name, Handler = handler };
	}

	protected void RegisterAnimation(int index, string name, float speed)
	{
		_animations[index] = new AnimationInfo() { Name = name, Speed = speed };
	}

	// ********************************************************************************
	// Runtime utilities
	protected void SetState(int index)
	{
		if ((index < 0) || (index == _currentStateIndex))
			return;

		if (!_states.TryGetValue(index, out var state))
			return;

		_currentStateIndex = index;
	}

	protected void PlayAnimation(int index)
	{
		if ((index < 0) || (index == _currentAnimationIndex))
			return;

		if (!_animations.TryGetValue(index, out var animation))
			return;

		_currentAnimationIndex = index;

		Animator.Play(animation.Name);

		if (animation.Speed > 0.0f)
			Animator.speed = animation.Speed;
		else
			Animator.speed = 1.0f;
	}
}
