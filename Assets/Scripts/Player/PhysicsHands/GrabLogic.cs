using UnityEngine;
using System.Collections.Generic;
using System;

namespace RogueApeStudios.SecretsOfIgnacios.Player.PhysicsHands
{
    public class GrabLogic : MonoBehaviour
    {
        private Rigidbody _grabbedRb;
        private int _oldGrabbableLayer;
        private LayerMask _grabLayerMask;
        private float _initialGrabDist;
        private Vector3 _grabEndVelocity;
        private Vector3 _oldGrabPosition = new Vector3(0,0,0);
        [SerializeField] private ConfigurableJoint _grabbedJoint;
        [SerializeField] private float _grabRange;
        [SerializeField] private Rigidbody _grabbingRb;
        [SerializeField] private List<String> _layersToIgnore;
        [SerializeField] private Transform _grabCenter;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            LayerMask mask = Physics.AllLayers;

            foreach (string layername in _layersToIgnore)
            {
                LayerMask layer = LayerMask.NameToLayer(layername);
                mask &= ~(1 << layer);
            }
            _grabLayerMask = mask;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //check if the grabbed object is close enough, and end grab if it is too far away
            if (_grabbedRb != null) 
            {
                var dist = Vector3.Magnitude(_grabbedRb.position - _grabCenter.position) - _initialGrabDist;
                if(dist > 1.5 * _grabRange)
                {
                    EndGrab();
                }
                Vector3 potentialVelocity = (transform.position - _oldGrabPosition)/Time.fixedDeltaTime;
                if (potentialVelocity != Vector3.zero) 
                {
                    _grabEndVelocity = potentialVelocity;
                }
                _oldGrabPosition = transform.position;
                
            }

        }

        public void CheckForGrabbables()
        {
            if (_grabbedRb == null)
            {
                //do multi sphere cast from the grabbing center, then get the closest object. This becomes _grabbedRb
                RaycastHit[] spherecasthits;
                List<Rigidbody> potentialRigidbodies = new List<Rigidbody>();

                
                
                spherecasthits = Physics.SphereCastAll(_grabCenter.position, _grabRange, Vector3.forward, _grabRange,_grabLayerMask);

                foreach (RaycastHit hit in spherecasthits)
                {
                    GameObject hitObject = hit.collider.gameObject;
                    Rigidbody oldRb;
                    if (hitObject.TryGetComponent<Rigidbody>(out oldRb))
                    {
                        if (!oldRb.isKinematic) {

                            potentialRigidbodies.Add(oldRb);
                            
                        }
                    }
                }

                if (potentialRigidbodies.Count > 0)
                {
                    _grabbedRb = getClosestRigidbody(_grabCenter, potentialRigidbodies);
                    _initialGrabDist = Vector3.Magnitude(_grabbedRb.position - _grabCenter.position);
                    StartGrab();
                    Debug.Log(_grabbedRb.gameObject.name);
                }
            }
        }
        private void StartGrab() 
        { 
            //use the serialized grabbed joint and set its rigidbody. Then change layer
            _grabbedJoint.connectedBody = _grabbedRb;
            _oldGrabbableLayer = _grabbedRb.gameObject.layer;
            _grabbedRb.gameObject.layer = 14;

        }

        public void EndGrab()
        {
            if (_grabbedRb != null) { 
                _grabbedJoint.connectedBody = null;
                _grabbedJoint.gameObject.layer = _oldGrabbableLayer;
                _grabbedRb.linearVelocity = _grabEndVelocity*1;
                _grabbedRb = null;
            }
        }

        private Rigidbody getClosestRigidbody(Transform point, List<Rigidbody> rigidbodies)
        {
            if(rigidbodies.Count == 0)
            {
                return null;
            }

            int bestIndex = 0;

            //if there is more than one potential thing, loop through all of them until we've found the closest one

            if (rigidbodies.Count > 1)
            {
                float furthestDistance = Mathf.Pow(_grabRange,2); //power of 2 so we can use the more performant sqrmagnitude
                for (int i = 0; i < rigidbodies.Count; i++)
                {
                    Rigidbody curRb = rigidbodies[i];
                    float distance =  Vector3.SqrMagnitude(curRb.transform.position - point.position);

                    if (distance < furthestDistance) 
                    { 
                        furthestDistance = distance;
                        bestIndex = i;
                    }

                }
            }


            return rigidbodies[bestIndex];
        }

    }
}
