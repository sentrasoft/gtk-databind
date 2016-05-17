
using System;
using System.ComponentModel;
using Gdk;
using Gtk;
using Gtk.DataBindings;
using System.Data.Bindings;
using GLib;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
	public class LinkLabel : CellDrawingArea
	{
		static GType type;

		private DrawingCellEditText link = new DrawingCellEditText();
		private DrawingCellActivePixbuf icon = new DrawingCellActivePixbuf();
		
		private static string x_SpecialName = "application/x-dotnet-link";
		
		private static TargetEntry[] validTargets = ClipboardHelper.GetTextTargetsWithSpecial (x_SpecialName, (int) TransferDataType.X_Special);
		protected static TargetEntry[] ValidTargets {
			get { return (validTargets); }
		}
		
		private string linkText = "";
		/// <value>
		/// Specifies text displayed in Label
		/// </value>
		public string LinkText {
			get { return (linkText); }
			set {
				if (linkText == value)
					return;
				linkText = value;
				link.Text = "<span foreground=\"blue\"><u>" + DisplayText + "</u></span>";
				ResetLayout();
				OnPropertyChanged ("LinkText");
			}
		}
		
		private string linkURL = "";
		/// <value>
		/// Specifies destination url
		/// </value>
		public string LinkURL {
			get { return (linkURL); }
			set {
				if (linkURL == value)
					return;
				linkURL = value;
				TooltipText = linkURL;
				OnPropertyChanged ("PROPERTYNAME");
			}
		}

		/// <value>
		/// Specifies text which is displayed
		/// </value>
		[Browsable (false)]
		protected string DisplayText {
			get { return ((LinkText != "") ? LinkText : LinkURL); }
		}
		
		/// <value>
		/// Specifies text which is displayed
		/// </value>
		[Browsable (false)]
		protected string TargetUrl {
			get { return ((LinkURL != "") ? LinkURL : LinkText); }
		}
		
		private bool iconVisible = true;
		/// <value>
		/// Specifies if link icon is visible or not
		/// </value>
		public bool IconVisible {
			get { return (iconVisible); }
			set {
				if (iconVisible == value)
					return;
				iconVisible = value;
				ResetLayout();
				OnPropertyChanged ("IconVisible");
			}
		}
		
		/// <value>
		/// Specifies pixbuf shown as icon
		/// </value>
		public Gdk.Pixbuf Pixbuf {
			get { return (icon.Pixbuf); }
			set {
				if (icon.Pixbuf == value)
					return;
				icon.Pixbuf = value;
				ResetLayout();
				OnPropertyChanged ("Pixbuf");
			}
		}
		
		private event LinkClickedEvent linkClicked = null;
		/// <summary>
		/// Event passed when link is clicked
		/// </summary>
		public event LinkClickedEvent LinkClicked {
			add { linkClicked += value; }
			remove { linkClicked -= value; }
		}
	
		/// <summary>
		/// Executes link click registered delegates
		/// </summary>
		protected void OnLinkClicked()
		{
			if (linkClicked != null)
				linkClicked (this, new LinkClickedEventArgs (LinkText, LinkURL));
		}
		
		/// <summary>
		/// Resets layout of the widget
		/// </summary>
		protected void ResetLayout()
		{
			double w, h;
			icon.Visible = (IconVisible == true) && (Pixbuf != null);
			MainBox.GetSize (out w, out h);
			if (Allocation.Width < w)
				WidthRequest = System.Convert.ToInt32 (w);
			if (Allocation.Height < h)
				HeightRequest = System.Convert.ToInt32 (h);
			CellsChanged();
		}

		/// <summary>
		/// Mothod handling mouse button press event
		/// </summary>
		/// <param name="evnt">
		/// Arguments <see cref="Gdk.EventButton"/>
		/// </param>
		/// <returns>
		/// true if handled, false if not <see cref="System.Boolean"/>
		/// </returns>
		protected override bool OnButtonPressEvent (Gdk.EventButton evnt)
		{
			if (this.IsSensitive() == true) {
				OnLinkClicked();
				return (true);
			}
			return base.OnButtonPressEvent (evnt);
		}

		/// <summary>
		/// Fills selection with data
		/// </summary>
		/// <param name="aData">
		/// Data <see cref="SelectionData"/>
		/// </param>
		/// <param name="aInfo">
		/// Type <see cref="System.UInt32"/>
		/// </param>
		private void FillSelectionData (SelectionData aData, uint aInfo) 
		{
			aData.Set (aData.Target, 8, System.Text.Encoding.UTF8.GetBytes (TargetUrl),
			           System.Text.Encoding.UTF8.GetBytes (TargetUrl).Length);
		}
		
		/// <summary>
		/// Handles drag begin
		/// </summary>
		/// <param name="o">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="DragBeginArgs"/>
		/// </param>
		protected void HandleDragBegin(object o, DragBeginArgs args)
		{
			Gdk.Pixbuf dragpic = GetDragPixbuf();
			Gtk.Drag.SetIconPixbuf (args.Context, dragpic, 0, 0);
		}

		/// <summary>
		/// Starts filling data into selection
		/// </summary>
		/// <param name="o">
		/// Object <see cref="System.Object"/>
		/// </param>
		/// <param name="args">
		/// Arguments <see cref="DragDataGetArgs"/>
		/// </param>
		protected void HandleDragDataGet(object o, DragDataGetArgs args)
		{
			for (int i=0; i<ValidTargets.Length; i++)
				FillSelectionData (args.SelectionData, (i<ValidTargets.Length-1) ? (uint) 0 : (uint) 1);
			args.SelectionData.Text = TargetUrl;
		}

		/// <summary>
		/// Creates new box
		/// </summary>
		/// <returns>
		/// Returns horizontal cell box <see cref="DrawingCellBox"/>
		/// </returns>
		protected override DrawingCellBox CreateBox ()
		{
			return (new DrawingCellHBox());
		}

		public LinkLabel()
		{
			HandlesPrelight = true;
			GtkResources.RegisterDefaultResourceHandler();
			icon.StateResolving = ValueResolveMethod.FromOwner;
			Spacing = 2;
			MainBox.PackEnd (icon, false);
			MainBox.PackEnd (link, true);

			this.Realized += delegate(object sender, EventArgs e) {
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
			};

			Gtk.Drag.SourceSet (this, Gdk.ModifierType.Button1Mask, ValidTargets, DragAction.Copy);
			DragBegin += HandleDragBegin;
			DragDataGet += HandleDragDataGet;
		}
	}
}
