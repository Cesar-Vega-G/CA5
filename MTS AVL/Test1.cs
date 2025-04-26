using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

[TestClass]
public class AVLTreeTests
{
    private AVLTree _avlTree;
    private StringWriter _stringWriter;
    private TextWriter _originalConsoleOut;

    [TestInitialize]
    public void TestInitialize()
    {
        _avlTree = new AVLTree();
        _stringWriter = new StringWriter();
        _originalConsoleOut = Console.Out;
        Console.SetOut(_stringWriter);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Console.SetOut(_originalConsoleOut);
        _stringWriter.Dispose();
    }

    #region Insert Tests

    [TestMethod]
    public void Insert_SingleElement_TreeContainsElement()
    {
        _avlTree.Insert(10);
        Assert.IsTrue(_avlTree.Search(10), "Should find the single inserted element.");
    }

    [TestMethod]
    public void Insert_MultipleElements_NoRotationNeeded_AllElementsFound()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);

        Assert.IsTrue(_avlTree.Search(10), "Should find 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should find 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should find 30.");
        Assert.IsFalse(_avlTree.Search(15), "Should not find 15.");
    }

    [TestMethod]
    public void Insert_DuplicateElement_TreeRemainsUnchanged()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);

        _avlTree.InOrder();
        string initialInOrder = _stringWriter.ToString();
        _stringWriter.GetStringBuilder().Clear();

        _avlTree.Insert(20);

        _avlTree.InOrder();
        string afterDuplicateInOrder = _stringWriter.ToString();

        Assert.AreEqual(initialInOrder, afterDuplicateInOrder, "Tree should not change after inserting a duplicate key.");
    }

    [TestMethod]
    public void Insert_TriggersLeftRotation_RRCase()
    {
        _avlTree.Insert(10);
        _avlTree.Insert(20);
        _avlTree.Insert(30);

        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after RR rotation.");

        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersRightRotation_LLCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(10);

        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after LL rotation.");

        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersLeftRightRotation_LRCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(10);
        _avlTree.Insert(20);

        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after LR rotation.");

        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersRightLeftRotation_RLCase()
    {
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(20);

        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after RL rotation.");

        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_MultipleRotations_TreeRemainsBalanced()
    {
        _avlTree.Insert(50);
        _avlTree.Insert(30);
        _avlTree.Insert(70);
        _avlTree.Insert(20);
        _avlTree.Insert(40);
        _avlTree.Insert(60);
        _avlTree.Insert(80);

        _avlTree.InOrder();
        Assert.AreEqual("20 30 40 50 60 70 80 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after multiple insertions/rotations.");

        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(40));
        Assert.IsTrue(_avlTree.Search(50));
        Assert.IsTrue(_avlTree.Search(60));
        Assert.IsTrue(_avlTree.Search(70));
        Assert.IsTrue(_avlTree.Search(80));
        Assert.IsFalse(_avlTree.Search(99));
    }

    #endregion

    #region Search Tests

    [TestMethod]
    public void Search_EmptyTree_ReturnsFalse()
    {
        Assert.IsFalse(_avlTree.Search(10), "Searching in an empty tree should return false.");
    }

    [TestMethod]
    public void Search_KeyExists_ReturnsTrue()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        Assert.IsTrue(_avlTree.Search(10), "Should find existing key 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should find existing key 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should find existing key 30.");
    }

    [TestMethod]
    public void Search_KeyDoesNotExist_ReturnsFalse()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        Assert.IsFalse(_avlTree.Search(15), "Should not find non-existing key 15.");
        Assert.IsFalse(_avlTree.Search(5), "Should not find non-existing key 5.");
        Assert.IsFalse(_avlTree.Search(35), "Should not find non-existing key 35.");
    }

    #endregion

    #region Delete Tests

    [TestMethod]
    public void Delete_EmptyTree_DoesNothing()
    {
        _avlTree.Delete(10);
        Assert.IsFalse(_avlTree.Search(10));
        _avlTree.InOrder();
        Assert.AreEqual("\r\n", _stringWriter.ToString(), "InOrder traversal should be empty after deleting from an empty tree.");
    }

    [TestMethod]
    public void Delete_LeafNode()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);

        _avlTree.Delete(10);

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted node 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should still find 30.");

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of leaf.");
    }

    [TestMethod]
    public void Delete_NodeWithOneChild_LeftChild()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(5);
        _avlTree.Delete(10);

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted node 10.");
        Assert.IsTrue(_avlTree.Search(5), "Should still find 5.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should still find 30.");

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("5 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with one child.");
    }

    [TestMethod]
    public void Delete_NodeWithOneChild_RightChild()
    {
        _avlTree.Insert(10);
        _avlTree.Insert(20);
        _avlTree.Insert(30);
        _avlTree.Insert(35);
        _avlTree.Delete(30);

        Assert.IsFalse(_avlTree.Search(30), "Should not find deleted node 30.");
        Assert.IsTrue(_avlTree.Search(10), "Should still find 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(35), "Should still find 35.");

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 20 35 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with one child.");
    }

    [TestMethod]
    public void Delete_NodeWithTwoChildren()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(5);
        _avlTree.Insert(15);
        _avlTree.Insert(25);
        _avlTree.Insert(35);

        _avlTree.Delete(20);

        Assert.IsFalse(_avlTree.Search(20), "Should not find deleted node 20.");
        Assert.IsTrue(_avlTree.Search(5));
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(15));
        Assert.IsTrue(_avlTree.Search(25));
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsFalse(_avlTree.Search(99));

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("5 10 15 25 30 35 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with two children.");
    }

    [TestMethod]
    public void Delete_RootNode_SingleElement()
    {
        _avlTree.Insert(10);
        _avlTree.Delete(10);

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted root.");
        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("\r\n", _stringWriter.ToString(), "Tree should be empty after deleting the root which was the only element.");
    }

    [TestMethod]
    public void Delete_NonExistingKey_TreeRemainsUnchanged()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);

        _avlTree.InOrder();
        string initialInOrder = _stringWriter.ToString();
        _stringWriter.GetStringBuilder().Clear();

        _avlTree.Delete(99);

        _avlTree.InOrder();
        string afterDeleteNonExistingInOrder = _stringWriter.ToString();

        Assert.AreEqual(initialInOrder, afterDeleteNonExistingInOrder, "Tree should not change after deleting a non-existing key.");
    }

    [TestMethod]
    public void Delete_TriggersLeftRotation_AfterDeletionRRCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(40);
        _avlTree.Insert(50);
        _avlTree.Insert(35);

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 30 35 40 50 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(20);

        Assert.IsFalse(_avlTree.Search(20), "Should not find deleted 20.");
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsTrue(_avlTree.Search(40));
        Assert.IsTrue(_avlTree.Search(50));

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("30 35 40 50 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and RR rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersRightRotation_AfterDeletionLLCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(40);
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(25);

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 20 25 30 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(40);

        Assert.IsFalse(_avlTree.Search(40), "Should not find deleted 40.");
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(25));
        Assert.IsTrue(_avlTree.Search(30));

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 20 25 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and LL rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersLeftRightRotation_AfterDeletionRLCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(10);
        _avlTree.Insert(40);
        _avlTree.Insert(35);

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 30 35 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(10);

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted 10.");
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsTrue(_avlTree.Search(40));

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("30 35 40 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and RL rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersRightLeftRotation_AfterDeletionLRCase()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(40);
        _avlTree.Insert(20);
        _avlTree.Insert(25);

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 25 30 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(40);

        Assert.IsFalse(_avlTree.Search(40), "Should not find deleted 40.");
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(25));
        Assert.IsTrue(_avlTree.Search(30));

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 25 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and LR rebalance.");
    }

    #endregion

    #region Traversal Tests

    [TestMethod]
    public void InOrder_EmptyTree_PrintsNothing()
    {
        _avlTree.InOrder();
        Assert.AreEqual("\r\n", _stringWriter.ToString(), "InOrder on an empty tree should print only a newline.");
    }

    [TestMethod]
    public void InOrder_MultipleElements_PrintsSorted()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(5);
        _avlTree.Insert(15);
        _avlTree.Insert(25);
        _avlTree.Insert(35);

        _avlTree.InOrder();
        Assert.AreEqual("5 10 15 20 25 30 35 \r\n", _stringWriter.ToString(), "InOrder traversal should print elements in sorted order.");
    }

    #endregion
}
