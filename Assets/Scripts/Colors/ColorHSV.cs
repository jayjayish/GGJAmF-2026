using System;
using Data;
using UnityEngine;

namespace Colors
{
	public class ColorHSV : System.Object
	{
		private float _h, _s, _v, _a;

		/**
		* Construct without alpha (which defaults to 1)
		*/
		public ColorHSV(float h, float s, float v)
		{
			SetAllVariables(h,  s, v, 1f);
		}
		
		/**
		* Construct without alpha or s or v (which defaults to 1)
		*/
		public ColorHSV(float h)
		{
			var v = (Mathf.Cos(h * Mathf.Deg2Rad * 3f) - 1f) * 0.1f + 1f;
			SetAllVariables(h, 1f, v, 1f);
		}

		public ColorHSV(float h, float s, float v, float a)
		{
			SetAllVariables(h, s, v, a);
		}

		/**
		* Construct without alpha (which defaults to 1)
		*/
		private void SetAllVariables(float h, float s, float v, float a)
		{
			if (h < 0f)
			{
				h += 360;
			}
			h %= 360f;
			_h = h;
			_s = s;
			_v = v;
			_a = a;
		}

		/**
		* Create from an RGBA color object
		*/
		public ColorHSV(Color color)
		{
			var min = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
			var max = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
			var delta = max - min;

			// value is our max color
			_v = max;

			// saturation is percent of max
			if (!Mathf.Approximately(max, 0))
			{
				_s = delta / max;
			}
			else
			{
				// all colors are zero, no saturation and hue is undefined
				_s = 0;
				_h = -1;
				return;
			}

			// grayscale image if min and max are the same
			if(Mathf.Approximately(min, max))
			{
				_v = max;
				_s = 0;
				_h = -1;
				return;
			}

			// hue depends which color is max (this creates a rainbow effect)
			if (Mathf.Approximately(color.r, max))
			{
				// between yellow  magenta
				_h = (color.g - color.b) / delta;
			}
			else if (Mathf.Approximately(color.g, max))
			{
				// between cyan  yellow
				_h = 2 + (color.b - color.r) / delta;
			}
			else
			{
				// between magenta  cyan
				_h = 4 + ( color.r - color.g ) / delta;
			}

			// turn hue into 0-360 degrees
			_h *= 60;
			if (_h < 0)
			{
				_h += 360;
			}
		}

		/**
		* Return an RGBA color object
		*/
		public Color ToColor()
		{
			// no saturation, we can return the value across the board (grayscale)
			if (_s == 0)
			{
				return new Color(_v, _v, _v, _a);
			}

			// which chunk of the rainbow are we in?
			var sector = _h / 60f;

			// split across the decimal (ie 3.87 into 3 and 0.87)
			int i = (int)Mathf.Floor(sector);
			var f = sector - i;

			var v = _v;
			var p = v * ( 1 - _s );
			var q = v * ( 1 - _s * f );
			var t = v * ( 1 - _s * ( 1 - f ) );

			// build our rgb color
			Color color = new (0, 0, 0, _a);

			switch(i)
			{
				case 0:
					color.r = v;
					color.g = t;
					color.b = p;
					break;
				case 1:
					color.r = q;
					color.g = v;
					color.b = p;
					break;
				case 2:
					color.r  = p;
					color.g  = v;
					color.b  = t;
					break;
				case 3:
					color.r  = p;
					color.g  = q;
					color.b  = v;
					break;
				case 4:
					color.r  = t;
					color.g  = p;
					color.b  = v;
					break;
				default:
					color.r  = v;
					color.g  = p;
					color.b  = q;
					break;
			}

			return color;
		}

		/**
		* Format nicely
		*/
		public override string ToString()
		{
			return String.Format("h: {0:0.00}, s: {1:0.00}, v: {2:0.00}, a: {3:0.00}", _h, _s, _v, _a);
		}

		public static Color GenerateRandomRbgColor()
		{
			ColorHSV hsv = new (UnityEngine.Random.Range(0.0f, 360.0f), 1.0f, 1.0f);
			return hsv.ToColor();
		}

		public static Color GenerateRandomRbgColor(float saturation)
		{
			ColorHSV hsv = new (UnityEngine.Random.Range(0.0f, 360.0f), saturation, 1.0f);
			return hsv.ToColor();
		}

		public static GlobalTypes.Color GetClosest(float hue)
		{
			if (hue < 0f)
			{
				hue += 360;
			}

			hue += 30f;
			hue %= 360f;
			hue /= 60f;
			var intHue = Mathf.FloorToInt(hue);
			return (GlobalTypes.Color)intHue;
		}
	}
}
