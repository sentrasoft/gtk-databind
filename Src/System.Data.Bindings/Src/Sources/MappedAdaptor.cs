// MappedAdaptor.cs created with MonoDevelop
// User: matooo at 7:59 PM 2/23/2008
//
// To change standard headers go to Edit->Preferences->Coding->Standard Headers
//

using System;
using System.Data.Bindings.Cached;

namespace System.Data.Bindings
{
	/// <summary>
	/// Adaptor suited to point across hierarchy by relating to mapped properties
	/// </summary>
	public class MappedAdaptor : Adaptor
	{
		/// <summary>
		/// Resolves final target for Adaptor 
		/// </summary>
		/// <returns>
		/// Object of the final target this adaptor is pointing to <see cref="System.Object"/>
		/// </returns>
		protected override object DoGetFinalTarget (out bool aCallControl)
		{
			aCallControl = false;
			if (GetDefaultProperty() == null)
				return (null);
			object res = base.DoGetFinalTarget (out aCallControl);
			finalTarget.Target = null;
			if (res == null)
				return (null);
			// Resolve forward if mapped property is adaptor
			object tgt = null;
			if (GetDefaultProperty() == null)
				return (null);
			if (CachedProperty.UncachedGetValue (res, GetDefaultProperty().Name, out tgt) == false)
				tgt = ConnectionProvider.ResolveTargetForObject (tgt);
			// set checks
			if (DataSourceType != null) {
				if (TypeValidator.IsCompatible(tgt.GetType(), DataSourceType) == true)
					return (tgt);
				return (null);
			}
			else
				return (tgt);
		}
		
		/// <summary>
		/// Created MappedAdaptor
		/// </summary>
		public MappedAdaptor()
		{
		}

		/// <summary>
		/// Created MappedAdaptor
		/// </summary>
		public MappedAdaptor (string aMappings)
			: base ()
		{
			Mappings = aMappings;
		}
	}
}
