using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Kana
{
    public static class WordAnalyze
    {

        private static string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""|DataDirectory|\KanaData.mdf"";Trusted_Connection=True;";

        private static Dictionary<string, string> KanaDic = null;

        private static Dictionary<string, string> LongVoiceDic = null;

        private static Dictionary<string, string> RendakuDic = null;

        private static List<KanaData> KanaDataCache = null;
        private static Dictionary<string, KanaConnection> KanaConnectionCache = null;
        private static Dictionary<PronounceData, string> PronounceDataCache = null;

        private const int kanjiLengthCache = 16;

        private static bool isInitialized = false;

        private static SqlConnection conn = new SqlConnection(connectionString);

        private static Dictionary<RomajiType, int> RomajiTypeIndicies = new()
        {
            { RomajiType.Nihon, 1 },
            { RomajiType.Kunrei, 2 },
            { RomajiType.OldHepburn, 3 },
            { RomajiType.NewHepburn, 3 }
        };

        static WordAnalyze()
        {
            string hiragana = "ぁあぃいぅうぇえぉおかがきぎくぐけげこごさざしじすずせぜそぞただちぢっつづてでとどなにぬねのはばぱひびぴふぶぷへべぺほぼぽまみむめもゃやゅゆょよらりるれろゎわゐゑをんゔゕゖ";
            string katakana = "ァアィイゥウェエォオカガキギクグケゲコゴサザシジスズセゼソゾタダチヂッツヅテデトドナニヌネノハバパヒビピフブプヘベペホボポマミムメモャヤュユョヨラリルレロヮワヰヱヲンヴヵヶ";
            //string vowels = "ああいいううええおおああいいううええおおああいいううええおおああいいっううええおおあいうえおあああいいいうううえええおおおあいうえおああううおおあいうえおああいえおんうああ";
            string vowels = "ああいいうういいううああいいうういいううああいいうういいううああいいっうういいううあいういうあああいいいううういいいうううあいういうああううううあいういうああいいうんうああ";
            KanaDic = new Dictionary<string, string>();
            LongVoiceDic = new Dictionary<string, string>();
            for (int i = 0; i < hiragana.Length; i++)
            {
                KanaDic.Add(hiragana[i].ToString(), hiragana[i].ToString());
                KanaDic.Add(katakana[i].ToString(), hiragana[i].ToString());
                LongVoiceDic.Add(hiragana[i].ToString(), vowels[i].ToString());
            }

            string rendakuBefore = "かきくけこさしすせそたちつてとはひふへほぱぴぷぺぽ";
            string rendakuAfter = "がぎぐげござじずぜぞだぢづでどばびぶべぼばびぶべぼ";

            RendakuDic = new Dictionary<string, string>();
            for (int i = 0; i < rendakuBefore.Length; i++)
            {
                RendakuDic.Add(rendakuBefore[i].ToString(), rendakuAfter[i].ToString());
            }
            conn.Open();
            AppDomain.CurrentDomain.ProcessExit += OnClose;
        }

        public static async Task InitAsync()
        {
            await Task.Run(Init);
        }

        public static void Init()
        {
            string query = $"select Kana, Prefix, Suffix from KanaConnection";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                KanaConnectionCache = new Dictionary<string, KanaConnection>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string kana = reader.GetString("Kana");
                    if (!KanaConnectionCache.TryGetValue(kana, out KanaConnection kanaConnectionData))
                    {
                        kanaConnectionData = new KanaConnection();
                        kanaConnectionData.Prefix = new List<string>();
                        KanaConnectionCache.Add(kana, kanaConnectionData);
                    }
                    string prefix = reader.IsDBNull("Prefix") ? null : reader.GetString("Prefix");
                    string suffix = reader.IsDBNull("Suffix") ? null : reader.GetString("Suffix");

                    if (prefix == "*")
                    {
                        kanaConnectionData.Prefix.Clear();
                        kanaConnectionData.Prefix.Add(prefix);
                    }
                    else if (prefix != null)
                    {
                        if (!kanaConnectionData.Prefix.Contains(prefix) && !kanaConnectionData.Prefix.Contains("*"))
                        {
                            kanaConnectionData.Prefix.Add(prefix);
                        }
                    }

                    if (suffix != null)
                    {
                        kanaConnectionData.Suffix = suffix;
                    }
                }
                reader.Close();

            }

            query = $"select Kana, Type, Pronounce from PronounceData";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                PronounceDataCache = new Dictionary<PronounceData, string>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string kana = reader.GetString("Kana");
                    int type = reader.GetInt32("Type");
                    string pronounce = reader.GetString("Pronounce");
                    PronounceData pronounceData = new(kana, type);
                    PronounceDataCache.Add(pronounceData, pronounce);
                }
                reader.Close();

            }

            query = $"select Kanji, Kana from KanaData where LEN(Kanji) > " + kanjiLengthCache + " order by LEN(Kanji) desc";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                KanaDataCache = new List<KanaData>();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {

                    string kanji = reader.GetString("Kanji");
                    string kana = reader.GetString("Kana");
                    var kanaData = KanaDataCache.FirstOrDefault(k => k.Kanji == kanji);
                    if (kanaData == null)
                    {
                        kanaData = new() { Kanji = kanji, Kana = [] };
                        KanaDataCache.Add(kanaData);
                    }
                    kanaData.Kana.Add(kana);
                }
                reader.Close();
            }

            isInitialized = true;
        }

        public static List<WordSegment> AnalyzeText(string text, RomajiType romajiType, bool shortenLongVoice = true)
        {
            if (!isInitialized)
            {
                Init();
            }
            var result = GetPronouciation(text);
            result = PostProcess(result);
            AppendRomaji(result, romajiType, shortenLongVoice);
            return result;
        }

        public static List<WordSegment> GetPronouciation(string text)
        {
            var inputLength = text.Length;
            int pos = 0;
            int endPos = 0;
            string currentWord = null;
            List<WordSegment> resultList = new List<WordSegment>();

            string query = $"select Kana from KanaData where Kanji = @Kanji";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                while (pos < inputLength)
                {
                    bool isInCache = false;
                    foreach (var kanaData in KanaDataCache)
                    {
                        int length = kanaData.Kanji.Length;
                        if (pos + length > inputLength)
                        {
                            continue;
                        }
                        if (text[pos..(pos + length)] == kanaData.Kanji)
                        {
                            currentWord = text[pos..(pos + length)];
                            var result = new WordSegment() { Word = currentWord, Type = WordType.Kanji, Pronunciations = [] };
                            foreach (var kana in kanaData.Kana)
                            {
                                result.Pronunciations.Add(new WordPronunciation() { Kana = kana });
                            }

                            resultList.Add(result);
                            pos += length;
                            isInCache = true;
                            break;
                        }
                    }
                    if (isInCache)
                    {
                        continue;
                    }

                    endPos = pos + kanjiLengthCache;
                    if (endPos > inputLength)
                    {
                        endPos = inputLength;
                    }
                    while (true)
                    {
                        currentWord = text[pos..endPos];
                        if (endPos - pos == 1)
                        {
                            if (currentWord == "ー")
                            {
                                var result = new WordSegment
                                {
                                    Word = currentWord,
                                    Type = WordType.LongVoice,
                                    Pronunciations = [new WordPronunciation() { Kana = string.Empty }]
                                };
                                resultList.Add(result);
                                pos++;
                                break;
                            }
                            else if (KanaDic.TryGetValue(currentWord, out string value))
                            {
                                var result = new WordSegment
                                {
                                    Word = currentWord,
                                    Type = WordType.Kana,
                                    Pronunciations = [new WordPronunciation() { Kana = value }]
                                };
                                resultList.Add(result);
                                pos++;
                                break;
                            }
                        }
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@Kanji", SqlDbType.NVarChar, 400).Value = currentWord;
                        var reader = cmd.ExecuteReader();
                        try
                        {
                            if (reader.Read())
                            {
                                var wordType = WordType.Kanji;
                                if (currentWord == "々" || currentWord == "〻") wordType = WordType.SpRepeatMark;
                                var result = new WordSegment() { Word = currentWord, Type = wordType };
                                result.Pronunciations = [new WordPronunciation() { Kana = reader.GetString("Kana") }];

                                while (reader.Read())
                                {
                                    result.Pronunciations.Add(new WordPronunciation() { Kana = reader.GetString("Kana") });
                                }
                                resultList.Add(result);
                                pos = endPos;
                                break;
                            }
                            else
                            {
                                endPos--;
                                if (endPos == pos)
                                {
                                    resultList.Add(new WordSegment() { Word = currentWord, Type = WordType.Null });
                                    pos++;
                                    break;
                                }
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
            }

            return resultList;
        }

        public static List<WordSegment> PostProcess(List<WordSegment> words)
        {
            var result = new List<WordSegment>();
            WordSegment lastWord = null;
            string lastWordSuffix = null;


            foreach (WordSegment word in words)
            {
                bool isCombined = false;
                if (word.Type == WordType.Kana)
                {
                    if (lastWordSuffix != null && lastWord?.Type == WordType.Kana)
                    {
                        if (lastWordSuffix == "*" || word.Pronunciations[0].Kana.StartsWith(lastWordSuffix))
                        {
                            lastWord.Word += word.Word;
                            lastWord.Pronunciations[0].Kana += word.Pronunciations[0].Kana;
                            isCombined = true;
                        }
                    }
                    lastWordSuffix = null;

                    List<string> prefixList = null;
                    string suffix = null;

                    if (KanaConnectionCache.TryGetValue(word.Word, out KanaConnection kanaConnection))
                    {
                        prefixList = kanaConnection.Prefix;
                        suffix = kanaConnection.Suffix;
                    }

                    if (prefixList != null && lastWord?.Type == WordType.Kana)
                    {
                        foreach (var prefix in prefixList)
                        {
                            if (prefix == "*" || lastWord.Pronunciations[0].Kana.EndsWith(prefix))
                            {
                                lastWord.Word += word.Word;
                                lastWord.Pronunciations[0].Kana += word.Pronunciations[0].Kana;
                                isCombined = true;
                                break;
                            }
                        }
                    }

                    lastWordSuffix = suffix;
                }
                else if (word.Type == WordType.LongVoice)
                {
                    lastWordSuffix = null;
                    if (lastWord != null)
                    {
                        lastWord.Word += word.Word;
                        if (lastWord.Pronunciations != null)
                        {
                            foreach (var pronounciation in lastWord.Pronunciations)
                            {
                                if (LongVoiceDic.TryGetValue(pronounciation.Kana[^1].ToString(), out string vowel))
                                {
                                    pronounciation.Kana += vowel;
                                }
                                else
                                {
                                    pronounciation.Kana += pronounciation.Kana[^1].ToString();
                                }
                            }
                        }
                        isCombined = true;
                    }
                }
                else if (word.Type == WordType.SpRepeatMark)
                {
                    lastWordSuffix = null;
                    if (lastWord != null)
                    {
                        int lastIndex = lastWord.Word.Length - 1;
                        int repeatCount = 0;
                        char lastChar = '\0';
                        while (lastIndex >= 0 && (lastChar = lastWord.Word[lastIndex]) == '々')
                        {
                            lastIndex--;
                            repeatCount++;
                        }

                        if (lastIndex != -1)
                        {
                            string lastKanji = lastChar.ToString();
                            if (lastChar >= 0xdc00 && lastChar < 0xe000)
                            {
                                lastKanji = lastWord.Word[(lastIndex - 1)..lastIndex];
                            }
                            if (lastWord.Word.Length - repeatCount != lastKanji.Length)
                            {
                                lastWord.Word += word.Word;
                                List<string> kanjiPronounce = new List<string>();

                                if (KanaDic.TryGetValue(lastKanji, out string kanaWord))
                                {
                                    kanjiPronounce.Add(kanaWord);
                                }
                                else
                                {
                                    string query = $"select Kana from KanaData where Kanji = @Kanji order by LEN(Kana) desc";

                                    using (SqlCommand cmd = new SqlCommand(query, conn))
                                    {
                                        cmd.Parameters.Add("@Kanji", SqlDbType.NVarChar, 400).Value = lastKanji;
                                        var reader = cmd.ExecuteReader();

                                        while (reader.Read())
                                        {
                                            kanjiPronounce.Add(reader.GetString("Kana"));
                                        }
                                        reader.Close();
                                    }
                                }

                                foreach (var pronounciation in lastWord.Pronunciations)
                                {
                                    foreach (var kana in kanjiPronounce)
                                    {
                                        if (!RendakuDic.TryGetValue(kana[0].ToString(), out string gana))
                                        {
                                            gana = kana;
                                        }
                                        else
                                        {
                                            gana += kana[1..];
                                        }

                                        if (pronounciation.Kana.EndsWith(kana) || pronounciation.Kana.EndsWith(gana))
                                        {
                                            pronounciation.Kana += gana;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lastWord.Word += word.Word;
                                foreach (var pronounciation in lastWord.Pronunciations)
                                {
                                    var length = pronounciation.Kana.Length / (repeatCount + 1);
                                    var kana = pronounciation.Kana[0..length];
                                    if (!RendakuDic.TryGetValue(kana[0].ToString(), out string gana))
                                    {
                                        gana = kana;
                                    }
                                    else
                                    {
                                        gana += kana[1..];
                                    }
                                    pronounciation.Kana += gana;
                                }
                            }
                            isCombined = true;
                        }
                    }
                }
                else
                {
                    lastWordSuffix = null;
                }

                if (!isCombined)
                {
                    lastWord = word;
                    result.Add(word);
                }
            }
            return result;
        }

        public static void AppendRomaji(List<WordSegment> words, RomajiType romajiType, bool shortenLongVoice)
        {
            int romajiTypeIndex = RomajiTypeIndicies[romajiType];
            string currentKana = null;
            List<WordSegment> resultList = new List<WordSegment>();


            for (int i = 0; i < words.Count; i++)
            {
                WordSegment word = words[i];
                if (word.Pronunciations == null) continue;
                foreach (WordPronunciation wordPronunciation in word.Pronunciations)
                {
                    int pos = 0;
                    int endPos = 0;
                    StringBuilder sb = new StringBuilder();
                    int kanaLength = wordPronunciation.Kana.Length;
                    while (pos < kanaLength)
                    {
                        endPos = pos + 2;
                        if (endPos > kanaLength)
                        {
                            endPos = kanaLength;
                        }
                        while (true)
                        {
                            currentKana = wordPronunciation.Kana[pos..endPos];

                            if (currentKana == "っ" || currentKana == "ん")
                            {
                                sb.Append(currentKana);
                                pos = endPos;
                                break;
                            }

                            if (PronounceDataCache.TryGetValue(new(currentKana, romajiTypeIndex), out string pronounce))
                            {
                                sb.Append(pronounce);
                                pos = endPos;
                                break;
                            }
                            else
                            {
                                endPos--;
                                if (endPos == pos)
                                {
                                    pos++;
                                    break;
                                }
                            }
                        }
                    }
                    var currentRomaji = sb.ToString();
                    sb.Clear();
                    int j = 0;
                    int shortVoiceCount = 0;
                    while (j < currentRomaji.Length)
                    {

                        if (currentRomaji[j] == 'っ')
                        {
                            shortVoiceCount++;
                        }
                        else if (currentRomaji[j] == 'ん')
                        {
                            if (shortVoiceCount > 0)
                            {
                                sb.Append('\'');
                                shortVoiceCount = 0;
                            }

                            if (j != currentRomaji.Length - 1)
                            {
                                if (romajiType == RomajiType.OldHepburn && (currentRomaji[j + 1] == 'b' || currentRomaji[j + 1] == 'p' || currentRomaji[j + 1] == 'm'))
                                {
                                    sb.Append('m');
                                }
                                else
                                {
                                    sb.Append('n');
                                }

                                if (currentRomaji[j + 1] == 'a' || currentRomaji[j + 1] == 'i' || currentRomaji[j + 1] == 'u'
                                    || currentRomaji[j + 1] == 'e' || currentRomaji[j + 1] == 'o' || currentRomaji[j + 1] == 'y')
                                {
                                    if (romajiType == RomajiType.OldHepburn)
                                    {
                                        sb.Append('-');
                                    }
                                    else
                                    {
                                        sb.Append('\'');
                                    }
                                }
                            }
                            else
                            {
                                sb.Append('n');
                            }
                        }
                        else
                        {
                            if (shortVoiceCount > 0)
                            {
                                if (currentRomaji[j] != 'a' && currentRomaji[j] != 'i' && currentRomaji[j] != 'u' && currentRomaji[j] != 'e' && currentRomaji[j] != 'o')
                                {
                                    var shortVoice = currentRomaji[j];
                                    if (j != currentRomaji.Length - 1 && (romajiType == RomajiType.OldHepburn || romajiType == RomajiType.NewHepburn))
                                    {
                                        if (currentRomaji[j] == 'c' && currentRomaji[j + 1] == 'h')
                                        {
                                            shortVoice = 't';
                                        }
                                    }
                                    for (int k = 0; k < shortVoiceCount; k++)
                                    {
                                        sb.Append(shortVoice);
                                    }
                                    shortVoiceCount = 0;
                                }
                                else
                                {
                                    sb.Append('\'');
                                }
                            }
                            sb.Append(currentRomaji[j]);
                        }
                        j++;
                    }
                    if (shortVoiceCount > 0)
                    {
                        sb.Append('\'');
                    }
                    currentRomaji = sb.ToString();
                    if (shortenLongVoice)
                    {
                        if (romajiType == RomajiType.Kunrei)
                        {
                            currentRomaji = currentRomaji.Replace("oo", "ô").Replace("ou", "ô").Replace("uu", "û");
                        }
                        else
                        {
                            currentRomaji = currentRomaji.Replace("oo", "ō").Replace("ou", "ō").Replace("uu", "ū");
                        }
                    }
                    wordPronunciation.Romaji = currentRomaji;
                }
            }

        }

        private static void OnClose(object sender, EventArgs e)
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
}
