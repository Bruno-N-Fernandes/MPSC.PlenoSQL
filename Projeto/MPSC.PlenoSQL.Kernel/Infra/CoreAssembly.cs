﻿using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace MPSC.PlenoSQL.Kernel.Infra
{
	public static class CoreAssembly
	{
		public static readonly Assembly ThisAssembly = Assembly.GetAssembly(typeof(CoreAssembly));
		public static readonly Assembly CallingAssembly = Assembly.GetCallingAssembly();
		public static readonly Assembly EntryAssembly = Assembly.GetEntryAssembly();
		public static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();
		public static readonly Assembly Reference = EntryAssembly;

		private static readonly Version Version = Reference.GetName().Version;
		private static readonly FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Reference.Location);

		public static readonly String ApplicationVersion = Application.ProductVersion;
		public static readonly String AssemblyVersion = Version.ToString();
		public static readonly String ProductVersion = FileVersionInfo.ProductVersion;
		public static readonly String FileVersion = FileVersionInfo.FileVersion;
		public static String VersionString { get { return String.Format("{0} ({1} - {2})", CoreAssembly.AssemblyVersion, CoreAssembly.ProductVersion, CoreAssembly.FileVersion); } }
	}
}
