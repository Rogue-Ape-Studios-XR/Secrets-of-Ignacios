using System;
using UnityEngine;

namespace RogueApeStudios.SecretsOfIgnacios.Interactables.Earth
{
	internal class Resizable : EarthInteractable
	{
		[SerializeField] private Vector3 _shrinkSize;
		[SerializeField] private Vector3  _defaultSize;
		[SerializeField] private Vector3 _growSize;

		[SerializeField] private Transform _targetObject;

		[SerializeField] private bool _shrunk;
		[SerializeField] private bool _grown;

		internal event Action onSizeChanged;
		
		public bool Shrunk => _shrunk;
        public bool Grown => _grown;
        
		internal override void Awake()
		{
			//Sorry, but this was necessary for now, if you have something better, feel free to say so :)
			base.Awake();
			
			if (_shrunk)
				_targetObject.localScale = _shrinkSize;
			else if (_grown)
				_targetObject.localScale = _growSize;
			else
				_targetObject.localScale = _defaultSize;
		}

		internal override void Touched()
		{
			ResizeObject();
		}

		private void ResizeObject()
		{
			if (_isGrowSpellActive && !_grown)
				GrowObject();

			else if (_isShrinkSpellActive && !_shrunk)
				ShrinkObject();
		}

		private void GrowObject()
		{
			if (_shrunk)
			{
				_targetObject.localScale = _defaultSize;
				_shrunk = false;
			}
			else
			{
				_targetObject.localScale = _growSize;
				_grown = true;
			}

			_isGrowSpellActive = false;
            onSizeChanged?.Invoke();
		}

		private void ShrinkObject()
		{
			if (_grown)
			{
				_targetObject.localScale = _defaultSize;
				_grown = false;
			}

			else
			{
				_targetObject.localScale = _shrinkSize;
				_shrunk = true;
			}

			_isShrinkSpellActive = false;
            onSizeChanged?.Invoke();
		}
	}
}