
using RestSharpServices;

namespace TestGitHubApi
{
    [TestFixture]
    [FixtureLifeCycle(LifeCycle.SingleInstance)]
    [NonParallelizable]
    public class TestGitHubApi
    {
        private GitHubApiClient client;

        private const string Repo = "Your_repository_name";

        private long lastCommentId;
        private long lastIssueNumber;

        [SetUp]
        public void Setup()
        {
            string username =
                "GITHUB_USERNAME";

            string token =
                "GITHUB_TOKEN";

            client = new GitHubApiClient(
                "https://api.github.com/repos/YourGitHubName/",
                username,
                token);
        }

        [Test, Order(1)]
        public void Test_CreateGitHubIssue()
        {
            string title = "Test Issue";
            string body = "Test content";

            var issue = client.CreateIssue(Repo, title, body);

            Assert.That(issue, Is.Not.Null);

            lastIssueNumber = issue!.Number;

            Assert.That(issue.Id, Is.GreaterThan(0));
            Assert.That(issue.Number, Is.GreaterThan(0));
            Assert.That(issue.Title, Is.EqualTo(title));
            Assert.That(issue.Body, Is.EqualTo(body));

            Console.WriteLine($"Created issue: {issue.Number}");
        }

        [Test, Order(2)]
        public void Test_CreateCommentOnGitHubIssue()
        {
            string body = "Test comment";

            var comment = client.CreateCommentOnGitHubIssue(
                Repo, lastIssueNumber, body);

            Assert.That(comment, Is.Not.Null);

            lastCommentId = comment!.Id;

            Assert.That(comment.Id, Is.GreaterThan(0));
            Assert.That(comment.Body, Is.EqualTo(body));

            Console.WriteLine($"Created comment: {comment.Id}");
        }

        [Test, Order(3)]
        public void Test_GetIssueByValidNumber()
        {
            var issue = client.GetIssueByNumber(
                Repo, lastIssueNumber);

            Assert.That(issue, Is.Not.Null);
            Assert.That(issue!.Id, Is.GreaterThan(0));
            Assert.That(issue.Number, Is.EqualTo(lastIssueNumber));
            Assert.That(issue.Title, Is.Not.Empty);
            Assert.That(issue.Body, Is.Not.Empty);
        }

        [Test, Order(4)]
        public void Test_GetAllLabelsForIssue()
        {
            var labels = client.GetAllLabelsForIssue(
                Repo, lastIssueNumber);

            Assert.That(labels, Is.Not.Null);

            foreach (var label in labels!)
            {
                Assert.That(label.Id, Is.GreaterThan(0));
                Assert.That(label.Name, Is.Not.Empty);

                Console.WriteLine(
                    $"Label ID: {label.Id}, Name: {label.Name}");
            }
        }

        [Test, Order(5)]
        public void Test_GetAllCommentsForIssue()
        {
            var comments = client.GetAllCommentsForIssue(
                Repo, lastIssueNumber);

            Assert.That(comments, Is.Not.Null);
            Assert.That(comments, Is.Not.Empty);

            foreach (var comment in comments!)
            {
                Assert.That(comment.Id, Is.GreaterThan(0));
                Assert.That(comment.Body, Is.Not.Empty);
            }
        }

        [Test, Order(6)]
        public void Test_EditCommentOnGitHubIssue()
        {
            string newBody = "This is an edited comment content";

            var createdComment = client.CreateCommentOnGitHubIssue(
                Repo, lastIssueNumber, "Comment to edit");

            Assert.That(createdComment, Is.Not.Null);

            lastCommentId = createdComment!.Id;

            var comment = client.EditCommentOnGitHubIssue(
                Repo, lastCommentId, newBody);

            Assert.That(comment, Is.Not.Null);
            Assert.That(comment!.Id, Is.EqualTo(lastCommentId));
            Assert.That(comment.Body, Is.EqualTo(newBody));
        }

        [Test, Order(7)]
        public void Test_GetCommentById()
        {
            var comment = client.GetCommentById(
                Repo, lastCommentId);

            Assert.That(comment, Is.Not.Null);
            Assert.That(comment!.Id, Is.EqualTo(lastCommentId));
            Assert.That(comment.Body, Is.Not.Empty);
        }

        [Test, Order(8)]
        public void Test_DeleteCommentOnGitHubIssue()
        {
            var createdComment = client.CreateCommentOnGitHubIssue(
                Repo, lastIssueNumber, "Comment to delete");

            Assert.That(createdComment, Is.Not.Null);

            lastCommentId = createdComment!.Id;

            var result = client.DeleteCommentOnGitHubIssue(
                Repo, lastCommentId);

            Assert.That(result, Is.True);

            var deletedComment = client.GetCommentById(
                Repo, lastCommentId);

            Assert.That(deletedComment, Is.Null);
        }

        [Test, Order(9)]
        public void Test_GetAllIssuesFromARepo()
        {
            var issues = client.GetAllIssues(Repo);

            Assert.That(issues, Is.Not.Null);
            Assert.That(issues, Is.Not.Empty);

            foreach (var issue in issues!)
            {
                Assert.That(issue.Id, Is.GreaterThan(0));
                Assert.That(issue.Number, Is.GreaterThan(0));
                Assert.That(issue.Title, Is.Not.Empty);

                Console.WriteLine(
                    $"Issue: {issue.Number}, Title: {issue.Title}");
            }
        }
    }
}