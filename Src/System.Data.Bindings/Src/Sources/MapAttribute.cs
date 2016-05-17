// MapAttribute.cs created with MonoDevelop
// User: matooo at 1:06 PM 2/24/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;

namespace System.Data.Bindings
{
	/// <summary>
	/// MapAttribute specifies fact that some property is mapped and where is mapped
	/// </summary>
	[AttributeUsage (AttributeTargets.Property, AllowMultiple=false)]
	public class MapAttribute : Attribute
	{
		private string mapsTo = "";
		/// <summary>
		/// Specifies property name which is mapped by this attribute
		/// </summary>
		public string MapsTo {
			get { return (mapsTo); }
		}

		/// <summary>
		/// Creates MapAttribute
		/// </summary>
		/// <param name="aMapsTo">
		/// Name of the property <see cref="System.String"/>
		/// </param>
		public MapAttribute (string aMapsTo)
		{
			mapsTo = aMapsTo;
			if (MapsTo == "")
				throw new ExceptionMapAttributeNonSpecified();
		}
	}
}
