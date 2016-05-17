
using System;
using System.ComponentModel;
using Gdk;
using Gtk;
using Gtk.DataBindings;
using System.Data.Bindings;

namespace Gtk.ExtraWidgets
{
	/// <summary>
	/// 
	/// </summary>
	[ToolboxItem (true)]
	[Category ("Extra Gtk Widgets")]
//	[GtkWidgetFactoryProvider ("cairocolor", "DefaultFactoryCreate")]
//	[GtkWidgetFactoryProvider ("simplecairocolor", "DefaultFactoryCreate")]
//	[GtkTypeWidgetFactoryProvider ("cairocolorhandler", "DefaultFactoryCreate", typeof (Cairo.Color))]
	public class ColorLabel : CellDrawingArea
	{
		/// <summary>
		/// Registered factory creation method
		/// </summary>
		/// <param name="aArgs">
		/// Arguments <see cref="FactoryInvocationArgs"/>
		/// </param>
		/// <returns>
		/// Result widget <see cref="IAdaptableControl"/>
		/// </returns>
/*		public static IAdaptableControl DefaultFactoryCreate (FactoryInvocationArgs aArgs)
		{
			ColorLabel wdg = new ColorLabel();
			if (aArgs.InvokationHandler == "simplecairocolor")
			    wdg.NameVisible = false;
			wdg.Mappings = aArgs.PropertyName;
			return (wdg);
		}*/
		
		private DrawingCellEditText name = new DrawingCellEditText();
		private DrawingCellGdkColorFrame colorbox = new DrawingCellGdkColorFrame();
		
		private static string x_SpecialName = "application/x-dotnet-color";
		
		private static TargetEntry[] validTargets = ClipboardHelper.GetTextTargetsWithSpecial (x_SpecialName, (int) TransferDataType.X_Special);
		protected static TargetEntry[] ValidTargets {
			get { return (validTargets); }
		}

		private bool smallFonts = false;
		public bool SmallFonts {
			get { return (smallFonts); }
			set {
				if (smallFonts == value)
					return;
				smallFonts = value;
				ResetLayout();
				QueueDraw();
				OnPropertyChanged ("SmallFonts");
			}
		}
		
		public void SetText()
		{
			if (ResolveColorNames == false)
				name.Text = colorName;
			else
				name.Text = ColorNames.GetName (Color);
		}
		
		private bool resolveColorNames = false;
		/// <value>
		/// Specifies if color names should be resolved trough responsible event registered in ColorNames
		/// </value>
		public bool ResolveColorNames {
			get { return (resolveColorNames); }
			set {
				if (resolveColorNames == value)
					return;
				resolveColorNames = value;
				ResetLayout();
				QueueDraw();
				OnPropertyChanged ("ResolveColorNames");
			}
		}
			
		private string colorName = "";
		/// <value>
		/// Specifies name of the color
		/// </value>
		public string ColorName {
			get { return (colorName); }
			set {
				if (colorName == value)
					return;
				colorName = value;
				ResetLayout();
				QueueDraw();
				OnPropertyChanged ("ColorName");
			}
		}
		
		private Cairo.Color color = new Cairo.Color (0, 0, 0);
		/// <value>
		/// Specifies destination url
		/// </value>
		public Cairo.Color Color {
			get { return (color); }
			set {
				if (color.Equals(value) == true)
					return;
				color = value;
				if (ResolveColorNames == true)
					name.Text = ColorNames.GetName (Color);
				colorbox.FillColor = Color;
				TooltipText = color.GetGdkColor().ToHtmlColor();
				ResetLayout();
				QueueDraw();
				OnPropertyChanged ("Color");
			}
		}

		private bool nameVisible = true;
		/// <value>
		/// Specifies if link icon is visible or not
		/// </value>
		public bool NameVisible {
			get { return (nameVisible); }
			set {
				if (nameVisible == value)
					return;
				nameVisible = value;
				ResetLayout();
				QueueDraw();
				OnPropertyChanged ("NameVisible");
			}
		}
		
		private event ColorClickedEvent colorClicked = null;
		/// <summary>
		/// Event passed when link is clicked
		/// </summary>
		public event ColorClickedEvent ColorClicked {
			add { colorClicked += value; }
			remove { colorClicked -= value; }
		}
	
		/// <summary>
		/// Executes link click registered delegates
		/// </summary>
		protected void OnColorClicked()
		{
			if (colorClicked != null)
				colorClicked (this, new ColorClickedEventArgs (Color));
		}
		
		/// <summary>
		/// Resets layout of the widget
		/// </summary>
		protected void ResetLayout()
		{
			if (ResolveColorNames == false)
				name.Text = colorName;
			else
				name.Text = ColorNames.GetName (Color);
			if (SmallFonts == true)
				name.Text = "<small>" + name.Text + "</small>";
			double w, h;
			colorbox.Visible = true;
			name.Visible = (NameVisible == true);
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
				OnColorClicked();
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
			aData.Set (aData.Target, 8, System.Text.Encoding.UTF8.GetBytes (Color.GetGdkColor().ToHtmlColor()),
			           System.Text.Encoding.UTF8.GetBytes (Color.GetGdkColor().ToHtmlColor()).Length);
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
			args.SelectionData.Text = Color.GetGdkColor().ToHtmlColor();
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

		public ColorLabel()
		{
			colorbox.BorderVisible = true;
			colorbox.BorderWidth = 2;
			colorbox.BorderColor = new Cairo.Color (0, 0, 0);
			colorbox.FillVisible = true;
			colorbox.FillColor = Color;
			HandlesPrelight = true;
			GtkResources.RegisterDefaultResourceHandler();
//			icon.StateResolving = ValueResolveMethod.FromOwner;
			Spacing = 4;
			MainBox.PackEnd (colorbox, false);
			MainBox.PackEnd (name, true);

			this.Realized += delegate(object sender, EventArgs e) {
				GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
			};

			Gtk.Drag.SourceSet (this, Gdk.ModifierType.Button1Mask, ValidTargets, DragAction.Copy);
			DragBegin += HandleDragBegin;
			DragDataGet += HandleDragDataGet;
		}
	}
}
