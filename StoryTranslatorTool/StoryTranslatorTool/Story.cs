using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StoryTranslatorTool
{
    [XmlRoot("Story")]
    public class Story
    {
        [XmlElement("Title")]
        public String Title { get; set; }
        [XmlElement("Text")]
        public String Text { get; set; }
    }
}
