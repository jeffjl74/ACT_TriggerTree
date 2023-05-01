using System.Xml.Serialization;

namespace ACT_TriggerTree
{

    /// <remarks/>
    [XmlRoot]
    public partial class Config
    {

        [XmlElement]
        public ConfigSettingsSerializer SettingsSerializer = new ConfigSettingsSerializer();

        [XmlElement]
        public ConfigResultsLoc ResultsLoc = new ConfigResultsLoc();

        [XmlElement]
        public ConfigResultsSize ResultsSize = new ConfigResultsSize();

        [XmlElement]
        public bool ResultsPopup = false;

    }

    /// <remarks/>
    public partial class ConfigSettingsSerializer
    {

        [XmlElement]
        public ConfigSettingsSerializerInt32 Int32 = new ConfigSettingsSerializerInt32();

    }

    public partial class ConfigSettingsSerializerInt32
    {

        [XmlAttribute]
        public string Name = "saveSplitterLoc";

        [XmlAttribute]
        public int Value = 150;

    }

    /// <remarks/>
    public partial class ConfigResultsLoc
    {
        [XmlAttribute]
        public int X;

        [XmlAttribute]
        public int Y;

    }

    public partial class ConfigResultsSize
    {

        [XmlAttribute]
        public int Height;

        [XmlAttribute]
        public int Width;

    }
}