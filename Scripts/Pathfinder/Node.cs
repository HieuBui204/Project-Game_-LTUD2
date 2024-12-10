using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // Thuộc tính lưu vị trí của node trên grid sử dụng Vector3, là readonly vì vị trí không đổi.
    public readonly Vector3 Position;

    // Chi phí gốc từ điểm bắt đầu đến node này (G-Cost)
    public float GCost { get; set; }

    // Chi phí heuristic từ node này đến đích (H-Cost)
    public float HCost { get; set; }

    // Chi phí tổng: GCost + HCost
    public float FCost => GCost + HCost;

    // Node cha, để theo dõi đường đi khi tìm ra lộ trình
    public Node Parent { get; set; }

    // Danh sách các node liền kề (các ô hàng xóm)
    public List<Node> Neighbors { get; private set; }

    // Constructor khởi tạo vị trí của node với Vector3
    public Node(Vector3 position)
    {
        Position = position;
        GCost = 0;
        HCost = 0;
        Parent = null;
        Neighbors = new List<Node>();
    }

    // Đặt Node cha (Parent) để tiện cho việc theo dõi lộ trình
    public void SetParent(Node parentNode)
    {
        Parent = parentNode;
    }

    // Tính toán chi phí heuristic với khoảng cách Euclidean giữa các điểm (Vector3.Distance)
    public void CalculateHeuristic(Vector3 targetPosition)
    {
        HCost = Vector3.Distance(Position, targetPosition);
    }

    // Reset tất cả chi phí và node cha
    public void ResetCosts()
    {
        GCost = 0;
        HCost = 0;
        Parent = null;
    }

    // Thêm một node lân cận (neighbor) vào danh sách Neighbors
    public void AddNeighbor(Node neighbor)
    {
        if (!Neighbors.Contains(neighbor)) // Nếu neighbor chưa có trong danh sách
        {
            Neighbors.Add(neighbor);
            neighbor.AddNeighbor(this);
        }
    }

    // Loại bỏ một node lân cận khỏi danh sách Neighbors
    public void RemoveNeighbor(Node neighbor)
    {
        if (Neighbors.Contains(neighbor))
        {
            Neighbors.Remove(neighbor);
        }
    }

    // Kiểm tra xem node hiện tại có phải là neighbor của một node khác không
    public bool IsNeighbor(Node node)
    {
        return Neighbors.Contains(node);
    }
}
