using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{
    public Text[] board;
    public GameObject gameOverPanel;
    public Text gameOverText;

    private int moveCount;

    public GameObject restartButton;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    public GameObject startInfo;

    private string playerSide;
    private string computerSide;
    public bool playerMove;
    private int value;

    private string[] boardClone;
    private int score;
    private int scoreMax;
    private int scoreMin;
    private int bestScore = Int32.MinValue;
    private int bestScoreMax = Int32.MinValue;
    private int bestScoreMin = Int32.MaxValue;
    private int bestMove;
    private bool max = true;
    private bool min = false;
    private string emptyChar = "";

    public int maxSearchDepth = 9; // Установите здесь максимальную глубину поиска

    void Awake()
    {
        gameOverPanel.SetActive(false);
        SetGameControllerReferenceOnButtons();
        moveCount = 0;
        restartButton.SetActive(false);
        playerMove = true;
        boardClone = new string[board.Length];        
    }


    void Update()
    {
        if (!playerMove)
        {
            int bestMoveIndex = FindBestMove();
            if (bestMoveIndex != -1)
            {
                board[bestMoveIndex].text = GetComputerSide();
                board[bestMoveIndex].GetComponentInParent<Button>().interactable = false;
            }

            EndTurn();
        }
    }

    public int FindBestMove()
    {
        int bestScore = int.MinValue;
        int bestMove = -1;

        for (int i = 0; i < board.Length; i++)
        {
            if (board[i].text == emptyChar)
            {
                board[i].text = computerSide;
                int score = Minimax(board.Select(x => x.text).ToArray(), 0, false, int.MinValue, int.MaxValue);
                board[i].text = emptyChar;

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
            }
        }

        return bestMove;
    }

    public bool checkBoard(string[] boardCloneMM, string player)
    {
       if ((boardCloneMM[0] == player && boardCloneMM[1] == player && boardCloneMM[2] == player) ||
           (boardCloneMM[3] == player && boardCloneMM[4] == player && boardCloneMM[5] == player) ||
           (boardCloneMM[6] == player && boardCloneMM[7] == player && boardCloneMM[8] == player) ||
           (boardCloneMM[0] == player && boardCloneMM[3] == player && boardCloneMM[6] == player) ||
           (boardCloneMM[1] == player && boardCloneMM[4] == player && boardCloneMM[7] == player) ||
           (boardCloneMM[2] == player && boardCloneMM[5] == player && boardCloneMM[8] == player) ||
           (boardCloneMM[0] == player && boardCloneMM[4] == player && boardCloneMM[8] == player) ||
           (boardCloneMM[2] == player && boardCloneMM[4] == player && boardCloneMM[6] == player)) return true;
       else return false;
    }

    public bool Draw(string[] boardCloneMM)
    {
        bool draw;
        int a = 0;
        for (int i = 0; i < board.Length; i++)
        {
            if (boardCloneMM[i] == emptyChar) a++;
        }
        if (a == 0) draw = true;
        else draw = false;
        
        return draw;        
    }

    // Функция Minimax с адаптивной глубиной
    public int Minimax(string[] boardCloneMM, int depth, bool isMaximizing, int alpha, int beta)
    {
        // Проверка на выигрыш, проигрыш или ничью
        if (checkBoard(boardCloneMM, computerSide))
            return 10 - depth;
        else if (checkBoard(boardCloneMM, playerSide))
            return depth - 10;
        else if (Draw(boardCloneMM))
            return 0;

        // Вычисление оставшихся пустых клеток на доске
        int emptyCells = 0;
        foreach (string cell in boardCloneMM)
        {
            if (cell == emptyChar)
                emptyCells++;
        }

        // Адаптивное определение глубины в зависимости от оставшихся пустых клеток
        int maxDepth = Mathf.Clamp(emptyCells, 1, maxSearchDepth);

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (boardCloneMM[i] == emptyChar)
                {
                    boardCloneMM[i] = computerSide;
                    int score = Minimax(boardCloneMM, depth + 1, false, alpha, beta);
                    boardCloneMM[i] = emptyChar;
                    bestScore = Mathf.Max(bestScore, score);
                    alpha = Mathf.Max(alpha, bestScore);
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < board.Length; i++)
            {
                if (boardCloneMM[i] == emptyChar)
                {
                    boardCloneMM[i] = playerSide;
                    int score = Minimax(boardCloneMM, depth + 1, true, alpha, beta);
                    boardCloneMM[i] = emptyChar;
                    bestScore = Mathf.Min(bestScore, score);
                    beta = Mathf.Min(beta, bestScore);
                    if (beta <= alpha)
                        break;
                }
            }
            return bestScore;
        }
    }


public void SetGameControllerReferenceOnButtons()
    {
        for (int i = 0; i < board.Length; i++)
        {
            board[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }

    public void SetStartingSide(string startingSide)
    {
        playerSide = startingSide;
        if (playerSide == "X")
        {
            computerSide = "O";
            playerMove = true;
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            computerSide = "X";            
            playerMove = false;
            SetPlayerColors(playerO, playerX);
        }

        StartGame();
    }

    void StartGame()
    {
        SetBoardInteractable(true);
        SetPlayerButtons(false);
        startInfo.SetActive(false);
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public string GetComputerSide()
    {
        return computerSide;
    }

    public void EndTurn()
    {
        moveCount++;

        if (board[0].text == playerSide && board[1].text == playerSide && board[2].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[3].text == playerSide && board[4].text == playerSide && board[5].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[6].text == playerSide && board[7].text == playerSide && board[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[0].text == playerSide && board[3].text == playerSide && board[6].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[1].text == playerSide && board[4].text == playerSide && board[7].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[2].text == playerSide && board[5].text == playerSide && board[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[0].text == playerSide && board[4].text == playerSide && board[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (board[2].text == playerSide && board[4].text == playerSide && board[6].text == playerSide)
        {
            GameOver(playerSide);
        }

        else if (board[0].text == computerSide && board[1].text == computerSide && board[2].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[3].text == computerSide && board[4].text == computerSide && board[5].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[6].text == computerSide && board[7].text == computerSide && board[8].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[0].text == computerSide && board[3].text == computerSide && board[6].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[1].text == computerSide && board[4].text == computerSide && board[7].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[2].text == computerSide && board[5].text == computerSide && board[8].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[0].text == computerSide && board[4].text == computerSide && board[8].text == computerSide)
        {
            GameOver(computerSide);
        }
        else if (board[2].text == computerSide && board[4].text == computerSide && board[6].text == computerSide)
        {
            GameOver(computerSide);
        }

        else if (moveCount >= 9)
        {
            GameOver("draw");
        }
        else
        { 
            ChengeSide();
        }
    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);

        if (winningPlayer == "draw")
        {
            SetGameOverText("It's a draw!");
            SetPlayerColorsInactive();
        }
        else
        {
            SetGameOverText(winningPlayer + " wins!");
        }
        
        restartButton.SetActive(true);
    }

    void ChengeSide()
    {
        //       playerSide = (playerSide == "X") ? "O" : "X";
        playerMove = (playerMove == true) ? false : true;

        //if (playerSide == "X")
        if (playerMove == true)
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }
    }

    void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }

    public void RestartGame()    
    {        
        moveCount = 0;
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();
        startInfo.SetActive(true);
        playerMove = true;

        for (int i = 0; i < board.Length; i++)
        {
            board[i].text = emptyChar;
            boardClone[i] = emptyChar;
        }
    }

    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < board.Length; i++)
        {
            board[i].GetComponentInParent<Button>().interactable = toggle;
        }
    }

    void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }
}
