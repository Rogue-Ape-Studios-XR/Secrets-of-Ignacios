using System.Collections.Generic;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Player
{
    public class MagicCirclePosition : MonoBehaviour
    {

        [SerializeField] List<Transform> _handpositions;
        [SerializeField] float _aheadValue = 0f;
        [SerializeField] float _upValue = 0f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void FixedUpdate()
        {

            //get rotation between the two hands, not accounting for height
            Vector3 dirvector = Quaternion.AngleAxis(-90, Vector3.up) *  new Vector3(_handpositions[0].position.x - _handpositions[1].position.x, 0, _handpositions[0].position.z - _handpositions[1].position.z).normalized; 
            //lerp rotation between last rotation and now (smoothes the process)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dirvector), 0.5f);
            //lerp position between last position and now (smoothes the process)
            transform.position = Vector3.Lerp(transform.position, (_handpositions[0].position + _handpositions[1].position)/2 + dirvector * _aheadValue + Vector3.up * _upValue,0.5f);

        }
    }
}
