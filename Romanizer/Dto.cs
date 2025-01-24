namespace Kana
{
    public class KanaConnection
    {
        public List<string> Prefix { get; set; }
        public string Suffix { get; set; }
    }

    public record PronounceData(string Kana, int Type);

    public class KanaData
    {
        public string Kanji { get; set; }
        public List<string> Kana { get; set; }
    }
}
