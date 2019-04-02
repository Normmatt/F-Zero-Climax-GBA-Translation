using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProfileTranslatorTool
{
    [XmlRoot("Profile")]
    public class Profile
    {
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("BloodType")]
        public int BloodType { get; set; }
        [XmlElement("Age")]
        public int Age { get; set; }
        [XmlElement("BirthDay")]
        public int BirthDay { get; set; }
        [XmlElement("BirthMonth")]
        public int BirthMonth { get; set; }
        [XmlElement("CharacterProfile")]
        public String CharacterProfile { get; set; }
        [XmlElement("VehicleProfile")]
        public String VehicleProfile { get; set; }
    }
}
