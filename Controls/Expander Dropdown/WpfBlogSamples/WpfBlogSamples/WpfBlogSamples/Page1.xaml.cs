using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfBlogSamples
{
	/// <summary>
	/// Interaction logic for Page1.xaml
	/// </summary>

	public partial class Page1 : Page
	{
		// To use PageLoaded put Loaded="PageLoaded" in root element of .xaml file.
		private void PageLoaded(object sender, RoutedEventArgs e)
		{
			CountryInfo[] countries = new CountryInfo[31];

			countries[0] = new CountryInfo("North America", "United States");
			countries[1] = new CountryInfo("North America", "Canada");
			countries[2] = new CountryInfo("North America", "Mexico");
			countries[3] = new CountryInfo("Europe", "United Kingdom");
			countries[4] = new CountryInfo("Europe", "France");
			countries[5] = new CountryInfo("Europe", "Spain");
			countries[6] = new CountryInfo("Asia", "Japan");
			countries[7] = new CountryInfo("Asia", "China");
			countries[8] = new CountryInfo("Asia", "Singapore");
			countries[9] = new CountryInfo("Asia", "Vietnam");
			countries[10] = new CountryInfo("Asia", "North Korea");
			countries[11] = new CountryInfo("Asia", "South Korea");
			countries[12] = new CountryInfo("Asia", "Mongolia");
			countries[13] = new CountryInfo("South America", "Brasil");
			countries[14] = new CountryInfo("South America", "Peru");
			countries[15] = new CountryInfo("South America", "Argentina");
			countries[16] = new CountryInfo("South America", "Chile");
			countries[17] = new CountryInfo("South America", "Columbia");
			countries[18] = new CountryInfo("Europe", "Germany");
			countries[19] = new CountryInfo("Europe", "Italy");
			countries[20] = new CountryInfo("Europe", "Austria");
			countries[21] = new CountryInfo("Africa", "South Africa");
			countries[22] = new CountryInfo("Africa", "Egypt");
			countries[23] = new CountryInfo("Africa", "Moracco");
			countries[24] = new CountryInfo("Africa", "Sudan");
			countries[25] = new CountryInfo("Africa", "Kenya");
			countries[26] = new CountryInfo("Africa", "Rwanda");
			countries[27] = new CountryInfo("Africa", "Zimbabwe");
			countries[28] = new CountryInfo("Africa", "Kenya");
			countries[29] = new CountryInfo("Australia", "Australia");
			countries[30] = new CountryInfo("Australia", "New Zealand");

			((CollectionViewSource)this.Resources["GroupedData"]).Source = countries;
		}
	}

	/// <summary>
	/// DataObject
	/// </summary>
	public struct CountryInfo
	{
		public CountryInfo(string continent, string country)
		{
			this.continent = continent;
			this.country = country;
		}
		public string Continent
		{
			get { return this.continent; }
		}
		public string Country
		{
			get { return this.country; }
		}

		public string continent;
		public string country;
	}
}