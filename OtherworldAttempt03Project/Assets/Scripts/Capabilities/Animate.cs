using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    private Animator _animator;
    [SerializeField]
    private InputController _inputController;
    private bool _running;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        _running = _inputController.RetrieveMoveInput() > 0;
    }
}
