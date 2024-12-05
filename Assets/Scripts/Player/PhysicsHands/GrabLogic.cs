using UnityEngine;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Datums;
using System;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    public class GrabLogic : MonoBehaviour
    {
        private Rigidbody _grabbedRb;
        private int _oldGrabbableLayer;
        private LayerMask _grabLayerMask;
        private float _initialGrabDist;
        private ConfigurableJoint _grabbedJoint;
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
                if(dist > .5 * _grabRange)
                {
                    EndGrab();
                }
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
            //attach a configurable joint to the grabbed rb and configure it
            _grabbedJoint = _grabbedRb.gameObject.AddComponent<ConfigurableJoint>();
            _grabbedJoint.connectedBody = _grabbingRb;
            _grabbedJoint.xMotion = ConfigurableJointMotion.Locked;
            _grabbedJoint.yMotion = ConfigurableJointMotion.Locked;
            _grabbedJoint.zMotion = ConfigurableJointMotion.Locked;
            _grabbedJoint.angularXMotion = ConfigurableJointMotion.Locked;
            _grabbedJoint.angularYMotion = ConfigurableJointMotion.Locked;
            _grabbedJoint.angularZMotion = ConfigurableJointMotion.Locked;
            _oldGrabbableLayer = _grabbedJoint.gameObject.layer;
            _grabbedJoint.gameObject.layer = 14;
        }

        public void EndGrab()
        {
            if (_grabbedRb != null) { 
                Debug.Log("i'm gonna stop grab u!!!!1");
                _grabbedJoint.gameObject.layer = _oldGrabbableLayer;
                //remove a configurable joint
                Destroy(_grabbedJoint);
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
