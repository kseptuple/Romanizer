using System.Text;

namespace Kana
{
    public partial class FormMain : Form
    {
        private static string pageTemplate = null;
        public FormMain()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory);
            pageTemplate = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Template.html"));
        }

        private void buttonSubmit_Click(object sender, EventArgs e)
        {
            var inputText = textBoxInput.Text;
            var romajiType = RomajiType.NewHepburn;
            if (radioButtonNihon.Checked) romajiType = RomajiType.Nihon;
            else if (radioButtonKunrei.Checked) romajiType = RomajiType.Kunrei;
            else if (radioButtonOldHepburn.Checked) romajiType = RomajiType.OldHepburn;

            bool isKana = checkBoxKana.Checked;
            bool shortForm = !checkBoxNoShortForm.Checked;
            var result = WordAnalyze.AnalyzeText(inputText, romajiType, shortForm);

            StringBuilder content = new StringBuilder();
            bool isLastWordReturn = false;
            foreach (var word in result)
            {
                if (word.Word == '\u000D'.ToString())
                {
                    isLastWordReturn = true;
                    content.Append("<br />").Append(Environment.NewLine);
                }
                else if (word.Word == '\u000A'.ToString())
                {
                    if (isLastWordReturn)
                    {
                        isLastWordReturn = false;
                        continue;
                    }
                    else
                    {
                        content.Append("<br />").Append(Environment.NewLine);
                    }
                }
                else
                {
                    isLastWordReturn = false;
                    content.Append("<div class=\"warpper\">").Append(Environment.NewLine);
                    if (word.Pronunciations != null)
                    {
                        foreach (var pronounciation in word.Pronunciations)
                        {
                            content.Append($"<div class=\"prn\">{(isKana ? pronounciation.Kana : pronounciation.Romaji)}</div>").Append(Environment.NewLine);
                        }
                    }
                    content.Append($"<div class=\"text\">{word.Word}</div>").Append(Environment.NewLine);
                    content.Append("</div>").Append(Environment.NewLine);
                }
            }

            var resultPage = pageTemplate.Replace("{{Content}}", content.ToString());
            webView2Result.NavigateToString(resultPage);
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            var task1 = webView2Result.EnsureCoreWebView2Async(null);
            var task2 = WordAnalyze.InitAsync();
            await Task.WhenAll(task1, task2);
            buttonSubmit.Enabled = true;
        }

        private void buttonDefault_Click(object sender, EventArgs e)
        {
            radioButtonNewHepburn.Checked = true;
            checkBoxKana.Checked = false;
            checkBoxNoShortForm.Checked = true;
        }

        private void checkBoxKana_CheckedChanged(object sender, EventArgs e)
        {
            groupBoxRomajiType.Enabled = !checkBoxKana.Checked;
            checkBoxNoShortForm.Enabled = !checkBoxKana.Checked;
        }
    }
}