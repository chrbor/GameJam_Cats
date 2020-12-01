using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class MousScript : MonoBehaviour
{
    Rigidbody2D rb;
    public Rigidbody2D ball;
    private LineRenderer line;
    public float maxDistance;
    public float bounce;
    private float distance;

    public static Vector2 yarnPos;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        line = ball.GetComponent<LineRenderer>();
        line.positionCount = 10;
        line.enabled = true;
    }

    void FixedUpdate()
    {
        if (!game_running) return;

        rb.position = GetMousePosWorld();
        yarnPos = ball.position;

        Vector2 diff = ball.position - rb.position;
        distance = diff.magnitude;

        Vector2 step = diff.normalized * distance/10;
        for (int i = 0; i < 10; i++) line.SetPosition(i, rb.position + step * i);


        if (distance < maxDistance) return;

        //Halte den Ball innerhalb der maximalen Distanz:
        
        ball.velocity = RotToVec(ReflectAngle(Mathf.Atan2(diff.y, diff.x), Mathf.Atan2(ball.velocity.y, ball.velocity.x))) * ball.velocity.magnitude * bounce + (distance - maxDistance) * diff.normalized;

        ball.position = rb.position + diff.normalized * maxDistance;// + ball.velocity/20;
    }

    private float ReflectAngle(float angle_base, float angle_in)
    {
        return Mathf.PI + angle_base + (angle_base - angle_in);
    }

    private Vector2 RotToVec(float rotation)
    {
        return new Vector2(Mathf.Cos(rotation), Mathf.Sin(rotation));
    }

    private Vector3 GetMousePosWorld()
    {
        return Camera.main.transform.position - new Vector3(Camera.main.aspect, 1) * Camera.main.orthographicSize + Input.mousePosition / Screen.height * Camera.main.orthographicSize * 2;
    }
}
