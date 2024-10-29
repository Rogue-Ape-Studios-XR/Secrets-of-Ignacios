using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Spells
{
    public class Indicator : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Cast _cast;

        [Header("Grow Settings")]
        [SerializeField] private float _growthSpeed = 10f;
        [SerializeField] private float _maxGrowth = 100f;

        private bool _rightIsGrowing = true;
        private bool _leftIsGrowing = true;

        private float _rightCurrentLength = 0f;
        private float _leftCurrentLength = 0f;

        private void FixedUpdate()
        {
            if (_cast._rightHandData._lineRenderer.enabled)
                UpdateBeam(_cast._rightHandData._lineRenderer, _cast._rightHandData._handTransform,
                    ref _rightIsGrowing, ref _rightCurrentLength);

            if (_cast._leftHandData._lineRenderer.enabled)
                UpdateBeam(_cast._leftHandData._lineRenderer, _cast._leftHandData._handTransform,
                    ref _leftIsGrowing, ref _leftCurrentLength);
        }

        private void UpdateBeam(LineRenderer lineRenderer, Transform indicatorTransform, ref bool isGrowing, ref float currentLength)
        {
            Vector3 direction = indicatorTransform.forward;

            lineRenderer.SetPosition(0, indicatorTransform.position);

            if (Physics.Raycast(indicatorTransform.position, direction, out RaycastHit hit, currentLength + 0.1f))
                HandleHit(lineRenderer, hit, ref isGrowing, ref currentLength);
            else
                ResumeGrowthIfNeeded(ref isGrowing);

            if (isGrowing)
                GrowBeam(lineRenderer, indicatorTransform, ref currentLength);
        }

        private void HandleHit(LineRenderer lineRenderer, RaycastHit hit, ref bool isGrowing, ref float currentLength)
        {
            if (isGrowing)
            {
                currentLength = Vector3.Distance(lineRenderer.GetPosition(0), hit.point);
                lineRenderer.SetPosition(1, hit.point);
                isGrowing = false;
            }
            else
            {
                currentLength = Vector3.Distance(lineRenderer.GetPosition(0), hit.point);
                lineRenderer.SetPosition(1, hit.point);
            }
        }

        private void ResumeGrowthIfNeeded(ref bool isGrowing)
        {
            if (!isGrowing)
                isGrowing = true;
        }

        private void GrowBeam(LineRenderer lineRenderer, Transform indicatorTransform, ref float currentLength)
        {
            if (currentLength < _maxGrowth)
            {
                currentLength += _growthSpeed * Time.deltaTime;
                lineRenderer.SetPosition(1, indicatorTransform.position + indicatorTransform.forward * currentLength);
            }
            else
                lineRenderer.SetPosition(1, indicatorTransform.position + indicatorTransform.forward * _maxGrowth);
        }

        public void ToggleRightIndicator(bool active)
        {
            if (_cast._rightHandData._canCast)
            {
                _cast._rightHandData._lineRenderer.enabled = active;
                _rightIsGrowing = active;

                if (!active)
                    _rightCurrentLength = 0f;
            }
        }

        public void ToggleLeftIndicator(bool active)
        {
            if (_cast._leftHandData._canCast)
            {
                _cast._leftHandData._lineRenderer.enabled = active;
                _leftIsGrowing = active;

                if (!active)
                    _leftCurrentLength = 0f;
            }
        }
    }
}
