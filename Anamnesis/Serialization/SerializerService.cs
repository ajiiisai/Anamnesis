﻿// © Anamnesis.
// Licensed under the MIT license.

namespace Anamnesis.Serialization
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Anamnesis.Serialization.Converters;
    using Anamnesis.Services;

    public class SerializerService : ServiceBase<SerializerService>
	{
		public static JsonSerializerOptions Options = new JsonSerializerOptions();

		static SerializerService()
		{
			Options.WriteIndented = true;
			Options.PropertyNameCaseInsensitive = false;
			Options.IgnoreNullValues = true;
			Options.AllowTrailingCommas = true;
			Options.ReadCommentHandling = JsonCommentHandling.Skip;

			Options.Converters.Add(new JsonStringEnumConverter());
			Options.Converters.Add(new Color4Converter());
			Options.Converters.Add(new ColorConverter());
			Options.Converters.Add(new QuaternionConverter());
			Options.Converters.Add(new VectorConverter());
			Options.Converters.Add(new IItemConverter());
			Options.Converters.Add(new IDyeConverter());
			Options.Converters.Add(new ItemCategoriesConverter());
		}

		public static string Serialize(object obj)
		{
			return JsonSerializer.Serialize(obj, Options);
		}

		public static void SerializeFile(string path, object obj)
		{
			string json = Serialize(obj);
			File.WriteAllText(path, json);
		}

		public static T DeserializeFile<T>(string path)
			where T : new()
		{
			string json = File.ReadAllText(path);
			json = json.Replace("\r", Environment.NewLine);
			T? result = JsonSerializer.Deserialize<T>(json, Options);

			if (result == null)
				throw new Exception("Failed to deserialize object");

			return result;
		}

		public static T Deserialize<T>(string json)
			where T : notnull
		{
			T? result = JsonSerializer.Deserialize<T>(json, Options);

			if (result == null)
				throw new Exception("Failed to deserialize object");

			return result;
		}

		public static object Deserialize(string json, Type type)
		{
			object? result = JsonSerializer.Deserialize(json, type, Options);

			if (result == null)
				throw new Exception("Failed to deserialize object");

			return result;
		}

		public static T? DeserializeUrl<T>(string url)
			where T : new()
		{
			try
			{
				var httpClient = new HttpClient();
				var response = httpClient.GetAsync(url).Result;
				string json = response.Content.ReadAsStringAsync().Result;
				T? result = JsonSerializer.Deserialize<T>(json, Options);
				return result;
			}
			catch (Exception)
			{
				Log.Information("Failed to deserialize json from url");
				return default(T);
			}
		}
	}
}
