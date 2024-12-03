using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;


namespace RogueApeStudios.SecretsOfIgnacios
{
    [System.Serializable]
    public class RigidBodyTransformConnection
    {
        [SerializeField] public Rigidbody _forcedRigidbody;
        [SerializeField] public Transform _targetTransform;
    }
    public class RigidbodyTransformFollower : MonoBehaviour
    {
        [SerializeField] private Rigidbody _forcedRigidbody;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private List<RigidBodyTransformConnection> _rigidbodyConnections;
        [SerializeField] private bool _followsPosition;
        [SerializeField] private bool _followsRotation;
        [SerializeField] private float _addedForce;
        [SerializeField] private float _addedAngularForce;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //set the thing's rotation and position so we dont get crazy distances
            _forcedRigidbody.gameObject.transform.position = _targetTransform.position;
            _forcedRigidbody.gameObject.transform.rotation = _targetTransform.rotation;
        }

        void FixedUpdate()
        {
            UpdatePhysics();
        }

        private void UpdatePhysics()
        {
            if (_followsPosition) 
            {
                UpdatePosition(_forcedRigidbody,_targetTransform);
                foreach (var connection in _rigidbodyConnections)
                {
                    UpdatePosition(connection._forcedRigidbody,connection._targetTransform);
                }
            }
            if (_followsRotation)
            {
                UpdateRotation(_forcedRigidbody, _targetTransform);
                foreach (var connection in _rigidbodyConnections)
                {
                    UpdateRotation(connection._forcedRigidbody, connection._targetTransform);
                }
            }
        }

        private void UpdatePosition(Rigidbody rb,Transform ttransform)
        {
            Vector3 dir = ( ttransform.position - rb.gameObject.transform.position);
            float dist = Mathf.Clamp(dir.magnitude,0,0.1f);
            dir = dir.normalized;

            Vector3 finalforce = dir * _addedForce * dist;

            
            rb.linearVelocity = finalforce;
        }

        private void UpdateRotation(Rigidbody rb, Transform ttransform)
        {
            /*var rotation = Quaternion.FromToRotation(rb.transform.forward, ttransform.forward).eulerAngles * -0.05f;
            print(rotation);
            rb.angularVelocity = rotation;*/
            // Rotations stack right to left,
            // so first we undo our rotation, then apply the target.
            var delta = ttransform.rotation * Quaternion.Inverse(rb.rotation);

            float angle; Vector3 axis;
            delta.ToAngleAxis(out angle, out axis);

            // We get an infinite axis in the event that our rotation is already aligned.
            if (float.IsInfinity(axis.x))
            {
                rb.angularVelocity = Vector3.zero;
                return;
            }

            if (angle > 180f)
                angle -= 360f;

            // Here I drop down to 0.9f times the desired movement,
            // since we'd rather undershoot and ease into the correct angle
            // than overshoot and oscillate around it in the event of errors.
            Vector3 angular = (0.96f * Mathf.Deg2Rad * angle/Time.fixedDeltaTime) * axis.normalized;

            rb.angularVelocity = angular;
        }
    }
}
