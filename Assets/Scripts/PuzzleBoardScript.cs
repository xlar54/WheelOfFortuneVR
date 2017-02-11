using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class PuzzleBoardScript : MonoBehaviour {

    private Stack<RollbackLocation> rollbackQueue = new Stack<RollbackLocation>();

    private struct RollbackLocation
    {
        public int row;
        public int col;
    }

    public enum BlockValueType
    {
        Null,
        Empty,
        Letter
    }

    public Texture[] ltrTextures = new Texture[27];

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public GameObject lightOffRowCol(int r, int c)
    {
        string blockgameObjectName = "R" + r.ToString() + "C" + c.ToString();
        GameObject go = GameObject.Find(blockgameObjectName);
        go.GetComponent<Renderer>().material.mainTexture = ltrTextures[26];

        return go;
    }

    public GameObject lightOnRowCol(int r, int c)
    {
        return lightOnRowCol(r, c, new Color(1f, 1f, 1f));
    }

    public GameObject lightOnRowCol(int r, int c, Color color)
    {
        string blockgameObjectName = "R" + r.ToString() + "C" + c.ToString();
        GameObject go = GameObject.Find(blockgameObjectName);
        go.GetComponent<Renderer>().material.color = color;

        return go;
    }

    public void assignLetterRowCol(string letter, int r, int c)
    {
        if (letter == " ")
        {
            GameObject go = lightOnRowCol(r, c);

            go.GetComponent<Renderer>().material.mainTexture = null;
        }
        else
        {
            byte[] asciiBytes = Encoding.ASCII.GetBytes(letter);

            int textureIndex = asciiBytes[0] - 65;

            GameObject go = lightOnRowCol(r, c);

            go.GetComponent<Renderer>().material.mainTexture = ltrTextures[textureIndex];

            RollbackLocation rb = new RollbackLocation();
            rb.row = r;
            rb.col = c;

            rollbackQueue.Push(rb);
        }

    }

    public BlockValueType getBlockValueType(int r, int c)
    {
        string blockgameObjectName = "R" + r.ToString() + "C" + c.ToString();
        GameObject go = GameObject.Find(blockgameObjectName);

        Texture t = go.GetComponent<Renderer>().material.mainTexture;

        // if assigned the blank texture
        if (t == null)
            return BlockValueType.Empty;
        
        if (t == ltrTextures[26])
            return BlockValueType.Null;

        return BlockValueType.Letter;
    }

    public string getBlockValue(int r, int c)
    {
        string blockgameObjectName = "R" + r.ToString() + "C" + c.ToString();
        GameObject go = GameObject.Find(blockgameObjectName);

        Texture t = go.GetComponent<Renderer>().material.mainTexture;

        // if assigned the blank texture
        if (t == null)
            return " ";

        if (t == ltrTextures[26])
            return "";

        for(int z=0; z<26; z++)
        {
            if (t == ltrTextures[z])
            {
                byte b = (byte)(65 + z);
                return Encoding.ASCII.GetString(new byte[] { b });
            }
        }

        // Shouldnt get here
        return "";
    }

    public string getVisibleAnswer()
    {
        string answer = "";

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if ((r == 0 || r == 3) && c > 11)
                {
                    break;
                }

                answer += getBlockValue(r, c);

            }
        }

        return answer.Trim();
    }

    public int getEmptyBlockCount()
    {
        int emptyBlockCount = 0;

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if ((r == 0 || r == 3) && c > 11)
                {
                    break;
                }

                var blockValueType = getBlockValueType(r, c);

                if (blockValueType == PuzzleBoardScript.BlockValueType.Empty)
                    emptyBlockCount++;
            }
        }

        return emptyBlockCount;
    }

    public void getFirstEmptyBlock(out int row, out int col)
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if ((r == 0 || r == 3) && c > 11)
                {
                    break;
                }

                var blockValueType = getBlockValueType(r, c);

                if (blockValueType == PuzzleBoardScript.BlockValueType.Empty)
                {
                    row = r;
                    col = c;

                    return;
                }
            }
        }

        row = -1;
        col = -1;

        return;
    }

    public void rollback(int lastMoves)
    {
        for (int x=0; x < lastMoves; x++)
        {
            RollbackLocation rb = rollbackQueue.Pop();

            assignLetterRowCol(" ", rb.row, rb.col);

        }
    }

    public void reset()
    {
        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 14; c++)
            {
                if ((r == 0 || r == 3) && c > 11)
                {
                    break;
                }

                string blockgameObjectName = "R" + r.ToString() + "C" + c.ToString();
                GameObject go = GameObject.Find(blockgameObjectName);
                go.GetComponent<Renderer>().material.mainTexture = null;
                go.GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f);

            }
        }

        rollbackQueue.Clear();
        

    }
}
