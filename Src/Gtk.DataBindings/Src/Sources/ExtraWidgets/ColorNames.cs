
using System;

namespace Gtk.DataBindings
{
	/// <summary>
	/// 
	/// </summary>
	public delegate string ResolveColorNameEvent (Cairo.Color aColor);
	
	/// <summary>
	/// 
	/// </summary>
	public static class ColorNames
	{
		private static event ResolveColorNameEvent resolveColor = null;
		public static event ResolveColorNameEvent ResolveColor {
			add { resolveColor += value; }
			remove { resolveColor -= value; }
		}
			
		private static string BasicColorNameResolver (Cairo.Color aColor)
		{
			return (string.Format("A:{0} R:{1} G:{2} B:{3}", aColor.A, aColor.R, aColor.G, aColor.B));
		}
		
		public static string GetName (Cairo.Color aColor)
		{
			if (resolveColor != null)
				return (resolveColor (aColor));
			return (BasicColorNameResolver (aColor));
		}
	}
}
