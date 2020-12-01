using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public static bool game_running;
    public static bool pause;

    private GameObject menu;
    private Text score, highScore;

    private GameObject yarn;
    public GameObject cat;
    private List<GameObject> cats = new List<GameObject>();

    private int maxScore = 0;

    // Start is called before the first frame update
    void Awake()
    {
        if (manager) { Destroy(gameObject); return; }
        manager = this;
        DontDestroyOnLoad(gameObject);

        Transform canvas = GameObject.Find("Canvas").transform;
        menu = canvas.GetChild(1).gameObject;

        score = canvas.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
        highScore = canvas.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();

        yarn = GameObject.FindGameObjectWithTag("Yarn");

        score.text = "0";
        highScore.text = "0";

        StartCoroutine(RunGame());
        StartCoroutine(ListenForQuit());
    }

    IEnumerator ListenForQuit()
    {
        yield return new WaitUntil(() => Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Q));
        Application.Quit(0);
        yield break;
    }

    IEnumerator RunGame()
    {
        while (true)
        {
            menu.SetActive(true);
            yarn.SetActive(false);
            score.text = "0";

            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            yield return new WaitUntil(() => Input.GetMouseButton(0));
            menu.SetActive(false);
            yarn.SetActive(true);
            game_running = true;
            Physics2D.autoSimulation = true;

            float counter = 0;
            int catCount = 0;
            int currentScore = 0;

            while (game_running)
            {
                yield return new WaitForEndOfFrame();
                counter += Time.deltaTime;

                currentScore = Mathf.RoundToInt(counter);
                score.text = currentScore.ToString();
                if(currentScore > maxScore)
                {
                    maxScore = currentScore;
                    highScore.text = maxScore.ToString();
                }


                if (catCount-1 < counter / 10)
                {
                    catCount++;
                    cats.Insert(0, Instantiate(cat, new Vector3( Random.Range(0, 2) > 0 ? -12 : 12 , -3.8f, 0), Quaternion.identity));
                    cats[0].GetComponent<CatScript>().type = Random.Range(0, 9);
                }
            }
            Physics2D.autoSimulation = false;

            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            yield return new WaitUntil(() => Input.GetMouseButton(0));

            foreach (var obj in cats) Destroy(obj);
            cats.Clear();
        }
    }
}
