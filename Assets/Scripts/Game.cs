using UnityEngine;
public class Game : MonoBehaviour
{
    public int width = 16;
    public int height = 16;
    private Board board;
    private Cell[,] state;
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
    }
    private void Start()
    {
        NewGame();
    }
    private void NewGame()
    {
        state = new Cell[width, height];
        GenerateCells();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        Camera.main.orthographicSize = Mathf.Max(width, height);
        board.Draw(state);
    }
    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                // we actually need just x and y, and can use Vector2 but Tilemap requires Vector3 that's why it is used here
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }
}
