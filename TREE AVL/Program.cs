using System;

// Nodo del AVL
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

// Implementación del árbol AVL
public class AVLTree
{
    private AVLNode root;

    // Obtener altura de un nodo
    private int Height(AVLNode node)
    {
        return node == null ? 0 : node.Height;
    }

    // Obtener el factor de balance (derecha - izquierda)
    private int GetBalance(AVLNode node)
    {
        return node == null ? 0 : Height(node.Right) - Height(node.Left);
    }

    // Rotación derecha
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

    // Rotación izquierda
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

    // ----------------------------------------------------------------------
    // Inserción 
    public void Insert(int key)
    {
        root = InsertRecursive(root, key);
    }

    // Inserción recursiva con rebalanceo
    private AVLNode InsertRecursive(AVLNode node, int key)
    {
        // BST estándar
        if (node == null)
            return new AVLNode(key);

        if (key < node.Key)
            node.Left = InsertRecursive(node.Left, key);
        else if (key > node.Key)
            node.Right = InsertRecursive(node.Right, key);
        else
            return node; // sin duplicados

        // Actualizar altura del anterior nodo padre
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

        // Obtener factor de balance
        int balance = GetBalance(node);

        // Rebalanceo: 4 casos
        // Caso Izquierda-Izquierda
        if (balance < -1 && key < node.Left.Key)
            return RightRotate(node);

        // Caso Derecha-Derecha
        if (balance > 1 && key > node.Right.Key)
            return LeftRotate(node);

        // Caso Izquierda-Derecha
        if (balance < -1 && key > node.Left.Key)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        // Caso Derecha-Izquierda
        if (balance > 1 && key < node.Right.Key)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        return node;
    }

    // ----------------------------------------------------------------------
    // Buscar clave
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
        return key < node.Key ? SearchRecursive(node.Left, key) : SearchRecursive(node.Right, key);
    }

    // ----------------------------------------------------------------------
    // Eliminación pública
    public void Delete(int key)
    {
        root = DeleteRecursive(root, key);
    }

    // Eliminación recursiva con rebalanceo
    private AVLNode DeleteRecursive(AVLNode node, int key)
    {
        if (node == null)
            return node;

        // BST estándar de eliminación
        if (key < node.Key)
            node.Left = DeleteRecursive(node.Left, key);
        else if (key > node.Key)
            node.Right = DeleteRecursive(node.Right, key);
        else
        {
            // Nodo con un hijo o sin hijos
            if (node.Left == null || node.Right == null)
            {
                AVLNode temp = node.Left ?? node.Right;
                if (temp == null)
                {
                    // Sin hijos
                    node = null;
                }
                else
                {
                    // Un solo hijo
                    node = temp;
                }
            }
            else
            {
                // Nodo con dos hijos: obtener sucesor inorder (mínimo de la derecha)
                AVLNode temp = MinValueNode(node.Right);
                node.Key = temp.Key;
                node.Right = DeleteRecursive(node.Right, temp.Key);
            }
        }

        // Si el árbol tenía solo un nodo
        if (node == null)
            return node;

        // Actualizar altura
        node.Height = 1 + Math.Max(Height(node.Left), Height(node.Right));

        // Obtener balance
        int balance = GetBalance(node);

        // Rebalanceo tras eliminación
        // Caso Derecha-Derecha
        if (balance > 1 && GetBalance(node.Right) >= 0)
            return LeftRotate(node);

        // Caso Derecha-Izquierda
        if (balance > 1 && GetBalance(node.Right) < 0)
        {
            node.Right = RightRotate(node.Right);
            return LeftRotate(node);
        }

        // Caso Izquierda-Izquierda
        if (balance < -1 && GetBalance(node.Left) <= 0)
            return RightRotate(node);

        // Caso Izquierda-Derecha
        if (balance < -1 && GetBalance(node.Left) > 0)
        {
            node.Left = LeftRotate(node.Left);
            return RightRotate(node);
        }

        return node;
    }

    // Obtener el nodo con el valor mínimo (sucesor inorder)
    private AVLNode MinValueNode(AVLNode node)
    {
        AVLNode current = node;
        while (current.Left != null)
            current = current.Left;
        return current;
    }

    // ----------------------------------------------------------------------
    // Recorridos

    // InOrden
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

