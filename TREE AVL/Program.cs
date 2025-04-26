using System;

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

        x.Right = y;
        y.Left = T2;

        y.Height = Math.Max(Height(y.Left), Height(y.Right)) + 1;
        x.Height = Math.Max(Height(x.Left), Height(x.Right)) + 1;

        return x;
    }

    private AVLNode LeftRotate(AVLNode x)
    {
        AVLNode y = x.Right;
        AVLNode T2 = y.Left;

        y.Left = x;
        x.Right = T2;

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
        if (node == null)
            return new AVLNode(key);

        if (key < node.Key)
            node.Left = InsertRecursive(node.Left, key);
        else if (key > node.Key)
            node.Right = InsertRecursive(node.Right, key);
        else
            return node;

        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        int balance = GetBalance(node);

        if (balance < -1 && key < node.Left.Key)
            return RightRotate(node);

        if (balance > 1 && key > node.Right.Key)
            return LeftRotate(node);

        if (balance < -1 && key > node.Left.Key)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        if (balance > 1 && key < node.Right.Key)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    public bool Search(int key)
    {
        return SearchRecursive(root, key) != null;
    }

    private AVLNode SearchRecursive(AVLNode node, int key)
    {
        if (node == null)
            return null;
        if (key == node.Key)
            return node;
        return key < node.Key
            ? SearchRecursive(node.Left, key)
            : SearchRecursive(node.Right, key);
    }

    public void Delete(int key)
    {
        root = DeleteRecursive(root, key);
    }

    private AVLNode DeleteRecursive(AVLNode node, int key)
    {
        if (node == null)
            return node;

        if (key < node.Key)
            node.Left = DeleteRecursive(node.Left, key);
        else if (key > node.Key)
            node.Right = DeleteRecursive(node.Right, key);
        else
        {
            if (node.Left == null || node.Right == null)
            {
                AVLNode temp = node.Left ?? node.Right;
                node = temp;
            }
            else
            {
                AVLNode temp = MinValueNode(node.Right);
                node.Key = temp.Key;
                node.Right = DeleteRecursive(node.Right, temp.Key);
            }
        }

        if (node == null)
            return node;

        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));
        int balance = GetBalance(node);

        if (balance > 1 && GetBalance(node.Right) >= 0)
            return LeftRotate(node);

        if (balance > 1 && GetBalance(node.Right) < 0)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        if (balance < -1 && GetBalance(node.Left) <= 0)
            return RightRotate(node);

        if (balance < -1 && GetBalance(node.Left) > 0)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        return node;
    }

    private AVLNode MinValueNode(AVLNode node)
    {
        AVLNode current = node;
        while (current.Left != null)
            current = current.Left;
        return current;
    }

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
    static void Main()
    {

    }
}

