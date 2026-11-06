
using System.Text.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharpServices.Models;

namespace RestSharpServices
{
    public class GitHubApiClient
    {
        private readonly RestClient client;
        private readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        public GitHubApiClient(string baseUrl, string username, string token)
        {
            RestClientOptions options = new RestClientOptions(baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(username, token)
            };

            client = new RestClient(options);
        }

        public List<Issue>? GetAllIssues(string repo)
        {
            RestRequest request = new RestRequest($"{repo}/issues");

            RestResponse response = Execute(request);

            return Deserialize<List<Issue>>(response);
        }

        public Issue? GetIssueByNumber(string repo, long issueNumber)
        {
            RestRequest request =
                new RestRequest($"{repo}/issues/{issueNumber}");

            RestResponse response = Execute(request);

            return Deserialize<Issue>(response);
        }

        public Issue? CreateIssue(string repo, string title, string body)
        {
            RestRequest request =
                new RestRequest($"{repo}/issues", Method.Post);

            request.AddJsonBody(new { title, body });

            RestResponse response = Execute(request);

            return Deserialize<Issue>(response);
        }

        public List<Label>? GetAllLabelsForIssue(
            string repo, long issueNumber)
        {
            RestRequest request =
                new RestRequest($"{repo}/issues/{issueNumber}/labels");

            RestResponse response = Execute(request);

            return Deserialize<List<Label>>(response);
        }

        public List<Comment>? GetAllCommentsForIssue(
            string repo, long issueNumber)
        {
            RestRequest request =
                new RestRequest($"{repo}/issues/{issueNumber}/comments");

            RestResponse response = Execute(request);

            return Deserialize<List<Comment>>(response);
        }

        public Comment? CreateCommentOnGitHubIssue(
            string repo, long issueNumber, string body)
        {
            RestRequest request =
                new RestRequest(
                    $"{repo}/issues/{issueNumber}/comments",
                    Method.Post);

            request.AddJsonBody(new { body });

            RestResponse response = Execute(request);

            return Deserialize<Comment>(response);
        }

        public Comment? GetCommentById(string repo, long commentId)
        {
            RestRequest request =
                new RestRequest($"{repo}/issues/comments/{commentId}");

            RestResponse response = Execute(request);

            return Deserialize<Comment>(response);
        }

        public Comment? EditCommentOnGitHubIssue(
            string repo, long commentId, string newBody)
        {
            RestRequest request =
                new RestRequest(
                    $"{repo}/issues/comments/{commentId}",
                    Method.Patch);

            request.AddJsonBody(new { body = newBody });

            RestResponse response = Execute(request);

            return Deserialize<Comment>(response);
        }

        public bool DeleteCommentOnGitHubIssue(
            string repo, long commentId)
        {
            RestRequest request =
                new RestRequest(
                    $"{repo}/issues/comments/{commentId}",
                    Method.Delete);

            RestResponse response = Execute(request);

            return response.IsSuccessful;
        }

        private RestResponse Execute(RestRequest request)
        {
            RestResponse response = client.Execute(request);

            if (!response.IsSuccessful)
            {
                throw new HttpRequestException(
                    $"GitHub API request failed. " +
                    $"Status: {response.StatusCode}. " +
                    $"Error: {response.ErrorMessage}. " +
                    $"Response: {response.Content}");
            }

            return response;
        }

        private T? Deserialize<T>(RestResponse response)
        {
            if (string.IsNullOrWhiteSpace(response.Content))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(
                response.Content,
                jsonOptions);
        }
    }
}
