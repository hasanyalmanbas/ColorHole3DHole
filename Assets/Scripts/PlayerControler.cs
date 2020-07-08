using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class PlayerControler : MonoBehaviour
{
    public float holeSize = 3.0f;
    public float holeSpeed = 0.15f;
    public float holeRadius = 3.0f;

    public float gravity = 10;

    public Transform models;

    public SphereCollider detector;
    public BoxCollider[] colliders;
    public List<Rigidbody> cubeRigidbodies;

    public float maxDragDistance = 40f;

    private Vector3 _moveDirection;

    private float _currentDragDistance;
    private Vector3 _firstTouchPosition;


    private bool isBusy;

    private void Start()
    {
        GameController.Instance.nextArenaEvent.AddListener(NextArena);

        SetHoleProperties();
    }

    private void Update()
    {
        if (isBusy)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TapDown();
        }
        else if (Input.GetMouseButton(0))
        {
            Holding();
        }

        var nearbyObjects = Physics.OverlapSphere(transform.position, holeRadius);
        foreach (var nearbyObject in nearbyObjects)
        {
            if (nearbyObject.gameObject == gameObject || nearbyObject.gameObject.layer != 9)
            {
                continue;
            }

            Rigidbody nearbyObjectRb = nearbyObject.GetComponentInParent<Rigidbody>();
            if (!nearbyObjectRb || cubeRigidbodies.Contains(nearbyObjectRb))
            {
                continue;
            }
            else
            {
                cubeRigidbodies.Add(nearbyObjectRb);
                nearbyObject.gameObject.layer = 10;
                nearbyObjectRb.isKinematic = false;
            }
        }
    }

    private void TapDown()
    {
        _firstTouchPosition = GetMousePosition();
    }

    private void Holding()
    {
        transform.position += GetDirection() * holeSpeed * _currentDragDistance * Time.deltaTime;

        if (Mathf.Abs(transform.position.x) > 9)
        {
            if (transform.position.x > 0)
            {
                transform.position = new Vector3(9, transform.position.y, transform.position.z);
            }
            else if (transform.position.x < 0)
            {
                transform.position = new Vector3(-9, transform.position.y, transform.position.z);
            }
        }

        Vector3 centerPoint = GameController.Instance.GetCurrentArenaCenterPosition();

        if (transform.position.z > centerPoint.z + 25)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, centerPoint.z + 25);
        }
        else if (transform.position.z < centerPoint.z - 25)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, centerPoint.z - 25);
        }
    }

    private Vector3 GetMousePosition()
    {
        return Input.mousePosition;
    }

    private Vector3 GetDirection()
    {
        Vector3 mousePos = GetMousePosition();

        _currentDragDistance = (mousePos - _firstTouchPosition).magnitude;

        if (_currentDragDistance > maxDragDistance)
        {
            _firstTouchPosition = mousePos - _moveDirection * maxDragDistance;
            _currentDragDistance = (mousePos - _firstTouchPosition).magnitude;
        }

        _moveDirection = (mousePos - _firstTouchPosition).normalized;

        return new Vector3(_moveDirection.x, 0, _moveDirection.y);
    }

    private void FixedUpdate()
    {
        if (isBusy)
        {
            return;
        }

        foreach (var cubeRigidbody in cubeRigidbodies)
        {
            cubeRigidbody.AddForce(Vector3.down * holeSize * gravity * Time.fixedDeltaTime,
                ForceMode.VelocityChange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.position.y < 0)
        {
            //other.gameObject.SetActive(false);
            if (other.gameObject.CompareTag("Danger"))
            {
                Debug.Log("Gameover");
                GameController.Instance.GameOver();
            }
            GameController.Instance.destroyCubeEvent.Invoke(other.gameObject);
            Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
            if (otherRigidbody)
            {
                cubeRigidbodies.Remove(otherRigidbody);
            }
        }
    }

    private void NextArena(Vector3 nextPosition)
    {
        StartCoroutine(GoToNextArena(nextPosition));
    }

    IEnumerator GoToNextArena(Vector3 nextPosition)
    {
        isBusy = true;
        Vector3 position = new Vector3();
        position.x = 0;
        position.y = transform.position.y;
        position.z = transform.position.z;

        while (transform.position.x != 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, 0.25f);
            yield return null;
        }


        position = new Vector3();
        position.x = 0;
        position.y = transform.position.y;
        position.z = nextPosition.z;

        while (transform.position != position)
        {
            transform.position = Vector3.MoveTowards(transform.position, position, 0.25f);
            yield return null;
        }

        isBusy = false;
    }

    private void SetHoleProperties()
    {
        holeSize++;
        holeRadius++;
        models.localScale = new Vector3(holeSize, holeSize, holeSize);
        models.localPosition = new Vector3(0, -holeSize / 2f - 0.49f, 0);
        detector.center = new Vector3(0, -1f - holeSize / 2f, 0);
        detector.radius = holeSize / 2f;

        foreach (var coll in colliders)
        {
            var direction = coll.center.normalized * 50.025f;
            coll.center = direction * (1 + holeSize / 100f);
        }
    }
}