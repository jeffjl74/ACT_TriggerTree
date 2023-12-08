using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        [XmlElement]
        public bool AlternateEncoding = false;

        public List<string> autoCats = new List<string>();

        public CatGroupsList catGroupings = new CatGroupsList();

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

    public partial class CatGroupsList : List<ConfigCatGroup>, IComparer<ConfigCatGroup>
    {
        public string GetGroupName(string category)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].Cats.Contains(category))
                    return this[i].GroupName;
            }
            return string.Empty;
        }

        public void AddGroup(string group)
        {
            ConfigCatGroup grp = new ConfigCatGroup { GroupName = group };
            if (!this.Contains(grp))
            {
                this.Add(grp);
            }
        }

        public void AddCatIfNotGrouped(string group, string category)
        {
            string existingGrp = GetGroupName(category);
            // only need a new group if it doesn't have and existing one
            if(existingGrp == string.Empty || existingGrp == TriggerTree.defaultGroupName)
            {
                //Debug.WriteLine($"adding group {group} category {category}");
                ConfigCatGroup grp = this[group];
                if (grp == null)
                {
                    //Debug.WriteLine($"** new group {group}");
                    AddGroup(group);
                    grp = this[group];
                }
                if(existingGrp == TriggerTree.defaultGroupName)
                {
                    //Debug.WriteLine($"** {category} removed from default");
                    ConfigCatGroup defgrp = this[TriggerTree.defaultGroupName];
                    defgrp.Cats.Remove(category);
                }
                if (grp != null)
                {
                    if (!grp.Cats.Contains(category))
                    {
                        //Debug.WriteLine($"** added {category} to {group}");
                        grp.Cats.Add(category);
                    }
                }
            }
            else
            {
                //Debug.WriteLine($"not moving {category} from {existingGrp}");
            }
        }

        public void PutCatInGroup(string group, string category)
        {
            string existingGrp = GetGroupName(category);
            //Debug.WriteLine($"Put in group {group} category {category}");
            ConfigCatGroup grp = this[group];
            if (grp == null)
            {
                //Debug.WriteLine($"** new group {group}");
                AddGroup(group);
                grp = this[group];
            }
            if (grp != null && !string.IsNullOrEmpty(existingGrp) && !grp.GroupName.Equals(existingGrp))
            {
                //Debug.WriteLine($"** {category} removed from {existingGrp}");
                ConfigCatGroup defgrp = this[existingGrp];
                defgrp.Cats.Remove(category);
            }
            if (grp != null)
            {
                if (!grp.Cats.Contains(category))
                {
                    //Debug.WriteLine($"** added {category} to {group}");
                    grp.Cats.Add(category);
                }
            }
        }

        public int Compare(ConfigCatGroup x, ConfigCatGroup y)
        {
            return x.GroupName.CompareTo(y.GroupName);
        }

        public ConfigCatGroup this[string name]
        {
            get { return this.FirstOrDefault(t => t.GroupName == name); }
        }
    }

    public partial class ConfigCatGroup : IEqualityComparer<ConfigCatGroup>, IComparer<ConfigCatGroup>
    {
        [XmlAttribute]
        public string GroupName;

        [XmlAttribute]
        public bool Collapsed = false;

        public List<string> Cats = new List<string>();

        public int Compare(ConfigCatGroup x, ConfigCatGroup y)
        {
            return x.GroupName.CompareTo(y.GroupName);
        }

        public bool Equals(ConfigCatGroup x, ConfigCatGroup y)
        {
            return x.GroupName.Equals(y.GroupName);
        }

        public IEnumerator<ConfigCatGroup> GetEnumerator()
        {
            return GetEnumerator();
        }

        public int GetHashCode(ConfigCatGroup obj)
        {
            return GroupName.GetHashCode();
        }

        public override string ToString()
        {
            return GroupName;
        }
    }
}