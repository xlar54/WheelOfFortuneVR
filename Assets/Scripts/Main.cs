using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Assets.Scripts;

public class Main : MonoBehaviour {

    public Canvas UICanvas;

    public AudioClip selectLetterSound;
    public AudioClip newPuzzleSound;
    public AudioClip newGameSound;
    public AudioClip buzzerSound;
    public AudioClip puzzleSolveSound;
    public AudioClip bankruptSound;
    public AudioClip loseTurnSound;

    public int round = 1;
    public int currentPlayer = 0;
    public string availableConsonants = "BCDFGHJKLMNPQRSTVWXYZ";
    public string availableVowels = "AEIOU";
    public string wheelSpinResult = "";
    public Puzzle puzzle = new Puzzle();
    public GameObject puzzleBoardContainer;

    public int redScore;
    public int blueScore;
    public int yellowScore;

    public string lastWedgeValue;
    public string lastAILetter;

    private bool solveMode = false;
    private int counter = 0;

    private System.Random random = new System.Random();

    // Use this for initialization
    void Start () {

        GetComponent<AudioSource>().playOnAwake = false;

        UICanvas.GetComponent<UserInterface>().ShowMainMenu(true);

    }

    public UserInterface.AllowedPlayerActions SetupAllowedUIActions(bool allowPickConsonant, bool allowPickVowel, bool allowBuyVowel, bool allowSolve, bool allowSpin)
    {
        UserInterface.AllowedPlayerActions allowedActions = new UserInterface.AllowedPlayerActions();
        allowedActions.allowPickConsonant = allowPickConsonant;
        allowedActions.allowPickVowel = allowPickVowel;
        allowedActions.allowBuyVowel = allowBuyVowel;
        allowedActions.allowSolve = allowSolve;
        allowedActions.allowSpin = allowSpin;

        return allowedActions;
    }

    private void InitializePuzzle()
    {
        GetComponent<AudioSource>().clip = newPuzzleSound;
        GetComponent<AudioSource>().Play();

        puzzle.GeneratePuzzle();

        GameObject go = GameObject.Find("CategoryText");
        go.GetComponent<TextMesh>().text = puzzle.Category;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if((r==0 || r==3) && c > 11)
                {
                    break;
                }
                if(puzzle.AnswerLine[r].Substring(c,1) != " ")
                    puzzleBoardContainer.GetComponent<PuzzleBoardScript>().lightOnRowCol(r, c);
                else
                    puzzleBoardContainer.GetComponent<PuzzleBoardScript>().lightOffRowCol(r, c);
            }
        }

    }

    public void Play()
    {
        GameObject go = GameObject.Find("Audio Source");
        go.GetComponent<AudioSource>().Stop();

        PuzzleBoardScript pzlScript = puzzleBoardContainer.GetComponent<PuzzleBoardScript>();
        pzlScript.reset();

        InitializePuzzle();

        availableConsonants = "BCDFGHJKLMNPQRSTVWXYZ";
        availableVowels = "AEIOU";

        if (currentPlayer == 0)
        {
            UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(false, false, false, true, true);
            UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);
        }
        else
        {
            Spin();
        }
        

        go = GameObject.Find("pnlLogo");
        go.SetActive(false);
    }

    public void Spin()
    {
        UICanvas.GetComponent<UserInterface>().Hide();

        if (currentPlayer == 0)
        {
            GameObject go = GameObject.Find("Wheel");
            go.GetComponent<WheelScript>().currentPlayer = currentPlayer;
            go.GetComponent<WheelScript>().allowSpin = true;
        }
        else
            StartCoroutine("AISpin");
    }

    public void OnSpin()
    {
        if (lastWedgeValue != "")
        {
            if (lastWedgeValue == "Bankrupt")
            {
                GetComponent<AudioSource>().clip = bankruptSound;
                GetComponent<AudioSource>().Play();
                ResetScore(currentPlayer);
                NextPlayer();
                Spin();
            }
            else if (lastWedgeValue == "Lose A Turn")
            {
                GetComponent<AudioSource>().clip = loseTurnSound;
                GetComponent<AudioSource>().Play();
                NextPlayer();
                Spin();
            }
            else
            {
                if (currentPlayer == 0)
                {
                    UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(true, false, false, false, false);
                    UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);
                }
                else
                {
                    lastAILetter = " ";
                    while (lastAILetter == " ")
                    {
                        int randomNumber = random.Next(0, availableConsonants.Length);
                        lastAILetter = availableConsonants.Substring(randomNumber, 1);
                    }

                    StartCoroutine("AIPickAConsonant");
                    
                }
            }
        }
    }

    public void OnPickConsonant(string letter)
    {
        if (solveMode)
        {
            OnSolve(letter);
            return;
        }

        StartCoroutine(LightUpLetter(letter));
        
        availableConsonants = availableConsonants.Replace(letter, " ");

        if (currentPlayer == 0)
        {
            UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(false, false, true, true, true);
            UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);
        }
        else
        {
            Spin();
        }
        

    }
    
    public void PickVowel()
    {
        UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(false, true, false, false, false);
        UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);
    }

    public void OnPickVowel(string letter)
    {
        if (solveMode)
        {
            OnSolve(letter);
            return;
        }

        UpdateScore(0, -200);

        StartCoroutine(LightUpLetter(letter));

        availableVowels = availableVowels.Replace(letter, " ");

        UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(false, false, true, true, true);
        UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);

    }

    public void Solve()
    {
        solveMode = true;

        UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(true, true, false, false, false);
        UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);

        EnableSolveCursor();
    }

    private void OnSolve(string letter)
    {
        counter++;

        PuzzleBoardScript pzlScript = puzzleBoardContainer.GetComponent<PuzzleBoardScript>();

        int row = -1;
        int col = -1;

        pzlScript.getFirstEmptyBlock(out row, out col);

        if (row != -1)
        {
            // place selected letter at current cursor location
            pzlScript.assignLetterRowCol(letter, row, col);

            // continue on to select next letter
            solveMode = (pzlScript.getEmptyBlockCount() > 0);

            EnableSolveCursor();
        }

        if (!solveMode)
        {
            // All letters have been entered
            string visibleAnswer = pzlScript.getVisibleAnswer();

            if (puzzle.Answer.Replace(" ", "") == visibleAnswer)
            {
                WinRound();
            }
            else
            {
                GetComponent<AudioSource>().clip = buzzerSound;
                GetComponent<AudioSource>().Play();

                pzlScript.rollback(counter);
                counter = 0;

                UserInterface.AllowedPlayerActions allowedActions = SetupAllowedUIActions(false, false, false, true, true);
                UICanvas.GetComponent<UserInterface>().Show(availableConsonants, availableVowels, allowedActions);

            }

        }

    }

    private void WinRound()
    {
        GetComponent<AudioSource>().clip = puzzleSolveSound;
        GetComponent<AudioSource>().Play();

        UICanvas.GetComponent<UserInterface>().ShowMainMenu(false);
        counter = 0;
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void EnableSolveCursor()
    {
        // Place cursor at first empty block
        PuzzleBoardScript pzlScript = puzzleBoardContainer.GetComponent<PuzzleBoardScript>();

        int row = -1;
        int col = -1;

        pzlScript.getFirstEmptyBlock(out row, out col);

        if (row != -1)
        {
            pzlScript.lightOnRowCol(row, col, new Color(1, 0.92f, 0.016f, 1));
        }
    }

    private IEnumerator LightUpLetter(string letter)
    {
        int found = 0;
        bool isVowel = "AEIOU".Contains(letter);

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if ((r == 0 || r == 3) && c > 11)
                {
                    break;
                }

                if (puzzle.AnswerLine[r].Substring(c, 1) == letter)
                {
                    found++;

                    GetComponent<AudioSource>().clip = selectLetterSound;
                    GetComponent<AudioSource>().Play();

                    puzzleBoardContainer.GetComponent<PuzzleBoardScript>().assignLetterRowCol(letter, r, c);

                    int iWedgeValue = 0;
                    if (!isVowel && int.TryParse(lastWedgeValue, out iWedgeValue))
                    {
                        UpdateScore(currentPlayer, iWedgeValue);
                    }

                    if (puzzleBoardContainer.GetComponent<PuzzleBoardScript>().getEmptyBlockCount() == 0)
                    {
                        WinRound();
                    }


                    yield return new WaitForSeconds(1.0f);
                }
            }
        }

        if (found == 0)
        {
            GetComponent<AudioSource>().clip = buzzerSound;
            GetComponent<AudioSource>().Play();
            NextPlayer();
        }

        
    }

    public void UpdateScore(int player, int adjustment)
    {

        if (player == 0)
        {
            yellowScore += adjustment;
            GameObject scoreGo = GameObject.Find("Score-Yellow");
            scoreGo.GetComponent<TextMesh>().text = "$" + yellowScore.ToString();
        }

        if (player == 1)
        {
            blueScore += adjustment;
            GameObject scoreGo = GameObject.Find("Score-Blue");
            scoreGo.GetComponent<TextMesh>().text = "$" + blueScore.ToString();
        }

        if (player == 2)
        {
            redScore += adjustment;
            GameObject scoreGo = GameObject.Find("Score-Red");
            scoreGo.GetComponent<TextMesh>().text = "$" + redScore.ToString();
        }

        
    }

    public void ResetScore(int player)
    {

        if (player == 0)
        {
            yellowScore = 0;
            GameObject scoreGo = GameObject.Find("Score-Yellow");
            scoreGo.GetComponent<TextMesh>().text = "$" + yellowScore.ToString();
        }

        if (player == 1)
        {
            blueScore = 0;
            GameObject scoreGo = GameObject.Find("Score-Blue");
            scoreGo.GetComponent<TextMesh>().text = "$" + blueScore.ToString();
        }

        if (player == 2)
        {
            redScore = 0;
            GameObject scoreGo = GameObject.Find("Score-Red");
            scoreGo.GetComponent<TextMesh>().text = "$" + redScore.ToString();
        }
    }

    private void NextPlayer()
    {
        currentPlayer++;
        if (currentPlayer == 3)
            currentPlayer = 0;
    }

    IEnumerator AIPickAConsonant()
    {
        string file = "female1-" + lastAILetter;
        AudioClip clip = (AudioClip)Resources.Load(file, typeof(AudioClip));
        GameObject go = null;

        if (currentPlayer == 1)
            go = GameObject.Find("AIPlayer-Red");
        if (currentPlayer == 2)
            go = GameObject.Find("AIPlayer-Blue");

        go.GetComponent<AudioSource>().volume = 1;
        go.GetComponent<AudioSource>().clip = clip;
        go.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(clip.length+1);

        OnPickConsonant(lastAILetter);
    }

    IEnumerator AISpin()
    {
        string file = "female1-Ill_spin";
        AudioClip clip = (AudioClip)Resources.Load(file, typeof(AudioClip));
        GameObject go = null;

        if (currentPlayer == 1)
            go = GameObject.Find("AIPlayer-Red");
        if (currentPlayer == 2)
            go = GameObject.Find("AIPlayer-Blue");

        go.GetComponent<AudioSource>().volume = 1;
        go.GetComponent<AudioSource>().clip = clip;
        go.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(clip.length + 1);

        GameObject gw = GameObject.Find("Wheel");
        gw.GetComponent<WheelScript>().currentPlayer = currentPlayer;
        gw.GetComponent<WheelScript>().allowSpin = true;
    }

}
