using FluentAssertions;
using NuVerNet.DependencyResolver.ViewModels;
using NuVerNet.TreeTraversal;

namespace NuVerNet.Test.TreeTraversal
{
    public class TreeTests
    {
        [Fact(DisplayName =
            "There is only one node, When the tree gets traversed, Then only the root node should be visited successfully")]
        public void TraverseTreeWithOnlyOneNodeSuccessfully()
        {
            var tree = Tree<ProjectModel>.New();
            tree.WithRootNode(new Node<ProjectModel>(new ProjectModel
            {
                Name = "Root",
                Path = "Root.csproj",
                Version = "1.0.0"
            }));

            var visitedNodes = new List<string>();

            tree.TraverseDfs(node => visitedNodes.Add(node.Name));

            visitedNodes.Should().ContainSingle();
            visitedNodes[0].Should().Be("Root");
        }

        [Fact(DisplayName =
            "When traversing a tree inclduing a root and two children nodes, Then all nodes should be visited DFS order successfully")]
        public void TraverseTreeWithThreeNodesOneRootAndTwoChildrenSuccessfully()
        {
            var tree = Tree<ProjectModel>.New();

            var rootNode = new Node<ProjectModel>(
                new ProjectModel { Name = "Root", Path = "Root.csproj", Version = "1.0.0" }
            );

            rootNode.AddChild(
                new ProjectModel { Name = "ChildA", Path = "ChildA.csproj", Version = "1.0.0" }
            );

            rootNode.AddChild(
                new ProjectModel { Name = "ChildB", Path = "ChildB.csproj", Version = "1.0.0" }
            );

            tree.WithRootNode(rootNode);

            var visitedNodes = new List<string>();

            tree.TraverseDfs(node => visitedNodes.Add(node.Name));

            visitedNodes.Should().HaveCount(3);
            visitedNodes[0].Should().Be("Root");
            visitedNodes[1].Should().Be("ChildA");
            visitedNodes[2].Should().Be("ChildB");
        }

        [Fact(DisplayName =
            "When traversing a tree inclduing a root and some children nodes which they have children nodes, Then all nodes should be visited in DFS order successfully")]
        public void TraverseTreeWithMultiLevelNodesInDfsOrderSuccessfully()
        {
            var tree = Tree<ProjectModel>.New();

            var rootNode = new Node<ProjectModel>(
                new ProjectModel { Name = "Root", Path = "Root.csproj", Version = "1.0.0" }
            );

            var childANode = rootNode.AddChild(
                new ProjectModel { Name = "ChildA", Path = "ChildA.csproj", Version = "1.0.0" }
            );

            childANode.AddChild(
                new ProjectModel { Name = "GrandChildA1", Path = "GrandChildA1.csproj", Version = "1.0.0" }
            );

            var childBNode = rootNode.AddChild(
                new ProjectModel { Name = "ChildB", Path = "ChildB.csproj", Version = "1.0.0" }
            );

            childBNode.AddChild(
                new ProjectModel { Name = "GrandChildB1", Path = "GrandChildB1.csproj", Version = "1.0.0" }
            );

            tree.WithRootNode(rootNode);

            var visitedNodes = new List<string>();

            tree.TraverseDfs(node => visitedNodes.Add(node.Name));

            visitedNodes.Should().HaveCount(5);
            visitedNodes[0].Should().Be("Root");
            visitedNodes[1].Should().Be("ChildA");
            visitedNodes[2].Should().Be("GrandChildA1");
            visitedNodes[3].Should().Be("ChildB");
            visitedNodes[4].Should().Be("GrandChildB1");
        }
    }
}