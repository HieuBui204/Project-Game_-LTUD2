using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    public Tilemap tileMap;
    // public GameObject facePrefab;
    public float maxPlacementDistance = 10f;
    private List<Node> nodes = new List<Node>(); // Danh sách lưu trữ các Node
    private List<Vector3> highlightedNodePositions = new List<Vector3>(); // List to store multiple node positions

    void Start()
    {
        if (tileMap == null)
        {
            tileMap = GetComponent<Tilemap>();
        }
        ClearNodes();
        CreateMap();
        CreateConnections();
    }

    void CreateMap()
    {
        BoundsInt bounds = tileMap.cellBounds;

        for (int y = bounds.y; y < bounds.yMax; y++)
        {
            for (int x = bounds.x; x < bounds.xMax; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase tile = tileMap.GetTile(cellPosition);

                if (tile != null)
                {
                    // Vị trí ô phía trên
                    Vector3Int aboveCellPosition = new Vector3Int(x, y + 1, 0);
                    TileBase aboveTile = tileMap.GetTile(aboveCellPosition);

                    // Chỉ tạo node nếu ô phía trên không có tile
                    if (aboveTile == null)
                    {
                        CreateNode(cellPosition);
                    }
                }
            }
        }
    }

    // Phương thức tạo Node tại vị trí nếu Node chưa tồn tại
    void CreateNode(Vector3Int cellPosition)
    {
        Vector3 worldPosition = tileMap.GetCellCenterWorld(cellPosition);
        Vector3 position = worldPosition + new Vector3(0, 0, 0);
        // Kiểm tra xem Node tại vị trí này đã tồn tại chưa
        Node existingNode = nodes.Find(n => n.Position == position);
        if (existingNode == null) // Nếu chưa có Node nào tại vị trí này
        {
            Node newNode = new Node(position); // Tạo Node mới
            nodes.Add(newNode); // Thêm Node vào danh sách nodes
            // Debug.Log($"Node created at: {position}");
        }
    }

    public void CreateConnections()
    {
        // Kết nối trong khoảng cách
        foreach (var node in nodes)
        {
            foreach (var otherNode in nodes)
            {
                // Đảm bảo không tự kết nối với chính mình
                if (node != otherNode)
                {
                    // Tính khoảng cách giữa hai node
                    float distance = Vector3.Distance(node.Position, otherNode.Position);

                    // Nếu khoảng cách nhỏ hơn 1.5f, thêm vào danh sách neighbors
                    if (distance < 1.5f)
                    {
                        node.AddNeighbor(otherNode);
                    }
                }
            }
        }

        foreach (var node in nodes)
        {
            if (NodeType(node.Position) == new Vector2(1, 0))
            {
                if (NodeDetect(node.Position, "toRight") != null)
                {
                    node.AddNeighbor(NodeDetect(node.Position, "toRight"));
                }

            }
            else if (NodeType(node.Position) == new Vector2(0, 1))
            {
                if (NodeDetect(node.Position, "toLeft") != null)
                {
                    node.AddNeighbor(NodeDetect(node.Position, "toLeft"));
                }
            }
        }
    }

    private Node NodeDetect(Vector3 nodePosition, string message)
    {
        Vector3 startPosition = Vector3.zero;
        Vector3 direction = Vector3.zero;

        if (message == "toRight")
        {
            startPosition = nodePosition + new Vector3(1, 0, 0);
            // Tạo vector hướng với góc -70 độ
            float angle = -70f * Mathf.Deg2Rad; // Chuyển từ độ sang radian
            direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f).normalized;
        }
        else
        {
            startPosition = nodePosition + new Vector3(-1, 0, 0);
            // Tạo vector hướng với góc -110 độ
            float angle = -110f * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f).normalized;
        }

        // Kiểm tra xem tại startPosition có tile hay không
        TileBase tileAtStart = tileMap.GetTile(tileMap.WorldToCell(startPosition));
        TileBase bellowTileAtStart = tileMap.GetTile(tileMap.WorldToCell(startPosition + new Vector3(0, -1, 0)));

        if (tileAtStart != null || bellowTileAtStart != null)
        {
            return null; // Không thực hiện raycast nếu có tile tại startPosition hoặc phía dưới
        }

        // Thực hiện raycast
        RaycastHit2D hit = Physics2D.Raycast(startPosition, direction, 20f, LayerMask.GetMask("Platform"));

        // Hiển thị raycast trên Scene view để debug
        // Debug.DrawRay(startPosition, direction * 20f, Color.red, 1000f);

        if (hit.collider != null)
        {
            // Kiểm tra nếu collider thuộc layer "Platform"
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                // Tìm node gần nhất từ điểm hit
                Node nearestNode = FindNearestNode(hit.point);

                if (nearestNode != null)
                {
                    return nearestNode; // Trả về node gần nhất
                }
            }
        }

        return null; // Không phát hiện được node
    }

    // Hàm tìm Node gần nhất với vị trí điểm hit
    private Node FindNearestNode(Vector3 hitPoint)
    {
        Node nearestNode = null;
        float nearestDistance = float.MaxValue;

        // Duyệt qua tất cả các node để tìm node gần nhất
        foreach (var node in nodes) // Giả sử bạn có danh sách tất cả các node
        {
            float distance = Vector3.Distance(hitPoint, node.Position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestNode = node;
            }
        }
        return nearestNode;
    }

    private Vector2 NodeType(Vector3 nodePosition)
    {
        // Xác định khoảng cách kiểm tra
        float checkDistance = 1.0f; // Khoảng cách để xác định có node hay không

        // Kiểm tra node bên trái
        bool hasLeftNode = nodes.Exists(node => Mathf.Approximately(node.Position.x, nodePosition.x - checkDistance) &&
                                                Mathf.Approximately(node.Position.y, nodePosition.y));

        // Kiểm tra node bên phải
        bool hasRightNode = nodes.Exists(node => Mathf.Approximately(node.Position.x, nodePosition.x + checkDistance) &&
                                                 Mathf.Approximately(node.Position.y, nodePosition.y));

        // Tạo kết quả: (1 nếu có, 0 nếu không)
        Vector2 result = new Vector2(hasLeftNode ? 1 : 0, hasRightNode ? 1 : 0);

        return result;
    }

    void ClearNodes()
    {
        nodes.Clear();
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.yellow; // Set color for Gizmos
        // foreach (Vector3 position in highlightedNodePositions)
        // {
        //     Gizmos.DrawSphere(position, 0.3f); // Draw a sphere at each stored position
        // }
        // Vẽ các node

        Gizmos.color = Color.green; // Chọn màu cho các node
        foreach (var node in nodes)
        {
            Gizmos.DrawSphere(node.Position, 0.1f); // Vẽ sphere tại vị trí của node
        }

        // Vẽ các vị trí va chạm
        // foreach (var hitPosition in raycastHits)
        // {
        //     Gizmos.DrawSphere(hitPosition, 0.1f); // Vẽ sphere tại vị trí va chạm
        // }

        // Vẽ đường kết nối giữa các node
        Gizmos.color = Color.blue;
        foreach (var node in nodes)
        {
            foreach (var neighbor in node.Neighbors)
            {
                Gizmos.DrawLine(node.Position, neighbor.Position); // Vẽ đường kết nối giữa các node
            }
        }
    }

    public List<Vector3> FindPath(Vector3 start, Vector3 end)
    {
        // Tìm node gần nhất với vị trí bắt đầu và kết thúc
        Node startNode = GetClosestNode(start);
        Node endNode = GetClosestNode(end);

        // Nếu không tìm thấy node nào, trả về null
        if (startNode == null || endNode == null)
        {
            Debug.Log("Start or end node not found. Returning null.");
            return null;
        }

        // Sử dụng A* algorithm để tìm đường đi
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();
        Dictionary<Node, float> gScore = nodes.ToDictionary(node => node, node => float.MaxValue);
        Dictionary<Node, float> fScore = nodes.ToDictionary(node => node, node => float.MaxValue);

        gScore[startNode] = 0;
        fScore[startNode] = Heuristic(startNode, endNode);

        while (openSet.Count > 0)
        {
            Node current = openSet.OrderBy(node => fScore[node]).First();

            // Nếu đã đến node cuối cùng, reconstruct đường đi và trả về
            if (current == endNode)
            {
                List<Vector3> path = ReconstructPath(cameFrom, current);

                // Nếu đường đi không hợp lệ, trả về null
                if (path == null)
                {
                    Debug.Log("Invalid path. Returning null.");
                    return null;
                }
                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (var neighbor in current.Neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                float tentative_gScore = gScore[current] + Vector3.Distance(current.Position, neighbor.Position);

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentative_gScore >= gScore[neighbor])
                {
                    continue;
                }

                // Cập nhật các giá trị đường đi
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, endNode);
            }
        }

        // Không tìm được đường đi
        Debug.Log("No valid path found. Returning null.");
        return null;
    }


    // Hàm tính khoảng cách heuristic giữa hai node
    private float Heuristic(Node nodeA, Node nodeB)
    {
        return Vector3.Distance(nodeA.Position, nodeB.Position);
    }

    // Hàm tìm node gần nhất với vị trí cho trước
    private Node GetClosestNode(Vector3 position)
    {
        Node closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (var node in nodes)
        {
            float distance = Vector3.Distance(node.Position, position);
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }
        return closestNode;
    }

    // Hàm dựng lại đường đi từ node hiện tại đến node bắt đầu, kiểm tra kết nối giữa các node
    private List<Vector3> ReconstructPath(Dictionary<Node, Node> cameFrom, Node current)
    {
        List<Vector3> totalPath = new List<Vector3> { current.Position };
        while (cameFrom.ContainsKey(current))
        {
            Node previousNode = cameFrom[current];

            // Kiểm tra xem node hiện tại và previousNode có kết nối với nhau hay không
            if (!current.Neighbors.Contains(previousNode))
            {
                Debug.Log("Path contains nodes that are not connected. Returning null.");
                return null; // Nếu không kết nối, trả về null
            }

            current = previousNode;
            totalPath.Insert(0, current.Position); // Thêm vào đầu để đảm bảo thứ tự
        }
        return totalPath;
    }

}
