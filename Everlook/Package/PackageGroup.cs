﻿//
//  PackageGroup.cs
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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Everlook.Package
{
	/// <summary>
	/// Package group. Handles a group of packages as one cohesive unit, where files residing in the
	/// same path in different packages can override one another in order to provide updates.
	///
	/// Files and packages are registered on an alphabetical first-in last-out basis.
	/// </summary>
	public sealed class PackageGroup : IDisposable
	{
		/// <summary>
		/// Gets the name of the package group.
		/// </summary>
		/// <value>The name of the group.</value>
		public string GroupName
		{
			get;
			private set;
		}

		/// <summary>
		/// The root package directory.
		/// </summary>
		private string RootPackageDirectory;

		/// <summary>
		/// The packages handled by this package group.
		/// </summary>
		private readonly List<PackageInteractionHandler> Packages = new List<PackageInteractionHandler>();

		/// <summary>
		/// The package listfiles. 
		/// Key: The package name.
		/// Value: A list of all files present in the package.
		/// </summary>
		public readonly Dictionary<string, List<string>> PackageListfiles = new Dictionary<string, List<string>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Everlook.Package.PackageGroup"/> class.
		/// </summary>
		public PackageGroup(string GroupName, string InRootPackageDirectory)
		{
			if (String.IsNullOrEmpty(GroupName))
			{
				throw new ArgumentNullException(GroupName, "A package group must be provided with a name.");
			}

			this.RootPackageDirectory = InRootPackageDirectory;

			this.GroupName = GroupName;
			
			// Grab all packages in the game directory
			List<string> PackagePaths = Directory.EnumerateFiles(RootPackageDirectory, "*.*", SearchOption.AllDirectories)
				.OrderBy(a => a)
				.Where(s => s.EndsWith(".mpq") || s.EndsWith(".MPQ"))
				.ToList();

			PackagePaths.Sort();

			foreach (string PackagePath in PackagePaths)
			{
				try
				{
					Packages.Add(new PackageInteractionHandler(PackagePath));				
				}
				catch (FileLoadException fex)
				{
					Console.WriteLine(String.Format("FileLoadException for package \"{0}\": {1}", PackagePath, fex.Message));
				}
			}

			foreach (PackageInteractionHandler Package in Packages)
			{
				PackageListfiles.Add(Package.PackageName, Package.GetFileList());
			}
		}

		/// <summary>
		/// Extracts a file from a specific package in the package group. If the file does not exist in
		/// the package referenced in <paramref name="fileReference"/>, this method returned will return null.
		/// </summary>
		/// <returns>The unversioned file or null.</returns>
		/// <param name="fileReference">File reference.</param>
		public byte[] ExtractUnversionedFile(ItemReference fileReference)
		{
			PackageInteractionHandler Package = GetPackageByName(fileReference.PackageName);
			if (Package != null)
			{
				return Package.ExtractReference(fileReference);
			}

			return null;
		}

		/// <summary>
		/// Extracts a file from the package group. This method returns the most recently overridden version
		/// of the specified file with no regard for the origin package. The returned file may originate from the 
		/// package referenced in the <paramref name="fileReference"/>, or it may originate from a patch package.
		///
		/// If the file does not exist in any package, this method will return null.
		/// </summary>
		/// <returns>The file or null.</returns>
		/// <param name="fileReference">File reference.</param>
		public byte[] ExtractFile(ItemReference fileReference)
		{
			for (int i = Packages.Count - 1; i >= 0; --i)
			{
				if (Packages[i].ContainsFile(fileReference))
				{
					return Packages[i].ExtractReference(fileReference);
				}
			}
			return null;
		}

		/// <summary>
		/// Gets a package handler by the name of the package.
		/// </summary>
		/// <returns>The package by name.</returns>
		/// <param name="PackageName">Package name.</param>
		private PackageInteractionHandler GetPackageByName(string PackageName)
		{
			foreach (PackageInteractionHandler Package in Packages)
			{
				if (Package.PackageName == PackageName)
				{
					return Package;
				}
			}

			return null;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Everlook.Package.PackageGroup"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Everlook.Package.PackageGroup"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Everlook.Package.PackageGroup"/> in an unusable state. After
		/// calling <see cref="Dispose"/>, you must release all references to the <see cref="Everlook.Package.PackageGroup"/>
		/// so the garbage collector can reclaim the memory that the <see cref="Everlook.Package.PackageGroup"/> was occupying.</remarks>
		public void Dispose()
		{
			foreach (PackageInteractionHandler Package in Packages)
			{
				Package.Dispose();
			}
		}
	}
}

