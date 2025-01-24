namespace Kana
{
    public class WordSegment
    {
        public string Word { get; set; }
        public WordType Type { get; set; }
        public List<WordPronunciation> Pronunciations { get; set; }
    }

    public class WordPronunciation
    {
        public string Kana { get; set; }
        public string Romaji { get; set; }
    }

    public enum WordType
    {
        Null,
        Kanji,
        Kana,
        SpRepeatMark,
        LongVoice
    }

    public enum RomajiType
    {
        Nihon,
        Kunrei,
        OldHepburn,
        NewHepburn
    }
}
