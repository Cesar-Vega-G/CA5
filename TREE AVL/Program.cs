using System; // AVL Tree Implementation in C#

public class AVLNode
{
    public int Key;
    public int Height;
    public AVLNode Left, Right;

    public AVLNode(int key)
    {
        Key = key;
        Height = 1;
        Left = Right = null;
    }
}

public class AVLTree
{
    private AVLNode root;

    private int Height(AVLNode node)
    {
        return node == null ? 0 : node.Height;
    }

    private int GetBalance(AVLNode node)
    {
        return node == null ? 0 : Height(node.Right) - Height(node.Left);
    }

    private AVLNode RightRotate(AVLNode y)
    {
        AVLNode x = y.Left;
        AVLNode T2 = x.Right;

        // Rotación
        x.Right = y;
        y.Left = T2;

        // Actualizar alturas
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        return x;
    }

    private AVLNode LeftRotate(AVLNode x)
    {
        AVLNode y = x.Right;
        AVLNode T2 = y.Left;

        // Rotación
        y.Left = x;
        x.Right = T2;

        // Actualizar alturas
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;
        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;

        return y;
    }

    public void Insert(int key)
    {
        root = InsertRecursive(root, key);
    }

    private AVLNode InsertRecursive(AVLNode node, int key)
    {
        // Inserción estándar BST
        if (node == null)
            return new AVLNode(key);

        if (key < node.Key)
            node.Left = InsertRecursive(node.Left, key);
        else if (key > node.Key)
            node.Right = InsertRecursive(node.Right, key);
        else
            return node; // no duplicados

        // Actualizar altura
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

        // Obtener balance
        int balance = GetBalance(node);

        // Casos de desequilibrio
        // Izquierda-Izquierda
        if (balance > 1 && key < node.Left.Key)
            return RightRotate(node);

        // Derecha-Derecha
        if (balance < -1 && key > node.Right.Key)
            return LeftRotate(node);

        // Izquierda-Derecha
        if (balance > 1 && key > node.Left.Key)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        // Derecha-Izquierda
        if (balance < -1 && key < node.Right.Key)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    // Recorrido inorden para mostrar el árbol
    public void InOrder()
    {
        InOrderTraversal(root);
        Console.WriteLine();
    }

    private void InOrderTraversal(AVLNode node)
    {
        if (node != null)
        {
            InOrderTraversal(node.Left);
            Console.Write(node.Key + " ");
            InOrderTraversal(node.Right);
        }
    }
}

class Program
{
    static void Main(string[] args)
    {

    }
}

