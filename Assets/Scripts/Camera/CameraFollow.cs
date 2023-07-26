using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform _ship;
    [SerializeField] Vector3 _dis;
    [SerializeField] float _speed;

    void Update()
    {
        if (Player.camOn==true)
            transform.position = Vector3.Lerp(transform.position, _dis + _ship.position, _speed * Time.deltaTime);
    }
}
