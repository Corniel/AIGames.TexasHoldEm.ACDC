using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AIGames.TexasHoldEm.ACDC.Simulator
{
    [Serializable, XmlRoot("positions")]
    public class Positions
    {
        [XmlElement("pos")]
        public List<Position> Items { get; set; }

        #region I/O

        public void Save(string fileName) => Save(new FileInfo(fileName));

        public void Save(FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }

        }
        public void Save(Stream stream) => serializer.Serialize(stream, this);

        public static Positions Load(string fileName) => Load(new FileInfo(fileName));

        public static Positions Load(FileInfo file)
        {
            using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                return Load(stream);
            }
        }
        public static Positions Load(Stream stream) => (Positions)serializer.Deserialize(stream);

        private static readonly XmlSerializer serializer = new XmlSerializer(typeof(Positions));

        #endregion
    }
}
