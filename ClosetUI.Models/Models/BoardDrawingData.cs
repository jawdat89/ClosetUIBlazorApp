namespace ClosetUI.Models.Models
{
    public class BoardDrawingData
    {
        public List<Board> Boards { get; set; } = [];
    }

    public class Board
    {
        public List<PartDrawingInfo> Parts { get; set; } = [];
        public int BoardIndex { get; set; }
    }

    public class PartDrawingInfo
    {
        public int ID { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Color { get; set; }
    }
}
