using UnityEngine;
using System.Collections.Generic;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class GrabLogic : MonoBehaviour
    {
        private Rigidbody _grabbedRb;
        private ConfigurableJoint _grabbedJoint;
        [SerializeField] private float _grabRange;
        [SerializeField] private Rigidbody _grabbingRb;
        [SerializeField] private Collider _grabTrigger;
        [SerializeField] private Transform _grabCenter;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //check if the grabbed object is close enough, and end grab if it is too far away
        }


        public void CheckForGrabbables()
        {
            Debug.Log("i'm gonna grab u!!!!1");
            if (_grabbedRb == null)
            {
                //do multi sphere cast from the grabbing center, then get the closest object. This becomes _grabbedRb
                RaycastHit[] spherecasthits;
                List<Rigidbody> potentialRigidbodies = null;
                spherecasthits = Physics.SphereCastAll(_grabCenter.position, _grabRange, Vector3.forward, _grabRange);

                foreach (RaycastHit hit in spherecasthits)
                {
                    GameObject hitObject = hit.collider.gameObject;
                    Rigidbody oldrb;
                    if (hitObject.TryGetComponent<Rigidbody>(out oldrb))
                    {
                       
                        //oldrb.AddExplosionForce(800, _grabCenter.position, _grabRange);
                        if (!oldrb.isKinematic) {

                            potentialRigidbodies.Add(oldrb);
                            
                        }
                    }
                }

                if (potentialRigidbodies.Count > 0)
                {
                    _grabbedRb = getClosestRigidbody(_grabCenter, potentialRigidbodies);
                    StartGrab();
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
            _grabbedJoint.gameObject.layer = 14;
        }

        public void EndGrab()
        {
            Debug.Log("i'm gonna stop grab u!!!!1");
            Destroy(_grabbedJoint);
            _grabbedRb = null;
            //remove a configurable joint and configure it
        }

        private Rigidbody getClosestRigidbody(Transform point, List<Rigidbody> rigidbodies)
        {
            return rigidbodies[0];
        }

    }
}
