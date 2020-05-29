﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class JumpingState : State
{
    [SerializeField]
    private Rigidbody _rigidbody;
    [SerializeField]
    private Vector2 _jumpStrength, _fallStrength;
    [SerializeField]
    private float _postJumpDelay, _wallTouchPadding = 0.5f;
    [SerializeField]
    private AnimationCurve _fallCurve;
    [SerializeField]
    private ParticleSystem _landingParticles;

    protected override void Enter()
    {
        float xDirection = Mathf.Sign(4 - transform.position.x);
        _rigidbody.AddForce(new Vector2(_jumpStrength.x * xDirection, _jumpStrength.y), ForceMode.VelocityChange);
        StartCoroutine(ApplyJumpForce(Time.time, xDirection));
        _landingParticles.transform.localEulerAngles = Vector3.up * (xDirection > 0 ? 0 : 180);
        _landingParticles.transform.localPosition = Vector3.right * 0.5f * -xDirection;
        _landingParticles.Play();
    }

    private IEnumerator ApplyJumpForce (float startTime, float xDirection)
    {
        float timer = 0;
        int layerMask = LayerMask.GetMask("Default");
        while (!Physics.Raycast(transform.position, Vector3.right * xDirection, _wallTouchPadding, layerMask, QueryTriggerInteraction.Ignore))
        {
            float fallX = xDirection * _fallStrength.x;
            float fallY = -_fallCurve.Evaluate(timer) * _fallStrength.y;
            _rigidbody.AddForce(new Vector2(fallX, fallY) * Time.deltaTime);
            yield return null;
            timer = Time.time - startTime;
        }
        _rigidbody.velocity = Vector3.up * _rigidbody.velocity.y;
        ChangeState("Slide");
    }
}
