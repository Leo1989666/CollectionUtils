using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinPathTest : MonoBehaviour
{
    public Transform Gun;
    
    [Range(0f, 999f)]
    public float Speed;
    
    [Range(0f, 999f)]
    public float SinAmptitude;

    [Range(0f, 999f)]
    public float RotationFrequency;
    
    private Vector3 _Axis;

    private Vector3 _Position;
    
    // Start is called before the first frame update
    void Start()
    {
        _Axis = Gun.right;
        _Position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        _Position = _Position + (Time.deltaTime * Speed * Gun.forward);
        Vector3 position = _Position + Mathf.Sin(Time.time * RotationFrequency) * SinAmptitude * _Axis;
        transform.position = position;
    }
}
