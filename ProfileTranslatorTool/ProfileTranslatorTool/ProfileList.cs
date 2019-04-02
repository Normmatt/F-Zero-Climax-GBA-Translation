using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProfileTranslatorTool
{
    public class ProfileList
    {
        [XmlArray("ProfilesArray")]
        [XmlArrayItem("ProfileObject")]
        public List<Profile> Profiles { get; set; }

        public ProfileList()
        {
            Profiles = new List<Profile>();
        }

        /// <summary>
        /// Saves to an xml file
        /// </summary>
        /// <param name="FileName">File path of the new xml file</param>
        public void Save(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(typeof(ProfileList));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        /// <summary>
        /// Load an object from an xml file
        /// </summary>
        /// <param name="FileName">Xml file name</param>
        /// <returns>The object created from the xml file</returns>
        public void Load(string FileName)
        {
            if (File.Exists(FileName))
            using (var stream = System.IO.File.OpenRead(FileName))
            {
                var serializer = new XmlSerializer(typeof(ProfileList));
                Profiles = (serializer.Deserialize(stream) as ProfileList).Profiles;
            }
        }
    }
}
