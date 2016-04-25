using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour{

    public delegate void ManageTimer();
    public event ManageTimer Trigger;

    private float duration;
    private int repeat;
    private int iteration;
    private bool running = false;

    public int Iteration {
        get {
            return iteration;
        }
    }

    //Starts a timer that lasts for @dur and sends out the Trigger event when finished
    public void Go(float dur) {
        duration = dur;
        repeat = 1;
        iteration = 0;
        StartCoroutine("RunTimer");
    }

    //Starts a timer that lasts for @dur and repeats @rep times, sending out a Trigger event each time
    public void Go(float dur, int rep) {
        duration = dur;
        repeat = rep;
        iteration = 0;
        running = true;
        StartCoroutine("RunTimer");
    }

    public void Cancel() {
        StopAllCoroutines();
    }

    public bool Running() {
        return running;
    }

    //Restarts the timer with the same settings
    public void Reset() {
        StopAllCoroutines();
        iteration = 0;
        StartCoroutine("RunTimer");
    }

    IEnumerator RunTimer() {
        yield return new WaitForSeconds(duration);
        Trigger();
        running = false;
        iteration++;
        if (iteration < repeat)
            StartCoroutine("RunTimer");
    }

}
