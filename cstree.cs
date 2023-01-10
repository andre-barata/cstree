using System;
using System.Text;

public class Tree
{
    private Node root = null;
    public void AddNode(int val)
    {
        if (root == null) { root = new Leaf() { value = val }; return; }

        if (root is Leaf)
        {
            var lastVal = (root as Leaf).value;

            int xor = lastVal ^ val;
            int pos = 0;
            while ((xor >>= 1)!=0) // unroll for more speed...
            {
                pos++;
            }

            Branch newBranch = new Branch();
            newBranch.level = pos;
            newBranch.footprint = val;
            if ((val | (1 << pos)) != 0)
            {
                newBranch.LeftNode = root;
                newBranch.RightNode = new Leaf() { value = val };
            } else {
                newBranch.LeftNode = new Leaf() { value = val };
                newBranch.RightNode = root;
            }
            root = newBranch;
        }

        if (root is Branch)
        {
            Branch curBranch = root as Branch;

            while (curBranch is Branch)
            {
                int xor = (curBranch as Branch).footprint ^ val;
                int pos = 0;
                while ((xor >>= 1)!=0) // unroll for more speed...
                {
                    pos++;
                }
                if (pos > (curBranch as Branch).level) {/* add branch*/
                    Branch newBranch = new Branch();
                    newBranch.level = pos;
                    newBranch.footprint = val;
                    if ((val | (1 << pos)) != 0)
                    {
                        newBranch.LeftNode = curBranch;
                        newBranch.RightNode = new Leaf() { value = val };
                    } else {
                        newBranch.LeftNode = new Leaf() { value = val };
                        newBranch.RightNode = curBranch;
                    }
                    curBranch = newBranch;
                    return;
                }
                else { // navigate to child 
                    if(((val | (1 << pos)) != 0)) curBranch = (curBranch as Branch).RightNode as Branch;
                    else curBranch = (curBranch as Branch).LeftNode as Branch;
                }

            }
        }
    }

    public void Print() 
    {
        PrintNode(root, "");
    }
    public void PrintNode(Node node, string	linePrepend) 
    {
        if (node is Leaf)
            Console.WriteLine("{0}[{1}]", linePrepend, node.ToString());
        else if (node is Branch) {
            Console.WriteLine("{0}[branch]", linePrepend);
            PrintNode((node as Branch).LeftNode, linePrepend + "|-");
            PrintNode((node as Branch).RightNode, linePrepend + "|-");
        }
    }
}

public abstract class Node { }
public class Branch : Node {
    public Node LeftNode;
    public Node RightNode;
    public int level;
    public int footprint;
}
public class Leaf : Node {
    public int value;
    public override string ToString() {
        return Utils.toBin(value);
    }
}



public class Program
{
    static void Main(string[] args)
    {
        Tree tree = new Tree();

        Console.WriteLine("-> adding "+Utils.toBin(0b00000100));
        tree.AddNode(0b00000100);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b00010100));
        tree.AddNode(0b00010100);
        tree.Print();

        Console.WriteLine("-> adding "+Utils.toBin(0b01010000));
        tree.AddNode(0b01010000);
        tree.Print();
    }
}



public static class Utils
{
    public static string toBin(int value)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 1 << 7; i != 0; i >>= 1) sb.Append((value & i) > 0 ? "1" : "0");
        return sb.ToString();
    }
    public static int fromBin(string value)
    {
        int ret = 0;
        char[] aValue = value.ToCharArray();
        for (int i = 0; i < value.Length; i++) if (aValue[i] == '1') ret += 1 << i;
        return ret;
    }
}

