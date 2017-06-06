﻿//
//  EverlookPreferencesElements.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2016 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using Gtk;
using UIElement = Gtk.Builder.ObjectAttribute;

// ReSharper disable UnassignedReadonlyField
#pragma warning disable 649
#pragma warning disable 1591

namespace Everlook.UI
{
	public partial class EverlookPreferences
	{
		[UIElement] private readonly Dialog NewGamePathDialog;
		[UIElement] private readonly Entry AliasEntry;
		[UIElement] private readonly ComboBox GameVersionCombo;
		[UIElement] private readonly FileChooserButton PathChooser;

		[UIElement] private readonly TreeView GamePathSelectionTreeView;
		[UIElement] private readonly ListStore GamePathListStore;
		[UIElement] private readonly Button AddPathButton;
		[UIElement] private readonly Button RemovePathButton;

		[UIElement] private readonly ColorButton ViewportColourButton;
		//[UIElement] private readonly CheckButton ShowUnknownFilesCheckButton;

		[UIElement] private readonly FileChooserButton DefaultExportDirectoryFileChooserButton;
		[UIElement] private readonly ComboBox DefaultModelExportFormatComboBox;
		[UIElement] private readonly ComboBox DefaultImageExportFormatComboBox;
		[UIElement] private readonly ComboBox DefaultAudioExportFormatComboBox;
		[UIElement] private readonly CheckButton KeepDirectoryStructureCheckButton;

		[UIElement] private readonly CheckButton SendStatsCheckButton;
	}
}