using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

[TestClass]
public class AVLTreeTests
{
    private AVLTree tree;

    [TestInitialize]
    public void SetUp()
    {
        tree = new AVLTree();
    }

    [TestMethod]
    public void Insert_SingleNode_ShouldAppearInOutput()
    {
        tree.Insert(10);
        string output = CaptureInOrder(tree);

        Assert.AreEqual("10 ", output);
    }

    [TestMethod]
    public void Insert_MultipleNodes_ShouldBeBalancedInOrder()
    {
        tree.Insert(30);
        tree.Insert(20);
        tree.Insert(10); // Causa rotación derecha

        string output = CaptureInOrder(tree);
        Assert.AreEqual("10 20 30 ", output);
    }

    [TestMethod]
    public void Insert_CausesLeftRightRotation_ShouldBeBalanced()
    {
        tree.Insert(30);
        tree.Insert(10);
        tree.Insert(20); // Izquierda-Derecha: rotación izquierda-derecha

        string output = CaptureInOrder(tree);
        Assert.AreEqual("10 20 30 ", output);
    }

    [TestMethod]
    public void Insert_CausesRightLeftRotation_ShouldBeBalanced()
    {
        tree.Insert(10);
        tree.Insert(30);
        tree.Insert(20); // Derecha-Izquierda: rotación derecha-izquierda

        string output = CaptureInOrder(tree);
        Assert.AreEqual("10 20 30 ", output);
    }

    [TestMethod]
    public void Insert_DuplicateKey_ShouldNotInsertAgain()
    {
        tree.Insert(15);
        tree.Insert(15); // Duplicado

        string output = CaptureInOrder(tree);
        Assert.AreEqual("15 ", output); // Solo uno
    }

    // Helper method to capture Console output
    private string CaptureInOrder(AVLTree tree)
    {
        var sw = new StringWriter();
        Console.SetOut(sw);
        tree.InOrder();
        return sw.ToString();
    }
}

