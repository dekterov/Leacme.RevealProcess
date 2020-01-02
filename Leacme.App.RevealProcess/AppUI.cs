// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Humanizer;
using Leacme.Lib.RevealProcess;

namespace Leacme.App.RevealProcess {

	public class AppUI {

		private StackPanel rootPan = (StackPanel)Application.Current.MainWindow.Content;
		private Library lib = new Library();

		public AppUI() {

			var hzh = App.HorizontalStackPanel;
			hzh.HorizontalAlignment = HorizontalAlignment.Center;

			var blr = App.TextBlock;
			blr.TextAlignment = TextAlignment.Center;
			blr.Text = "Local Machine Processes:";

			var blt = App.Button;
			blt.Content = "Refresh";

			var kp = App.HorizontalFieldWithButton;
			kp.holder.Margin = new Thickness(0);
			kp.label.Text = "Kill Process by ID:";
			kp.button.Content = "Kill";

			var pg = App.DataGrid;
			pg.SetValue(DataGrid.WidthProperty, AvaloniaProperty.UnsetValue);
			pg.Height = App.Current.MainWindow.Height - 100;
			pg.Items = lib.GetCurrentProcesses();

			pg.AutoGeneratingColumn += (z, zz) => {
				zz.Column.Header = ((string)zz.Column.Header).Humanize(LetterCasing.Title);
				if (((string)zz.Column.Header).Equals("CPU")) {
					zz.Column.Header = "CPU %";
				}

				if (((string)zz.Column.Header).Equals("Id")) {
					zz.Column.Header = "Process ID";
				}
			};

			App.Current.MainWindow.PropertyChanged += (z, zz) => {
				if (zz.Property.Equals(Window.HeightProperty)) {
					pg.Height = App.Current.MainWindow.Height - 100;
				}
			};

			blt.Click += (z, zz) => pg.Items = lib.GetCurrentProcesses();
			kp.button.Click += (z, zz) => {
				try {
					lib.KillProcessById(int.Parse(kp.field.Text));
				} catch { }
				kp.field.Text = "";
				pg.Items = lib.GetCurrentProcesses();
			};

			hzh.Children.AddRange(new List<IControl> { blr, blt, new Control { Width = 30 }, kp.holder });
			rootPan.Children.AddRange(new List<IControl> { hzh, pg });
		}
	}
}