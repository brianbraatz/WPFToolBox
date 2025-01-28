using System;
using System.Diagnostics;

namespace BindableObjectDemo
{
	/// <summary>
	/// Stupid class which derives from BindableObject.
	/// </summary>
	class Wheel : BindableObject
	{
		private bool _isBroken;
		private int _rotationAngle;

		public bool IsBroken
		{
			get { return _isBroken; }
			set 
			{
				if (value == _isBroken)
					return;

				_isBroken = value;

				base.RaisePropertyChanged("IsBroken");
			}
		}

		public int RotationAngle
		{
			get { return _rotationAngle; }
			set 
			{
				if (value == _rotationAngle)
					return;

				_rotationAngle = value;

				base.RaisePropertyChanged("RotationAngle");
			}
		}

		protected override void AfterPropertyChanged(string propertyName)
		{
			Debug.WriteLine("Wheel property changed: " + propertyName);
		}
	}
}