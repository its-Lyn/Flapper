using System.Xml.Serialization;

namespace FlappyBird.Utilities.XML;

public class SaveData {
    [XmlElement("HighScore")]
    public int HighScore { get; set;}
}
