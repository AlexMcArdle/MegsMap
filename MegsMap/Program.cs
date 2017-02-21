using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;

class Program
{
	private static HttpClient Client = new HttpClient();

	private static string baseGoogleUrl = "https://maps.googleapis.com/maps/api/geocode/json?";
	private static string baseTimestampUrl = "https://maps.googleapis.com/maps/api/timezone/json?";
	private static string kGoogleAPI = "&key=AIzaSyDZ5QRD6D1wwX1zGHnM41TV7-TfkfhjCCM";

	static void Main(string[] args)
	{

		Console.WriteLine("Location");
		var airport = Console.ReadLine();
		Console.WriteLine(airport);

		runprog(airport);



		// keep open
		Console.ReadLine();
	}

	static async void runprog(string airport)
	{
		var url = baseGoogleUrl + "address=" + airport + kGoogleAPI;
		Console.WriteLine(url);
		using (var response = Client.GetAsync(url).Result)
		{
			var results = await response.Content.ReadAsStringAsync();
			RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(results);
			var lat = rootObject.results[0].geometry.location.lat;
			var lng = rootObject.results[0].geometry.location.lng;
			Console.WriteLine(lat + " , " + lng);
			TimeSpan span = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
			var timestamp = "&timestamp=" + span.TotalSeconds.ToString();
			var timestampUrl = baseTimestampUrl + "location=" + lat + "," + lng + timestamp + kGoogleAPI;
			using (var response2 = Client.GetAsync(timestampUrl).Result)
			{
				var results2 = await response2.Content.ReadAsStringAsync();
				Console.WriteLine(results2);
				GTSObject googleTS = JsonConvert.DeserializeObject<GTSObject>(results2);

				// add rawoffset and dstoffset to current utc time
				var mytime = DateTime.UtcNow;
				var newtime = mytime.AddSeconds(googleTS.rawOffset + googleTS.dstOffset);

				Console.WriteLine(newtime.Hour + ":" + newtime.Minute);
			}
		}
	}

	public class AddressComponent
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}

	public class Northeast
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Southwest
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Bounds
	{
		public Northeast northeast { get; set; }
		public Southwest southwest { get; set; }
	}

	public class Location
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Northeast2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Southwest2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Viewport
	{
		public Northeast2 northeast { get; set; }
		public Southwest2 southwest { get; set; }
	}

	public class Geometry
	{
		public Bounds bounds { get; set; }
		public Location location { get; set; }
		public string location_type { get; set; }
		public Viewport viewport { get; set; }
	}

	public class Result
	{
		public List<AddressComponent> address_components { get; set; }
		public string formatted_address { get; set; }
		public Geometry geometry { get; set; }
		public string place_id { get; set; }
		public List<string> postcode_localities { get; set; }
		public List<string> types { get; set; }
	}

	public class GTSObject
	{
		public double dstOffset { get; set; }
		public double rawOffset { get; set; }
		public string status { get; set; }
		public string timeZoneId { get; set; }
		public string timeZoneName { get; set; }
	}

	public class RootObject
	{
		public List<Result> results { get; set; }
		public string status { get; set; }
	}
}
