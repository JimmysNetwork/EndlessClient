﻿//miscellaneous extension functions and some constant values are defined here

using System;
using Microsoft.Xna.Framework;

namespace EOLib
{
	public static class Hashes
	{
		public static int stupid_hash(int i)
		{
			++i;
			return 110905 + (i % 9 + 1) * ((11092004 - i) % ((i % 11 + 1) * 119)) * 119 + i % 2004;
		}
	}

	public static class ArrayExtension
	{
		public static T[] SubArray<T>(this T[] arr, int offset, int count)
		{
			T[] ret = new T[count];

			if (count == 1)
				ret[0] = arr[offset];

			for (int i = offset; i < offset + count; ++i)
				ret[i - offset] = arr[i];

			return ret;
		}
	}

	public static class DateTimeExtension
	{
		public static int ToEOTimeStamp(this DateTime dt)
		{
			return dt.Hour * 360000 + dt.Minute * 6000 + dt.Second * 100 + dt.Millisecond / 10;
		}
	}

	public static class RectExtension
	{
		/// <summary>
		/// Returns a new rectangle with the position set to the specified location
		/// </summary>
		/// <param name="orig"></param>
		/// <param name="loc">New position for the rectangle</param>
		/// <returns></returns>
		public static Rectangle SetPosition(this Rectangle orig, Vector2 loc)
		{
			return new Rectangle((int)loc.X, (int)loc.Y, orig.Width, orig.Height);
		}
	}
	
	public static class Constants
	{
		public const int ChatBubbleTimeout = 5000;
		public const int ResponseTimeout = 5000;
		public const int ResponseFileTimeout = 10000;

		public const byte MajorVersion = 0;
		public const byte MinorVersion = 0;
		public const byte ClientVersion = 28;

		public const string Host = "127.0.0.1";
		public const int Port = 8078;

		public const string ItemFilePath = "pub/dat001.eif";
		public const string NPCFilePath = "pub/dtn001.enf";
		public const string SpellFilePath = "pub/dsl001.esf";
		public const string ClassFilePath = "pub/dat001.ecf";

		public const string DataFilePath = "data/";

		public const byte ViewLength = 16;

		public const int NPCDropProtectionSeconds = 30;
		public const int PlayerDropProtectionSeconds = 5;
		public const int LockerMaxSingleItemAmount = 200;

		public const string LogFilePath = "log/debug.log";
		public const string LogFileFmt  = "log/{0}-debug.log";
	}

	public static class ConfigStrings
	{
		public const string Connection = "CONNECTION";
		public const string Host = "Host";
		public const string Port = "Port";

		public const string Version = "VERSION";
		public const string Major = "Major";
		public const string Minor = "Minor";
		public const string Client = "Client";

		public const string Settings = "SETTINGS";
		public const string ShowShadows = "ShowShadows";
		public const string ShowTransition = "ShowTransition";
		public const string EnableLogging = "EnableLogging";
		public const string Music = "Music";
		public const string Sound = "Sound";
		public const string ShowBaloons = "ShowBaloons";

		public const string Custom = "CUSTOM";
		public const string NPCDropProtectTime = "NPCDropProtectTime";
		public const string PlayerDropProtectTime = "PlayerDropProtectTime";

		public const string LANGUAGE = "LANGUAGE";
		public const string Language = "Language";

		public const string Chat = "CHAT";
		public const string Filter = "Filter";
		public const string FilterAll = "FilterAll";
		public const string HearWhisper = "HearWhisper";
		public const string Interaction = "Interaction";
		public const string LogChat = "LogChat";
	}
}
