using System;
using System.Collections.Generic;
using System.IO;
using MMOTFG_Bot.Navigation;
using MMOTFG_Bot.Events;
using Newtonsoft.Json;

namespace MMOTFG_Bot.Navigation
{
    class MapReader
    {
		//public class EventConverterTypeDiscriminator : JsonConverter<Event>
  //      {
		//	enum TypeDiscriminator
  //          {
		//		GetItem = 1
  //          }
		//	public override bool CanConvert(Type typeToConvert) =>
		//		typeof(Event).IsAssignableFrom(typeToConvert);

  //          public override Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  //          {
		//		Utf8JsonReader readerClone = reader;

		//		if(readerClone.TokenType != JsonTokenType.StartObject)
  //              {
		//			throw new JsonException();
  //              }

		//		readerClone.Read();
		//		if(readerClone.TokenType != JsonTokenType.PropertyName)
  //              {
		//			throw new JsonException();
  //              }

		//		string? propertyName = readerClone.GetString();
		//		if (propertyName != "TypeDiscriminator")
		//		{
		//			throw new JsonException();
		//		}

		//		readerClone.Read();
		//		if (readerClone.TokenType != JsonTokenType.Number)
		//		{
		//			throw new JsonException();
		//		}

		//		TypeDiscriminator typeDiscriminator = (TypeDiscriminator)readerClone.GetInt32();
		//		Event ev = null;
		//		switch(typeDiscriminator) {
		//			case TypeDiscriminator.GetItem:
		//				Console.WriteLine("AAAAAAAAAAAAAAAAAAAA");
		//				JsonSerializer.Deserialize<eGiveItem>(ref reader);
		//				break;
		//			default:
		//				throw new JsonException();
		//		};
		//		return ev;
		//	}

  //          public override void Write(Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
  //          {
  //              throw new NotImplementedException();
  //          }
  //      }

        public static Map BuildMap(string path)
        {
			string mapText = "";
			try
			{
				mapText = File.ReadAllText(path);
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine("ERROR: map.json couldn't be found in assets folder.");
				Environment.Exit(-1);
			}

			Map mapTest = null;
			try
			{
				//var jsonOptions = new JsonSerializerOptions { };
				////jsonOptions.Converters.Add(new EventConverterTypeDiscriminator());
				mapTest = JsonConvert.DeserializeObject<Map>(mapText);
			}
			catch (JsonException e)
			{
				Console.WriteLine("ERROR: map.json isn't formatted correctly. \nError message:" + e.Message);
				Environment.Exit(-1);
			}
			return mapTest;
		}
    }
}
