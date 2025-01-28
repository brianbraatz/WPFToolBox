using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestAndSamples
{
    /// <summary>
    /// Interaction logic for DataBoundPanels.xaml
    /// </summary>

    public partial class DataBoundPanels : System.Windows.Controls.Page
    {
        public DataBoundPanels()
        {
            InitializeComponent();
        }

    }

    // Obrigado BeaCosta.com :)
    public class GreekGodsAndHeros : ObservableCollection<object>
    {
        public GreekGodsAndHeros()
        {
            Add(new GreekGod("Aphrodite", "Goddess of love, beauty and fertility"));
            Add(new GreekGod("Apollo", "God of prophesy, music and healing"));
            Add(new GreekGod("Ares", "God of war"));
            Add(new GreekGod("Artemis", "Virgin goddess of the hunt"));
            Add(new GreekGod("Athena", "Goddess of crafts and the domestic arts"));
            Add(new GreekHero("Bellerophon"));
            Add(new GreekGod("Demeter", "Goddess of agriculture"));
            Add(new GreekGod("Dionysus", "God of wine"));
            Add(new GreekGod("Hephaestus", "God of fire and crafts"));
            Add(new GreekGod("Hera", "Goddess of marriage"));
            Add(new GreekHero("Hercules"));
            //Add(new GreekGod("Hermes", "Messenger of the gods and guide of dead souls to the Underworld"));
            //Add(new GreekHero("Jason"));
            //Add(new GreekHero("Odysseus"));
            //Add(new GreekHero("Perseus"));
            //Add(new GreekGod("Poseidon", "God of the sea, earthquakes and horses"));
            //Add(new GreekHero("Theseus"));
            //Add(new GreekGod("Zeus", "The supreme god of the Olympians"));
        }
    }

    public class GreekGod
    {
        private string godName;

        public string GodName
        {
            get { return godName; }
            set { godName = value; }
        }

        private string godDescription;

        public string GodDescription
        {
            get { return godDescription; }
            set { godDescription = value; }
        }

        public GreekGod(string godName, string godDescription)
        {
            this.GodName = godName;
            this.GodDescription = godDescription;
        }
    }

    public class GreekHero
    {
        private string heroName;

        public string HeroName
        {
            get { return heroName; }
            set { heroName = value; }
        }

        public GreekHero(string heroName)
        {
            this.HeroName = heroName;
        }
    }
}