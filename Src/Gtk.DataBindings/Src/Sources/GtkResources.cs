
using System;
using System.Data.Bindings;

namespace Gtk
{
	/// <summary>
	/// Takes care for Gtk resources
	/// </summary>
	public class GtkResources
	{
		private static bool pixbufLoaderRegistered = false;
		
		/// <summary>
		/// Loads Gdk.Pixbuf resource
		/// </summary>
		/// <param name="aName">
		/// Picture resource name <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// Picture object <see cref="System.Object"/>
		/// </returns>
		protected static object LoadPictureResource (string aName)
		{
			return (new Gdk.Pixbuf (AssemblyEngine.EntryAssembly.LoadBinaryResource(aName)));
		}

		/// <summary>
		/// Registers default resource handler for picture resources
		/// </summary>
		public static void RegisterDefaultResourceHandler()
		{
			if (pixbufLoaderRegistered == true)
				return;
			PictureResourceStore.LoadPictureResource += LoadPictureResource;
			pixbufLoaderRegistered = true;
		}
	}
}
