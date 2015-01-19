using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StoryTranslatorTool
{
    public class StoryList
    {
        [XmlArray("StoriesArray")]
        [XmlArrayItem("StoryObject")]
        public List<Story> Stories { get; set; }

        public StoryList()
        {
            Stories = new List<Story>();
        }

        /// <summary>
        /// Saves to an xml file
        /// </summary>
        /// <param name="FileName">File path of the new xml file</param>
        public void Save(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(typeof(StoryList));
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
                var serializer = new XmlSerializer(typeof(StoryList));
                Stories = (serializer.Deserialize(stream) as StoryList).Stories;
            }
        }
    }
}
