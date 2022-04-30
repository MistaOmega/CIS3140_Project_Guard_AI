using GameLogic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float health = 10;
    private Camera _camera;

    [Range(1, 15)] private float _moveSpeed = 6;
    private Rigidbody _rb;

    private Vector3 _velocity;
    public static Controller Instance { get; private set; }

    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = Camera.main;
        Instance = this;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 mousePos = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
            _camera.transform.position.y));
        transform.LookAt(mousePos + Vector3.up * transform.position.y);

        _velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized *
                    _moveSpeed;
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _velocity * Time.fixedDeltaTime);
    }

    public void DamagePlayer()
    {
        Debug.Log("OW!");
        health -= 1;

        if (health != 0) return;
        _moveSpeed = 0;
        ExitScript.Instance.End();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}