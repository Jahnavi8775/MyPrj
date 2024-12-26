using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace Avrhil.IT.Website.Pages
{
    public class AIChatModel : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty]
        public string UserMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            // Add user's message
            Messages.Add(new Message { User = "You", Text = UserMessage });

            // Call AI API
            var aiResponse = await GetAiResponse(UserMessage);

            // Add AI response
            Messages.Add(new Message { User = "Avya", Text = aiResponse });

            // Return partial view with updated messages
            return Partial("_ChatMessagesPartial", this);
        }

        private async Task<string> GetAiResponse(string userInput)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "http://210.89.44.217:1235/v1/chat/completions");
            var content = new StringContent("{\n    \"model\": \"llama-3.2-1b-instruct\",\n    \"messages\": [ \n      { \"role\": \"user\", \"content\": \""+userInput+"\" }\n    ], \n    \"temperature\": 0.7, \n    \"max_tokens\": -1,\n    \"stream\": false\n  }", null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            var json = JsonDocument.Parse(responseString);
            return json.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
        public List<Message> Messages { get; set; } = new List<Message>();
    }

    public class Message
    {
        public string User { get; set; }
        public string Text { get; set; }
    }
}
