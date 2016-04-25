using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {


    public bool shaking;
    public float shakeTime;
    public float shakeForce;

    private Timer shakeTimer;

    void Awake() {
        shakeTimer = gameObject.AddComponent<Timer>();
        shakeTimer.Trigger += ShakeOff;
    }

    void FixedUpdate() {
        if (shaking)
            Shake();
    }

    void Shake() {
        float moveX = Random.Range(-1f, 1f);
        float moveY = Random.Range(-1f, 1f);
        Vector2 shakeVector = new Vector2(moveX, moveY);
        if (shakeVector.magnitude != 0)
            shakeVector /= shakeVector.magnitude;
        GetComponent<Rigidbody>().AddForce(shakeVector*shakeForce);
    }

    void ShakeOff() {
        shaking = false;
    }

    public void ShakeMe(float timeToShake) {
        shaking = true;
        shakeTimer.Go(timeToShake);
    }

}
