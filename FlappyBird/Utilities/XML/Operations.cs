using System.Xml.Serialization;

namespace FlappyBird.Utilities.XML;

public static class Operations {
    public static T Deserialise<T>(string path) {
        XmlSerializer serialiser = new XmlSerializer(typeof(T));

        using Stream fileStream = new FileStream(path, FileMode.Open);
        return (T)serialiser.Deserialize(fileStream)!;
    }

    public static void Serialise<T>(T data, string path) {
        XmlSerializer serialiser = new XmlSerializer(typeof(T));

        using StreamWriter writer = new StreamWriter(path);
        serialiser.Serialize(writer, data);
    }
}
