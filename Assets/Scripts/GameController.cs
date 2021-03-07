using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text instructionText;
    public Text timerText;
    public Text resultText;
    public Text breakText;

    public GameObject[] outlines;
    public GameObject breakIndicator;
    public MeshDrawer drawer;
    public Rigidbody2D[] toBreak;

    public Collider2D groundCollider;

    public float timer = 20f;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FlickerBreakIndicator());
        StartCoroutine(CountDownToGameStart());
    }


    IEnumerator FlickerBreakIndicator()
    {
        while (timer > 0)
        {
            breakIndicator.SetActive(!breakIndicator.activeSelf);
            breakText.gameObject.SetActive(!breakText.gameObject.activeSelf);
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }

    IEnumerator CountDownToGameStart()
    {
        while(timer > 0)
        {
            timerText.text = "" + Mathf.CeilToInt(timer);
            timer -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(WaitForGameResult());
        yield return null;
    }

    IEnumerator WaitForGameResult()
    {
        drawer.gameOn = false;

        instructionText.gameObject.SetActive(false);
        //timerText.gameObject.SetActive(false);
        breakText.gameObject.SetActive(false);
        breakIndicator.SetActive(false);
        foreach (GameObject line in outlines)
        {
            line.SetActive(false);
        }
        drawer.illustrativeLine.SetActive(false);

        for (int i = 0; i < toBreak.Length; i++)
        {
            toBreak[i].gameObject.SetActive(true);
        }

        float timey = 6f;
        timerText.color = Color.white;

        while(timey > 0f)
        {
            timerText.text = ""+Mathf.CeilToInt(timey);
            timey -= Time.deltaTime;
            yield return null;
        }

        bool win = true;
        for (int i = 0; i < toBreak.Length; i++)
        {
            if(toBreak[i].bodyType == RigidbodyType2D.Dynamic)
            {
                if (toBreak[i].IsTouching(groundCollider))
                {
                    win = false;
                }
            }
        }

        if (win)
        {
            resultText.text = "WIN";
        } else
        {
            resultText.text = "FAILED";
        }
        timerText.gameObject.SetActive(false);

        yield return null;
    }
}
