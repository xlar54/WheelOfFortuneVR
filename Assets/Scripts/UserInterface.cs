using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

    public Canvas DisabledCanvas;

    Button[] buttons;

    public struct AllowedPlayerActions
    {
        public bool allowSpin;
        public bool allowBuyVowel;
        public bool allowSolve;
        public bool allowPickConsonant;
        public bool allowPickVowel;
    }

    // Use this for initialization
    void Start () {

        buttons = GetComponentsInChildren<Button>();
        this.Hide();
	}
	
    public void ShowMainMenu(bool withLogo)
    {
        gameObject.SetActive(true);

        AllowedPlayerActions allowedActions = new AllowedPlayerActions();
        allowedActions.allowPickConsonant = false;
        allowedActions.allowPickVowel = false;
        allowedActions.allowBuyVowel = false;
        allowedActions.allowSolve = false;
        allowedActions.allowSpin = false;

        this.Show("", "", allowedActions);

        if (withLogo)
        {
            GameObject go = GameObject.Find("pnlLogo");
            go.gameObject.SetActive(true);
        }

        foreach (Button b in buttons)
        {
            if (b.gameObject.name == "btnPlay" || b.gameObject.name == "btnQuit")
            {
                b.gameObject.SetActive(true);
            }
        }

    }

    public void Show(string availableConsonants, string availableVowels, AllowedPlayerActions allowedPlayerActions)
    {
        if (availableVowels == "     ")
            allowedPlayerActions.allowBuyVowel = false;

        gameObject.SetActive(true);

        foreach (Button b in buttons)
        {
            b.gameObject.SetActive(false);
        }

        if (allowedPlayerActions.allowSolve)
        {
            foreach (Button b in buttons)
            {
                if (b.gameObject.name == "btnSolve")
                {
                    b.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (allowedPlayerActions.allowSpin)
        {
            foreach (Button b in buttons)
            {
                if (b.gameObject.name == "btnSpin")
                {
                    b.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (allowedPlayerActions.allowBuyVowel)
        {
            foreach (Button b in buttons)
            {
                if (b.gameObject.name == "btnBuyVowel")
                {
                    b.gameObject.SetActive(true);
                    break;
                }
            }
        }

        if (allowedPlayerActions.allowPickVowel)
        {
            for (int x = 0; x < availableVowels.Length; x++)
            {
                if (availableVowels.Substring(x, 1) != " ")
                {
                    string btnLetter = "btn" + availableVowels.Substring(x, 1);

                    foreach (Button b in buttons)
                    {
                        if (b.gameObject.name == btnLetter)
                            b.gameObject.SetActive(true);
                    }
                }
            }
        }

        if (allowedPlayerActions.allowPickConsonant)
        {
            for (int x = 0; x < availableConsonants.Length; x++)
            {
                if (availableConsonants.Substring(x, 1) != " ")
                {
                    string btnLetter = "btn" + availableConsonants.Substring(x, 1);

                    foreach (Button b in buttons)
                    {
                        if (b.gameObject.name == btnLetter)
                            b.gameObject.SetActive(true);
                    }
                }
            }
        }


        if (allowedPlayerActions.allowPickConsonant || allowedPlayerActions.allowPickVowel)
            DisabledCanvas.gameObject.SetActive(true);
        else
            DisabledCanvas.gameObject.SetActive(false);


    }


    public void Hide()
    {
        gameObject.SetActive(false);
        DisabledCanvas.gameObject.SetActive(false);
    }

	// Update is called once per frame
	void Update () {



	}
}
