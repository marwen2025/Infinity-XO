using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Player { None, X, O }
    [SerializeField] TextMeshProUGUI _xPlayerScore;
    [SerializeField] TextMeshProUGUI _yPlayerScore;
    [SerializeField] TextMeshProUGUI _winnerText;
    [SerializeField] GameObject _gameEnded;
    private int _xscore, _yscore;
    public Player currentPlayer = Player.X;
    private Player[,] board = new Player[3, 3];
    private int moveLimit = 3;
    private Color _dequeuColor = new Color(102f / 255f, 102f / 255f, 102f / 255f);
    private Dictionary<Player, Queue<(int row, int column)>> playerMoves = new Dictionary<Player, Queue<(int row, int column)>>()
    {
        { Player.X, new Queue<(int row, int column)>() },
        { Player.O, new Queue<(int row, int column)>() }
    };

    private void OnEnable()
    {
        Tile.OnTileClicked.AddListener(OnTileClicked);
    }
    private void OnDisable()
    {
        Tile.OnTileClicked.RemoveListener(OnTileClicked);

    }
    private void Start()
    {
        // Initialize the board with None (empty)
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = Player.None;
            }
        }
    }
    private Tile GetTileAtPosition(int row, int column)
    {
        // Find all tile objects in the scene
        Tile[] tiles = GameObject.FindObjectsOfType<Tile>();

        // Iterate through all tiles and return the one with matching position
        foreach (Tile tile in tiles)
        {
            if (tile.row == row && tile.column == column)
            {
                return tile;
            }
        }

        return null; // No tile found at the given position
    }
    private void OnTileClicked(Tile tile)
    {
        // Check if the tile is already occupied
        if (board[tile.row, tile.column] == Player.None)
        {
            if (playerMoves[currentPlayer].Count >= moveLimit - 1)
            {
                Debug.Log("dakhlet lenna ");
                var oldestMove = playerMoves[currentPlayer].Peek();
                // Get the corresponding tile object of the oldest move
                Tile oldestTile = GetTileAtPosition(oldestMove.row, oldestMove.column);
                if (oldestTile != null)
                {

                    // Find the X or O object within the tile and deactivate it
                    GameObject oldestSymbolObject = currentPlayer == Player.X ? oldestTile._x : oldestTile._o;
                    if (oldestSymbolObject != null)
                    {
                        ResetImageColor(oldestSymbolObject, _dequeuColor);
                    }
                }
            }
            // Remove oldest move if move limit reached
            if (playerMoves[currentPlayer].Count >= moveLimit)
            {

                Debug.Log("move limit reched");
                var oldestMove = playerMoves[currentPlayer].Dequeue();
                board[oldestMove.row, oldestMove.column] = Player.None;
                // Get the corresponding tile object of the oldest move
                Tile oldestTile = GetTileAtPosition(oldestMove.row, oldestMove.column);
                if (oldestTile != null)
                {

                    // Find the X or O object within the tile and deactivate it
                    GameObject oldestSymbolObject = currentPlayer == Player.X ? oldestTile._x : oldestTile._o;
                    if (oldestSymbolObject != null)
                    {
                        ResetImageColor(oldestSymbolObject, Color.white);
                        oldestSymbolObject.SetActive(false);
                    }
                    oldestMove = playerMoves[currentPlayer].Peek();
                    // Get the corresponding tile object of the oldest move
                    oldestTile = GetTileAtPosition(oldestMove.row, oldestMove.column);
                    if (oldestTile != null)
                    {

                        // Find the X or O object within the tile and deactivate it
                        oldestSymbolObject = currentPlayer == Player.X ? oldestTile._x : oldestTile._o;
                        if (oldestSymbolObject != null)
                        {
                            ResetImageColor(oldestSymbolObject, _dequeuColor);
                        }
                    }
                }
            }

            // Find the X or O object within the tile
            GameObject symbolObject = null;
            if (currentPlayer == Player.X)
                symbolObject = tile._x;
            else
                symbolObject = tile._o;


            symbolObject.SetActive(true);

            // Update the board
            board[tile.row, tile.column] = currentPlayer;
            playerMoves[currentPlayer].Enqueue((tile.row, tile.column));

            // Check for win condition
            if (CheckWinCondition())
            {
                if (currentPlayer == Player.X)
                {
                    Debug.Log("x won");
                    _gameEnded.SetActive(true);
                    _xscore++;
                    _winnerText.text = "Player 1 Won";
                }
                else
                {
                    Debug.Log("o won");
                    _gameEnded.SetActive(true);
                    _yscore++;
                    _winnerText.text = "Player 2 Won";
                }

                _xPlayerScore.text = _xscore.ToString();
                _yPlayerScore.text = _yscore.ToString();

            }
            else
            {
                // Switch to the next player
                currentPlayer = (currentPlayer == Player.X) ? Player.O : Player.X;
            }
        }
    }
    private bool CheckWinCondition()
    {
        // Check rows for win condition
        for (int i = 0; i < 3; i++)
        {
            if (board[i, 0] != Player.None && board[i, 0] == board[i, 1] && board[i, 0] == board[i, 2])
                return true;
        }

        // Check columns for win condition
        for (int j = 0; j < 3; j++)
        {
            if (board[0, j] != Player.None && board[0, j] == board[1, j] && board[0, j] == board[2, j])
                return true;
        }

        // Check diagonals for win condition
        if (board[0, 0] != Player.None && board[0, 0] == board[1, 1] && board[0, 0] == board[2, 2])
            return true;
        if (board[0, 2] != Player.None && board[0, 2] == board[1, 1] && board[0, 2] == board[2, 0])
            return true;

        return false; // No win condition found
    }
    public void RetryGame()
    {
        // Clear the game board
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                board[i, j] = Player.None;
            }
        }

        // Clear player moves queues
        playerMoves[Player.X].Clear();
        playerMoves[Player.O].Clear();

        // Deactivate all X and O symbols
        Tile[] tiles = GameObject.FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            GameObject xObject = tile._x;
            if (xObject != null)
                ResetImageColor(xObject, Color.white);
            xObject.SetActive(false);

            GameObject oObject = tile._o;
            if (oObject != null)
                ResetImageColor(oObject, Color.white);

            oObject.SetActive(false);
        }

        // Reset current player to X
        currentPlayer = Player.X;
    }
    void ResetImageColor(GameObject test, Color color)
    {
        Image imageComponent;
        if (test.TryGetComponent<Image>(out imageComponent))
        {
            // Change the color of the Image component
            imageComponent.color = color; // Change to your desired color
        }
    }
}
