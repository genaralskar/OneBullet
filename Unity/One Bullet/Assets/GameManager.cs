using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Timer Stuff")]
    public TextMeshProUGUI timerText;
    private Coroutine timeC;
    public string currentTime;

    [Header("Final Count")]
    public TextMeshProUGUI finalCountText;
    public Animator finalCountAnims;
    private int finalCount = 0;

    [Header("Events")]
    public UnityEvent GameStart;
    public UnityEvent GameEnd;
    public UnityEvent BulletBreakStart;
    public UnityEvent BulletsBroken;

    public UnityAction<float> TimeUpdated;

    public void StartGame()
    {
        StartTimer();
    }

    #region Timer Stuff

    public void StartTimer()
    {
        timeC = StartCoroutine(TimeDisplay());
    }

    public void EndTimer()
    {
        StopCoroutine(timeC);
    }

    public string GetTimeDisplay(float time)
    {

        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        float milliseconds = time * 100 % 100;
        if (milliseconds > 99) milliseconds = 0;
        
        string textTime = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        TimeUpdated?.Invoke(time);

        return textTime;
    }

    private IEnumerator TimeDisplay()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float startTime = Time.time;
        while(true)
        {

            currentTime = GetTimeDisplay(Time.time - startTime);
            timerText.text = currentTime;
            yield return wait;
        }
    }

    #endregion

    #region End Game Stuff

    public void EndGame()
    {
        GameEnd.Invoke();
    }

    public void FreezeObjects()
    {
        Rigidbody[] rbs = FindObjectsOfType<Rigidbody>();
        foreach(var rb in rbs)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            rb.isKinematic = true;
        }
    }

    public void StartBreakBullets()
    {
        StartCoroutine(BreakBullets());
    }

    private IEnumerator BreakBullets()
    {
        BulletController[] bcs = FindObjectsOfType<BulletController>();
        foreach(var bc in bcs)
        {
            bc.generation = 10;
        }

        float waitTime = 0.6f;

        yield return new WaitForSeconds(0.5f);

        foreach(var bc in bcs)
        {
            yield return new WaitForSeconds(waitTime);
            bc.Die();
            AddToFinalCount();
            waitTime /= 1.2f;
        }

        BulletsBroken.Invoke();
    }

    private void AddToFinalCount()
    {
        finalCount++;
        finalCountText.text = finalCount.ToString();
        finalCountAnims.Play("FCAdd");
    }
    #endregion

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
