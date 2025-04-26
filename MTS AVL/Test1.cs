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
        // Se ejecuta antes de cada método de prueba
        _avlTree = new AVLTree();

        // Configurar StringWriter para capturar la salida de la consola
        _stringWriter = new StringWriter();
        _originalConsoleOut = Console.Out; // Guardar la salida original
        Console.SetOut(_stringWriter); // Redirigir la salida de la consola al StringWriter
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // Se ejecuta después de cada método de prueba
        // Restaurar la salida de la consola a su estado original
        Console.SetOut(_originalConsoleOut);
        _stringWriter.Dispose();
    }

    // Helper para obtener la raíz (para pruebas de estructura/altura interna si es necesario,
    // aunque es mejor probar el comportamiento público)
    // Reflection podría usarse para acceder a 'root', pero para mantenerlo simple
    // y centrado en el API público, nos basaremos en Search y Traversals.
    // Sin embargo, para verificar alturas o balances en casos específicos de rotación,
    // un helper es útil. Podemos añadirlo usando reflection si es necesario,
    // pero evitemos depender demasiado de detalles internos si es posible.
    // Por ahora, verificaremos las rotaciones implícitamente a través del orden de los elementos
    // en los recorridos y la correcta funcionalidad de Search/Delete.

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

        // Capture initial in-order traversal
        _avlTree.InOrder();
        string initialInOrder = _stringWriter.ToString();
        _stringWriter.GetStringBuilder().Clear(); // Clear the buffer

        _avlTree.Insert(20); // Insert duplicate

        // Capture in-order traversal after inserting duplicate
        _avlTree.InOrder();
        string afterDuplicateInOrder = _stringWriter.ToString();

        Assert.AreEqual(initialInOrder, afterDuplicateInOrder, "Tree should not change after inserting a duplicate key.");
    }

    [TestMethod]
    public void Insert_TriggersLeftRotation_RRCase()
    {
        // Insert 10, 20, 30 -> Triggers RR rotation (root becomes 20)
        _avlTree.Insert(10);
        _avlTree.Insert(20);
        _avlTree.Insert(30); // Imbalance at 10 (balance factor +2, Right child balance +0/1)

        // In-order traversal should be sorted
        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after RR rotation.");

        // Verify search for all elements
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersRightRotation_LLCase()
    {
        // Insert 30, 20, 10 -> Triggers LL rotation (root becomes 20)
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(10); // Imbalance at 30 (balance factor -2, Left child balance 0/-1)

        // In-order traversal should be sorted
        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after LL rotation.");

        // Verify search for all elements
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersLeftRightRotation_LRCase()
    {
        // Insert 30, 10, 20 -> Triggers LR rotation (root becomes 20)
        _avlTree.Insert(30);
        _avlTree.Insert(10);
        _avlTree.Insert(20); // Imbalance at 30 (balance factor -2, Left child balance +1)

        // In-order traversal should be sorted
        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after LR rotation.");

        // Verify search for all elements
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_TriggersRightLeftRotation_RLCase()
    {
        // Insert 10, 30, 20 -> Triggers RL rotation (root becomes 20)
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(20); // Imbalance at 10 (balance factor +2, Right child balance -1)

        // In-order traversal should be sorted
        _avlTree.InOrder();
        Assert.AreEqual("10 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be sorted after RL rotation.");

        // Verify search for all elements
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(30));
    }

    [TestMethod]
    public void Insert_MultipleRotations_TreeRemainsBalanced()
    {
        // Insert a sequence known to cause multiple rotations
        _avlTree.Insert(50);
        _avlTree.Insert(30);
        _avlTree.Insert(70);
        _avlTree.Insert(20); // LL at 30 -> rotates (20, 30, 50, 70)
        _avlTree.Insert(40); // LR at 50 -> rotates (20, 30, 40, 50, 70)
        _avlTree.Insert(60); // RL at 70 -> rotates (20, 30, 40, 50, 60, 70)
        _avlTree.Insert(80); // RR at 70 -> rotates (20, 30, 40, 50, 60, 70, 80)

        // Expected sorted order
        _avlTree.InOrder();
        Assert.AreEqual("20 30 40 50 60 70 80 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after multiple insertions/rotations.");

        // Verify search for all elements
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
        // Deleting from an empty tree should not throw an error
        _avlTree.Delete(10);

        // Verify it's still empty (e.g., search returns false, traversal is empty)
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

        _avlTree.Delete(10); // Delete leaf

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted node 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should still find 30.");

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of leaf.");
    }

    [TestMethod]
    public void Delete_NodeWithOneChild_LeftChild()
    {
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(10); // Rotates: 20 (10, 30)

        // Tree:
        //     20
        //    /  \
        //   10  30

        _avlTree.Insert(5); // Insert 5 -> 10 gets left child (5)
        // Tree:
        //     20
        //    /  \
        //   10  30
        //  /
        // 5

        _avlTree.Delete(10); // Delete node 10 (has one child, 5)

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted node 10.");
        Assert.IsTrue(_avlTree.Search(5), "Should still find 5.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(30), "Should still find 30.");

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("5 20 30 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with one child.");
    }

    [TestMethod]
    public void Delete_NodeWithOneChild_RightChild()
    {
        _avlTree.Insert(10);
        _avlTree.Insert(20);
        _avlTree.Insert(30); // Rotates: 20 (10, 30)

        // Tree:
        //     20
        //    /  \
        //   10  30

        _avlTree.Insert(35); // Insert 35 -> 30 gets right child (35)
        // Tree:
        //     20
        //    /  \
        //   10  30
        //         \
        //         35

        _avlTree.Delete(30); // Delete node 30 (has one child, 35)

        Assert.IsFalse(_avlTree.Search(30), "Should not find deleted node 30.");
        Assert.IsTrue(_avlTree.Search(10), "Should still find 10.");
        Assert.IsTrue(_avlTree.Search(20), "Should still find 20.");
        Assert.IsTrue(_avlTree.Search(35), "Should still find 35.");


        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("10 20 35 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with one child.");
    }


    [TestMethod]
    public void Delete_NodeWithTwoChildren()
    {
        // Build a tree where 20 has two children (10 and 30)
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(5);
        _avlTree.Insert(15);
        _avlTree.Insert(25);
        _avlTree.Insert(35);

        // Tree structure (might be slightly different due to balancing, but 20 will have two children):
        //       20
        //      /  \
        //     10   30
        //    / \  / \
        //   5  15 25 35

        _avlTree.Delete(20); // Delete node 20 (with two children) - successor should be 25

        Assert.IsFalse(_avlTree.Search(20), "Should not find deleted node 20.");
        Assert.IsTrue(_avlTree.Search(5));
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(15));
        Assert.IsTrue(_avlTree.Search(25)); // Successor should still be there
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsFalse(_avlTree.Search(99));

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        // Expected order after deleting 20 and replacing with 25 (successor)
        Assert.AreEqual("5 10 15 25 30 35 \r\n", _stringWriter.ToString(), "InOrder traversal should reflect deletion of node with two children.");
    }

    [TestMethod]
    public void Delete_RootNode_SingleElement()
    {
        _avlTree.Insert(10);
        _avlTree.Delete(10);

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted root.");
        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("\r\n", _stringWriter.ToString(), "Tree should be empty after deleting the root which was the only element.");
    }


    [TestMethod]
    public void Delete_NonExistingKey_TreeRemainsUnchanged()
    {
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);

        // Capture initial in-order traversal
        _avlTree.InOrder();
        string initialInOrder = _stringWriter.ToString();
        _stringWriter.GetStringBuilder().Clear(); // Clear the buffer

        _avlTree.Delete(99); // Delete non-existing key

        // Capture in-order traversal after deleting non-existing key
        _avlTree.InOrder();
        string afterDeleteNonExistingInOrder = _stringWriter.ToString();

        Assert.AreEqual(initialInOrder, afterDeleteNonExistingInOrder, "Tree should not change after deleting a non-existing key.");
    }

    // --- Delete with Rebalancing Tests ---

    [TestMethod]
    public void Delete_TriggersLeftRotation_AfterDeletionRRCase()
    {
        // Build a tree where deleting 10 causes imbalance at 30 (BF=2) and 30's Right child (40) has BF >= 0
        _avlTree.Insert(30);
        _avlTree.Insert(10);
        _avlTree.Insert(40);
        _avlTree.Insert(50); // Tree: 30(10, 40(null, 50)) -> Imbalance at 30 (BF=2). Need to insert something else to balance 40 or get desired structure
        _avlTree.Insert(35); // Tree: 30(10, 40(35, 50)). Root 30, BF=0.

        // Let's build a simpler case: 20(10, 30(null, 40)) -> Insert 20, 10, 30, 40. Root 20. BF=1. Need to insert more to create imbalance after deletion.
        _avlTree = new AVLTree(); // Reset tree
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(30);
        _avlTree.Insert(40); // Tree structure after balance might be different. Let's use a known sequence.

        // Sequence to cause RR case after deletion:
        // Insert 30, 20, 40, 50, 35 (This gives a balanced tree: 30(20, 40(35, 50))).
        // Deleting 20 leaves 30(null, 40(35, 50)) -> 30 has BF=2. 40 has BF=1. RR case.
        _avlTree = new AVLTree();
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(40);
        _avlTree.Insert(50);
        _avlTree.Insert(35); // Balanced tree: 30(20, 40(35, 50))

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 30 35 40 50 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(20); // Delete 20

        // After deleting 20, the subtree rooted at 30 becomes unbalanced (BF=2).
        // Node 30's right child is 40, and 40's BF is (50-35)=1 >= 0. This triggers Left Rotate at 30.
        // Expected tree after rotation: 40(30(35), 50)

        Assert.IsFalse(_avlTree.Search(20), "Should not find deleted 20.");
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsTrue(_avlTree.Search(40));
        Assert.IsTrue(_avlTree.Search(50));


        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("30 35 40 50 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and RR rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersRightRotation_AfterDeletionLLCase()
    {
        // Sequence to cause LL case after deletion:
        // Insert 30, 40, 20, 10, 25 (Balanced tree: 30(20(10, 25), 40)).
        // Deleting 40 leaves 30(20(10, 25), null) -> 30 has BF=-2. 20 has BF=0. LL case.
        _avlTree.Insert(30);
        _avlTree.Insert(40);
        _avlTree.Insert(20);
        _avlTree.Insert(10);
        _avlTree.Insert(25); // Balanced tree: 30(20(10, 25), 40)

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 20 25 30 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(40); // Delete 40

        // After deleting 40, the subtree rooted at 30 becomes unbalanced (BF=-2).
        // Node 30's left child is 20, and 20's BF is (25-10)=0 <= 0. This triggers Right Rotate at 30.
        // Expected tree after rotation: 20(10, 30(25))

        Assert.IsFalse(_avlTree.Search(40), "Should not find deleted 40.");
        Assert.IsTrue(_avlTree.Search(10));
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(25));
        Assert.IsTrue(_avlTree.Search(30));

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("10 20 25 30 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and LL rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersLeftRightRotation_AfterDeletionRLCase()
    {
        // Sequence to cause RL case after deletion:
        // Insert 30, 20, 40, 35, 50. (Balanced tree: 30(20, 40(35, 50))) - Oops, this was the RR case from Insert. Let's re-think.
        // We need imbalance at node X (BF > 1) and X.Right.Left is heavier (BF < 0).
        // Insert: 20, 10, 40, 30, 50. (Balanced tree: 20(10, 40(30, 50)))
        // Delete 10. Node 20 becomes unbalanced (BF = 2), Node 40 has BF = (50-30) = 0 >= 0. This is an RR case.

        // Let's try: 30, 10, 40, 5, 20, 35 (Balanced tree: 20(10(5), 30(null, 40(35)))) - Still not right
        // How about: 30, 20, 40, 35, 50, 45 (Balanced tree: 40(30(20, 35), 50(45)))
        // Delete 20. 30 becomes leaf. 40 becomes 40(30(35), 50(45)). BF of 40 is (50-30) = 2. Right child 50 has BF = (45-null) = -1. RL case.
        _avlTree.Insert(30);
        _avlTree.Insert(20);
        _avlTree.Insert(40);
        _avlTree.Insert(35);
        _avlTree.Insert(50);
        _avlTree.Insert(45); // Balanced tree: 40(30(20, 35), 50(45))

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 30 35 40 45 50 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(20); // Delete 20

        // After deleting 20, 30 becomes a leaf. The node 40 has BF = (Height(50)-Height(30)) = (2-1) = 1. No imbalance yet.
        // Let's try deleting a node that makes the sibling side much shorter.
        // Insert 50, 30, 70, 20, 40, 60, 80 (balanced, root 50)
        // Delete 20: 30 becomes leaf. 50(30(null, 40), 70(60, 80)). BF at 50 is (H(70) - H(30)) = (2-1) = 1.
        // Delete 40: 30 becomes leaf. 50(30(20), 70(60, 80)). BF at 50 is (H(70)-H(30))=(2-1)=1.
        // Delete 60: 70 becomes 70(null, 80). 50(30(20, 40), 70(null, 80)). BF at 50 is (H(70)-H(30))=(2-2)=0.
        // Delete 80: 70 becomes 60 child. 50(30(20, 40), 60(null, 70)). BF at 50 is (H(60)-H(30))=(2-2)=0.

        // Let's use a simpler case that forces RL after deletion:
        // Insert 30, 10, 40, 35. Tree: 30(10, 40(35)). Balanced.
        // Delete 10. Tree becomes 30(null, 40(35)). Node 30 has BF=2. Node 40 has BF=-1. This is the RL case!
        _avlTree = new AVLTree(); // Reset tree
        _avlTree.Insert(30);
        _avlTree.Insert(10);
        _avlTree.Insert(40);
        _avlTree.Insert(35); // Tree: 30(10, 40(35))

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("10 30 35 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(10); // Delete 10

        // After deleting 10, node 30 has BF=2. Node 40 has BF=-1. Triggers RL rotation at 30.
        // 30.Right = RightRotate(40) -> 30.Right = 35(null, 40)
        // Then LeftRotate(30) -> 35(30, 40)
        // Expected tree after deletion and RL rotation: 35(30, 40)

        Assert.IsFalse(_avlTree.Search(10), "Should not find deleted 10.");
        Assert.IsTrue(_avlTree.Search(30));
        Assert.IsTrue(_avlTree.Search(35));
        Assert.IsTrue(_avlTree.Search(40));

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
        _avlTree.InOrder();
        Assert.AreEqual("30 35 40 \r\n", _stringWriter.ToString(), "InOrder traversal should be correct after deletion and RL rebalance.");
    }

    [TestMethod]
    public void Delete_TriggersRightLeftRotation_AfterDeletionLRCase()
    {
        // Sequence to cause LR case after deletion:
        // We need imbalance at node X (BF < -1) and X.Left.Right is heavier (BF > 0).
        // Insert 30, 40, 20, 25. Tree: 30(20(null, 25), 40). Balanced.
        // Delete 40. Tree becomes 30(20(null, 25), null). Node 30 has BF=-2. Node 20 has BF=1. This is the LR case!
        _avlTree.Insert(30);
        _avlTree.Insert(40);
        _avlTree.Insert(20);
        _avlTree.Insert(25); // Tree: 30(20(null, 25), 40)

        _stringWriter.GetStringBuilder().Clear();
        _avlTree.InOrder();
        Assert.AreEqual("20 25 30 40 \r\n", _stringWriter.ToString(), "Initial InOrder is correct.");

        _avlTree.Delete(40); // Delete 40

        // After deleting 40, node 30 has BF=-2. Node 20 has BF=1. Triggers LR rotation at 30.
        // 30.Left = LeftRotate(20) -> 30.Left = 25(20, null)
        // Then RightRotate(30) -> 25(20, 30)
        // Expected tree after deletion and LR rotation: 25(20, 30)

        Assert.IsFalse(_avlTree.Search(40), "Should not find deleted 40.");
        Assert.IsTrue(_avlTree.Search(20));
        Assert.IsTrue(_avlTree.Search(25));
        Assert.IsTrue(_avlTree.Search(30));

        _stringWriter.GetStringBuilder().Clear(); // Clear buffer
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