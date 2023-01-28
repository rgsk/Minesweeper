using UnityEngine;
public class Game : MonoBehaviour {
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;
    private Board board;
    private Cell[,] state;
    private void Awake() {
        board = GetComponentInChildren<Board>();
    }
    private void Start() {
        NewGame();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            // left click
            Reveal();
        }

        if (Input.GetMouseButtonDown(1)) {
            // right click
            Flag();
        }
    }
    private void NewGame() {
        state = new Cell[width, height];
        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        // RevealAllForTesting();
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        Camera.main.orthographicSize = Mathf.Max(width, height);
        board.Draw(state);
    }
    private void GenerateCells() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Cell cell = new Cell();
                // we actually need just x and y, and can use Vector2 but Tilemap requires Vector3 that's why it is used here
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }
    private void GenerateMines() {
        for (int i = 0; i < mineCount; i++) {
            int x;
            int y;
            do {
                x = Random.Range(0, width);
                y = Random.Range(0, height);
            } while (state[x, y].type == Cell.Type.Mine);

            state[x, y].type = Cell.Type.Mine;
        }
    }
    private void GenerateNumbers() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Cell cell = state[x, y];
                if (cell.type == Cell.Type.Mine) {
                    continue;
                }
                int count = CountMines(x, y);
                if (count > 0) {
                    cell.number = count;
                    cell.type = Cell.Type.Number;
                    state[x, y] = cell;
                }
            }
        }
    }
    private int CountMines(int cellX, int cellY) {
        int count = 0;
        for (int offsetX = -1; offsetX <= 1; offsetX++) {
            for (int offsetY = -1; offsetY <= 1; offsetY++) {
                if (offsetX == 0 && offsetY == 0) continue;
                int x = cellX + offsetX;
                int y = cellY + offsetY;
                if (IsValidCoordinates(x, y)) {
                    if (state[x, y].type == Cell.Type.Mine) {
                        count++;
                    }
                }
            }
        }
        return count;
    }
    private void RevealAllForTesting() {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                state[x, y].revealed = true;
            }
        }
    }

    private void Flag() {
        var cellPositon = GetCellPosition();
        int x = cellPositon.x;
        int y = cellPositon.y;
        if (IsValidCoordinates(x, y)) {
            var cell = state[x, y];
            if (!cell.revealed) {
                cell.flagged = !cell.flagged;
                state[x, y] = cell;
                board.Draw(state);
            }
        }
    }


    private void Reveal() {
        var cellPositon = GetCellPosition();
        int x = cellPositon.x;
        int y = cellPositon.y;
        if (IsValidCoordinates(x, y)) {
            var cell = state[x, y];
            if (!cell.revealed && !cell.flagged) {
                if (cell.type == Cell.Type.Empty) {
                    FloodFillAndMarkAsRevealedEmptyCells(x, y);
                    board.Draw(state);
                } else {
                    cell.revealed = true;
                    state[x, y] = cell;
                    board.Draw(state);
                }

            }
        }
    }

    private void FloodFillAndMarkAsRevealedEmptyCells(int x, int y) {
        if (IsValidCoordinates(x, y)) {
            var cell = state[x, y];
            if (cell.type != Cell.Type.Mine && !cell.revealed) {
                // reveal for both Cell.Type.Number and Cell.Type.Empty
                cell.revealed = true;
                state[x, y] = cell;
                // flood for adjacent cells only if cell was empty 
                if (cell.type == Cell.Type.Empty) {
                    FloodFillAndMarkAsRevealedEmptyCells(x - 1, y);
                    FloodFillAndMarkAsRevealedEmptyCells(x + 1, y);
                    FloodFillAndMarkAsRevealedEmptyCells(x, y + 1);
                    FloodFillAndMarkAsRevealedEmptyCells(x, y - 1);
                }
            }
        }
    }

    private Vector3Int GetCellPosition() {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var cellPositon = board.tilemap.WorldToCell(worldPosition);
        return cellPositon;
    }
    private bool IsValidCoordinates(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
}
