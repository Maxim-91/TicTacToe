using System;
using System.Collections;
using System.Collections.Generic;
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
        if (playerMove == false)
        {
            for (int j = 0; j < board.Length; j++) boardClone[j] = board[j].text;            
            bestScore = Int32.MinValue;            

            for (int i = 0; i < board.Length; i++)
            {
                if (boardClone[i] == emptyChar)
                {
                    boardClone[i] = computerSide;                    
                    score = MinMax(boardClone, 0, min);
                    boardClone[i] = emptyChar;

                    if (score > bestScore)
                    {
                        bestScore = score;                        
                        bestMove = i;
                        Debug.Log("bestScore=" + bestScore + ", bestMove=" + bestMove);
                    }
                }
            }
            
            board[bestMove].text = GetComputerSide();
            board[bestMove].GetComponentInParent<Button>().interactable = false;

            EndTurn();
        }        
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

    public int MinMax(string[] boardCloneMM, int depth, bool isMaximizing)
    {
        if (checkBoard(boardCloneMM, computerSide) == true) return (10 - depth);
        else if (checkBoard(boardCloneMM, playerSide) == true) return (depth - 10);
        else if (Draw(boardCloneMM) == true) return 0;

        else
        {
            if (isMaximizing)
            {
                bestScoreMax = Int32.MinValue;

                for (int i = 0; i < board.Length; i++)
                {
                    if (boardCloneMM[i] == emptyChar)
                    {
                        boardCloneMM[i] = computerSide;                       
                        scoreMax = MinMax(boardCloneMM, depth++, min);                        
                        boardCloneMM[i] = emptyChar;
                        bestScoreMax = Math.Max(bestScoreMax, scoreMax);
                        //Debug.Log("scoreMax=" + scoreMax + ", i=" + i);
                    }
                }
                return bestScoreMax;
            }
            else
            {
                bestScoreMin = Int32.MaxValue;

                for (int i = 0; i < board.Length; i++)
                {
                    if (boardCloneMM[i] == emptyChar)
                    {
                        boardCloneMM[i] = playerSide;                        
                        scoreMin = MinMax(boardCloneMM, depth++, max);
                        boardCloneMM[i] = emptyChar;
                        bestScoreMin = Math.Min(bestScoreMin, scoreMin);
                        //Debug.Log("scoreMin=" + scoreMin + ", i=" + i);
                    }
                }
                return bestScoreMin;
            }
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
