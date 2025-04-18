using System.Collections.Generic;
using FluentAssertions;
using NuVerNet.DependencyResolver.ViewModels;
using NuVerNet.TreeTraversal;
using Xunit;

namespace NuVerNet.Test.TreeTraversal
{
    public class TreeTests
    {
        [Fact(DisplayName =
            "There is only one node, When the tree gets traversed, Then action should heppen for that one node")]
        public void TraverseTreeWithOnlyOneNodeSuccessfully()
        {
            var tree = Tree.New();
            var rootNode = new ProjectModel
            {
                Name = "Root",
                Path = "Root.csproj",
                Version = "1.0.0"
            };
            tree.WithRootNode(rootNode);

            var visitedNodes = new List<string>();

            // Act
            tree.TraverseDfs(node => visitedNodes.Add(node.Name));

            visitedNodes.Should().ContainSingle();
            visitedNodes[0].Should().Be("Root");
        }

        // [Fact(DisplayName =
        //     "When traversing a tree with one level of children, all nodes should be visited in DFS order")]
        // public void TraverseDfs_OneLevel_AllNodesVisitedInDfsOrder()
        // {
        //     // Arrange
        //     var tree = Tree.Create();
        //     var rootNode = new ProjectModel
        //     {
        //         Name = "Root",
        //         Path = "Root.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childA = new ProjectModel
        //     {
        //         Name = "ChildA",
        //         Path = "ChildA.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childB = new ProjectModel
        //     {
        //         Name = "ChildB",
        //         Path = "ChildB.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     rootNode.UsedIn.Add(childA);
        //     rootNode.UsedIn.Add(childB);
        //
        //     tree.WithRootNode(rootNode);
        //
        //     var visitedNodes = new List<string>();
        //
        //     // Act
        //     tree.TraverseDfs(node => visitedNodes.Add(node.Name));
        //
        //     // Assert
        //     visitedNodes.Should().HaveCount(3);
        //     visitedNodes[0].Should().Be("Root");
        //     visitedNodes[1].Should().Be("ChildA");
        //     visitedNodes[2].Should().Be("ChildB");
        // }
        //
        // [Fact(DisplayName = "When traversing a multi-level tree, all nodes should be visited in DFS order")]
        // public void TraverseDfs_MultiLevel_AllNodesVisitedInDfsOrder()
        // {
        //     // Arrange
        //     var tree = Tree.Create();
        //     var rootNode = new ProjectModel
        //     {
        //         Name = "Root",
        //         Path = "Root.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childA = new ProjectModel
        //     {
        //         Name = "ChildA",
        //         Path = "ChildA.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childB = new ProjectModel
        //     {
        //         Name = "ChildB",
        //         Path = "ChildB.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var grandChildA1 = new ProjectModel
        //     {
        //         Name = "GrandChildA1",
        //         Path = "GrandChildA1.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var grandChildB1 = new ProjectModel
        //     {
        //         Name = "GrandChildB1",
        //         Path = "GrandChildB1.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     rootNode.UsedIn.Add(childA);
        //     rootNode.UsedIn.Add(childB);
        //     childA.UsedIn.Add(grandChildA1);
        //     childB.UsedIn.Add(grandChildB1);
        //
        //     tree.WithRootNode(rootNode);
        //
        //     var visitedNodes = new List<string>();
        //
        //     // Act
        //     tree.TraverseDfs(node => visitedNodes.Add(node.Name));
        //
        //     // Assert
        //     visitedNodes.Should().HaveCount(5);
        //     visitedNodes[0].Should().Be("Root");
        //     visitedNodes[1].Should().Be("ChildA");
        //     visitedNodes[2].Should().Be("GrandChildA1");
        //     visitedNodes[3].Should().Be("ChildB");
        //     visitedNodes[4].Should().Be("GrandChildB1");
        // }
        //
        // [Fact(DisplayName = "When traversing a complex tree with cycles, each node should be visited exactly once")]
        // public void TraverseDfs_ComplexTreeWithCycles_EachNodeVisitedOnce()
        // {
        //     // Arrange
        //     var tree = Tree.Create();
        //     var rootNode = new ProjectModel
        //     {
        //         Name = "Root",
        //         Path = "Root.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childA = new ProjectModel
        //     {
        //         Name = "ChildA",
        //         Path = "ChildA.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     var childB = new ProjectModel
        //     {
        //         Name = "ChildB",
        //         Path = "ChildB.csproj",
        //         Version = "1.0.0"
        //     };
        //
        //     // Create a cycle
        //     rootNode.UsedIn.Add(childA);
        //     rootNode.UsedIn.Add(childB);
        //     childA.UsedIn.Add(childB);
        //     childB.UsedIn.Add(childA);
        //
        //     tree.WithRootNode(rootNode);
        //
        //     var visitedNodeNames = new List<string>();
        //     var visitedNodePaths = new HashSet<string>();
        //
        //     // Act
        //     tree.TraverseDfs(node =>
        //     {
        //         visitedNodeNames.Add(node.Name);
        //         visitedNodePaths.Add(node.Path);
        //     });
        //
        //     // Assert
        //     // We should have 3 unique nodes (Root, ChildA, ChildB)
        //     visitedNodePaths.Should().HaveCount(3);
        //
        //     // But the traversal might visit them multiple times due to cycles
        //     // Note: The current implementation doesn't handle cycles, so this test will likely fail
        //     // with a stack overflow. This test is included to highlight this limitation.
        //     visitedNodeNames.Should().Contain(new[] { "Root", "ChildA", "ChildB" });
        // }
    }
}