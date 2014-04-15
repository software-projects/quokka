#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.IO;
using System.Xml.Serialization;
using Quokka.Diagnostics;
using Quokka.Stomp.Internal;

namespace Quokka.Stomp
{
	public static class StompFrameExtensions
	{
		public static bool GetBoolean(this StompFrame frame, string header, bool defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			bool value;
			if (!bool.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public static int GetInt32(this StompFrame frame, string header, int defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			int value;
			if (!int.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public static long GetInt64(this StompFrame frame, string header, long defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			long value;
			if (!long.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public static void SetExpires(this StompFrame frame, TimeSpan timeSpan)
		{
			var expiresAt = DateTimeOffset.Now + timeSpan;
			frame.SetExpires(expiresAt);
		}
		
		public static void SetExpires(this StompFrame frame, DateTimeOffset expiresAt)
		{
			var text = ExpiresTextUtils.ToString(expiresAt);
			frame.Headers[StompHeader.NonStandard.Expires] = text;
		}

		public static bool IsExpired(this StompFrame frame)
		{
			return frame.IsExpiredAt(DateTimeOffset.Now);
		}

		public static bool IsExpiredAt(this StompFrame frame, DateTimeOffset dateTime)
		{
			var expiresAtText = frame.Headers[StompHeader.NonStandard.Expires];
			if (string.IsNullOrEmpty(expiresAtText))
			{
				return false;
			}

			string dateTimeText = ExpiresTextUtils.ToString(dateTime);
			return frame.IsExpiredAt(dateTimeText);
		}

		public static bool IsExpiredAt(this StompFrame frame, string dateTimeText)
		{
			var expiresAtText = frame.Headers[StompHeader.NonStandard.Expires];
			if (string.IsNullOrEmpty(expiresAtText))
			{
				return false;
			}

			return ExpiresTextUtils.Compare(expiresAtText, dateTimeText) < 0;
		}

		public static void Serialize(this StompFrame frame, Type type, object payload)
		{
			Verify.ArgumentNotNull(type, "type");
			Verify.ArgumentNotNull(payload, "payload");
			var serializer = new XmlSerializer(type);
			// the type name includes the full name, and the assembly name without any version info
			var typeName = type.FullName + "," + type.Assembly.GetName().Name;
			frame.Body = new MemoryStream();
			serializer.Serialize(frame.Body, payload);
			frame.Headers[StompHeader.ContentType] = "application/xml";
			frame.Headers[StompHeader.ContentLength] = frame.Body.Length.ToString();
			frame.Headers[StompHeader.NonStandard.ClrType] = typeName;
		}

		public static void Serialize(this StompFrame frame, object payload)
		{
			Verify.ArgumentNotNull(payload, "payload");
			frame.Serialize(payload.GetType(), payload);
		}

		public static bool CanDeserialize(this StompFrame frame)
		{
			return frame.Headers[StompHeader.ContentType] == "application/xml"
			       && frame.Headers[StompHeader.NonStandard.ClrType] != null
			       && frame.Body != null;
		}

		public static object Deserialize(this StompFrame frame)
		{
			if (frame.Headers[StompHeader.ContentType] != "application/xml")
			{
				throw new InvalidOperationException("Cannot deserialize: content-type:" + frame.Headers[StompHeader.ContentType]);
			}
			if (frame.Body == null)
			{
				throw new InvalidOperationException("Cannot deserialize: body missing");
			}
			var typeName = frame.Headers[StompHeader.NonStandard.ClrType];
			if (typeName == null)
			{
				throw new InvalidOperationException("Cannot deserialize: no clr-type specified");
			}

			var type = Type.GetType(typeName, false);
			if (type == null)
			{
				throw new InvalidOperationException("Cannot deserialize: cannot find clr-type:" + typeName);
			}

			var serializer = new XmlSerializer(type);
			frame.Body.Seek(0, SeekOrigin.Begin);
			try
			{
				var obj = serializer.Deserialize(frame.Body);
				return obj;
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}
				// sorry for catching an Exception, but the documentation does not say what
				// exceptions this method throws
				throw new InvalidOperationException("Cannot deserialize type " + typeName + ": " + ex.Message, ex);
			}
		}
	}
}