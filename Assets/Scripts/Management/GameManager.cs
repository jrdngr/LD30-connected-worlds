using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    private int currentLevel;
    public int CurrentLevel {
        get { return currentLevel; }
    }
    private bool levelStarted = false;
    public bool LevelStarted {
        get { return levelStarted; }
    }

    public void SetLevel(int level) {
        currentLevel = level;
    }

    public void StartLevel() {
        levelStarted = true;
    }

    public void BlackHoled() {
        levelStarted = false;
    }

}
