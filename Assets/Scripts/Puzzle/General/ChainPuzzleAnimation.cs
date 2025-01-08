using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios
{
    public class ChainPuzzleAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _chainAnimator;

        public void HandleChainWeighedDown()
        {
            Debug.Log("Going down");
            _chainAnimator.SetBool("ChainDown",true);
        }
        public void HandleChainLighter()
        {
            Debug.Log("Going up");
            _chainAnimator.SetBool("ChainDown",false);
        }

    }
}
