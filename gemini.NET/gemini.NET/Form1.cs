using System;
using System.Drawing;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GeminiApiExample
{
    public partial class Form1 : Form
    {
        //==================================================
        string api_key = ""; //輸入自己的api key
        //==================================================
        string ApiUrl;

        public Form1()
        {
            InitializeComponent();
            label1.Font = new Font(label1.Font.FontFamily, 20);
            label3.Font = new Font(label1.Font.FontFamily, 24);

            ApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent?key={api_key}";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string userInput = textBox1.Text;
            label1.Text = "使用者說:" + userInput;
            richTextBox1.Text = "正在等待api回應...";
            // 將用戶輸入的文本轉換為 JSON 格式
            string jsonData = JsonConvert.SerializeObject(new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = "(請使用繁體中文回答)" + userInput
                            }
                        }
                    }
                }
            });

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(ApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    // 使用 Newtonsoft.Json 解析 JSON
                    string text = ParseApiResponse(responseContent);

                    richTextBox1.Text = text;
                }
                else
                {
                    richTextBox1.Text = "Error: " + response;
                }
            }
        }

        private string ParseApiResponse(string jsonResponse)
        {
            try
            {
                // 使用 JObject 解析 JSON
                JObject json = JObject.Parse(jsonResponse);

                // 從 JSON 中提取文本部分
                string text = (string)json["candidates"][0]["content"]["parts"][0]["text"];

                return text;
            }
            catch (Exception ex)
            {
                return "Error parsing API response: " + ex.Message;
            }
        }


    }
}
