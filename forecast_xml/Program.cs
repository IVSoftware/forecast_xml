using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace forecast_xml
{
	class Program
	{
		static void Main(string[] args)
		{

			try
			{
				XElement xml;
#if true
				xml = XElement.Parse(minimalSource);
#else
				// WARNING: This download took around 30 seconds when I tried it.
			    WebClient ftp = new WebClient();
				using (StreamReader sr = new StreamReader(
					ftp.OpenRead(@"https://ims.gov.il/sites/default/files/ims_data/xml_files/isr_cities_1week_6hr_forecast.xml")))
				{
					xml = XElement.Parse(sr.ReadToEnd());
				}
#endif
				// NEEDS ERROR CHECKING FOR NULLS AND SUCH!
				// Obtain the location.
				var location =
					xml
					.Descendants("Location")
					.First(match => match.IsMatchLocationId(4));

				Console.WriteLine(
					$"City: {location.Element("LocationMetaData").Element("LocationNameEng")}");
				var locationData = location.Element("LocationData");
				var forecasts = locationData.Elements("Forecast").Select(forecast => forecast.Format());
				Console.WriteLine(string.Join($"{Environment.NewLine}", forecasts));
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.Assert(false, ex.Message);
			}
		}

	const string minimalSource =
	@"<?xml version=""1.0"" encoding=""utf-8""?>
	<LocationForecasts>
		<Identification>
			<Organization>Israel Meteorological Service</Organization>
			<Title>6-hours forecast for 80 cities</Title>
			<IssueDateTime>Sun Jul 17 18:45:24 IDT 2022</IssueDateTime>
		</Identification>
		<Location>
			<LocationMetaData>
				<LocationId>1</LocationId>
				<LocationNameEng>Jerusalem</LocationNameEng>
				<DisplayLat>31.7780</DisplayLat>
				<DisplayLon>35.2000</DisplayLon>
				<DisplayHeight>780</DisplayHeight>
			</LocationMetaData>
			<LocationData>
				<Forecast>
					<ForecastTime>2022-07-16 03:00:00</ForecastTime>
					<Temperature>19</Temperature>
					<RelativeHumidity>90</RelativeHumidity>
					<WindSpeed>6</WindSpeed>
					<WindDirection>113</WindDirection>
					<DewPointTemp>17</DewPointTemp>
					<HeatStress>18</HeatStress>
					<HeatStressLevel>0</HeatStressLevel>
					<FeelsLike>19</FeelsLike>
					<WindChill>19</WindChill>
					<Rain>0.00</Rain>
					<WeatherCode>1220</WeatherCode>
					<MinTemp>19</MinTemp>
					<MaxTemp>20</MaxTemp>
					<UVIndex>0</UVIndex>
					<UVIndexMax>0</UVIndexMax>
				</Forecast>
			 </LocationData>
		</Location>	   <Location>
			<LocationMetaData>
				<LocationId>4</LocationId>
				<LocationNameEng>Rishon le Zion</LocationNameEng>
				<DisplayLat>31.9640</DisplayLat>
				<DisplayLon>34.8040</DisplayLon>
				<DisplayHeight>50</DisplayHeight>
			</LocationMetaData>
			<LocationData>
				<Forecast>
					<ForecastTime>2022-07-16 03:00:00</ForecastTime>
					<Temperature>24</Temperature>
					<RelativeHumidity>76</RelativeHumidity>
					<WindSpeed>3</WindSpeed>
					<WindDirection>360</WindDirection>
					<DewPointTemp>19</DewPointTemp>
					<HeatStress>22</HeatStress>
					<HeatStressLevel>0</HeatStressLevel>
					<FeelsLike>24</FeelsLike>
					<WindChill>24</WindChill>
					<Rain>0.00</Rain>
					<WeatherCode>1220</WeatherCode>
					<MinTemp>21</MinTemp>
					<MaxTemp>26</MaxTemp>
					<UVIndex>0</UVIndex>
					<UVIndexMax>0</UVIndexMax>
				</Forecast>
				<Forecast>
					<ForecastTime>2022-07-16 09:00:00</ForecastTime>
					<Temperature>28</Temperature>
					<RelativeHumidity>64</RelativeHumidity>
					<WindSpeed>9</WindSpeed>
					<WindDirection>68</WindDirection>
					<DewPointTemp>20</DewPointTemp>
					<HeatStress>25</HeatStress>
					<HeatStressLevel>2</HeatStressLevel>
					<FeelsLike>29</FeelsLike>
					<WindChill>28</WindChill>
					<Rain>0.00</Rain>
					<WeatherCode>1220</WeatherCode>
					<MinTemp>22</MinTemp>
					<MaxTemp>28</MaxTemp>
					<UVIndex>4</UVIndex>
					<UVIndexMax>4</UVIndexMax>
				</Forecast>
				<Forecast>
					<ForecastTime>2022-07-16 15:00:00</ForecastTime>
					<Temperature>29</Temperature>
					<RelativeHumidity>56</RelativeHumidity>
					<WindSpeed>18</WindSpeed>
					<WindDirection>113</WindDirection>
					<DewPointTemp>20</DewPointTemp>
					<HeatStress>26</HeatStress>
					<HeatStressLevel>2</HeatStressLevel>
					<FeelsLike>31</FeelsLike>
					<WindChill>29</WindChill>
					<Rain>0.00</Rain>
					<WeatherCode>1220</WeatherCode>
					<MinTemp>28</MinTemp>
					<MaxTemp>30</MaxTemp>
					<UVIndex>6</UVIndex>
					<UVIndexMax>10</UVIndexMax>
				</Forecast>
				<Forecast>
					<ForecastTime>2022-07-16 21:00:00</ForecastTime>
					<Temperature>26</Temperature>
					<RelativeHumidity>68</RelativeHumidity>
					<WindSpeed>8</WindSpeed>
					<WindDirection>180</WindDirection>
					<DewPointTemp>20</DewPointTemp>
					<HeatStress>24</HeatStress>
					<HeatStressLevel>1</HeatStressLevel>
					<FeelsLike>26</FeelsLike>
					<WindChill>26</WindChill>
					<Rain>0.00</Rain>
					<WeatherCode>1250</WeatherCode>
					<MinTemp>26</MinTemp>
					<MaxTemp>30</MaxTemp>
					<UVIndex>0</UVIndex>
					<UVIndexMax>4</UVIndexMax>
				</Forecast>
			</LocationData>
		</Location> 
	</LocationForecasts>";
	}
	static class Extensions
	{
		public static bool IsMatchLocationId(this XElement xLocation, int id)
		{
			if (xLocation.Name.LocalName != "Location") return false;
			var meta = xLocation.Element("LocationMetaData");
			if (meta == null) return false;
			var locationId = meta.Element("LocationId");
			if (locationId == null) return false;
			return id == (int)locationId;
		}
		public static string Format(this XElement xForecast)
		{
			// THIS IS JUST A BASIC EXAMPLE
			return $"\tTime: {xForecast.Element("ForecastTime")} Temperature: {xForecast.Element("Temperature")}";
		}
	}
}
