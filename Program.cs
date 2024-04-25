using System.Drawing;

class Edge : IComparable
{
    public int weight;
    public int from;
    public int to;

    public Edge(int weight, int from, int to)
    {
        this.weight = weight;
        this.from = from;
        this.to = to;
    }

    public int CompareTo(object? o)
    {
        if (o is Edge edge) return weight.CompareTo(edge.weight);
        else throw new ArgumentException("Некорректное значение параметра");
    }
}

class Point
{
    public int a;
    public int b;

    public Point(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public int calculateDistance(Point to)
    {
        return Math.Abs(a - to.a) + Math.Abs(b - to.b);
    }
}

class SpanningTreeCalculator
{
    private static int treeWeight;
    private static int maxEdgeWeight;
    private static int[] verticeCounter;

    public static void rockPox(int graphSize, Dictionary<int, List<Edge>> vertices, List<Edge> edges)
    {
        treeWeight = 0;
        maxEdgeWeight = 0;
        int iterations = (graphSize / 2) - 1;
        int edgeCounter = 0;
        verticeCounter = new int[graphSize];
        for (int i = 0; i < graphSize; i++)
        {
            verticeCounter[i] = 0;
        }

        Dictionary<int, List<int>> tree = new Dictionary<int, List<int>>();
        for (int i = 1; i <= graphSize; i++)
        {
            tree.Add(i, new List<int>());
        }

        //Step 1: Cover Rockpox boils with foam!
        int currentVertice = edges[0].from;
        for (int i = 0; i < iterations; i++)
        {
            foreach (Edge edge in vertices[currentVertice])
            {
                if (verticeCounter[edge.to - 1] == 0)
                {
                    verticeCounter[currentVertice - 1] += 1;
                    verticeCounter[edge.to - 1] += 1;
                    edgeCounter++;
                    treeWeight += edge.weight;
                    if (edge.weight > maxEdgeWeight)
                    {
                        maxEdgeWeight = edge.weight;
                    }

                    if (currentVertice < edge.to)
                    {
                        tree[currentVertice].Add(edge.to);
                    }
                    else
                    {
                        tree[edge.to].Add(currentVertice);
                    }
                }

                if (verticeCounter[currentVertice - 1] == 3)
                {
                    break;
                }
            }

            foreach (Edge edge in edges)
            {
                if (verticeCounter[edge.from - 1] == 1 && verticeCounter[edge.to - 1] == 0)
                {
                    currentVertice = edge.from;
                    break;
                }
                else if (verticeCounter[edge.to - 1] == 1 && verticeCounter[edge.from - 1] == 0)
                {
                    currentVertice = edge.to;
                    break;
                }
             }
        }

        //Step 2: Vacuum up all the foam with the LithoVac!
        foreach (Edge edge in edges)
        {
            if (verticeCounter[edge.from - 1] == 1)
            {
                currentVertice = edge.from;
                break;
            }
            else if (verticeCounter[edge.to - 1] == 1)
            {
                currentVertice = edge.to;
                break;
            }
        }

        for (int i = 0; i < iterations; i++)
        {
            foreach (Edge edge in vertices[currentVertice])
            {
                if (verticeCounter[edge.to - 1] == 1)
                {
                    verticeCounter[currentVertice - 1] += 1;
                    verticeCounter[edge.to - 1] += 1;
                    edgeCounter++;
                    treeWeight += edge.weight;
                    if (edge.weight > maxEdgeWeight)
                    {
                        maxEdgeWeight = edge.weight;
                    }

                    if (currentVertice < edge.to)
                    {
                        tree[currentVertice].Add(edge.to);
                    }
                    else
                    {
                        tree[edge.to].Add(currentVertice);
                    }
                }

                if (verticeCounter[currentVertice - 1] == 3)
                {
                    break;
                }
            }

            foreach (Edge edge in edges)
            {
                if (verticeCounter[edge.from - 1] == 2)
                {
                    currentVertice = edge.from;
                    break;
                }
                else if (verticeCounter[edge.to - 1] == 2)
                {
                    currentVertice = edge.to;
                    break;
                }
            }
        }

        //Step 3: Finish off any Rockpox creatures remaining!
        int firstRemainingVertice = (Array.FindIndex(verticeCounter, element => element == 2) + 1);
        int secondRemainingVertice = (Array.FindLastIndex(verticeCounter, element => element == 2) + 1);
        Edge finalEdge = vertices[firstRemainingVertice].Find(edge => edge.to == secondRemainingVertice);
        treeWeight += finalEdge.weight;
        if (finalEdge.weight > maxEdgeWeight)
        {
            maxEdgeWeight = finalEdge.weight;
        }
        if (firstRemainingVertice < secondRemainingVertice)
        {
            tree[firstRemainingVertice].Add(secondRemainingVertice);
        }
        else
        {
            tree[secondRemainingVertice].Add(firstRemainingVertice);
        }
        edgeCounter++;

        //Step 4: Rock and Stone!
        StreamWriter writer = new StreamWriter("E:\\ThreeTree\\Graphs\\Answers\\RockPox" + graphSize + ".txt");
        writer.WriteLine("c Вес кубического подграфа = " + treeWeight + ", самое длинное ребро = " + maxEdgeWeight + ",");
        writer.WriteLine("p edge " + graphSize + " " + edgeCounter);
        foreach (int key in tree.Keys)
        {
            tree[key].Sort();
            foreach (int value in tree[key])
            {
                writer.WriteLine("e " + key + " " + value);
            }
        }

        writer.Close();
    }
}

class Program
{
    private static int graphSize;
    private static Point[] points;
    public static Dictionary<int, List<Edge>> vertices;
    public static List<Edge> edges;

    static void parseFile(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);

        String line = reader.ReadLine();
        graphSize = Int32.Parse(line.Split("=")[1].Trim());
        points = new Point[graphSize];
        vertices = new Dictionary<int, List<Edge>>();
        edges = new List<Edge>();
        line = reader.ReadLine();

        for (int i = 0; line != null; i++)
        {

            string[] coordinates = line.Split("\t");

            Point point = new Point(Int32.Parse(coordinates[0].Trim()), Int32.Parse(coordinates[1].Trim()));
            points[i] = point;
            vertices.Add(i + 1, new List<Edge>());

            line = reader.ReadLine();
        }
        reader.Close();

        fillEdges();
    }

    static void fillEdges()
    {
        for (int i = 0; i < graphSize; i++)
        {
            for (int j = 0; j < graphSize; j++)
            {
                if (i == j)
                {
                    continue;
                }

                Edge edge = new Edge(points[i].calculateDistance(points[j]), i + 1, j + 1);

                if (i > j)
                {
                    edges.Add(edge);
                }

                vertices[i + 1].Add(edge);
            }
            vertices[i + 1].Sort();
        }
        edges.Sort();
    }

    static void checkFile(string filePath)
    {
        StreamReader reader = new StreamReader(filePath);

        String line = reader.ReadLine();
        line = reader.ReadLine();
        int[] counter = new int[Int32.Parse(line.Split(" ")[2])];
        line = reader.ReadLine();

        while (line != null)
        {
            string[] values = line.Split(" ");
            counter[Int32.Parse(values[1]) - 1]++;
            counter[Int32.Parse(values[2]) - 1]++;
            line = reader.ReadLine();
        }

        foreach (int val in counter)
        {
            if (val != 3)
            {
                throw new Exception("Проебался дурак");
            }
        }
    }

    static void Main(string[] args)
    {
        int[] fileSizes = { 64, 128, 512, 2048, 4096 };
        foreach (int size in fileSizes)
        {
            parseFile("E:\\ThreeTree\\Graphs\\Benchmark\\Taxicab_" + size + ".txt");
            SpanningTreeCalculator.rockPox(graphSize, vertices, edges);
            vertices.Clear();
            edges.Clear();
            checkFile("E:\\ThreeTree\\Graphs\\Answers\\RockPox" + size + ".txt");
        }
    }
}